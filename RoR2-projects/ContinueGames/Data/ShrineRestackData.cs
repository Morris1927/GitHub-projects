using RoR2;
using System;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class ShrineRestackData {

        public SerializableTransform transform;

        public bool available;

        public static ShrineRestackData SaveShrineRestack(ShrineRestackBehavior shrine) {
            ShrineRestackData shrineRestackData = new ShrineRestackData();
            shrineRestackData.transform = new SerializableTransform(shrine.transform);
            shrineRestackData.available = shrine.GetComponent<PurchaseInteraction>().available;

            return shrineRestackData;
        }

        public void LoadShrineRestack() {
            GameObject g = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscShrineRestack").DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
            ShrineRestackBehavior shrine = g.GetComponent<ShrineRestackBehavior>();

            shrine.GetComponent<PurchaseInteraction>().SetAvailable(available);

        }

    }
}
