using RoR2;
using System;
using System.Collections;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class FanData {
        private const string Path = "Prefabs/NetworkedObjects/HumanFan";
        public SerializableTransform transform;

        public int cost;
        public bool available;

        public SerializableVector3 jumpVelocity;


        public static FanData SaveFan(PurchaseInteraction fan) {
            var fanData = new FanData();
            var jumpVolume = fan.GetComponentInChildren<JumpVolume>(true);

            fanData.transform = new SerializableTransform(fan.transform);

            fanData.cost = fan.cost;
            fanData.available = fan.available;
            fanData.jumpVelocity = new SerializableVector3(jumpVolume.jumpVelocity);

            return fanData;
        }

        public void LoadFan() {
            var gameobject = GameObject.Instantiate(Resources.Load<GameObject>(Path), transform.position.GetVector3(), transform.rotation.GetQuaternion());
            var purchaseInteraction = gameobject.GetComponent<PurchaseInteraction>();
            var jumpVolume = purchaseInteraction.GetComponentInChildren<JumpVolume>(true);

            purchaseInteraction.cost = cost;
            purchaseInteraction.available = available;
            if (!available) {
                var stateMachine = gameobject.GetComponent<EntityStateMachine>();
                stateMachine.SetNextState(new EntityStates.Barrel.ActivateFan());
            }

            jumpVolume.jumpVelocity = jumpVelocity.GetVector3();
        }

    }
}
