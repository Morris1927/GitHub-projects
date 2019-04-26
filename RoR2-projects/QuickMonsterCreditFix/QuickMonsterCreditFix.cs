using System;
using BepInEx;
using UnityEngine;

namespace QuickMonsterCreditFix
{
    [BepInPlugin("com.morris1927.QuickMonsterCreditFix", "QuickMonsterCreditFix", "1.0.0")]
    public class QuickMonsterCreditFix : BaseUnityPlugin
    {

        public void Awake() {
            On.RoR2.CombatDirector.AttemptSpawnOnTarget += (orig, self, gameobject) => {
                bool test = orig(self, gameobject);
                return test;
            };
            On.RoR2.CombatDirector.Simulate += (orig, self, deltaTime) => {
                self.monsterCredit = Mathf.Min(self.monsterCredit, 14400);
                orig(self, deltaTime);
            };
        }
    }
}
