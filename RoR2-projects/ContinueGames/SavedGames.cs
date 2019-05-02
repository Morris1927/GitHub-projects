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

    [BepInPlugin("com.morris1927.SavedGames", "SavedGames", "2.0.0")]
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
                RoR2.Console.instance.SubmitCmd(null, "save test");
            }
            if (Input.GetKeyDown(KeyCode.F8)) {
                //Load
                RoR2.Console.instance.SubmitCmd(null, "load test");
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
            SaveData.Save(ArgsHelper.GetValue(args.userArgs, 0));
        }

        private void ClearStage() {
            foreach (var item in FindObjectsOfType<PurchaseInteraction>()) {
                Destroy(item.gameObject);
            }
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

            FieldInfo getSpawnedAnyPlayer = typeof(Stage).GetField("spawnedAnyPlayer", BindingFlags.NonPublic | BindingFlags.Instance);
            //yield return new WaitUntil(() => FindObjectsOfType<CharacterBody>().Length != 0);
            //yield return new WaitUntil(() => Stage.instance != null);
            //yield return null;
            //getSpawnedAnyPlayer.SetValue(Stage.instance, false);
            //yield return null;
            //yield return new WaitUntil(() => (bool) getSpawnedAnyPlayer.GetValue(Stage.instance));
            //yield return new WaitUntil(() => );
            //yield return new WaitForFixedUpdate();
            //yield return null; 

            yield return new WaitForSeconds(Stage.instance == null ? 1f : 0.75f);
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
