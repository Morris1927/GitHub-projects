using RoR2;
using System;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class ShrineRestackData {

        public SerializableTransform transform;

        public static ShrineRestackData SaveShrineRestack(ShrineRestackBehavior shrine) {
            ShrineRestackData shrineRestackData = new ShrineRestackData();
            shrineRestackData.transform = new SerializableTransform(shrine.transform);

            return shrineRestackData;
        }

        public void LoadShrineRestack() {
            GameObject g = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscShrineRestack").DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
            ShrineRestackBehavior shrineBoss = g.GetComponent<ShrineRestackBehavior>();

        }

    }
}
