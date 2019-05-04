using RoR2;
using System;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class ShrineChanceData {
        private const string Path = "SpawnCards/InteractableSpawnCard/iscShrineChance";
        public SerializableTransform transform;

        public bool available;

        public int successfulPurchaseCount;
        public int cost;
        
        public static ShrineChanceData SaveShrineChance(ShrineChanceBehavior shrine) {
            var shrineChanceData = new ShrineChanceData();
            var purchaseInteraction = shrine.GetComponent<PurchaseInteraction>();

            shrineChanceData.transform = new SerializableTransform(shrine.transform);
            shrineChanceData.successfulPurchaseCount = shrine.GetFieldValue<int>("successfulPurchaseCount");
            shrineChanceData.cost = purchaseInteraction.cost;
            shrineChanceData.available = purchaseInteraction.available;

            return shrineChanceData;
        }

        public void LoadShrineChance() {
            var gameobject = Resources.Load<SpawnCard>(Path).DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
            var shrineChance = gameobject.GetComponent<ShrineChanceBehavior>();
            var purchaseInteraction = shrineChance.GetComponent<PurchaseInteraction>();

            shrineChance.SetFieldValue("successfulPurchaseCount", successfulPurchaseCount);

            purchaseInteraction.cost = cost;
            purchaseInteraction.SetAvailable(available);

        }

    }
}
