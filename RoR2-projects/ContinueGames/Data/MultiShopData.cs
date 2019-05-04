using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SavedGames.Data {

    [Serializable]
    public class MultiShopData {
        private const string Path = "SpawnCards/InteractableSpawnCard/isc";
        public static FieldInfo getTerminalGameObjects = typeof(MultiShopController).GetField("terminalGameObjects", BindingFlags.NonPublic | BindingFlags.Instance);

        public SerializableTransform transform;

        public string name;

        public List<int> itemIndexes = new List<int>();
        public List<bool> hidden = new List<bool>();

        public int cost;

        public bool available;

        public static MultiShopData SaveMultiShop(MultiShopController multiShop) {
            var multiShopData = new MultiShopData();

            multiShopData.transform = new SerializableTransform(multiShop.transform);
            multiShopData.name = multiShop.name.Replace("(Clone)", "");

            foreach (var item in (GameObject[]) getTerminalGameObjects.GetValue(multiShop)) {
                var shopTerminal = item.GetComponent<ShopTerminalBehavior>();

                multiShopData.itemIndexes.Add((int) shopTerminal.GetFieldValue<PickupIndex>("pickupIndex").itemIndex);
                multiShopData.hidden.Add((bool)  shopTerminal.pickupIndexIsHidden);
            }

            multiShopData.cost = multiShop.GetFieldValue<int>("cost");
            multiShopData.available = multiShop.GetFieldValue<bool>("available");
            
            return multiShopData;
        }

        public void LoadMultiShop() {
            var gameobject = Resources.Load<SpawnCard>(Path + name).DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
            var multiShop = gameobject.GetComponent<MultiShopController>();

            multiShop.Networkavailable = available;

            SavedGames.instance.StartCoroutine(WaitForStart(multiShop));

        }

        IEnumerator WaitForStart(MultiShopController multiShop) {
            yield return null; 

            foreach (var item in (GameObject[]) getTerminalGameObjects.GetValue(multiShop)) {
                var purchaseInteraction = item.GetComponent<PurchaseInteraction>();
                var shopTerminal = item.GetComponent<ShopTerminalBehavior>();

                shopTerminal.SetPickupIndex(new PickupIndex((ItemIndex)itemIndexes[0]), hidden[0]);

                hidden.RemoveAt(0);
                itemIndexes.RemoveAt(0);

                purchaseInteraction.cost = cost;
                purchaseInteraction.SetAvailable(available);
            }

            multiShop.SetFieldValue("cost", cost);
        }

    }
}
