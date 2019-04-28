using RoR2;
using SavedGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace SavedGames
{
    class LoadSceneDirector : MonoBehaviour {

        //public static FieldInfo getDropPickup = typeof(ChestBehavior).GetField("dropPickup");

        public void PopulateScene(SaveData save) {
            foreach (var item in save.chests) {
                item.LoadChest();
            }
            foreach (var item in save.barrels) {
                item.LoadBarrel();
            }
        }

        private static void LoadChests(SaveData save) {

        }

        private static IEnumerator Test(ChestBehavior chest) {
            yield return new WaitForSeconds(5f);

        }

        public GameObject DoSpawn(GameObject prefab, Vector3 position, Quaternion rotation) {
            GameObject g = GameObject.Instantiate(prefab, position, rotation);
            NetworkServer.Spawn(g);
            return g;
        }
    }
}
