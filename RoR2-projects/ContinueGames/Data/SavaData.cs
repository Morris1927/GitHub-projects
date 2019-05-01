using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using Object = UnityEngine.Object;

namespace SavedGames.Data {
    [Serializable]
    public class SaveData {


        public RunData run;

        public List<PlayerData> players;
        public List<EnemyData> enemies;
        public List<ChestData> chests;
        public List<BarrelData> barrels;
        public List<PrinterData> printers;
        public List<BrokenDroneData> brokenDrones;
        public List<MultiShopData> multiShops;
        public List<ShrineChanceData> chanceShrines;
        public List<ShrineBloodData> bloodShrines;
        public List<ShrineBossData> bossShrines;
        public List<ShrineCombatData> combatShrines;
        public List<ShrineHealingData> healingShrines;
        public List<ShrineRestackData> orderShrines;
        public List<ShrineGoldshoresAccessData> goldshoreShrines;
        public List<ItemDropletData> itemDroplets;
        public List<PortalData> portals;
        public List<BazaarPodData> bazaarPods;


        public TeleporterData teleporter;

        public static void Save(string saveFile) {
            SaveData save = new SaveData();
            save.players = new List<PlayerData>();
            save.enemies = new List<EnemyData>();
            save.chests = new List<ChestData>();
            save.barrels = new List<BarrelData>();
            save.printers = new List<PrinterData>();
            save.multiShops = new List<MultiShopData>();
            save.brokenDrones = new List<BrokenDroneData>();

            save.chanceShrines = new List<ShrineChanceData>();
            save.bloodShrines = new List<ShrineBloodData>();
            save.bossShrines = new List<ShrineBossData>();
            save.combatShrines = new List<ShrineCombatData>();
            save.goldshoreShrines = new List<ShrineGoldshoresAccessData>();
            save.healingShrines = new List<ShrineHealingData>();
            save.orderShrines = new List<ShrineRestackData>();

            save.itemDroplets = new List<ItemDropletData>();
            save.portals = new List<PortalData>();

            foreach (var item in NetworkUser.readOnlyInstancesList) {
                save.players.Add(PlayerData.SavePlayer(item));
            }

            foreach (var item in Object.FindObjectsOfType<CharacterMaster>()) {
                if (item.GetBody() != null) {
                    if (!item.GetBody().isPlayerControlled) {
                        save.enemies.Add(EnemyData.SaveEnemy(item));

                    }
                }
            }
            foreach (var item in Object.FindObjectsOfType<ChestBehavior>()) {
                save.chests.Add(ChestData.SaveChest(item));
            }
            foreach (var item in Object.FindObjectsOfType<BarrelInteraction>()) {
                save.barrels.Add(BarrelData.SaveBarrel(item));
            }
            foreach (var item in Object.FindObjectsOfType<ShopTerminalBehavior>()) {
                if (item.name.Contains("Duplicator")) {
                    save.printers.Add(PrinterData.SavePrinter(item));
                }
            }
            foreach (var item in Object.FindObjectsOfType<MultiShopController>()) {
                save.multiShops.Add(MultiShopData.SaveMultiShop(item));
            }
            foreach (var item in Object.FindObjectsOfType<ShrineChanceBehavior>()) {
                save.chanceShrines.Add(ShrineChanceData.SaveShrineChance(item));
            }
            foreach (var item in Object.FindObjectsOfType<ShrineBloodBehavior>()) {
                save.bloodShrines.Add(ShrineBloodData.SaveShrineBlood(item));
            }
            foreach (var item in Object.FindObjectsOfType<ShrineBossBehavior>()) {
                save.bossShrines.Add(ShrineBossData.SaveShrineBoss(item));
            }
            foreach (var item in Object.FindObjectsOfType<ShrineCombatBehavior>()) {
                save.combatShrines.Add(ShrineCombatData.SaveShrineCombat(item));
            }
            foreach (var item in Object.FindObjectsOfType<ShrineHealingBehavior>()) {
                save.healingShrines.Add(ShrineHealingData.SaveShrineHealing(item));
            }
            foreach (var item in Object.FindObjectsOfType<ShrineRestackBehavior>()) {
                save.orderShrines.Add(ShrineRestackData.SaveShrineRestack(item));
            }
            foreach (var item in Object.FindObjectsOfType<PortalStatueBehavior>()) {
                if (item.name.Contains("Goldshores")) {
                    save.goldshoreShrines.Add(ShrineGoldshoresAccessData.SaveShrineGoldshores(item));
                }
            }
            foreach (var item in Object.FindObjectsOfType<SummonMasterBehavior>()) {
                save.brokenDrones.Add(BrokenDroneData.SaveBrokenDrone(item));
            }
            foreach (var item in Object.FindObjectsOfType<GenericPickupController>()) {
                save.itemDroplets.Add(ItemDropletData.SaveItemDroplet(item));
            }
            foreach (var item in Object.FindObjectsOfType<SceneExitController>()) {
                save.portals.Add(PortalData.SavePortal(item));
            }
            if (TeleporterInteraction.instance) {
                save.teleporter = TeleporterData.SaveTeleporter(TeleporterInteraction.instance);
            }

            save.run = RunData.SaveRun(Object.FindObjectOfType<Run>());

            string json = TinyJson.JSONWriter.ToJson(save);
            Debug.Log(json);
            PlayerPrefs.SetString("Save" + saveFile, json);
        }


