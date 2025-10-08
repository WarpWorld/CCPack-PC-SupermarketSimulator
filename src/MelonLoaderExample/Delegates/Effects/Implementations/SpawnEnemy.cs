using ConnectorLib.JSON;
using Il2CppActors.Enemies;
using Il2CppAssets.Scripts.Actors.Enemies;
using Il2CppAssets.Scripts.Actors.Player;
using Il2CppAssets.Scripts.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect("spawnEnemy")]
public class SpawnEnemy : Effect
{
    public SpawnEnemy(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    public override EffectResponse Start(EffectRequest request)
    {
        // Get the player
        GameObject player = MyPlayer.Instance.gameObject;
        if (player == null) return EffectResponse.Failure(request.ID, "Player not found.");

        // Get the enemy manager
        EnemyManager enemyManager = EnemyManager.Instance;
        if (enemyManager == null) return EffectResponse.Failure(request.ID, "EnemyManager not found.");

        // Get player position
        Vector3 playerPos = player.transform.position;
        
        // Spawn position near player (random direction, 3-8 units away)
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float spawnDistance = Random.Range(3f, 8f);
        Vector3 spawnPos = playerPos + new Vector3(randomDirection.x, 0, randomDirection.y) * spawnDistance;

        // Get a random enemy type (excluding bosses for this effect)
        EEnemy[] enemyTypes = new[]
        {
            EEnemy.Skeleton,
            EEnemy.GoldenSkeleton,
            EEnemy.XpSkeleton,
            EEnemy.ArmoredSkeleton,
            EEnemy.SkeletonDusty,
            EEnemy.ArmoredSkeletonDusty,
            EEnemy.Ghoul,
            EEnemy.Mummy,
            EEnemy.Slime,
            EEnemy.Goblin,
            EEnemy.GoblinStrong,
            EEnemy.GoblinTank,
            EEnemy.Ghost,
            EEnemy.GreaterGhost,
            EEnemy.SkeletonMage,
            EEnemy.Ent1,
            EEnemy.Ent2,
            EEnemy.Ent3,
            EEnemy.BoomerSpider,
            EEnemy.Golem,
            EEnemy.Bee,
            EEnemy.Scorpion,
            EEnemy.Wisp,
            EEnemy.CactusShooter,
            EEnemy.ScorpionMedium,
            EEnemy.MummyTank,
            EEnemy.MummyAncient,
            EEnemy.Tumblebone,
            EEnemy.Bandit,
            EEnemy.Bush
        };

        EEnemy randomEnemyType = enemyTypes[Random.Range(0, enemyTypes.Length)];

        try
        {
            // Spawn the enemy
            Enemy spawnedEnemy = enemyManager.SpawnBoss(randomEnemyType, 0, EEnemyFlag.None, spawnPos);
            
            if (spawnedEnemy != null)
            {
                CrowdControlMod.Instance.Logger.Msg($"Spawned enemy {randomEnemyType} at position {spawnPos}");
                return EffectResponse.Success(request.ID);
            }

            return EffectResponse.Failure(request.ID, "Failed to spawn enemy.");
        }
        catch (Exception ex)
        {
            CrowdControlMod.Instance.Logger.Error($"Error spawning enemy: {ex.Message}");
            return EffectResponse.Failure(request.ID, $"Error spawning enemy: {ex.Message}");
        }
    }
}
