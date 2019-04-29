using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SavedGames.Data {

    [Serializable]
    public class MultiShopData {

        public static FieldInfo getTerminalGameObjects = typeof(MultiShopController).GetField("terminalGameObjects", BindingFlags.NonPublic | BindingFlags.Instance);

        public SerializableTransform transform;
        public List<int> itemIndexes = new List<int>();
        public List<bool> hidden = new List<bool>();

        public int cost;

        public static MultiShopData SaveMultiShop(MultiShopController multiShop) {
            MultiShopData multiShopData = new MultiShopData();
            multiShopData.transform = new SerializableTransform(multiShop.transform);
            foreach (var item in (GameObject[]) getTerminalGameObjects.GetValue(multiShop)) {
                multiShopData.itemIndexes.Add((int) item.GetComponent<ShopTerminalBehavior>().GetFieldValue<PickupIndex>("pickupIndex").itemIndex);
                multiShopData.hidden.Add((bool)item.GetComponent<ShopTerminalBehavior>().pickupIndexIsHidden);
            }
            multiShopData.cost = multiShop.GetFieldValue<int>("cost");
            return multiShopData;
        }

        public void LoadMultiShop() {
            GameObject g = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscTripleShop").DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
            MultiShopController multiShop = g.GetComponent<MultiShopController>();
            SavedGames.instance.StartCoroutine(WaitForStart(multiShop));
        }

        IEnumerator WaitForStart(MultiShopController multiShop) {
            yield return new WaitUntil(() => getTerminalGameObjects.GetValue(multiShop) != null);
            foreach (var item in (GameObject[]) getTerminalGameObjects.GetValue(multiShop)) {
                item.GetComponent<ShopTerminalBehavior>().SetPickupIndex(new PickupIndex((ItemIndex)itemIndexes[itemIndexes.Count - 1]), hidden[hidden.Count - 1]);
                item.GetComponent<PurchaseInteraction>().cost = cost;

                hidden.RemoveAt(hidden.Count - 1);
                itemIndexes.RemoveAt(itemIndexes.Count -1);
            }
            multiShop.SetFieldValue("cost", cost);
        }

    }
}
