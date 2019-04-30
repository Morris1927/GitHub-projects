using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace SavedGames.Data
{
    [Serializable]
    public class EnemyData
    {

        public SerializableTransform transform;

        public string enemyName;

        public int teamIndex;

        public int[] items;
        public int equipmentIndex;

        public float health;
        public float shields;

        public bool isBoss;
        

        public static EnemyData SaveEnemy(CharacterMaster enemy) {
            EnemyData enemyData = new EnemyData();
            Inventory inventory = enemy.inventory;

            enemyData.transform = new SerializableTransform(enemy.GetBody().transform);
            enemyData.enemyName = enemy.name.Replace("Master(Clone)", "");
            enemyData.teamIndex = (int) enemy.teamIndex;

            enemyData.items = new int[(int) ItemIndex.Count - 1];
            for (int i = 0; i < (int) ItemIndex.Count -1; i++) {
                enemyData.items[i] = inventory.GetItemCount((ItemIndex)i);
            }

            enemyData.equipmentIndex = (int) inventory.GetEquipmentIndex();
            enemyData.health = (float) enemy.GetBody()?.healthComponent?.health;
            enemyData.shields = (float)enemy.GetBody()?.healthComponent?.shield;
            enemyData.isBoss = enemy.isBoss;
            return enemyData;
        }

        public void LoadEnemy() {

            if (enemyName.Contains("EngiTurret") || enemyName.Contains("BeetleGuardAlly")) {
                return;
            }

            GameObject g = GameObject.Instantiate(MasterCatalog.FindMasterPrefab(enemyName + "Master"));
            NetworkServer.Spawn(g);
            CharacterMaster enemy = g.GetComponent<CharacterMaster>();
            Inventory inventory = enemy.inventory;
            if (enemyName == "BeetleQueen")
                enemyName = "BeetleQueen2";

            enemy.SpawnBody(BodyCatalog.FindBodyPrefab(enemyName + "Body"), transform.position.GetVector3(), transform.rotation.GetQuaternion());

            enemy.teamIndex = (TeamIndex) teamIndex;
            for (int i = 0; i < (int)ItemIndex.Count - 1; i++) {
                inventory.GiveItem((ItemIndex)i, items[i]);
            }
            inventory.SetEquipmentIndex((EquipmentIndex)equipmentIndex);

            enemy.isBoss = isBoss;
            if (isBoss) {
                BossGroup.instance.AddMember(enemy);
            }

            SavedGames.instance.StartCoroutine(WaitForStart(enemy));
        }

        IEnumerator WaitForStart(CharacterMaster enemy) {
            yield return null;
            HealthComponent healthComponent = enemy.GetBody().healthComponent;

            healthComponent.health = health;
            healthComponent.shield = shields;
        }

    }
}
