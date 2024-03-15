//using UnityEngine;
//using System.Collections.Generic;
//using Unity.Netcode;

//public class HexagonMapGenerator : NetworkBehaviour
//{
//    public GameObject waterPrefab;
//    public GameObject lowlandPrefab;
//    public GameObject highlandPrefab;
//    public GameObject mountainPrefab;
//    public GameObject crystalPrefab;
//    public GameObject desertPrefab;
//    public GameObject grasslandPrefab;
//    public GameObject treePrefab2;
//    public GameObject palmTreePrefab;
//    public GameObject extraMountainPrefab;
//    public GameObject treePrefab;

//    public int _MapWidth = 100;
//    public int _MapHeight = 100;
//    public float _tileSize = 1f;
//    public float _noiseFrequency = 100f;
//    public float _waterThreshold = 0.2f;
//    public float _desertThreshold = 0.3f;
//    public float _lowlandThreshold = 0.4f;
//    public float _grasslandThreshold = 0.65f;
//    public float _highlandThreshold = 0.8f;
//    public float _mountainThreshold = 1.0f;
//    public float _numCrystals = 15f;
//    private int _noiseSeed = -1;

//    private float _palmTreeSpawnChance = 0.05f; // 5% chance
//    private float _mountainExtraSpawnChance = 0.1f; // 10% chance
//    private GameObject[,] map;

//    private MapManager mapManager;
//    private networkUI networkUI;
//    string serializedMapData;
//    void Awake()
//    {
//        mapManager = GetComponent<MapManager>();
//        networkUI = GetComponent<networkUI>();
//        map = new GameObject[_MapWidth, _MapHeight];
//        networkUI.SubscribeToNetworkEvents();


//    }
//    private void Start()
//    {
//        GetComponentInChildren<SwarmUI>().ShowMap();

//    }
//    private Vector3 GetHexCoords(int x, int z)
//    {
//        float xPos = x * _tileSize * Mathf.Cos(Mathf.Deg2Rad * 30);
//        float zPos = z * _tileSize + (x % 2 == 1 ? _tileSize * 0.5f : 0);

//        return new Vector3(xPos, 0, zPos);
//    }

//    public void GenerateHexagonGrid()
//    {
//        for (int z = 0; z < _MapHeight; z++)
//        {
//            for (int x = 0; x < _MapWidth; x++)
//            {
//                Vector3 hexCoords = GetHexCoords(x, z);

//                if (_noiseSeed == -1)
//                {
//                    _noiseSeed = Random.Range(0, 99999999);
//                }

//                float noiseValue = Mathf.PerlinNoise((hexCoords.x + _noiseSeed) / _noiseFrequency, (hexCoords.z + _noiseSeed) / _noiseFrequency);

//                float rotationAngle = 90f;

//                GameObject tilePrefab = GetTilePrefab(noiseValue, x, z);
//                GameObject instantiatedTile = Instantiate(tilePrefab, hexCoords, Quaternion.Euler(0, rotationAngle, 0));


//                HexNode node = null;

//                instantiatedTile.TryGetComponent<HexNode>(out node);

//                if (node == null)
//                {
//                    node = instantiatedTile.AddComponent<HexNode>();
//                }
//                node.initialize(hexCoords.x, hexCoords.z, x, z);
//                node.type = tilePrefab.name;
//                map[x, z] = instantiatedTile;
//                if (tilePrefab == waterPrefab)
//                {
//                    node.terrainDif = 110;
//                }
//                // Check for special cases
//                if (tilePrefab == desertPrefab && Random.value < _palmTreeSpawnChance)
//                {
//                    // Spawn palm tree on a desert
//                    Instantiate(palmTreePrefab, hexCoords, Quaternion.Euler(0, rotationAngle, 0), instantiatedTile.transform);
//                    node.terrainDif = 115;
//                }
//                else if (tilePrefab == grasslandPrefab)
//                {
//                    GameObject selectedTreePrefab;
//                    float randomValue = Random.value;

