using RoR2;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace SavedGames.Data {
    public class BazaarPodData {
        private const string Path = "Prefabs/NetworkedObjects/LunarShopTerminal";
        public SerializableTransform transform;

        public bool available;
        public int itemIndex;
        public int cost;

        public static BazaarPodData SavePod(ShopTerminalBehavior pod) {
            var bazaarPodData = new BazaarPodData();
            bazaarPodData.transform = new SerializableTransform(pod.transform);

            var purchaseInteraction = pod.GetComponent<PurchaseInteraction>();

            bazaarPodData.available = purchaseInteraction.available;
            bazaarPodData.itemIndex = (int)pod.CurrentPickupIndex().value;
            bazaarPodData.cost = purchaseInteraction.cost;
            return bazaarPodData;
        }

        public void LoadPod() {
            var gameobject = Object.Instantiate(Resources.Load<GameObject>(Path), transform.position.GetVector3(), transform.rotation.GetQuaternion());
            var pod = gameobject.GetComponent<ShopTerminalBehavior>();
            var purchaseInteraction = gameobject.GetComponent<PurchaseInteraction>();
            NetworkServer.Spawn(gameobject);
            gameobject.transform.localScale = Vector3.one;

            purchaseInteraction.SetAvailable(available);
            purchaseInteraction.cost = cost;
            SavedGames.instance.StartCoroutine(WaitForStart(pod));
        }


        IEnumerator WaitForStart(ShopTerminalBehavior pod) {
            yield return null;

            if (itemIndex >= (int)ItemIndex.Count) {
                pod.SetPickupIndex(new PickupIndex((EquipmentIndex)itemIndex - (int)ItemIndex.Count));
            } else {
                pod.SetPickupIndex(new PickupIndex((ItemIndex)itemIndex));
            }


        }
    }
}