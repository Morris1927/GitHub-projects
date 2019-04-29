using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx;
using RoR2;
using RoR2.Networking;
using UnityEngine;
using Utilities;
using SavedGames.Data;

using ArgsHelper = Utilities.Generic.ArgsHelper;
using System.Reflection;

namespace SavedGames
{

    [BepInPlugin("com.morris1927.ContinueGames", "ContinueGames", "1.0.0")]
    public class SavedGames : BaseUnityPlugin {

        public static SavedGames instance { get; set; }

        public static bool loadingScene;

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

            On.RoR2.SceneDirector.PopulateScene += (orig, self) => {
                if (!loadingScene) {
                    orig(self);
                }
            };

        }

        public void Update() {
            if (Input.GetKeyDown(KeyCode.F5)) {
                //Save
                RoR2.Console.instance.SubmitCmd(NetworkUser.readOnlyLocalPlayersList[0], "save test");
            }
            if (Input.GetKeyDown(KeyCode.F8)) {
                //Load
                RoR2.Console.instance.SubmitCmd(NetworkUser.readOnlyLocalPlayersList[0], "load test");
            }
            if (Input.GetKeyDown(KeyCode.F6)) {
                //Quick cheats
                RoR2.Console.instance.SubmitCmd(NetworkUser.readOnlyLocalPlayersList[0], "give_item hoof 30; god; kill_all; no_enemies");
            }

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

            loadingScene = true;
            if (Run.instance == null) {
                GameNetworkManager.singleton.desiredHost = new GameNetworkManager.HostDescription(new GameNetworkManager.HostDescription.HostingParameters {
                    listen = false,
                    maxPlayers = 1
                });
                yield return new WaitUntil(() => PreGameController.instance != null);
                PreGameController.instance?.StartLaunch();
                yield return new WaitUntil(() => Run.instance != null);
            }

            instance.StartCoroutine(instance.PopulateScene(save));

        }

        private static void SaveGame(string saveFile) {
            SaveData save = new SaveData();
            save.players = new List<PlayerData>();
            save.chests = new List<ChestData>();
            save.barrels = new List<BarrelData>();
            save.printers = new List<PrinterData>();
            save.multiShops = new List<MultiShopData>();

            save.chanceShrines = new List<ShrineChanceData>();
            save.bloodShrines = new List<ShrineBloodData>();
            save.bossShrines = new List<ShrineBossData>();
            save.combatShrines = new List<ShrineCombatData>();
            save.goldshoreShrines = new List<ShrineGoldshoresAccessData>();
            save.healingShrines = new List<ShrineHealingData>();
            save.orderShrines = new List<ShrineRestackData>();

            save.brokenDrones = new List<BrokenDroneData>();

            foreach (var item in NetworkUser.readOnlyInstancesList) {
                save.players.Add(PlayerData.SavePlayer(item));
            }

            foreach (var item in FindObjectsOfType<ChestBehavior>()) {
                save.chests.Add(ChestData.SaveChest(item));
            }
            foreach (var item in FindObjectsOfType<BarrelInteraction>()) {
                save.barrels.Add(BarrelData.SaveBarrel(item));
            }
            foreach (var item in FindObjectsOfType<ShopTerminalBehavior>()) {
                if (item.name.Contains("Duplicator")) {
                    save.printers.Add(PrinterData.SavePrinter(item));
                }
            }
            foreach (var item in FindObjectsOfType<MultiShopController>()) {
                save.multiShops.Add(MultiShopData.SaveMultiShop(item));
            }
            foreach (var item in FindObjectsOfType<ShrineChanceBehavior>()) {
                save.chanceShrines.Add(ShrineChanceData.SaveShrineChance(item));
            }
            foreach (var item in FindObjectsOfType<ShrineBloodBehavior>()) {
                save.bloodShrines.Add(ShrineBloodData.SaveShrineBlood(item));
            }
            foreach (var item in FindObjectsOfType<ShrineBossBehavior>()) {
                save.bossShrines.Add(ShrineBossData.SaveShrineBoss(item));
            }
            foreach (var item in FindObjectsOfType<ShrineCombatBehavior>()) {
                save.combatShrines.Add(ShrineCombatData.SaveShrineCombat(item));
            }
            foreach (var item in FindObjectsOfType<ShrineHealingBehavior>()) {
                save.healingShrines.Add(ShrineHealingData.SaveShrineHealing(item));
            }
            foreach (var item in FindObjectsOfType<ShrineRestackBehavior>()) {
                save.orderShrines.Add(ShrineRestackData.SaveShrineRestack(item));
            }
            save.teleporter = TeleporterData.SaveTeleporter(FindObjectOfType<TeleporterInteraction>());

            save.run = RunData.SaveRun(Run.instance);

            string json = TinyJson.JSONWriter.ToJson(save);
            Debug.Log(json);
            PlayerPrefs.SetString("Save" + saveFile, json);
        }

        IEnumerator PopulateScene(SaveData save) {
            save.run.LoadData();

            // yield return new WaitWhile(() => Stage.instance == null);
            yield return new WaitForSeconds(2f);

            foreach (var item in save.chests) {
                item.LoadChest();
            }
            foreach (var item in save.barrels) {
                item.LoadBarrel();
            }
            foreach (var item in save.printers) {
                item.LoadPrinter();
            }
            foreach (var item in save.multiShops) {
                item.LoadMultiShop();
            }
            foreach (var item in save.chanceShrines) {
                item.LoadShrineChance();
            }
            foreach (var item in save.bloodShrines) {
                item.LoadShrineBlood();
            }
            foreach (var item in save.bossShrines) {
                item.LoadShrineBoss();
            }
            foreach (var item in save.combatShrines) {
                item.LoadShrineCombat();
            }
            foreach (var item in save.healingShrines) {
                item.LoadShrineHealing();
            }
            foreach (var item in save.orderShrines) {
                item.LoadShrineRestack();
            }
            save.teleporter.LoadTeleporter();

            foreach (var item in save.players) {
                item.LoadPlayer();
            }


            loadingScene = false;
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
