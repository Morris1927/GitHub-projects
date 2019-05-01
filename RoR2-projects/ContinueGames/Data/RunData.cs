using RoR2;
using RoR2.CharacterAI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SavedGames.Data
{
    public class RunData
    {

        public string seed;
        public int difficulty;
        public float fixedTime;
        public float stopwatchTime;
        public int stageClearCount;
        public string sceneName;
        public int teamExp;

        public static RunData SaveRun(Run run) {
            RunData runData = new RunData();

            runData.seed = run.seed.ToString();
            runData.difficulty = (int)run.selectedDifficulty;
            runData.fixedTime = run.GetRunStopwatch();
            //runData.stopwatchTime = run.NetworkrunStopwatch.offsetFromFixedTime;
            runData.stageClearCount = run.stageClearCount;
            runData.sceneName = Stage.instance.sceneDef.sceneName;

            return runData;
        }


        public void LoadData() {
            Run newRun = UnityEngine.Object.FindObjectOfType<Run>();
            TeamManager.instance.GiveTeamExperience(TeamIndex.Player, (ulong)teamExp);
            newRun.seed = ulong.Parse(seed);
            newRun.selectedDifficulty = (DifficultyIndex) difficulty;
            newRun.fixedTime = fixedTime;
            Run.RunStopwatch stopwatchTest = newRun.GetFieldValue<Run.RunStopwatch>("runStopwatch");
            stopwatchTest.offsetFromFixedTime = 0f;// sceneName.Contains("bazaar") ? 0f : fixedTime;
            stopwatchTest.isPaused = false;
            newRun.SetFieldValue("runStopwatch", stopwatchTest);
            

            newRun.runRNG = new Xoroshiro128Plus(ulong.Parse(seed));
            newRun.nextStageRng = new Xoroshiro128Plus(newRun.runRNG.nextUlong);
            newRun.stageRngGenerator = new Xoroshiro128Plus(newRun.runRNG.nextUlong);

            int dummy;
            for (int i = 0; i < stageClearCount; i++) {
                dummy = (int)newRun.stageRngGenerator.nextUlong;
                dummy = newRun.nextStageRng.RangeInt(0, 1);
                dummy = newRun.nextStageRng.RangeInt(0, 1);
            }
            foreach (var item in TeamComponent.GetTeamMembers(TeamIndex.Player)) {
                CharacterBody body = item.GetComponent<CharacterBody>();
                if (body) {
                    if (!body.isPlayerControlled) item.GetComponent<HealthComponent>()?.Suicide();
                }
            }

            newRun.AdvanceStage(sceneName);
            newRun.stageClearCount = stageClearCount;
            //Run.instance.NetworkrunStopwatch.SetFieldValue("offsetFromFixedTime", stopwatchTime);

        }


    }
}
