using RoR2;

namespace SavedGames.Data {
    public class BazaarPodData {

        public SerializableTransform transform;

        public string name;

        public static BazaarPodData SavePod() {
            BazaarPodData bazaarPodData = new BazaarPodData();

            return bazaarPodData;
        }

        public void Load(ShopTerminalBehavior lunarPod) {
            

        }

    }
}