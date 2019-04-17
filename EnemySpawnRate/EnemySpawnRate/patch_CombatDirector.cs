using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
    public class patch_CombatDirector : CombatDirector
    {
        private extern bool orig_AttemptSpawnOnTarget(GameObject spawnTarget);

        private DirectorCard currentMonsterCard;
        private Xoroshiro128Plus rng;
        private EliteIndex currentActiveEliteIndex;
        public BossGroup bossGroup { get; private set; }

        private WeightedSelection<DirectorCard> monsterCards {
            get {
                return ClassicStageInfo.instance.monsterSelection;
            }

        }

        private bool AttemptSpawnOnTarget(GameObject spawnTarget) {
            if (spawnTarget) {
                if (this.currentMonsterCard == null) {
                    this.currentMonsterCard = this.monsterCards.Evaluate(this.rng.nextNormalizedFloat);
                    this.lastAttemptedMonsterCard = this.currentMonsterCard;
                    this.currentActiveEliteIndex = EliteCatalog.eliteList[this.rng.RangeInt(0, EliteCatalog.eliteList.Count)];
                }
                bool flag = !(this.currentMonsterCard.spawnCard as CharacterSpawnCard).noElites;
                float num = CombatDirector.maximumNumberToSpawnBeforeSkipping * (flag ? CombatDirector.eliteMultiplierCost : 1f);
                //this.monsterCredit = Mathf.Min(8000, this.monsterCredit);
                skipSpawnIfTooCheap = false;
                if (this.currentMonsterCard.CardIsValid() && this.monsterCredit >= (float)this.currentMonsterCard.cost && (!this.skipSpawnIfTooCheap || this.monsterCredit <= (float)this.currentMonsterCard.cost * num)) {
                    SpawnCard spawnCard = this.currentMonsterCard.spawnCard;
                    DirectorPlacementRule directorPlacementRule = new DirectorPlacementRule {
                        placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                        spawnOnTarget = spawnTarget.transform,
                        preventOverhead = this.currentMonsterCard.preventOverhead
                    };
                    DirectorCore.GetMonsterSpawnDistance(this.currentMonsterCard.spawnDistance, out directorPlacementRule.minDistance, out directorPlacementRule.maxDistance);
                    directorPlacementRule.minDistance *= this.spawnDistanceMultiplier;
                    directorPlacementRule.maxDistance *= this.spawnDistanceMultiplier;
                    GameObject gameObject = DirectorCore.instance.TrySpawnObject(spawnCard, directorPlacementRule, this.rng);
                    if (gameObject) {
                        int num2 = this.currentMonsterCard.cost;
                        float num3 = 1f;
                        float num4 = 1f;
                        CharacterMaster component = gameObject.GetComponent<CharacterMaster>();
                        GameObject bodyObject = component.GetBodyObject();
                        if (this.isBoss) {
                            if (!this.bossGroup) {
                                GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/BossGroup"));
                                NetworkServer.Spawn(gameObject2);
                                this.bossGroup = gameObject2.GetComponent<BossGroup>();
                                this.bossGroup.dropPosition = this.dropPosition;
                            }
                            this.bossGroup.AddMember(component);
                        }
                        if (flag && (float)num2 * CombatDirector.eliteMultiplierCost <= this.monsterCredit) {
                            num3 = 4.7f;
                            num4 = 2f;
                            component.inventory.SetEquipmentIndex(EliteCatalog.GetEliteDef(this.currentActiveEliteIndex).eliteEquipmentIndex);
                            num2 = (int)((float)num2 * CombatDirector.eliteMultiplierCost);
                        }
                        int num5 = num2;
                        this.monsterCredit -= (float)num5;
                        if (this.isBoss) {
                            int livingPlayerCount = Run.instance.livingPlayerCount;
                            num3 *= Mathf.Pow((float)livingPlayerCount, 1f);
                        }
                        component.inventory.GiveItem(ItemIndex.BoostHp, Mathf.RoundToInt((num3 - 1f) * 10f));
                        component.inventory.GiveItem(ItemIndex.BoostDamage, Mathf.RoundToInt((num4 - 1f) * 10f));
                        DeathRewards component2 = bodyObject.GetComponent<DeathRewards>();
                        if (component2) {
                            component2.expReward = (uint)((float)num2 * this.expRewardCoefficient * Run.instance.compensatedDifficultyCoefficient);
                            component2.goldReward = (uint)((float)num2 * this.expRewardCoefficient * 2f * Run.instance.compensatedDifficultyCoefficient);
                        }
                        if (this.spawnEffectPrefab && NetworkServer.active) {
                            Vector3 origin = gameObject.transform.position;
                            CharacterBody component3 = bodyObject.GetComponent<CharacterBody>();
                            if (component3) {
                                origin = component3.corePosition;
                            }
                            EffectManager.instance.SpawnEffect(this.spawnEffectPrefab, new EffectData {
                                origin = origin
                            }, true);
                        }
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
