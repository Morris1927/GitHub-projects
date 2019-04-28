using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace SavedGames.Data
{
    public class RunData
    {

        public string seed;
        public int difficulty;
        public float fixedTime;
        public int stageClearCount;
        public string sceneName;
        public int teamExp;

        public static RunData SaveRun(Run run) {
            RunData runData = new RunData();

            runData.seed = run.seed.ToString();
            runData.difficulty = (int)run.selectedDifficulty;
            runData.fixedTime = run.fixedTime;
            runData.stageClearCount = run.stageClearCount;
            runData.sceneName = Stage.instance.sceneDef.sceneName;

            return runData;
        }


        public void LoadData() {
            TeamManager.instance.GiveTeamExperience(TeamIndex.Player, (ulong)teamExp);
            Run.instance.seed = ulong.Parse(seed);
            Run.instance.selectedDifficulty = (DifficultyIndex) difficulty;
            Run.instance.fixedTime = fixedTime;
            Run.instance.stageClearCount = stageClearCount;

            Run.instance.runRNG = new Xoroshiro128Plus(ulong.Parse(seed));
            Run.instance.nextStageRng = new Xoroshiro128Plus(Run.instance.runRNG.nextUlong);
            Run.instance.stageRngGenerator = new Xoroshiro128Plus(Run.instance.runRNG.nextUlong);

            int dummy;
            for (int i = 0; i < Run.instance.stageClearCount + 1; i++) {
                dummy = (int)Run.instance.stageRngGenerator.nextUlong;
                dummy = Run.instance.nextStageRng.RangeInt(0, 1);
                dummy = Run.instance.nextStageRng.RangeInt(0, 1);
            }

            Run.instance.AdvanceStage(sceneName);
        }

    }
}
