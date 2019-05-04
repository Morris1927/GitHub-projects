using RoR2;
using System;
using System.Collections;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class BrokenDroneData {
        private const string Path = "SpawnCards/InteractableSpawnCard/isc";
        public SerializableTransform transform;

        public string name;
        public int cost;

        public static BrokenDroneData SaveBrokenDrone(SummonMasterBehavior drone) {
            var brokenDroneData = new BrokenDroneData();
            brokenDroneData.transform = new SerializableTransform(drone.transform);
            brokenDroneData.name = "Broken" + drone.name.Replace("Broken(Clone)", "");
            brokenDroneData.cost = drone.GetComponent<PurchaseInteraction>().cost;

            return brokenDroneData;
        }

        public void LoadBrokenDrone() {
            var gameobject = Resources.Load<SpawnCard>(Path + name).DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
            var drone = gameobject.GetComponent<SummonMasterBehavior>();

            drone.GetComponent<PurchaseInteraction>().cost = cost;

            SavedGames.instance.StartCoroutine(WaitForStart(drone));
        }

        IEnumerator WaitForStart(SummonMasterBehavior drone) {
            yield return null;
            drone.transform.position = transform.position.GetVector3();
        }

    }
}