//                    if (randomValue < 0.01f)
//                    {
//                        selectedTreePrefab = treePrefab; // 1% chance for treePrefab
//                    }
//                    else if (randomValue < 0.02f)
//                    {
//                        selectedTreePrefab = treePrefab2; // 1% chance for treePrefab2
//                    }
//                    else
//                    {
//                        // Default to no tree 
//                        selectedTreePrefab = null;
//                    }

//                    // Spawn tree on a grassland tile with a 1% chance for treePrefab and 1% chance for treePrefab2
//                    if (selectedTreePrefab != null)
//                    {
//                        Instantiate(selectedTreePrefab, hexCoords, Quaternion.Euler(0, rotationAngle, 0), instantiatedTile.transform);
//                        node.terrainDif = 115;
//                    }
//                }
//                else if (tilePrefab == mountainPrefab && Random.value < _mountainExtraSpawnChance)
//                {
//                    // Spawn tree on a mountain tile with a low percent chance
//                    Instantiate(extraMountainPrefab, hexCoords, Quaternion.Euler(0, rotationAngle, 0), instantiatedTile.transform);
//                    node.terrainDif = 100;
//                }

//            }
//        }
//    }

//    public void GenerateCrystals()
//    {
//        for (int i = 0; i < _numCrystals;)
//        {
//            int x = Random.Range(0, _MapWidth);
//            int z = Random.Range(0, _MapHeight);

//            if (IsCrystalTile(x, z) && !IsMountainTile(x, z))
//            {
//                Vector3 crystalCoords = GetHexCoords(x, z);
//                mapManager.setCrystal(x, z);
//                for (int j = 0; j < 3; j++)
//                {
//                    float yOffset = j * 0.2f;  // Adjust yOffset to the tile height
//                    Instantiate(crystalPrefab, new Vector3(crystalCoords.x, yOffset, crystalCoords.z), Quaternion.Euler(0, 90f, 0));
//                }

//                Instantiate(crystalPrefab, new Vector3(crystalCoords.x, 0.6f, crystalCoords.z), Quaternion.Euler(0, 90f, 0));

//                i++;
//            }
//        }
//    }

//    bool IsMountainTile(int x, int z)
//    {
//        Vector3 hexCoords = GetHexCoords(x, z);
//        float noiseValue = Mathf.PerlinNoise((hexCoords.x + _noiseSeed) / _noiseFrequency, (hexCoords.z + _noiseSeed) / _noiseFrequency);

//        // Adjusted condition to identify mountain tiles
//        return noiseValue >= _highlandThreshold;
//    }

//    bool IsCrystalTile(int x, int z)
//    {
//        Vector3 hexCoords = GetHexCoords(x, z);
//        float noiseValue = Mathf.PerlinNoise((hexCoords.x + _noiseSeed) / _noiseFrequency, (hexCoords.z + _noiseSeed) / _noiseFrequency);

//        // Adjusted condition to allow crystals in both lowland and highland, excluding water and mountains
//        return noiseValue >= _waterThreshold && noiseValue < _highlandThreshold && !IsMountainTile(x, z);
//    }

//    GameObject GetTilePrefab(float noiseValue, int x, int z)
//    {
//        // Check if all neighboring tiles have the same terrain type
//        if (AreAllNeighborsSame(x, z))
//        {
//            return GetExactTilePrefab(noiseValue);
//        }

//        float randomValue = Random.value;

//        if (noiseValue < _waterThreshold)
//        {
//            if (HasWaterNeighbor(x, z))
//                return waterPrefab;

//            return waterPrefab; // Default to water if no blending
//        }
//        else if (noiseValue < _desertThreshold)
//        {
//            if (HasWaterNeighbor(x, z) && randomValue < 0.05f)
//                return waterPrefab;

//            return desertPrefab;
//        }
//        else if (noiseValue < _lowlandThreshold)
//        {
//            if (HasWaterNeighbor(x, z) && randomValue < 0.05f)
//                return waterPrefab;

//            if (HasSameTypeNeighbor(x, z, desertPrefab))
//                return desertPrefab;

//            if (HasSameTypeNeighbor(x, z, lowlandPrefab))
//                return lowlandPrefab;

