using RoR2;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

namespace SavedGames.Data {
    [Serializable]
    public class TeleporterData {

        public static PropertyInfo getActivationState = typeof(TeleporterInteraction).GetProperty("activationState", BindingFlags.NonPublic | BindingFlags.Instance);


        public SerializableTransform transform;

        public int activationState;

        public float remainingCharge;

        public int bossShrineStacks;
        public bool blueOrb;
        public bool goldOrb;
        public bool celestialOrb;


        public static TeleporterData SaveTeleporter(TeleporterInteraction teleporter) {
            TeleporterData teleporterData = new TeleporterData();
            teleporterData.transform = new SerializableTransform(teleporter.transform);
            teleporterData.remainingCharge = teleporter.remainingChargeTimer;
            teleporterData.activationState = (int) getActivationState.GetValue(teleporter);
            teleporterData.bossShrineStacks = teleporter.shrineBonusStacks;
            teleporterData.blueOrb = teleporter.Network_shouldAttemptToSpawnShopPortal;
            teleporterData.goldOrb = teleporter.Network_shouldAttemptToSpawnGoldshoresPortal;
            teleporterData.celestialOrb = teleporter.Network_shouldAttemptToSpawnMSPortal;

            return teleporterData;
        }

        public void LoadTeleporter() {
            typeof(TeleporterInteraction).GetProperty("instance", BindingFlags.Public | BindingFlags.Static).SetValue(null, null);

            GameObject g = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscTeleporter").DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
            TeleporterInteraction teleporter = g.GetComponent<TeleporterInteraction>();

            if (activationState == 2) {
                BossGroup bossGroup = GameObject.Instantiate(Resources.Load<BossGroup>("Prefabs/NetworkedObjects/BossGroup"));
                NetworkServer.Spawn(bossGroup.gameObject);
                bossGroup.dropPosition = teleporter.bossDirector.dropPosition;
                teleporter.bossDirector.SetProperyValue("bossGroup", bossGroup);

            }
            if (activationState >= 2) {
                teleporter.bossDirector.monsterCredit = float.MinValue;
                activationState = 2;
            }
            teleporter.remainingChargeTimer = remainingCharge;
            getActivationState.SetValue(teleporter, activationState);
            teleporter.SetFieldValue("previousActivationState", 1);
            SavedGames.instance.StartCoroutine(Test(teleporter));
        }

        IEnumerator Test(TeleporterInteraction teleporter) {
            yield return null;
            teleporter.shrineBonusStacks = bossShrineStacks;
            teleporter.Network_shouldAttemptToSpawnShopPortal = blueOrb;
            teleporter.Network_shouldAttemptToSpawnGoldshoresPortal = goldOrb;
            teleporter.Network_shouldAttemptToSpawnMSPortal = celestialOrb;

        }
    }
}
