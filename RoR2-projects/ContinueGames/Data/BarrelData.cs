using RoR2;
using System;
using System.Reflection;
using UnityEngine;

namespace SavedGames.Data {

    [Serializable]
    public class BarrelData {

        private static FieldInfo getOpened = typeof(BarrelInteraction).GetField("opened", BindingFlags.Instance | BindingFlags.NonPublic);

        public SerializableTransform transform;

        public int goldReward;
        public int expReward;

        public static BarrelData SaveBarrel(BarrelInteraction barrel) {
            BarrelData barrelData = new BarrelData();

            if ((bool) getOpened.GetValue(barrel)) {
                return null;
            }

            barrelData.transform = new SerializableTransform(barrel.transform);
            barrelData.goldReward = barrel.goldReward;
            barrelData.expReward = (int) barrel.expReward;
            return barrelData;
        }

        public void LoadBarrel() {
            GameObject g = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscBarrel1").DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
            BarrelInteraction barrel = g.GetComponent<BarrelInteraction>();
        }
    }
}
