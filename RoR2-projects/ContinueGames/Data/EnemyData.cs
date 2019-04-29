using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace SavedGames.Data
{
    public class EnemyData
    {

        public SerializableTransform transform;

        public string enemyName;

        public int teamIndex;

        public int[] items;
        public int equipmentIndex;

        public float health;
        public float shields;


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

            return enemyData;
        }

        public void LoadEnemy() {
            //SavedGames.instance.StartCoroutine(Test());
            GameObject g = GameObject.Instantiate(MasterCatalog.FindMasterPrefab(enemyName + "Master"));
            NetworkServer.Spawn(g);
            CharacterMaster enemy = g.GetComponent<CharacterMaster>();
            Inventory inventory = enemy.inventory;
            enemy.SpawnBody(BodyCatalog.FindBodyPrefab(enemyName + "Body"), transform.position.GetVector3(), transform.rotation.GetQuaternion());
            HealthComponent healthComponent = enemy.GetBody().healthComponent;

            enemy.teamIndex = TeamIndex.Monster;
            for (int i = 0; i < (int)ItemIndex.Count - 1; i++) {
                inventory.GiveItem((ItemIndex)i, items[i]);
            }
            inventory.SetEquipmentIndex((EquipmentIndex)equipmentIndex);

            healthComponent.health = health;
            healthComponent.shield = shields;

            SavedGames.instance.StartCoroutine(Test(healthComponent));
        }

        IEnumerator Test(HealthComponent healthComponent) {
            yield return new WaitUntil(() => healthComponent.health != health);
            healthComponent.health = health;
            healthComponent.shield = shields;
        }
    }
}
