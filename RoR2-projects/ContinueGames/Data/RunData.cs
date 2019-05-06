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
            var runData = new RunData();

            runData.seed = run.seed.ToString();
            runData.difficulty = (int)run.selectedDifficulty;
            runData.fixedTime = run.GetRunStopwatch();
            runData.stageClearCount = run.stageClearCount;
            runData.sceneName = Stage.instance.sceneDef.sceneName;
            runData.teamExp = (int) TeamManager.instance.GetTeamExperience(TeamIndex.Player);

            return runData;
        }


        public void LoadData() {
            var newRun = Run.instance;
            TeamManager.instance.SetTeamLevel(TeamIndex.Player, 0);
            TeamManager.instance.GiveTeamExperience(TeamIndex.Player, (ulong)teamExp);
            TeamManager.instance.SetTeamLevel(TeamIndex.Monster, 1);

            newRun.seed = ulong.Parse(seed);
            newRun.selectedDifficulty = (DifficultyIndex) difficulty;
            newRun.fixedTime = fixedTime;

            var stopwatch = newRun.GetFieldValue<Run.RunStopwatch>("runStopwatch");
            stopwatch.offsetFromFixedTime = 0f;
            stopwatch.isPaused = false;

            newRun.SetFieldValue("runStopwatch", stopwatch);
            

            newRun.runRNG = new Xoroshiro128Plus(ulong.Parse(seed));
            newRun.nextStageRng = new Xoroshiro128Plus(newRun.runRNG.nextUlong);
            newRun.stageRngGenerator = new Xoroshiro128Plus(newRun.runRNG.nextUlong);

            int dummy;
            for (int i = 0; i < stageClearCount; i++) {
                dummy = (int)newRun.stageRngGenerator.nextUlong;
                dummy = newRun.nextStageRng.RangeInt(0, 1);
                dummy = newRun.nextStageRng.RangeInt(0, 1);
            }

            //Clearing drones to avoid them carrying over to the new loaded run
            foreach (var item in TeamComponent.GetTeamMembers(TeamIndex.Player)) {
                var body = item.GetComponent<CharacterBody>();
                if (body) {
                    if (!body.isPlayerControlled) item.GetComponent<HealthComponent>()?.Suicide();
                }
            }

            newRun.AdvanceStage(sceneName);
            newRun.stageClearCount = stageClearCount;
        }


    }
}
