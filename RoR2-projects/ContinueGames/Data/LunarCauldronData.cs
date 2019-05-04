using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace SavedGames.Data
{
    public class LunarCauldronData
    {
        private const string Path = "Prefabs/NetworkedObjects/LunarCauldron";
        public SerializableTransform transform;

        public int cost;
        public int costType;

        public int tier;
        public int itemIndex;


        public static LunarCauldronData SaveLunarCauldron(ShopTerminalBehavior lunarCauldron) {
            var lunarCauldronData = new LunarCauldronData();
            var purchaseInteraction = lunarCauldron.GetComponent<PurchaseInteraction>();

            lunarCauldronData.transform = new SerializableTransform(lunarCauldron.transform);
            lunarCauldronData.cost = purchaseInteraction.cost;
            lunarCauldronData.costType = (int) purchaseInteraction.costType;
            lunarCauldronData.tier = (int) lunarCauldron.itemTier;
            lunarCauldronData.itemIndex = (int) lunarCauldron.CurrentPickupIndex().value;

            return lunarCauldronData;
        }

        public void LoadLunarCauldron() {
            var gameobject = GameObject.Instantiate(Resources.Load<GameObject>(Path), transform.position.GetVector3(), transform.rotation.GetQuaternion());
            var lunarCauldron = gameobject.GetComponent<ShopTerminalBehavior>();
            NetworkServer.Spawn(gameobject);
            SavedGames.instance.StartCoroutine(WaitForStart(lunarCauldron));
        }

        IEnumerator WaitForStart(ShopTerminalBehavior lunarCauldron) {
            yield return null;

            var purchaseInteraction = lunarCauldron.GetComponent<PurchaseInteraction>();

            lunarCauldron.itemTier = (ItemTier)tier;
            if (itemIndex >= (int) ItemIndex.Count) {
                lunarCauldron.SetPickupIndex(new PickupIndex((EquipmentIndex) itemIndex - (int) ItemIndex.Count));
            } else {
                lunarCauldron.SetPickupIndex(new PickupIndex((ItemIndex)itemIndex));
            }

            purchaseInteraction.cost = cost;
            purchaseInteraction.costType = (CostType)costType;

        }
    }
}
