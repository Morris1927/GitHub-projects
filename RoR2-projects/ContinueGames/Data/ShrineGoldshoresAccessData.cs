using RoR2;
using System;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class ShrineGoldshoresAccessData {
        private const string Path = "SpawnCards/InteractableSpawnCard/iscShrineGoldshoresAccess";
        public SerializableTransform transform;

        public int portalIndex;

        public bool available;
        public int cost;

        public static ShrineGoldshoresAccessData SaveShrineGoldshores(PortalStatueBehavior shrine) {
            var shrineGoldshoresAccessData = new ShrineGoldshoresAccessData();
            var purchaseInteraction = shrine.GetComponent<PurchaseInteraction>();

            shrineGoldshoresAccessData.transform = new SerializableTransform(shrine.transform);
            shrineGoldshoresAccessData.cost = purchaseInteraction.cost;
            shrineGoldshoresAccessData.available = purchaseInteraction.available;

            shrineGoldshoresAccessData.portalIndex = (int) shrine.portalType;

            return shrineGoldshoresAccessData;
        }

        public void LoadShrineGoldshores() {
            var gameobject = Resources.Load<SpawnCard>(Path).DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
            var goldShrine = gameobject.GetComponent<PortalStatueBehavior>();
            var purchaseInteraction = gameobject.GetComponent<PurchaseInteraction>();

            purchaseInteraction.Networkcost = cost;
            purchaseInteraction.SetAvailable(available);

            goldShrine.portalType = (PortalStatueBehavior.PortalType) portalIndex;

        }

    }
}
