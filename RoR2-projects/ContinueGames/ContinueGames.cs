using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx;
using RoR2;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;
using Utilities;

using ArgsHelper = Utilities.Generic.ArgsHelper;

namespace ContinueGames
{

    [BepInPlugin("com.morris1927.ContinueGames", "ContinueGames", "1.0.0")]
    public class ContinueGames : BaseUnityPlugin
    {

        public static ContinueGames instance { get; set; }

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

            On.RoR2.Run.AdvanceStage += (orig, self, stage) => {
                orig(self, stage);
                //SaveGame();
            };
        }

        [ConCommand(commandName = "load", flags = ConVarFlags.Engine, helpText = "Load game")]
        private static void CCLoad(ConCommandArgs args) {
            GameNetworkManager.singleton.desiredHost = new GameNetworkManager.HostDescription(new GameNetworkManager.HostDescription.HostingParameters {
                listen = false,
                maxPlayers = 1
            });
            instance.StartCoroutine(instance.StartLoading());
        }
        [ConCommand(commandName = "save", flags = ConVarFlags.Engine, helpText = "Save game")]
        private static void CCSave(ConCommandArgs args) {
            SaveGame();
        }

        private IEnumerator StartLoading() {
            //yield return new WaitUntil(() => PreGameController.instance);
            yield return new WaitForSeconds(1f);
            PreGameController.instance?.StartLaunch();
            //yield return new WaitUntil(() => Run.instance);
            yield return new WaitForSeconds(1f);
            
            LoadGame();
        }

        private static void SaveGame() {
            SaveData save = new SaveData();
            save.playerList = new List<PlayerData>();

            foreach (var item in NetworkUser.readOnlyInstancesList) {
                SavePlayer(item, ref save);
            }
            save.teamExp = (int) TeamManager.instance.GetTeamExperience(TeamIndex.Player);

            save.difficulty = (int)Run.instance.selectedDifficulty;
            save.fixedTime = Run.instance.fixedTime;
            save.stageClearCount = Run.instance.stageClearCount;
            save.sceneName = Stage.instance.sceneDef.sceneName;

            string json = TinyJson.JSONWriter.ToJson(save);
            Debug.Log(json);
            PlayerPrefs.SetString("Save0", json);
        }

        private static void SavePlayer(NetworkUser player, ref SaveData save) {

            PlayerData playerData = new PlayerData();
            ulong steamID = GameNetworkManager.singleton.GetSteamIDForConnection(player.connectionToServer).value;
            
            CharacterBody body = player.master?.GetBody();
            Inventory inventory = body.inventory;
            playerData.steamID = steamID.ToString();
            playerData.items = new int[(int)ItemIndex.Count - 1];
            for (int i = 0; i < (int)ItemIndex.Count -1; i++) {
                playerData.items[i] = inventory.GetItemCount((ItemIndex)i);
            }

            playerData.equipItem0 = (int) inventory.GetEquipment(0).equipmentIndex;
            playerData.equipItem1 = (int) inventory.GetEquipment(1).equipmentIndex;
            playerData.equipItemCount = inventory.GetEquipmentSlotCount();

            playerData.characterBodyName = player.master.bodyPrefab.name.Replace(" (Clone)", "");

            save.playerList.Add(playerData);

        }

        private static void LoadGame() {
            string data = PlayerPrefs.GetString("Save0");
            SaveData save = TinyJson.JSONParser.FromJson<SaveData>(data);

            foreach (var item in save.playerList) {
                LoadPlayer(item);
            }

            TeamManager.instance.GiveTeamExperience(TeamIndex.Player, (ulong) save.teamExp);

            Run.instance.selectedDifficulty = (DifficultyIndex) save.difficulty;
            Run.instance.fixedTime = save.fixedTime;
            Run.instance.stageClearCount = save.stageClearCount - 1;
            Run.instance.AdvanceStage(save.sceneName);
            //TODO: Get SceneDef to load variation
        }

        private static void LoadPlayer(PlayerData playerData) {
            NetworkUser player = RetrievePlayerFromSteamID(ulong.Parse(playerData.steamID));
            if (player == null) {
                return;
            }

            CharacterBody body = player?.master?.GetBody();
            Inventory inventory = body?.inventory;

            GameObject bodyPrefab = BodyCatalog.FindBodyPrefab(playerData.characterBodyName);
            player.master.bodyPrefab = bodyPrefab;
            player.master.Respawn(Vector3.one, Quaternion.identity);

            for (int i = 0; i < playerData.items.Length -1; i++) {
                inventory.RemoveItem((ItemIndex)i, int.MaxValue);
                inventory.GiveItem((ItemIndex)i, playerData.items[i]);
            }

            inventory.SetEquipmentIndex((EquipmentIndex)playerData.equipItem0);
            if (playerData.equipItemCount == 2) {
                inventory.SetActiveEquipmentSlot((byte)1);
                inventory.SetEquipmentIndex((EquipmentIndex)playerData.equipItem1);
            }
        }

        public static NetworkUser RetrievePlayerFromSteamID(ulong steamID) {
            foreach (var item in NetworkUser.readOnlyInstancesList) {
                if (steamID == GameNetworkManager.singleton.GetSteamIDForConnection(item.connectionToServer).value) {
                    return item;
                }
            }

            return null;
        }
    }
}