//            return lowlandPrefab; // Default to lowland
//        }
//        else if (noiseValue < _grasslandThreshold)
//        {
//            if (randomValue < 0.1f)
//                return grasslandPrefab;

//            return grasslandPrefab; // Default to grassland
//        }
//        else if (noiseValue < _highlandThreshold)
//        {
//            if (randomValue < 0.05f)
//                return grasslandPrefab;

//            return highlandPrefab; // Default to highland
//        }
//        else
//        {
//            if (randomValue < 0.1f)
//                return highlandPrefab;

//            return mountainPrefab; // Default to mountain
//        }
//    }

//    bool HasWaterNeighbor(int x, int z)
//    {
//        List<GameObject> waterNeighbors = GetSameTypeNeighbors(x, z, waterPrefab);

//        return waterNeighbors.Count > 0;
//    }

//    bool HasSameTypeNeighbor(int x, int z, GameObject targetPrefab)
//    {
//        List<GameObject> neighbors = GetSameTypeNeighbors(x, z, targetPrefab);

//        return neighbors.Count > 0;
//    }

//    // Helper function to get the same type neighbors of a specific prefab
//    List<GameObject> GetSameTypeNeighbors(int x, int z, GameObject targetPrefab)
//    {
//        List<GameObject> neighbors = new List<GameObject>();

//        for (int i = -1; i <= 1; i++)
//        {
//            for (int j = -1; j <= 1; j++)
//            {
//                int neighborX = x + i;
//                int neighborZ = z + j;

//                // Skip the center tile
//                if (i == 0 && j == 0)
//                    continue;

//                if (!IsWithinMapBounds(neighborX, neighborZ))
//                    continue;

//                GameObject neighborPrefab = GetExactTilePrefabAt(neighborX, neighborZ);
//                if (neighborPrefab != null && neighborPrefab == targetPrefab)
//                {
//                    neighbors.Add(neighborPrefab);
//                }
//            }
//        }

//        return neighbors;
//    }

//    bool AreAllNeighborsSame(int x, int z)
//    {
//        for (int i = -1; i <= 1; i++)
//        {
//            for (int j = -1; j <= 1; j++)
//            {
//                int neighborX = x + i;
//                int neighborZ = z + j;

//                // Skip the center tile
//                if (i == 0 && j == 0)
//                    continue;

//                if (!IsWithinMapBounds(neighborX, neighborZ))
//                    continue;

//                // Check if the neighboring tile has a different terrain type
//                if (GetExactTilePrefabAt(neighborX, neighborZ) != GetExactTilePrefabAt(x, z))
//                    return false;
//            }
//        }

//        return true;
//    }

//    // Helper function to get the exact tile prefab without blending
//    GameObject GetExactTilePrefab(float noiseValue)
//    {
//        if (noiseValue < _waterThreshold)
//        {
//            return waterPrefab;
//        }
//        else if (noiseValue < _desertThreshold)
//        {
//            return desertPrefab;
//        }
//        else if (noiseValue < _lowlandThreshold)
//        {
//            return lowlandPrefab;
//        }
//        else if (noiseValue < _grasslandThreshold)
//        {
//            return grasslandPrefab;
//        }
//        else if (noiseValue < _highlandThreshold)
//        {
//            return highlandPrefab;
//        }
//        else
//        {
//            return mountainPrefab;
//        }
//    }

//    // Helper function to get the exact tile prefab without blending at a specific position
//    GameObject GetExactTilePrefabAt(int x, int z)
//    {
//        Vector3 hexCoords = GetHexCoords(x, z);

//        if (_noiseSeed == -1)
//        {
//            _noiseSeed = Random.Range(0, 99999999);
//        }

//        float noiseValue = Mathf.PerlinNoise((hexCoords.x + _noiseSeed) / _noiseFrequency, (hexCoords.z + _noiseSeed) / _noiseFrequency);
//        return GetExactTilePrefab(noiseValue);
//    }

//    // Helper function to check if a position is within map bounds
//    bool IsWithinMapBounds(int x, int z)
//    {
//        return x >= 0 && x < _MapWidth && z >= 0 && z < _MapHeight;
//    }


