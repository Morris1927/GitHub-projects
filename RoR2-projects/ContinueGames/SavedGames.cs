using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;

using RoR2;
using RoR2.Networking;
using UnityEngine;
using Utilities;
using SavedGames.Data;

using ArgsHelper = Utilities.Generic.ArgsHelper;
using System.Reflection;

namespace SavedGames
{

    [BepInPlugin("com.morris1927.SavedGames", "SavedGames", "2.0.0")]
    public class SavedGames : BaseUnityPlugin {

        public static SavedGames instance { get; set; }

        public static bool loadingScene;

        public static ConfigWrapper<int> loadKey { get; set; }
        public static ConfigWrapper<int> saveKey { get; set; }

        public void Awake() {
            if (instance == null) {
                instance = this;
            } else {
                Destroy(this);
            }

            //Config.Wrap
            loadKey = Config.Wrap<int>(
                "Keybinds", "LoadKey", null,
                (int) KeyCode.F5);
            saveKey = Config.Wrap<int>(
                "Keybinds", "SaveKey", null,
                (int) KeyCode.F8);

            On.RoR2.Console.Awake += (orig, self) => {
                Generic.CommandHelper.RegisterCommands(self);
                orig(self);
            };

            On.RoR2.SceneDirector.PopulateScene += (orig, self) => {
                if (!loadingScene) {
                    orig(self);
                }
                loadingScene = false;
            };

        }

        public void Update() {
            HandleInputs();

        }

        private void HandleInputs() {
            if (Input.GetKeyDown((KeyCode)loadKey.Value)) {
                //Save
                RoR2.Console.instance.SubmitCmd(null, "save quicksave");
            }
            if (Input.GetKeyDown((KeyCode)saveKey.Value)) {
                //Load
                RoR2.Console.instance.SubmitCmd(null, "load quicksave ");
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
            if (loadingScene) {
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
            SaveData.Save(ArgsHelper.GetValue(args.userArgs, 0));
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
            save.run.LoadData();

            yield return new WaitForSeconds(Stage.instance == null ? 1.5f : 0.75f);
            save.Load();
            
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
