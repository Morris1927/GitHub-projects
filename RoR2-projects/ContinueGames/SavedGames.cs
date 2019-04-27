using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx;
using RoR2;
using RoR2.Networking;
using SavedGames;
using UnityEngine;
using UnityEngine.Networking;
using Utilities;

using ArgsHelper = Utilities.Generic.ArgsHelper;

namespace SavedGames
{

    [BepInPlugin("com.morris1927.ContinueGames", "ContinueGames", "1.0.0")]
    public class SavedGames : BaseUnityPlugin
    {

        public static SavedGames instance { get; set; }

        public static bool loadingScene;
        public static ulong seed;

        public void Awake() {
            if (instance == null) {
                instance = this;
            } else {
                Destroy(this);
            }
            On.RoR2.Console.Awake += (orig, self) => {
                Generic.CommandHelper.RegisterCommands(self);
                orig(self);
            };
            On.RoR2.Run.Start += (orig, self) => {
                self.seed = seed == 0 ? self.seed : seed;
                orig(self);
            };
            On.RoR2.SceneDirector.PopulateScene += (orig, self) => {
                if (!loadingScene) {
                    orig(self);
                }
            };
        }

        [ConCommand(commandName = "load", flags = ConVarFlags.None, helpText = "Load game")]
        private static void CCLoad(ConCommandArgs args) {
            if (args.Count != 1) {
                Debug.Log("Command failed, requires 1 argument: load <filename>");
                return;
            }

            string saveString = PlayerPrefs.GetString("Save" + ArgsHelper.GetValue(args.userArgs, 0));
            if (saveString == "") {
                Debug.Log("Save does not exist.");
                return;
            }
            SaveData save = TinyJson.JSONParser.FromJson<SaveData>(saveString);
            instance.StartCoroutine(instance.StartLoading(save));
        }
        [ConCommand(commandName = "save", flags = ConVarFlags.None, helpText = "Save game")]
        private static void CCSave(ConCommandArgs args) {
            if (args.Count != 1) {
                Debug.Log("Command failed, requires 1 argument: save <filename>");
                return;
            }
            SaveGame(ArgsHelper.GetValue(args.userArgs, 0));
        }

        private IEnumerator StartLoading(SaveData save) {
            seed = ulong.Parse(save.seed);
            if (Run.instance == null) {
                GameNetworkManager.singleton.desiredHost = new GameNetworkManager.HostDescription(new GameNetworkManager.HostDescription.HostingParameters {
                    listen = false,
                    maxPlayers = 1
                });
                yield return new WaitForSeconds(1f);
                PreGameController.instance?.StartLaunch();
                yield return new WaitForSeconds(1f);
            }
            
            LoadGame(save);
        }

        private static void SaveGame(string saveFile) {
            SaveData save = new SaveData();
            save.playerList = new List<PlayerData>();

            foreach (var item in NetworkUser.readOnlyInstancesList) {
                SavePlayer(item, ref save);
            }
            SaveRun(ref save);

            string json = TinyJson.JSONWriter.ToJson(save);
            Debug.Log(json);
            PlayerPrefs.SetString("Save" + saveFile, json);
        }

        private static void SaveRun(ref SaveData save) {
            save.teamExp = (int)TeamManager.instance.GetTeamExperience(TeamIndex.Player);

            save.seed = Run.instance.seed.ToString();
            save.difficulty = (int)Run.instance.selectedDifficulty;
            save.fixedTime = Run.instance.fixedTime;
            save.stageClearCount = Run.instance.stageClearCount;
            save.sceneName = Stage.instance.sceneDef.sceneName;
        }

        private static void SavePlayer(NetworkUser player, ref SaveData save) {
            PlayerData playerData = new PlayerData();

            Inventory inventory = player.master.inventory;
            playerData.username = player.userName;

            playerData.items = new int[(int)ItemIndex.Count - 1];
            for (int i = 0; i < (int)ItemIndex.Count -1; i++) {
                playerData.items[i] = inventory.GetItemCount((ItemIndex)i);
            }
            
            playerData.equipItem0 = (int) inventory.GetEquipment(0).equipmentIndex;
            playerData.equipItem1 = (int) inventory.GetEquipment(1).equipmentIndex;
            playerData.equipItemCount = inventory.GetEquipmentSlotCount();

            playerData.characterBodyName = player.master.bodyPrefab.name;

            save.playerList.Add(playerData);

        }

        private static void LoadGame(SaveData save) {
            foreach (var item in save.playerList) {
                LoadPlayer(item);
            }

            LoadRun(save);

            LoadSceneDirector.PopulateScene(save);
            seed = 0;
        }

        private static void LoadRun(SaveData save) {
            TeamManager.instance.GiveTeamExperience(TeamIndex.Player, (ulong)save.teamExp);
            Run.instance.selectedDifficulty = (DifficultyIndex)save.difficulty;
            Run.instance.fixedTime = save.fixedTime;
            Run.instance.stageClearCount = save.stageClearCount - 1;

            Run.instance.runRNG = new Xoroshiro128Plus(seed);
            Run.instance.nextStageRng = new Xoroshiro128Plus(Run.instance.runRNG.nextUlong);
            Run.instance.stageRngGenerator = new Xoroshiro128Plus(Run.instance.runRNG.nextUlong);
            
            int dummy;
            for (int i = 0; i < Run.instance.stageClearCount + 1; i++) {
                dummy = (int)Run.instance.stageRngGenerator.nextUlong;
                dummy = Run.instance.nextStageRng.RangeInt(0,1);
                dummy = Run.instance.nextStageRng.RangeInt(0,1);
            }
           // if (Run.instance.stageClearCount > 0) {
                Run.instance.AdvanceStage(save.sceneName);
            // }
            
        }

        private static void LoadPlayer(PlayerData playerData) {
            NetworkUser player = GetPlayerFromUsername(playerData.username);
            if (player == null) {
                Debug.Log("Could not find player: " + playerData.username);
                return;
            }

            Inventory inventory = player.master?.inventory;

            GameObject bodyPrefab = BodyCatalog.FindBodyPrefab(playerData.characterBodyName);
            player.master.bodyPrefab = bodyPrefab;
            player.master.Respawn(Vector3.zero, Quaternion.identity);

            for (int i = 0; i < playerData.items.Length -1; i++) {
                inventory.RemoveItem((ItemIndex)i, int.MaxValue);
                inventory.GiveItem((ItemIndex)i, playerData.items[i]);
            }

            inventory.SetEquipmentIndex((EquipmentIndex)playerData.equipItem0);
            if (playerData.equipItemCount == 2) {
                inventory.SetActiveEquipmentSlot((byte)1);
                inventory.SetEquipmentIndex((EquipmentIndex)playerData.equipItem1);
            }
            player.master.money = 15;
        }

        public static NetworkUser GetPlayerFromUsername(string username) {
            foreach (var item in NetworkUser.readOnlyInstancesList) {
                if (username == item.userName) {
                    return item;
                }
            }

            return null;
        }
    }
}
