using RoR2;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace SavedGames.Data {

    [Serializable]
    public class BarrelData {

        private static FieldInfo getOpened = typeof(BarrelInteraction).GetField("opened", BindingFlags.Instance | BindingFlags.NonPublic);

        public SerializableTransform transform;

        public bool opened;

        public int goldReward;
        public int expReward;

        public static BarrelData SaveBarrel(BarrelInteraction barrel) {
            BarrelData barrelData = new BarrelData();

            barrelData.transform = new SerializableTransform(barrel.transform);
            barrelData.goldReward = barrel.goldReward;
            barrelData.expReward = (int) barrel.expReward;
            barrelData.opened = barrel.GetFieldValue<bool>("opened");
            return barrelData;
        }

        public void LoadBarrel() {
            GameObject g = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscBarrel1").DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
            BarrelInteraction barrel = g.GetComponent<BarrelInteraction>();



            SavedGames.instance.StartCoroutine(FixPosition(barrel));
        }

        IEnumerator FixPosition(BarrelInteraction barrel) {
            yield return null; // new WaitUntil(() => barrel.transform.position != transform.position.GetVector3());
            if (opened) {
                barrel.SetFieldValue("opened", opened);
                barrel.GetComponent<EntityStateMachine>().SetNextState(new EntityStates.Barrel.Opening());
            }
            barrel.transform.position = transform.position.GetVector3();
        }
    }
}
