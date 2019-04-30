using RoR2;
using System;
using System.Collections;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class ChestData {

        public SerializableTransform transform;

        public string name;

        public bool isEquipment;
        public int index;

        public int cost;
        public int costType;

        public bool opened;

        public static ChestData SaveChest(ChestBehavior chest) {
            ChestData chestData = new ChestData();
            EntityStateMachine stateMachine = chest.GetComponent<EntityStateMachine>();

            chestData.name = chest.name.Replace("(Clone)", "");
            chestData.opened = stateMachine.state.GetType().IsEquivalentTo(typeof(EntityStates.Barrel.Opened)) ? true : false;
            chestData.transform = new SerializableTransform(chest.transform);
            chestData.index = chest.GetFieldValue<PickupIndex>("dropPickup").value;
            chestData.isEquipment = chestData.index >= (int)ItemIndex.Count ? true : false;
            chestData.cost = chest.GetComponent<PurchaseInteraction>().cost;
            chestData.costType = (int) chest.GetComponent<PurchaseInteraction>().costType;

            return chestData;
        }

        public void LoadChest() {
            GameObject g = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/isc" + name).DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
            ChestBehavior chest = g.GetComponent<ChestBehavior>();
            if (isEquipment) {
                chest.SetFieldValue("dropPickup", new PickupIndex((EquipmentIndex)index - (int)ItemIndex.Count));
            } else {
                chest.SetFieldValue("dropPickup", new PickupIndex((ItemIndex)index));
            }

            g.GetComponent<PurchaseInteraction>().cost = cost;
            g.GetComponent<PurchaseInteraction>().costType = (CostType) costType;
            chest.dropRoller = new UnityEngine.Events.UnityEvent();
            SavedGames.instance.StartCoroutine(WaitForStart(chest));
        }

        IEnumerator WaitForStart(ChestBehavior chest) {
            yield return null;
            if (opened) {
                chest.Open();
                chest.GetComponent<PurchaseInteraction>().SetAvailable(false);
            }
            chest.transform.position = transform.position.GetVector3();
        }

    }
}
