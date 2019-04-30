using RoR2;
using System;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class ShrineGoldshoresAccessData {

        public SerializableTransform transform;

        public int portalIndex;

        public bool available;
        public int cost;

        public static ShrineGoldshoresAccessData SaveShrineGoldshores(PortalStatueBehavior shrine) {
            ShrineGoldshoresAccessData shrineGoldshoresAccessData = new ShrineGoldshoresAccessData();
            shrineGoldshoresAccessData.transform = new SerializableTransform(shrine.transform);
            shrineGoldshoresAccessData.cost = shrine.GetComponent<PurchaseInteraction>().cost;
            shrineGoldshoresAccessData.available = shrine.GetComponent<PurchaseInteraction>().available;

            shrineGoldshoresAccessData.portalIndex = (int) shrine.portalType;

            return shrineGoldshoresAccessData;
        }

        public void LoadShrineGoldshores() {
            GameObject g = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscShrineGoldshoresAccess").DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
            PortalStatueBehavior goldShrine = g.GetComponent<PortalStatueBehavior>();

            goldShrine.GetComponent<PurchaseInteraction>().cost = cost;
            goldShrine.portalType = (PortalStatueBehavior.PortalType) portalIndex;


        }

    }
}
