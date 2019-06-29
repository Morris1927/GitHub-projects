using RoR2;
using System;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class ShrineCombatData {
        private const string Path = "SpawnCards/InteractableSpawnCard/iscShrineCombat";
        public SerializableTransform transform;

        public bool available;


        public static ShrineCombatData SaveShrineCombat(ShrineCombatBehavior shrine) {
            var shrineCombatData = new ShrineCombatData();
            var purchaseInteraction = shrine.GetComponent<PurchaseInteraction>();

            shrineCombatData.transform = new SerializableTransform(shrine.transform);
            shrineCombatData.available = purchaseInteraction.available;

            return shrineCombatData;
        }

        public void LoadShrineCombat() {
            var gameobject = Resources.Load<SpawnCard>(Path).DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion(), null);
            var shrine = gameobject.GetComponent<ShrineCombatBehavior>();
            var purchaseInteraction = shrine.GetComponent<PurchaseInteraction>();

            purchaseInteraction.SetAvailable(available);

        }

    }
}
