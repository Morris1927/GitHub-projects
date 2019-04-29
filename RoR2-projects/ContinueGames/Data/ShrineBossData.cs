using RoR2;
using System;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class ShrineBossData {

        public SerializableTransform transform;

        public static ShrineBossData SaveShrineBoss(ShrineBossBehavior shrine) {
            ShrineBossData shrineBossData = new ShrineBossData();
            shrineBossData.transform = new SerializableTransform(shrine.transform);

            return shrineBossData;
        }

        public void LoadShrineBoss() {
            GameObject g = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscShrineBoss").DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
            ShrineBossBehavior shrineBoss = g.GetComponent<ShrineBossBehavior>();

        }

    }
}
