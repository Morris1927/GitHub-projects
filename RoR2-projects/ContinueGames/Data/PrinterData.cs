using RoR2;
using System;
using System.Collections;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class PrinterData {

        public SerializableTransform transform;

        public int itemIndex;

        public static PrinterData SavePrinter(ShopTerminalBehavior printer) {
            PrinterData printerData = new PrinterData();
            printerData.transform = new SerializableTransform(printer.transform);
            printerData.itemIndex = (int) printer.CurrentPickupIndex().itemIndex;

            return printerData;
        }

        public void LoadPrinter() {
            GameObject g = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscDuplicator").DoSpawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
            ShopTerminalBehavior printer = g.GetComponent<ShopTerminalBehavior>();
            SavedGames.instance.StartCoroutine(WaitForStart(printer));
        }

        IEnumerator WaitForStart(ShopTerminalBehavior printer) {

            yield return new WaitWhile(() => printer.CurrentPickupIndex().value != -1);
            printer.SetPickupIndex(new PickupIndex((ItemIndex) itemIndex));
        }
    }
}
