using RoR2;
using System;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class ShrineHealingData {
        private const string Path = "SpawnCards/InteractableSpawnCard/iscShrineHealing";

        public SerializableTransform transform;

        public int purchaseCount;
        public bool available;


        public static ShrineHealingData SaveShrineHealing(ShrineHealingBehavior shrine) {
            var shrineHealingData = new ShrineHealingData();
            var purchaseInteraction = shrine.GetComponent<PurchaseInteraction>();
            shrineHealingData.transform = new SerializableTransform(shrine.transform);
            shrineHealingData.purchaseCount = shrine.purchaseCount;
            shrineHealingData.available = purchaseInteraction.available;

            return shrineHealingData;
        }

        public void LoadShrineHealing() {
            var gameobject = Resources.Load<SpawnCard>(Path).DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion(), null);
            var shrineHealing = gameobject.GetComponent<ShrineHealingBehavior>();
            var purchaseInteraction = gameobject.GetComponent<PurchaseInteraction>();

            purchaseInteraction.SetAvailable(available);

            for (int i = 0; i < purchaseCount; i++) {
                var interactionDriver = NetworkUser.readOnlyInstancesList[0].master.GetBody().GetComponent<InteractionDriver>();
                shrineHealing.AddShrineStack(interactionDriver.interactor);
            }
        }

    }
}
