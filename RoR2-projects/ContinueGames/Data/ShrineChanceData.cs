using RoR2;
using System;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class ShrineChanceData {

        public SerializableTransform transform;

        public bool available;

        public int successfulPurchaseCount;
        public int cost;
        
        public static ShrineChanceData SaveShrineChance(ShrineChanceBehavior shrine) {
            ShrineChanceData shrineChanceData = new ShrineChanceData();
            shrineChanceData.transform = new SerializableTransform(shrine.transform);
            shrineChanceData.successfulPurchaseCount = shrine.GetFieldValue<int>("successfulPurchaseCount");
            shrineChanceData.cost = shrine.GetComponent<PurchaseInteraction>().cost;
            shrineChanceData.available = shrine.GetComponent<PurchaseInteraction>().available;
            return shrineChanceData;
        }

        public void LoadShrineChance() {
            GameObject g = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscShrineChance").DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
            ShrineChanceBehavior shrineChance = g.GetComponent<ShrineChanceBehavior>();
            shrineChance.SetFieldValue("successfulPurchaseCount", successfulPurchaseCount);
            g.GetComponent<PurchaseInteraction>().cost = cost;

            g.GetComponent<PurchaseInteraction>().SetAvailable(available);

        }

    }
}
