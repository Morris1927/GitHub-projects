using RoR2;
using System;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class PlayerData {

        public string username;

        public SerializableTransform transform;

        public int money;

        public int[] items;

        public int equipItem0;
        public int equipItem1;
        public int equipItemCount;

        public string characterBodyName;

        public bool alive;

        public static PlayerData SavePlayer(NetworkUser player) {
            PlayerData playerData = new PlayerData();

            Inventory inventory = player.master.inventory;
            playerData.username = player.userName;

            playerData.alive = player.master.alive;

            playerData.transform = new SerializableTransform(player.GetCurrentBody().transform);
            playerData.money = (int)player.master.money;
            playerData.items = new int[(int)ItemIndex.Count - 1];
            for (int i = 0; i < (int)ItemIndex.Count - 1; i++) {
                playerData.items[i] = inventory.GetItemCount((ItemIndex)i);
            }

            playerData.equipItem0 = (int)inventory.GetEquipment(0).equipmentIndex;
            playerData.equipItem1 = (int)inventory.GetEquipment(1).equipmentIndex;
            playerData.equipItemCount = inventory.GetEquipmentSlotCount();

            playerData.characterBodyName = player.master.bodyPrefab.name;

            return playerData;
        }


        public void LoadPlayer() {
            NetworkUser player = SavedGames.GetPlayerFromUsername(username);
            if (player == null) {
                Debug.Log("Could not find player: " + username);
                return;
            }

            Inventory inventory = player.master?.inventory;

            GameObject bodyPrefab = BodyCatalog.FindBodyPrefab(characterBodyName);
            player.master.bodyPrefab = bodyPrefab;
            if (alive) {
                player.master.Respawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
            } else {
                player.GetCurrentBody()?.healthComponent?.Suicide();
            }

            for (int i = 0; i < items.Length - 1; i++) {
                inventory.RemoveItem((ItemIndex)i, int.MaxValue);
                inventory.GiveItem((ItemIndex)i, items[i]);
            }

            inventory.SetEquipmentIndex((EquipmentIndex)equipItem0);
            if (equipItemCount == 2) {
                inventory.SetActiveEquipmentSlot((byte)1);
                inventory.SetEquipmentIndex((EquipmentIndex)equipItem1);
            }
            player.master.money = (uint)money;
            
        }

    }
}
