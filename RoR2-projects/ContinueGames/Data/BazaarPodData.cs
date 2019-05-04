using RoR2;
using UnityEngine;

namespace SavedGames.Data {
    public class BazaarPodData {

        public SerializableTransform transform;

        public bool available;
        public int itemIndex;
        public int cost;

        public static BazaarPodData SavePod(ShopTerminalBehavior pod) {
            BazaarPodData bazaarPodData = new BazaarPodData();
            bazaarPodData.transform = new SerializableTransform(pod.transform);

            PurchaseInteraction purchaseInteraction = pod.GetComponent<PurchaseInteraction>();

            bazaarPodData.available = purchaseInteraction.available;
            bazaarPodData.itemIndex = (int)pod.CurrentPickupIndex().value;
            bazaarPodData.cost = purchaseInteraction.cost;
            return bazaarPodData;
        }

        public void LoadPod() {
            GameObject g = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/NetworkedObjects/LunarShopTerminal"), transform.position.GetVector3(), transform.rotation.GetQuaternion());
            g.transform.localScale = Vector3.one;
            ShopTerminalBehavior pod = g.GetComponent<ShopTerminalBehavior>();
            if (itemIndex >= (int) ItemIndex.Count) {
                pod.SetPickupIndex(new PickupIndex((EquipmentIndex)itemIndex - (int) ItemIndex.Count));
            } else {
                pod.SetPickupIndex(new PickupIndex((ItemIndex)itemIndex));
            }

            PurchaseInteraction purchaseInteraction = g.GetComponent<PurchaseInteraction>();
            purchaseInteraction.SetAvailable(available);
            purchaseInteraction.cost = cost;
        }

    }
}