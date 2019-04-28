using RoR2;
using System;
using UnityEngine;

namespace SavedGames {
    [Serializable]
    public class ChestData {

        public SerializableTransform transform;

        public bool isEquipment;
        public int index;

        public int cost;

        public bool open;

        public static ChestData SaveChest(ChestBehavior chest, ref SaveData save) {
            ChestData chestData = new ChestData();
            EntityStateMachine stateMachine = chest.GetComponent<EntityStateMachine>();

            if (stateMachine.state.GetType().IsEquivalentTo(typeof(EntityStates.Barrel.Opened))) {
                return null;
            }
            chestData.open = stateMachine.state.GetType().IsEquivalentTo(typeof(EntityStates.Barrel.Opened)) ? true : false;
            chestData.transform = new SerializableTransform(chest.transform);
            chestData.index = chest.GetFieldValue<PickupIndex>("dropPickup").value;
            chestData.isEquipment = chestData.index > (int)ItemIndex.Count ? true : false;
            chestData.cost = chest.GetComponent<PurchaseInteraction>().cost;
            save.chests.Add(chestData);
            return chestData;
        }

        public void LoadChest() {
            GameObject g = null; //Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscChest1").DoSpawn(item.transform.position.GetVector3(), item.transform.rotation.GetQuaternion());
            ChestBehavior chest = null;// g.GetComponent<ChestBehavior>();
            if (isEquipment) {
                g = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscEquipmentBarrel").DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
                chest = g.GetComponent<ChestBehavior>();
                chest.SetFieldValue("dropPickup", new PickupIndex((EquipmentIndex)index - (int)ItemIndex.Count));
            } else {
                g = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscChest1").DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
                chest = g.GetComponent<ChestBehavior>();
                chest.SetFieldValue("dropPickup", new PickupIndex((ItemIndex)index));
            }
            g.GetComponent<PurchaseInteraction>().cost = cost;
            chest.dropRoller = new UnityEngine.Events.UnityEvent();
        }

    }
}
