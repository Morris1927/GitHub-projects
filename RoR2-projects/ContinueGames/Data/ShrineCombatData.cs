using RoR2;
using System;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class ShrineCombatData {

        public SerializableTransform transform;

        public static ShrineCombatData SaveShrineCombat(ShrineCombatBehavior shrine) {
            ShrineCombatData shrineCombatData = new ShrineCombatData();
            shrineCombatData.transform = new SerializableTransform(shrine.transform);

            return shrineCombatData;
        }

        public void LoadShrineCombat() {
            GameObject g = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscShrineCombat").DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
            ShrineCombatBehavior shrineChance = g.GetComponent<ShrineCombatBehavior>();
        }

    }
}
