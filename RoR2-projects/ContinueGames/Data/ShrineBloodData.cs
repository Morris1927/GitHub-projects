using RoR2;
using System;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class ShrineBloodData {

        public SerializableTransform transform;

        public bool available;

        public int purchaseCount;
        public int cost;

        public static ShrineBloodData SaveShrineBlood(ShrineBloodBehavior shrine) {
            ShrineBloodData shrineBloodData = new ShrineBloodData();
            shrineBloodData.transform = new SerializableTransform(shrine.transform);
            shrineBloodData.purchaseCount = shrine.GetFieldValue<int>("purchaseCount");
            shrineBloodData.cost = shrine.GetComponent<PurchaseInteraction>().cost;
            shrineBloodData.available = shrine.GetComponent<PurchaseInteraction>().available;

            return shrineBloodData;
        }

        public void LoadShrineBlood() {
            GameObject g = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscShrineBlood").DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
            ShrineBloodBehavior shrineBlood = g.GetComponent<ShrineBloodBehavior>();
            shrineBlood.SetFieldValue("purchaseCount", purchaseCount);
            shrineBlood.GetComponent<PurchaseInteraction>().cost = cost;

            shrineBlood.GetComponent<PurchaseInteraction>().SetAvailable(available);
        }

    }
}
