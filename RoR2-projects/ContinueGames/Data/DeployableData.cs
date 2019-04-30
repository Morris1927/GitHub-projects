using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace SavedGames.Data
{
    [Serializable]
    public class DeployableData
    {

        public SerializableTransform transform;

        public string name;


        public static DeployableData SaveDeployable(Deployable deployable) {
            DeployableData deployableData = new DeployableData();
            deployableData.transform = new SerializableTransform(deployable.transform);
            deployableData.name = deployable.name.Replace("(Clone)", "");


            return deployableData;
        }

        public void LoadDeployable(CharacterMaster playerMaster) {
            switch (name) {
                case "BeetleGuardAllyBody": {
                        CharacterMaster beetleMaster = GameObject.Instantiate(MasterCatalog.FindMasterPrefab("BeetleGuardAllyMaster")).GetComponent<CharacterMaster>();
                        NetworkServer.Spawn(beetleMaster.gameObject);
                        CharacterBody beetleBody = beetleMaster.SpawnBody(BodyCatalog.FindBodyPrefab("BeetleGuardAllyBody"), transform.position.GetVector3(), transform.rotation.GetQuaternion());
                        Inventory inventory = beetleMaster.inventory;
                        inventory.GiveItem(ItemIndex.BoostDamage, 30);
                        inventory.GiveItem(ItemIndex.BoostHp, 10);
                        playerMaster.AddDeployable(beetleBody.GetComponent<Deployable>(), DeployableSlot.BeetleGuardAlly);
                        beetleMaster.teamIndex = TeamIndex.Player;

                        break;
                    }
                case "EngiTurretMaster": {
                        CharacterMaster turretMaster = GameObject.Instantiate(MasterCatalog.FindMasterPrefab("EngiTurretMaster")).GetComponent<CharacterMaster>();
                        NetworkServer.Spawn(turretMaster.gameObject);
                        Inventory inventory = turretMaster.inventory;
                        inventory.CopyItemsFrom(playerMaster.inventory);
                        inventory.ResetItem(ItemIndex.WardOnLevel);
                        inventory.ResetItem(ItemIndex.BeetleGland);
                        inventory.ResetItem(ItemIndex.CrippleWardOnLevel);

                        Deployable deployable = turretMaster.gameObject.AddComponent<Deployable>();
                        deployable.onUndeploy = new UnityEngine.Events.UnityEvent();
                        deployable.onUndeploy.AddListener(new UnityEngine.Events.UnityAction(turretMaster.TrueKill));
                        playerMaster.AddDeployable(deployable, DeployableSlot.EngiTurret);
                        CharacterBody turretBody = turretMaster.SpawnBody(BodyCatalog.FindBodyPrefab("EngiTurretBody"), transform.position.GetVector3(), transform.rotation.GetQuaternion());
                        //playerMaster.AddDeployable(turretBody.add<Deployable>(), DeployableSlot.EngiTurret);
                        turretMaster.teamIndex = TeamIndex.Player;

                        break;
                    }
            }
        }
    }
}
