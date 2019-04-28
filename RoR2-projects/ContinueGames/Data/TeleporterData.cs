using RoR2;
using System;
using System.Reflection;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class TeleporterData {

        public static PropertyInfo getActivationState = typeof(TeleporterInteraction).GetProperty("activationState", BindingFlags.NonPublic | BindingFlags.Instance);


        public SerializableTransform transform;

        public int activationState;

        public float remainingCharge;

        public static TeleporterData SaveTeleporter(TeleporterInteraction teleporter) {
            TeleporterData teleporterData = new TeleporterData();
            teleporterData.transform = new SerializableTransform(teleporter.transform);
            teleporterData.remainingCharge = teleporter.remainingChargeTimer;
            teleporterData.activationState = (int) getActivationState.GetValue(teleporter);

            return teleporterData;
        }

        public void LoadTeleporter() {
            typeof(TeleporterInteraction).GetProperty("instance", BindingFlags.Public | BindingFlags.Static).SetValue(null, null);

            GameObject g = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscTeleporter").DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
            TeleporterInteraction teleporter = g.GetComponent<TeleporterInteraction>();
            teleporter.bossDirector.enabled = false;
            teleporter.remainingChargeTimer = remainingCharge;
            getActivationState.SetValue(teleporter, activationState);
        }
    }
}
