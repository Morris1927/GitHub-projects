using RoR2;
using System;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class ShrineBloodData {
        private const string Path = "SpawnCards/InteractableSpawnCard/iscShrineBlood";
        public SerializableTransform transform;

        public bool available;

        public int purchaseCount;
        public int cost;

        public static ShrineBloodData SaveShrineBlood(ShrineBloodBehavior shrine) {
            var shrineBloodData = new ShrineBloodData();
            var purchaseInteraction = shrine.GetComponent<PurchaseInteraction>();
            shrineBloodData.transform = new SerializableTransform(shrine.transform);
            shrineBloodData.purchaseCount = shrine.GetFieldValue<int>("purchaseCount");
            shrineBloodData.cost = purchaseInteraction.cost;
            shrineBloodData.available = purchaseInteraction.available;

            return shrineBloodData;
        }

        public void LoadShrineBlood() {
            var gameobject = Resources.Load<SpawnCard>(Path).DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
            var shrineBlood = gameobject.GetComponent<ShrineBloodBehavior>();
            var purchaseInteraction = shrineBlood.GetComponent<PurchaseInteraction>();

            shrineBlood.SetFieldValue("purchaseCount", purchaseCount);

            purchaseInteraction.cost = cost;
            purchaseInteraction.SetAvailable(available);
        }

    }
}
