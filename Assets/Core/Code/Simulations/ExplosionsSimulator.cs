using UnityEngine;

namespace Simulations
{
    public class ExplosionsSimulator : MonoBehaviour
    {
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private SimulationController simulationController;
        [SerializeField, Min(3f)] private float maxTimeBetweenExplosions = 15f;
        private Terrain terrain;

        private void OnEnable()
        {
            simulationController.OnSimulationMilestone += OnSimulationMilestone;
            simulationController.OnReplay += OnReplay;
        }

        private void OnDisable()
        {
            simulationController.OnSimulationMilestone -= OnSimulationMilestone;
            simulationController.OnReplay -= OnReplay;
        }

        private void OnSimulationMilestone(bool obj)
        {
            if (obj)
            {
                Invoke(nameof(SpawnRandomExplosion), Random.Range(1f, maxTimeBetweenExplosions));
                return;
            }

            CancelInvoke(nameof(SpawnRandomExplosion));
        }

        private void OnReplay()
        {
            OnSimulationMilestone(true);
        }

        private void Start()
        {
            terrain = Terrain.activeTerrain;
        }

        private void SpawnRandomExplosion()
        {
            Vector3 randomPosition = new Vector3(Random.Range(0, terrain.terrainData.size.x), 0, Random.Range(0, Terrain.activeTerrain.terrainData.size.z)) + Terrain.activeTerrain.transform.position;
            randomPosition.y = Terrain.activeTerrain.SampleHeight(randomPosition) + Terrain.activeTerrain.transform.position.y;
            Instantiate(explosionPrefab, randomPosition, Quaternion.identity);

            Invoke(nameof(SpawnRandomExplosion), Random.Range(1f, maxTimeBetweenExplosions));
        }
    }
}