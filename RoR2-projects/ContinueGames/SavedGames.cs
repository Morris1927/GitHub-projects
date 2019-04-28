using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using RoR2;
using RoR2.Networking;
using SavedGames;
using UnityEngine;
using UnityEngine.Networking;
using Utilities;
using UnityEditor;

using ArgsHelper = Utilities.Generic.ArgsHelper;

namespace SavedGames
{

    [BepInPlugin("com.morris1927.ContinueGames", "ContinueGames", "1.0.0")]
    public class SavedGames : BaseUnityPlugin
    {

        public static SavedGames instance { get; set; }
        
        public static bool loadingScene;
        //public static ulong seed;

        public static FieldInfo getDropPickup = typeof(ChestBehavior).GetField("dropPickup");

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

            foreach (var item in Resources.LoadAll<SpawnCard>("")) {
                Debug.Log(item.name);
            }
            //On.RoR2.Run.Start += (orig, self) => {
            //    self.seed = seed == 0 ? self.seed : seed;
            //    orig(self);
            //};
            On.RoR2.SceneDirector.PopulateScene += (orig, self) => {
                if (!loadingScene ) {
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
            //seed = ulong.Parse(save.seed);
            loadingScene = true;
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
            save.players = new List<PlayerData>();
            save.chests = new List<ChestData>();
            save.barrels = new List<BarrelData>();


            foreach (var item in NetworkUser.readOnlyInstancesList) {
                PlayerData.SavePlayer(item, ref save);
            }

            foreach (var item in FindObjectsOfType<ChestBehavior>()) {
                ChestData.SaveChest(item, ref save);
            }
            foreach (var item in FindObjectsOfType<BarrelInteraction>()) {
                BarrelData.SaveBarrel(item, ref save);
            }

            SaveRun(ref save);

            string json = TinyJson.JSONWriter.ToJson(save);
            Debug.Log(json);
            PlayerPrefs.SetString("Save" + saveFile, json);
        }

        private static void SaveChest(ChestBehavior item, ref SaveData save) {

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

            playerData.transform = new SerializableTransform(player.transform);
            playerData.money = (int) player.master.money;
            playerData.items = new int[(int)ItemIndex.Count - 1];
            for (int i = 0; i < (int)ItemIndex.Count -1; i++) {
                playerData.items[i] = inventory.GetItemCount((ItemIndex)i);
            }
            
            playerData.equipItem0 = (int) inventory.GetEquipment(0).equipmentIndex;
            playerData.equipItem1 = (int) inventory.GetEquipment(1).equipmentIndex;
            playerData.equipItemCount = inventory.GetEquipmentSlotCount();

            playerData.characterBodyName = player.master.bodyPrefab.name;

            save.players.Add(playerData);

        }

        private static void LoadGame(SaveData save) {

            LoadRun(save);

            instance.StartCoroutine(instance.PopulateScene(save));
            
        }

        IEnumerator PopulateScene(SaveData save) {
            yield return new WaitForSeconds(2f);
            LoadSceneDirector l = new LoadSceneDirector();
            l.PopulateScene(save);

            foreach (var item in save.players) {
                item.LoadPlayer();
            }

            loadingScene = false;
        }

        private static void LoadRun(SaveData save) {
            TeamManager.instance.GiveTeamExperience(TeamIndex.Player, (ulong)save.teamExp);
            Run.instance.selectedDifficulty = (DifficultyIndex)save.difficulty;
            Run.instance.fixedTime = save.fixedTime;
            Run.instance.stageClearCount = save.stageClearCount - 1;

            Run.instance.runRNG = new Xoroshiro128Plus(ulong.Parse(save.seed));
            Run.instance.nextStageRng = new Xoroshiro128Plus(Run.instance.runRNG.nextUlong);
            Run.instance.stageRngGenerator = new Xoroshiro128Plus(Run.instance.runRNG.nextUlong);

            int dummy;
            for (int i = 0; i < Run.instance.stageClearCount + 1; i++) {
                dummy = (int)Run.instance.stageRngGenerator.nextUlong;
                dummy = Run.instance.nextStageRng.RangeInt(0, 1);
                dummy = Run.instance.nextStageRng.RangeInt(0, 1);
            }

            Run.instance.AdvanceStage(save.sceneName);

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
