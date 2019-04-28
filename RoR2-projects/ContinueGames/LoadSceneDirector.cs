using RoR2;
using SavedGames.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace SavedGames
{
    // =========================== DEPRECATED ============================= //

    class LoadSceneDirector : MonoBehaviour {

        //public static FieldInfo getDropPickup = typeof(ChestBehavior).GetField("dropPickup");

        public void PopulateScene(SaveData save) {
            foreach (var item in save.chests) {
                item.LoadChest();
            }
            foreach (var item in save.barrels) {
                item.LoadBarrel();
            }
            foreach (var item in save.printers) {
                item.LoadPrinter();
            }
            save.teleporter.LoadTeleporter();
        }

    }
}
