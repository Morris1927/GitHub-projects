using System;
using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;

namespace QuickMonsterCreditFix
{
    [BepInPlugin("com.morris1927.QuickMonsterCreditFix", "QuickMonsterCreditFix", "1.0.0")]
    public class QuickMonsterCreditFix : BaseUnityPlugin
    {
        private const int maxMonsterCredits = 14399;

        public void Awake() {
            
            IL.RoR2.CameraRigController.Update += (il) => {
                var c = new ILCursor(il);

                //We use GotoNext to locate the code we want to edit
                //Notice we can specify a block of instructions to match, rather than only a single instruction
                //This is preferable as it is less likely to break something else because of an update

                c.GotoNext(
                    x => x.MatchLdloc(4),      // num14 *= 0.5f;
                    x => x.MatchLdcR4(0.5f),   // 
                    x => x.MatchMul(),         // 
                    x => x.MatchStloc(4),      // 
                    x => x.MatchLdloc(5),      // num15 *= 0.5f;
                    x => x.MatchLdcR4(0.5f),   //
                    x => x.MatchMul(),         //
                    x => x.MatchStloc(5));     //

                //When we GotoNext, the cursor is before the first instruction we match.
                //So we remove the next 8 instructions
                c.RemoveRange(8);
                
            };

            On.RoR2.PositionIndicator.UpdatePositions += (orig, uiCamera) => {
                
                if (uiCamera.cameraRigController.target != null)
                    orig(uiCamera);

            };

            On.RoR2.CombatDirector.Simulate += (orig, self, deltaTime) => {
                self.monsterCredit = Mathf.Min(self.monsterCredit, maxMonsterCredits);
               // if (self.monsterCredit < 0) {
               //     self.monsterCredit = maxMonsterCredits;
               // }
                orig(self, deltaTime);
            };
        }

    }
}