//    public void isHostBttnEvent()
//    {
//        if (!NetworkManager.Singleton.IsHost) { Debug.Log("host not started..."); }
//        if (!NetworkManager.Singleton.IsHost)
//        {
//            NetworkManager.Singleton.StartHost();
//            GenerateHexagonGrid();
//            mapManager.initMap(map);
//            GenerateCrystals();


//        }

//        if (NetworkManager.Singleton.IsHost)
//        {
//            SendMapDataToClients();
//            Debug.Log("host started...");
//        }
//    }
//    public void connectClient()
//    {

//        NetworkManager.Singleton.StartClient();
//        GenerateHexagonGrid();
//        mapManager.initMap(map);
//        GenerateCrystals();
//        //mapManager.SerializeMap();


//    }

//    public GameObject GetPrefabByType(string type)
//    {
//        switch (type)
//        {
//            case "Water":
//                return waterPrefab;
//            case "Lowland":
//                return lowlandPrefab;
//            case "Highland":
//                return highlandPrefab;
//            case "Mountain":
//                return mountainPrefab;
//            case "Crystal":
//                return crystalPrefab;
//            case "Desert":
//                return desertPrefab;
//            case "Grassland":
//                return grasslandPrefab;
//            case "TreePrefab2": // Assuming you want to map the treePrefab2 by a unique name
//                return treePrefab2;
//            case "PalmTree":
//                return palmTreePrefab;
//            case "ExtraMountain":
//                return extraMountainPrefab;
//            case "TreePrefab": // Assuming you want to map the treePrefab by a unique name
//                return treePrefab;
//            default:
//                Debug.LogError("Unrecognized tile type: " + type);
//                return null;
//        }
//    }


//    // Call this method to send map data to all clients after generation
//    public void SendMapDataToClients()
//    {
//        if (IsHost)
//        {
//            Debug.Log($"[HexagonMapGenerator] Sending map data to clients.");
//            serializedMapData = mapManager.SerializeMap(); // Ensure serialization happens before sending
//            ReceiveMapDataClientRpc(serializedMapData); // Directly call the ClientRpc to distribute the map data
//        }
//    }

//    //[ServerRpc(RequireOwnership = false)]
//    //void TransmitMapDataToServerRpc(string mapData)
//    //{
//    //    Debug.Log($"[HexagonMapGenerator] Transmitting map data to clients. Map Data Length: {mapData.Length}");

//    //    // Directly call the ClientRpc to relay the data to all clients
//    //   // ReceiveMapDataClientRpc(mapData);
//    //}

//    [ClientRpc]
//    public void ReceiveMapDataClientRpc(string mapData)
//    {
//        // This condition ensures that the host does not try to deserialize the map it already has
//        if (IsClient && !IsHost) // Adjusted for clarity and safety
//        {
//            Debug.Log($"[HexagonMapGenerator] Received map data on client. Map Data Length: {mapData.Length}");
//            GetComponent<MapManager>().DeserializeAndGenerateMap(mapData);
//        }
//    }

//    public void StartClientAndReceiveMap()
//    {
//        if (!NetworkManager.Singleton.IsClient)
//        {
//            // Starts the client
//            NetworkManager.Singleton.StartClient();
//            Debug.Log("[HexagonMapGenerator] Client started. Awaiting connection and map data...");


//            // Register a callback for when the client is connected and ready to receive map data
//            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;


//        }
//    }

//    private void OnClientConnected(ulong clientId)
//    {
//        // Check if the connected client is this instance
//        if (NetworkManager.Singleton.LocalClientId == clientId)
//        {
//            Debug.Log($"[HexagonMapGenerator] Client successfully connected to host. Local Client ID: {clientId}. Waiting for map data...");

//            // The client is now waiting for the map data to be sent from the host.
//            // The map data will be handled by ReceiveMapDataClientRpc method.
//        }
//    }

//    // Remember to unsubscribe from the event when the object is destroyed to prevent memory leaks.
//    void OnDestroy()
//    {
//        if (NetworkManager.Singleton != null)
//        {
//            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
//            Debug.Log("[HexagonMapGenerator] OnDestroy called. Unsubscribing from OnClientConnectedCallback.");

//        }
//    }



//}
