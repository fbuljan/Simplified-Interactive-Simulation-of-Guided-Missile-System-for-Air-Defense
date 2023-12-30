using UnityEngine;
using Utilities;

namespace Simulations
{
    public class SimulationController : MonoBehaviour
    {
        [SerializeField] private KeyCode startSimulationButton = KeyCode.Space;
        [SerializeField] private KeyCode endSimulationButton = KeyCode.Backspace;
        [SerializeField] private KeyCode replayButton = KeyCode.R;
        [SerializeField] private KeyCode cameraButton = KeyCode.C;
        [SerializeField] private GameObject planePrefab;
        [SerializeField] private GameObject missileLauncherPrefab;
        [SerializeField] private SoundPlayer backgroundSoundPlayer;
        [SerializeField, Range(0.0f, 1.0f)] private float spawningBounds = 0.1f;
        [SerializeField] private float minObjectDistance = 10f;
        [SerializeField] private float maxObjectDistance = 50f;
        [SerializeField] private float minHeight = 5f;
        [SerializeField] private float planeInitialHeight = 1000f;
        private MissileLauncherController missileLauncherController;
        private PlaneController planeController;
        private Terrain terrain;
        private GameObject plane;
        private GameObject missileLauncher;
        private Camera mainCamera;
        private Vector3 planePositionCache;
        private Vector3 planeTargetCache;
        private Vector3 launcherPositionCache;
        private bool simulationRunning = false;
        private bool replayAvailable = false;
        private int cameraPositionIndex = 0;

        public KeyCode EndSimulationButton => endSimulationButton;
        public KeyCode StartSimulationButton => startSimulationButton;

        private void Start()
        {
            terrain = Terrain.activeTerrain;
            mainCamera = Camera.main;
            ResetCamera();
        }

        private void Update()
        {
            CheckInput();
            ControlCamera();
        }

        private void CheckInput()
        {
            if (Input.GetKeyDown(startSimulationButton)) StartSimulation();
            if (Input.GetKeyDown(endSimulationButton)) StopSimulation(false);
            if (Input.GetKeyDown(cameraButton)) cameraPositionIndex = (cameraPositionIndex + 1) % 3;
            if (Input.GetKeyDown(replayButton) && replayAvailable)
                Simulate(planePositionCache, planeTargetCache, launcherPositionCache);
        }

        private void StartSimulation()
        {
            if (simulationRunning) return;

            backgroundSoundPlayer.PlaySound();
            InitSimulation();
            Debug.Log("Simulation started."); //todo add UI
        }

        private void InitSimulation()
        {
            //todo hide UI
            Vector3 planePosition = GenerateRandomPositionOnTerrainWithinBounds();
            Vector3 planeTarget = GetOppositePosition(planePosition);
            planeTarget += (planeTarget - planePosition) * 100f;
            Vector3 launcherPosition = GenerateLauncherPosition(planePosition);
            SaveCache(planePosition, planeTarget, launcherPosition);
            Simulate(planePosition, planeTarget, launcherPosition);
        }

        private void SaveCache(Vector3 planePosition, Vector3 planeTarget, Vector3 launcherPosition)
        {
            planePositionCache = planePosition;
            planeTargetCache = planeTarget;
            launcherPositionCache = launcherPosition;
        }

        private void Simulate(Vector3 planePosition, Vector3 planeTarget, Vector3 launcherPosition)
        {
            simulationRunning = true;
            replayAvailable = false;
            plane = Instantiate(planePrefab, planePosition, Quaternion.identity);
            planeController = plane.GetComponent<PlaneController>();
            planeController.TargetPosition = planeTarget;
            planeController.OnPlaneCrashed += OnPlaneCrashed;
            missileLauncher = Instantiate(missileLauncherPrefab, launcherPosition, Quaternion.Euler(-90f, 0f, 0f));
            missileLauncherController = missileLauncher.GetComponent<MissileLauncherController>();
            missileLauncherController.Init(plane.transform);
        }

        private void OnPlaneCrashed()
        {
            StopSimulation(true);
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

        private void StopSimulation(bool planeCrashed)
        {
            if (!simulationRunning) return;

            ResetCamera();
            simulationRunning = false;
            backgroundSoundPlayer.StopSound();
            Debug.Log("Simulation ended."); //todo add notifications to ui 
            if (plane) Destroy(plane);
            if (missileLauncher) Destroy(missileLauncher);
            if (planeCrashed) replayAvailable = true; ; //todo add replay text to instructions in this moment
        }

        private void ControlCamera()
        {
            if (planeController != null && planeController.IsHit && cameraPositionIndex != 0)
            {
                mainCamera.transform.parent = null;
                mainCamera.transform.position = planeController.HitPosition;
                mainCamera.transform.LookAt(planeController.transform.position);
                return;
            }

            if (!simulationRunning || cameraPositionIndex == 2 && missileLauncherController.ActiveMissile == null) cameraPositionIndex = 0;

            if (cameraPositionIndex == 0)
            {
                ResetCamera();
                return;
            }

            if (cameraPositionIndex == 1)
            {
                mainCamera.transform.parent = planeController.CameraRoot.transform;
                mainCamera.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                return;
            }
            
            mainCamera.transform.parent = missileLauncherController.ActiveMissile.CameraRoot.transform;
            mainCamera.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        private void ResetCamera()
        {
            mainCamera.transform.parent = null;
            mainCamera.transform.eulerAngles = new Vector3(30, 0, 0);
            mainCamera.transform.position = new Vector3(2040, 1660, -200);
        }
    }
}