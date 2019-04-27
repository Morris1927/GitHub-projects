using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SavedGames {
    [Serializable]
    public struct SaveData {

        public string seed;
        public int difficulty;
        public float fixedTime;
        public int stageClearCount;
        public string sceneName;
        public int teamExp;

        public List<PlayerData> playerList;
    }

    [Serializable]
    public struct PlayerData {

        public string username;

        public SerializableTransform transform;

        public int[] items;

        public int equipItem0;
        public int equipItem1;
        public int equipItemCount;

        public string characterBodyName;


    }

    [Serializable]
    public struct SerializableChest {

    }

    [Serializable]
    public struct SerializableTransform {

        public SerializableTransform(Vector3 position, Quaternion rotation) {
            this.position = new SerializableVector3(position);
            this.rotation = new SerializableQuaternion(rotation);
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
