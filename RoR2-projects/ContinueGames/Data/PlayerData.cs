using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class PlayerData {

        //public static FieldInfo deployablesList = typeof(CharacterMaster).GetField()


        public string username;

        public SerializableTransform transform;

        public int money;
        public int health;
        public int shields;
        public int infusion;

        public int[] items;
        public List<DeployableData> deployables;

        public int equipItem0;
        public int equipItem1;
        public int equipItemCount;

        public string characterBodyName;

        public bool alive;

        public static PlayerData SavePlayer(NetworkUser player) {
            var playerData = new PlayerData();
            var inventory = player.master.inventory;
            var healthComponent = player.GetCurrentBody().GetComponent<HealthComponent>();


            playerData.deployables = new List<DeployableData>();

            playerData.transform = new SerializableTransform(player.GetCurrentBody().transform);
            playerData.username = player.userName;
            playerData.alive = player.master.alive;
            playerData.money = (int)player.master.money;
            playerData.health = (int)healthComponent.health;
            playerData.shields = (int)healthComponent.shield;
            playerData.infusion = (int)inventory.infusionBonus;

            playerData.items = new int[(int)ItemIndex.Count - 1];
            for (int i = 0; i < (int)ItemIndex.Count - 1; i++) {
                playerData.items[i] = inventory.GetItemCount((ItemIndex)i);
            }

            playerData.equipItem0 = (int)inventory.GetEquipment(0).equipmentIndex;
            playerData.equipItem1 = (int)inventory.GetEquipment(1).equipmentIndex;
            playerData.equipItemCount = inventory.GetEquipmentSlotCount();

            playerData.characterBodyName = player.master.bodyPrefab.name;

            var deployablesList = player.master.GetFieldValue<List<DeployableInfo>>("deployablesList");
            if (deployablesList != null) {
                foreach (var item in deployablesList) {
                    playerData.deployables.Add(DeployableData.SaveDeployable(item.deployable));
                }
            }


            return playerData;
        }


        public void LoadPlayer() {
            var player = SavedGames.GetPlayerFromUsername(username);
            if (player == null) {
                Debug.Log("Could not find player: " + username);
                return;
            }

            var inventory = player.master.inventory;
            var healthComponent = player.GetCurrentBody().GetComponent<HealthComponent>();
            var bodyPrefab = BodyCatalog.FindBodyPrefab(characterBodyName);

            player.master.bodyPrefab = bodyPrefab;
            if (alive) {
                player.master.Respawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
            } else {
                healthComponent?.Suicide();
            }

            for (int i = 0; i < items.Length; i++) {
                inventory.RemoveItem((ItemIndex)i, int.MaxValue);
                inventory.GiveItem((ItemIndex)i, items[i]);
            }

            inventory.SetEquipmentIndex((EquipmentIndex)equipItem0);
            if (equipItemCount == 2) {
                inventory.SetActiveEquipmentSlot((byte)1);
                inventory.SetEquipmentIndex((EquipmentIndex)equipItem1);
            }
            
            inventory.AddInfusionBonus((uint)-inventory.infusionBonus);
            inventory.AddInfusionBonus((uint)infusion);

            player.master.money = (uint)money;


            foreach (var item in deployables) {
                item.LoadDeployable(player.master);
            }

            SavedGames.instance.StartCoroutine(WaitForStart(player));
        }

        IEnumerator WaitForStart(NetworkUser player) {
            yield return null;
            var healthComponent = player.GetCurrentBody().healthComponent;

            healthComponent.health = health;
            healthComponent.shield = shields;
        }
    }
}
