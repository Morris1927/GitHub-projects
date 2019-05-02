using EntityStates.Interactables.GoldBeacon;
using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SavedGames.Data
{
    public class BeaconData
    {

        public SerializableTransform transform;


        public bool available;
        public int cost;


        public static BeaconData SaveBeacon(PurchaseInteraction beacon) {
            var beaconData = new BeaconData();
            beaconData.transform = new SerializableTransform(beacon.transform);
            beaconData.available = beacon.available;
            beaconData.cost = beacon.cost;

            return beaconData;
        }

        public void LoadBeacon() {
            GameObject g = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscGoldshoresBeacon").DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
            PurchaseInteraction beacon = g.GetComponent<PurchaseInteraction>();
            beacon.available = available;
            beacon.cost = cost;
            GoldshoresMissionController.instance.beaconInstanceList.Add(g);
            if (!available) {
                SavedGames.instance.StartCoroutine(Test(g));//beacon.GetComponent<EntityStateMachine>().SetNextState(new Ready());
            }

        }

        IEnumerator Test(GameObject g) {
            yield return new WaitForSeconds(5f);
            g.GetComponent<EntityStateMachine>().SetNextState(new Ready());
        }
    }
}