        public void Load() {
            //Clear gold chests
            foreach (var item in Object.FindObjectsOfType<ChestBehavior>()) {
                Object.Destroy(item.gameObject);
            }


            foreach (var item in chests) {
                item.LoadChest();
            }
            foreach (var item in barrels) {
                item.LoadBarrel();
            }
            foreach (var item in printers) {
                item.LoadPrinter();
            }
            foreach (var item in multiShops) {
                item.LoadMultiShop();
            }
            foreach (var item in chanceShrines) {
                item.LoadShrineChance();
            }
            foreach (var item in bloodShrines) {
                item.LoadShrineBlood();
            }
            foreach (var item in bossShrines) {
                item.LoadShrineBoss();
            }
            foreach (var item in combatShrines) {
                item.LoadShrineCombat();
            }
            foreach (var item in healingShrines) {
                item.LoadShrineHealing();
            }
            foreach (var item in orderShrines) {
                item.LoadShrineRestack();
            }
            foreach (var item in goldshoreShrines) {
                item.LoadShrineGoldshores();
            }
            foreach (var item in brokenDrones) {
                item.LoadBrokenDrone();
            }
            foreach (var item in itemDroplets) {
                item.LoadDroplet();
            }
            foreach (var item in portals) {
                item.LoadPortal();
            }

            teleporter?.LoadTeleporter();

            foreach (var item in players) {
                item.LoadPlayer();
            }
            foreach (var item in enemies) {
                item.LoadEnemy();
            }
        }


    }

    [Serializable]
    public struct SerializableTransform {

        public SerializableTransform(Vector3 position, Quaternion rotation) {
            this.position = new SerializableVector3(position);
            this.rotation = new SerializableQuaternion(rotation);
        }
        public SerializableTransform(Transform transform) {
            this.position = new SerializableVector3(transform.position);
            this.rotation = new SerializableQuaternion(transform.rotation);
        }

        public SerializableVector3 position;
        public SerializableQuaternion rotation;

    }

    [Serializable]
    public struct SerializableVector3 {

        public SerializableVector3(Vector3 position) {
            x = position.x;
            y = position.y;
            z = position.z;
        }

        public Vector3 GetVector3() {
            return new Vector3(x, y, z);
        }

        public float x, y, z;

    }

    [Serializable]
    public struct SerializableQuaternion {

        public SerializableQuaternion(Quaternion position) {
            x = position.x;
            y = position.y;
            z = position.z;
            w = position.w;
        }

        public Quaternion GetQuaternion() {
            return new Quaternion(x, y, z, w);
        }

        public float x, y, z, w;

    }
}
