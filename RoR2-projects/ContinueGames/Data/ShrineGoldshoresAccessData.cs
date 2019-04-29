using RoR2;
using System;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class ShrineGoldshoresAccessData {

        public SerializableTransform transform;

        public static ShrineGoldshoresAccessData SaveShrineCombat(ShrineCombatBehavior shrine) {
            ShrineGoldshoresAccessData shrineGoldshoresAccessData = new ShrineGoldshoresAccessData();
            shrineGoldshoresAccessData.transform = new SerializableTransform(shrine.transform);

            return shrineGoldshoresAccessData;
        }

        public void LoadShrineChance() {
            GameObject g = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscShrineChance").DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
            ShrineCombatBehavior shrineChance = g.GetComponent<ShrineCombatBehavior>();
        }

    }
}
