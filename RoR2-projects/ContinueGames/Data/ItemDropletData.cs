using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;

namespace SavedGames.Data
{
    [Serializable]
    public class ItemDropletData
    {
        public SerializableTransform transform;

        public int itemIndex;
        public bool isEquipment;

        public static ItemDropletData SaveItemDroplet(GenericPickupController pickupDroplet) {
            ItemDropletData itemDropletData = new ItemDropletData();
            itemDropletData.transform = new SerializableTransform(pickupDroplet.transform);
            itemDropletData.itemIndex = (int) pickupDroplet.pickupIndex.value;
            itemDropletData.isEquipment = pickupDroplet.pickupIndex.value >= (int) ItemIndex.Count;

            return itemDropletData;
        }


        public void LoadDroplet() {
            if (isEquipment) {
                PickupDropletController.CreatePickupDroplet(new PickupIndex((EquipmentIndex)(itemIndex - ItemIndex.Count)), transform.position.GetVector3(), Vector3.zero);
            } else {
                PickupDropletController.CreatePickupDroplet(new PickupIndex((ItemIndex)itemIndex), transform.position.GetVector3(), Vector3.zero);
            }
        }
    }
}
