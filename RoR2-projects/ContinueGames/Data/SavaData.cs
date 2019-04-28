using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public struct SaveData {


        public RunData run;

        public List<PlayerData> players;
        public List<ChestData> chests;
        public List<BarrelData> barrels;
        public List<ShrineChanceData> chanceShrines;
        public List<ShrineBloodData> bloodShrines;
        public List<PrinterData> printers;
        public List<BrokenDroneData> brokenDrones;
        public List<MultiShopData> multiShops;

        public TeleporterData teleporter;
    }

    [Serializable]
    public class BrokenDroneData {
    }

    [Serializable]
    public struct SerializableTransform {

        public SerializableTransform(Vector3 position, Quaternion rotation) {
            this.position = new SerializableVector3(position);
            this.rotation = new SerializableQuaternion(rotation);
        }
        public SerializableTransform(Transform transform) {
            this.position = new SerializableVector3(transform.position);
            this.rotation = new SerializableQuaternion(transform.rotation);
        }

        public SerializableVector3 position;
        public SerializableQuaternion rotation;

    }

    [Serializable]
    public struct SerializableVector3 {

        public SerializableVector3(Vector3 position) {
            x = position.x;
            y = position.y;
            z = position.z;
        }

        public Vector3 GetVector3() {
            return new Vector3(x, y, z);
        }

        public float x, y, z;

    }

    [Serializable]
    public struct SerializableQuaternion {

        public SerializableQuaternion(Quaternion position) {
            x = position.x;
            y = position.y;
            z = position.z;
            w = position.w;
        }

        public Quaternion GetQuaternion() {
            return new Quaternion(x, y, z, w);
        }

        public float x, y, z, w;

    }
}
