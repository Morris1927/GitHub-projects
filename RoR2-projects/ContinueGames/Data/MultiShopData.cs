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

        public string name;

        public List<int> itemIndexes = new List<int>();
        public List<bool> hidden = new List<bool>();

        public int cost;

        public bool available;

        public static MultiShopData SaveMultiShop(MultiShopController multiShop) {
            MultiShopData multiShopData = new MultiShopData();
            multiShopData.transform = new SerializableTransform(multiShop.transform);
            multiShopData.name = multiShop.name.Replace("(Clone)", "");

            foreach (var item in (GameObject[]) getTerminalGameObjects.GetValue(multiShop)) {
                multiShopData.itemIndexes.Add((int) item.GetComponent<ShopTerminalBehavior>().GetFieldValue<PickupIndex>("pickupIndex").itemIndex);
                multiShopData.hidden.Add((bool)item.GetComponent<ShopTerminalBehavior>().pickupIndexIsHidden);
            }
            multiShopData.cost = multiShop.GetFieldValue<int>("cost");
            multiShopData.available = multiShop.GetFieldValue<bool>("available");
            
            return multiShopData;
        }

        public void LoadMultiShop() {
            GameObject g = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/isc" + name).DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
            MultiShopController multiShop = g.GetComponent<MultiShopController>();
            multiShop.Networkavailable = available;
            SavedGames.instance.StartCoroutine(WaitForStart(multiShop));
        }

        IEnumerator WaitForStart(MultiShopController multiShop) {
            yield return null; 
            foreach (var item in (GameObject[]) getTerminalGameObjects.GetValue(multiShop)) {
                item.GetComponent<ShopTerminalBehavior>().SetPickupIndex(new PickupIndex((ItemIndex)itemIndexes[0]), hidden[0]);
                item.GetComponent<PurchaseInteraction>().cost = cost;

                hidden.RemoveAt(0);
                itemIndexes.RemoveAt(0);
                item.GetComponent<PurchaseInteraction>().SetAvailable(available);
            }
            multiShop.SetFieldValue("cost", cost);
        }

    }
}
