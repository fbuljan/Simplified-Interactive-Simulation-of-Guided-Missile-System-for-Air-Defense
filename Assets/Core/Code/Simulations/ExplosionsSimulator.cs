using UnityEngine;

namespace Simulations
{
    public class ExplosionsSimulator : MonoBehaviour
    {
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField, Min(3f)] private float maxTimeBetweenExplosions = 15f;
        [SerializeField, Min(3f)] private float range = 50f;
        private Terrain terrain;


        private void Start()
        {
            terrain = Terrain.activeTerrain;
            Invoke(nameof(SpawnRandomExplosion), Random.Range(1f, maxTimeBetweenExplosions));
        }

        private void SpawnRandomExplosion()
        {
            Vector2 insideUnitCircle = Random.insideUnitCircle;
            Vector3 randomDirection = new Vector3(insideUnitCircle.x, 0f, insideUnitCircle.y);
            Vector3 randomPosition = transform.position + randomDirection * range;
            randomPosition.y = Terrain.activeTerrain.SampleHeight(randomPosition) + Terrain.activeTerrain.transform.position.y;
            Instantiate(explosionPrefab, randomPosition, Quaternion.identity);

            Invoke(nameof(SpawnRandomExplosion), Random.Range(1f, maxTimeBetweenExplosions));
        }

        private void OnDestroy()
        {
            CancelInvoke(nameof(SpawnRandomExplosion));
        }
    }
}