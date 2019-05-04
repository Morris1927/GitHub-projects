using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates.Missions.Goldshores;

using Object = UnityEngine.Object;
using MonoMod.ModInterop;

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

        public int expReward;
        public int goldReward;

        public bool isBoss;
        

        public static EnemyData SaveEnemy(CharacterMaster enemy) {
            var enemyData = new EnemyData();
            var inventory = enemy.inventory;

            enemyData.transform = new SerializableTransform(enemy.GetBody().transform);
            enemyData.enemyName = enemy.name.Replace("Master(Clone)", "");
            enemyData.teamIndex = (int) enemy.teamIndex;

            enemyData.items = new int[(int) ItemIndex.Count - 1];
            for (int i = 0; i < (int) ItemIndex.Count -1; i++) {
                enemyData.items[i] = inventory.GetItemCount((ItemIndex)i);
            }

            enemyData.equipmentIndex = (int) inventory.GetEquipmentIndex();
            enemyData.health = (float) enemy.GetBody()?.healthComponent?.health;
            enemyData.shields = (float) enemy.GetBody()?.healthComponent?.shield;

            enemyData.expReward = (int)enemy.GetBody().GetComponent<DeathRewards>().expReward;
            enemyData.goldReward = (int)enemy.GetBody().GetComponent<DeathRewards>().goldReward;

            enemyData.isBoss = enemy.isBoss;
            return enemyData;
        }

        public void LoadEnemy() {

            if (enemyName.Contains("EngiTurret") || enemyName.Contains("BeetleGuardAlly") || enemyName.Contains("ShopkeeperMaster")) {
                return;
            }

            var gameobject = GameObject.Instantiate(MasterCatalog.FindMasterPrefab(enemyName + "Master"));
            NetworkServer.Spawn(gameobject);
            var enemy = gameobject.GetComponent<CharacterMaster>();
            var inventory = enemy.inventory;
            if (enemyName == "BeetleQueen")
                enemyName = "BeetleQueen2";

            enemy.SpawnBody(BodyCatalog.FindBodyPrefab(enemyName + "Body"), transform.position.GetVector3(), transform.rotation.GetQuaternion());

            enemy.teamIndex = (TeamIndex) teamIndex;
            for (int i = 0; i < (int)ItemIndex.Count - 1; i++) {
                inventory.GiveItem((ItemIndex)i, items[i]);
            }
            inventory.SetEquipmentIndex((EquipmentIndex)equipmentIndex);

            enemy.isBoss = isBoss;

            if (enemyName.Contains("TitanGold")) {
                var bossFight = new GoldshoresBossfight();
                var bossGroupGameObject = Object.Instantiate(Resources.Load<GameObject>("Prefabs/NetworkedObjects/Bossgroup"));
                var bossGroup = bossGroupGameObject.GetComponent<BossGroup>();

                NetworkServer.Spawn(bossGroupGameObject);

                bossGroup.bossDropChance = 1f;
                bossGroup.dropPosition = GoldshoresMissionController.instance.bossSpawnPosition;

                bossGroup.AddMember(enemy);
                bossFight.SetFieldValue("bossGroup", bossGroup);
                bossFight.SetFieldValue("hasSpawnedBoss", true);
                bossFight.SetFieldValue("bossInstanceBody", enemy.GetBody());

                enemy.GetBody().AddBuff(BuffIndex.Immune);

                GoldshoresMissionController.instance.GetComponent<EntityStateMachine>().SetNextState(bossFight);
            } else if(isBoss) {
                BossGroup.instance.AddMember(enemy);
            }

            SavedGames.instance.StartCoroutine(WaitForStart(enemy));
        }

        IEnumerator WaitForStart(CharacterMaster enemy) {
            yield return null;
            var healthComponent = enemy.GetBody().healthComponent;

            healthComponent.health = health;
            healthComponent.shield = shields;

            var deathRewards = enemy.GetBody().GetComponent<DeathRewards>();

            deathRewards.expReward = (uint) expReward;
            deathRewards.goldReward = (uint) goldReward;
        }
    }
}
