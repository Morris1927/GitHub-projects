using System;
using System.Collections.Generic;
using System.Text;

namespace ContinueGames {
    [Serializable]
    public struct SaveData {
        public int difficulty;
        public float fixedTime;
        public int stageClearCount;
        public string sceneName;
        public int teamExp;

        public List<PlayerData> playerList;
    }

    [Serializable]
    public struct PlayerData {

        public string steamID;

        public int[] items;

        public int equipItem0;
        public int equipItem1;
        public int equipItemCount;

        public string characterBodyName;


    }
}
