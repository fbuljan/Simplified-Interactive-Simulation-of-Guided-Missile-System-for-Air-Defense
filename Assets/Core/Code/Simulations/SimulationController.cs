using UnityEngine;

namespace Simulations
{
    public class SimulationController : MonoBehaviour
    {
        [SerializeField] private KeyCode startSimulationButton = KeyCode.Space;
        [SerializeField] private KeyCode endSimulationButton = KeyCode.Backspace;
        [SerializeField] private GameObject planePrefab;
        [SerializeField] private GameObject missileLauncherPrefab;
        [SerializeField, Range(0.0f, 1.0f)] private float spawningBounds = 0.1f;
        [SerializeField] private float minObjectDistance = 10f;
        [SerializeField] private float maxObjectDistance = 50f;
        [SerializeField] private float minHeight = 5f;
        [SerializeField] private float planeInitialHeight = 1000f;
        private Terrain terrain;
        private bool simulationRunning = false;
        private GameObject plane;
        private GameObject missileLauncher;

        public KeyCode EndSimulationButton => endSimulationButton;
        public KeyCode StartSimulationButton => startSimulationButton;

        private void Start()
        {
            terrain = Terrain.activeTerrain;
        }

        private void Update()
        {
            CheckInput();
        }

        private void CheckInput()
        {
            if (Input.GetKeyDown(startSimulationButton)) StartSimulation();
            if (Input.GetKeyDown(endSimulationButton)) StopSimulation();
        }

        private void StartSimulation()
        {
            if (simulationRunning) return;

            simulationRunning = true;
            InitSimulation();
            Debug.Log("Simulation started.");
        }

        private void InitSimulation()
        {
            Vector3 planePosition = GenerateRandomPositionOnTerrainWithinBounds();
            Vector3 planeTarget = GetOppositePosition(planePosition);
            planeTarget += (planeTarget - planePosition) * 500f;
            plane = Instantiate(planePrefab, planePosition, Quaternion.identity);
            plane.GetComponent<PlaneController>().TargetPosition = planeTarget;
            Vector3 launcherPosition = GenerateLauncherPosition(planePosition);
            missileLauncher = Instantiate(missileLauncherPrefab, launcherPosition, Quaternion.Euler(-90f, 0f, 0f));
            missileLauncher.GetComponent<MissileLauncherController>().Init(plane.transform);
        }

        private Vector3 GenerateRandomPositionOnTerrainWithinBounds()
        {
            TerrainData terrainData = terrain.terrainData;
            float width = terrainData.size.x;
            float length = terrainData.size.z;

            float minX = width * spawningBounds;
            float maxX = width - minX;
            float minZ = length * spawningBounds;
            float maxZ = length - minZ;

            float randomX = Random.Range(minX, maxX);
            float randomZ = Random.Range(minZ, maxZ);
            float y = terrain.SampleHeight(new Vector3(randomX, 0, randomZ));

            return new Vector3(randomX, y + planeInitialHeight, randomZ) + terrain.transform.position;
        }

        private Vector3 GetOppositePosition(Vector3 position)
        {
            TerrainData terrainData = terrain.terrainData;
            float terrainWidth = terrainData.size.x;
            float terrainLength = terrainData.size.z;

            float oppositeX = terrainWidth - position.x;
            float oppositeZ = terrainLength - position.z;
            float y = terrain.SampleHeight(new Vector3(oppositeX, 0, oppositeZ));

            return new Vector3(oppositeX, y + planeInitialHeight, oppositeZ) + terrain.transform.position;
        }

        private Vector3 GenerateLauncherPosition(Vector3 referencePosition)
        {
            float randomDistance = Random.Range(minObjectDistance, maxObjectDistance);
            float randomAngle = Random.Range(0, 360) * Mathf.Deg2Rad;
            Vector3 randomDirection = new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle));
            Vector3 newPosition = referencePosition + randomDirection * randomDistance;
            newPosition = ClampPositionToTerrain(newPosition);
            newPosition.y = terrain.SampleHeight(newPosition) + terrain.transform.position.y;

            if (newPosition.y < minHeight) 
                return GenerateLauncherPosition(referencePosition);

            return newPosition;
        }

        private Vector3 ClampPositionToTerrain(Vector3 position)
        {
            float terrainWidth = terrain.terrainData.size.x;
            float terrainLength = terrain.terrainData.size.z;
            Vector3 terrainPosition = terrain.transform.position;

            position.x = Mathf.Clamp(position.x, terrainPosition.x, terrainPosition.x + terrainWidth);
            position.z = Mathf.Clamp(position.z, terrainPosition.z, terrainPosition.z + terrainLength);

            return position;
        }

        private void StopSimulation()
        {
            if (!simulationRunning) return;

            simulationRunning = false;
            Debug.Log("Simulation ended.");
            Destroy(plane);
            Destroy(missileLauncher);
        }
    }
}