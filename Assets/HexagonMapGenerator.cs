using UnityEngine;
using System.Collections.Generic;

public class HexagonMapGenerator : MonoBehaviour
{


    public GameObject waterPrefab;
    public GameObject lowlandPrefab;
    public GameObject highlandPrefab;
    public GameObject mountainPrefab;
    public GameObject crystalPrefab;
    public GameObject desertPrefab;
    public GameObject grasslandPrefab;
    public GameObject treePrefab2;
    public GameObject palmTreePrefab;
    public GameObject extraMountainPrefab;
    public GameObject treePrefab;
    public GameObject desertPyramidPrefab;
    public GameObject waterRocksPrefab;



    public int _MapWidth = 100;
    public int _MapHeight = 100;
    public float _tileSize = 1f;
    public float _noiseFrequency = 100f;
    public float _waterThreshold = 0.2f;
    public float _desertThreshold = 0.3f;
    public float _lowlandThreshold = 0.4f;
    public float _grasslandThreshold = 0.65f;
    public float _highlandThreshold = 0.8f;
    public float _mountainThreshold = 1.0f;
    public float _numCrystals = 15f;


    private System.Random _random;
    private int _noiseSeed = 420;

    private float _palmTreeSpawnChance = 0.05f; // 5% chance
    private float _mountainExtraSpawnChance = 0.1f; // 10% chance

    public float numElevation = 0.3f;

    public GameObject[,] tileMap;

    private MapManager mapManager;
    void Start()
    {
      

      
        

        /*GenerateHexagonGrid();*/
        GameObject.Find("PlayerManager").GetComponentInChildren<SwarmUI>().ShowMap();
    }
    void Awake()
    {
        _random = new System.Random(_noiseSeed);
        mapManager = GameObject.Find("Manager").GetComponent<MapManager>();
        tileMap = new GameObject[_MapWidth, _MapHeight];
    }

    public Vector3 GetHexCoords(int x, int z)
    {
        float xPos = x * _tileSize * Mathf.Cos(Mathf.Deg2Rad * 30);
        float zPos = z * _tileSize + (x % 2 == 1 ? _tileSize * 0.5f : 0);

        return new Vector3(xPos, 0, zPos);
    }


    public void GenerateHexagonGrid()
    {
        for (int z = 0; z < _MapHeight; z++)
        {
            for (int x = 0; x < _MapWidth; x++)
            {
                Vector3 hexCoords = GetHexCoords(x, z);

                float noiseValue = Mathf.PerlinNoise((hexCoords.x + _noiseSeed) / _noiseFrequency, (hexCoords.z + _noiseSeed) / _noiseFrequency);
                float elevation = CalculateElevation(noiseValue);
                float rotationAngle = 90f;

                GameObject tilePrefab = GetTilePrefab(noiseValue, x, z);
                GameObject instantiatedTile = Instantiate(tilePrefab, new Vector3(hexCoords.x, elevation, hexCoords.z), Quaternion.Euler(0, rotationAngle, 0));

                // Assign the instantiated tile to the tileMap
              

                HexNode node = null;

                instantiatedTile.TryGetComponent<HexNode>(out node);

                if (node == null)
                {
                    node = instantiatedTile.AddComponent<HexNode>();
                }
                node.initialize(hexCoords.x, hexCoords.z, x, z);
                node.type = tilePrefab.name;

                tileMap[x, z] = instantiatedTile;

               
                // Check for special cases
                if (tilePrefab == desertPrefab)
                {
                    // Use deterministic random number for special case probability
                    float randomValue = _random.Next(0, 100) / 100f; // Assuming _random is System.Random
                    if (randomValue < 0.01f) // 0.5% chance for palmTreePrefab
                    {
                        node.terrainDif = 115;
                        // Spawn palm tree on a desert
                        Instantiate(palmTreePrefab, new Vector3(hexCoords.x, elevation, hexCoords.z), Quaternion.Euler(0, rotationAngle, 0), instantiatedTile.transform);
                    }
                    else if (randomValue < 0.02f) // 0.5% chance for desertPyramidPrefab
                    {
                        // Spawn desert pyramid on a desert
                        Instantiate(desertPyramidPrefab, new Vector3(hexCoords.x, elevation, hexCoords.z), Quaternion.Euler(0, rotationAngle, 0), instantiatedTile.transform);
                        node.terrainDif = 115;
                    }

                }
                else if (tilePrefab == waterPrefab)
                {
                    node.terrainDif = 115;
                    // Use deterministic random number for special case probability
                    float randomValue = _random.Next(0, 100) / 100f; // Assuming _random is System.Random
                    if (randomValue < 0.01f) // 1% chance for waterRocksPrefab
                    {
                        // Spawn water rocks on water
                        Instantiate(waterRocksPrefab, new Vector3(hexCoords.x, elevation, hexCoords.z), Quaternion.Euler(0, rotationAngle, 0), instantiatedTile.transform);
                    }
                }
                else if (tilePrefab == grasslandPrefab)
                {
                    GameObject selectedTreePrefab = null;
                    // Use deterministic random number for selecting tree prefab
                    float randomValue = _random.Next(0, 100) / 100f; // Assuming _random is System.Random

                    if (randomValue < 0.01f)
                    {
                        selectedTreePrefab = treePrefab; // 1% chance for treePrefab
                    }
                    else if (randomValue < 0.02f)
                    {
                        selectedTreePrefab = treePrefab2; // 1% chance for treePrefab2
                    }
                    // No need for an 'else' condition here as 'selectedTreePrefab' is already initialized to null

                    if (selectedTreePrefab != null)
                    {
                        Instantiate(selectedTreePrefab, new Vector3(hexCoords.x, elevation, hexCoords.z), Quaternion.Euler(0, rotationAngle, 0), instantiatedTile.transform);
                    }
                }
                else if (tilePrefab == mountainPrefab)
                {
                    // Use deterministic random number for special case probability
                    float randomValue = _random.Next(0, 100) / 100f; // Assuming _random is System.Random
                    if (randomValue < _mountainExtraSpawnChance)
                    {
                        node.terrainDif = 115;
                        // Add 0.5 to the elevation for extraMountainPrefab
                        float extraMountainElevation = elevation + 0.2f;
                        Instantiate(extraMountainPrefab, new Vector3(hexCoords.x, extraMountainElevation, hexCoords.z), Quaternion.Euler(0, rotationAngle, 0), instantiatedTile.transform);
                    }
                }
            }
        }
    }


    public float CalculateElevation(float noiseValue)
    {
        float minY = 0.0f;   // Minimum Y value for the terrain
        float maxY = numElevation;   // Maximum Y value for the terrain

        float elevation = Mathf.Lerp(minY, maxY, noiseValue);

        return elevation;
    }



    public bool IsWaterTile(int x, int z)
    {
        Vector3 hexCoords = GetHexCoords(x, z);
        float noiseValue = Mathf.PerlinNoise((hexCoords.x + _noiseSeed) / _noiseFrequency, (hexCoords.z + _noiseSeed) / _noiseFrequency);

        return noiseValue < _waterThreshold;
    }

    public bool IsMountainTile(int x, int z)
    {
        Vector3 hexCoords = GetHexCoords(x, z);
        float noiseValue = Mathf.PerlinNoise((hexCoords.x + _noiseSeed) / _noiseFrequency, (hexCoords.z + _noiseSeed) / _noiseFrequency);

        return noiseValue >= _highlandThreshold;
    }

    public bool IsCrystalTile(int x, int z)
    {
        Vector3 hexCoords = GetHexCoords(x, z);
        float noiseValue = Mathf.PerlinNoise((hexCoords.x + _noiseSeed) / _noiseFrequency, (hexCoords.z + _noiseSeed) / _noiseFrequency);

        return noiseValue >= _waterThreshold && noiseValue < _highlandThreshold && !IsMountainTile(x, z);
    }

    GameObject GetTilePrefab(float noiseValue, int x, int z)
    {
        // Check if all neighboring tiles have the same terrain type
        if (AreAllNeighborsSame(x, z))
        {
            return GetExactTilePrefab(noiseValue);
        }

        float randomValue = Random.value;

        if (noiseValue < _waterThreshold)
        {
            if (HasSameTypeNeighbor(x, z, desertPrefab) && randomValue < 0.1f)
                return desertPrefab;

            return waterPrefab;
        }
        else if (noiseValue < _desertThreshold)
        {
            if (HasSameTypeNeighbor(x, z, waterPrefab) && randomValue < 0.1f)
                return waterPrefab;

            if (HasSameTypeNeighbor(x, z, lowlandPrefab) && randomValue < 0.1f)
                return lowlandPrefab;

            return desertPrefab;
        }
        else if (noiseValue < _lowlandThreshold)
        {
            if (HasSameTypeNeighbor(x, z, desertPrefab) && randomValue < 0.2f)
                return desertPrefab;

            if (HasSameTypeNeighbor(x, z, grasslandPrefab) && randomValue < 0.2f)
                return grasslandPrefab;

            return lowlandPrefab;
        }
        else if (noiseValue < _grasslandThreshold)
        {
            if (HasSameTypeNeighbor(x, z, lowlandPrefab) && randomValue < 0.1f)
                return lowlandPrefab;

            if (HasSameTypeNeighbor(x, z, highlandPrefab) && randomValue < 0.1f)
                return highlandPrefab;

            return grasslandPrefab; // Default to grassland
        }
        else if (noiseValue < _highlandThreshold)
        {
            if (HasSameTypeNeighbor(x, z, grasslandPrefab) && randomValue < 0.1f)
                return grasslandPrefab;

            return highlandPrefab; // Default to highland
        }
        else
        {
            if (randomValue < 0.1f)
                return highlandPrefab;

            return mountainPrefab; // Default to mountain
        }
    }

    public bool HasSameTypeNeighbor(int x, int z, GameObject prefab)
    {
        // Check neighbors in all directions
        if (IsSameType(x - 1, z, prefab) ||
            IsSameType(x + 1, z, prefab) ||
            IsSameType(x, z - 1, prefab) ||
            IsSameType(x, z + 1, prefab) ||
            IsSameType(x - 1, z + 1, prefab) ||
            IsSameType(x + 1, z - 1, prefab))
        {
            return true;
        }

        return false;
    }

    public bool IsSameType(int x, int z, GameObject prefab)
    {
        // Check if the position is within bounds
        if (IsInBounds(x, z))
        {
            return GetTilePrefabAtPosition(x, z) == prefab;
        }

        return false;
    }

    public bool IsInBounds(int x, int z)
    {
        return x >= 0 && x < _MapWidth && z >= 0 && z < _MapHeight;
    }

    public GameObject GetTilePrefabAtPosition(int x, int z)
    {
        if (IsInBounds(x, z))
        {
            return tileMap[x, z];
        }
        return null;
    }


    List<GameObject> GetSameTypeNeighbors(int x, int z, GameObject targetPrefab)
    {
        List<GameObject> neighbors = new List<GameObject>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int neighborX = x + i;
                int neighborZ = z + j;

                if (i == 0 && j == 0)
                    continue;

                if (!IsInBounds(neighborX, neighborZ))
                    continue;

                GameObject neighborPrefab = GetExactTilePrefabAt(neighborX, neighborZ);
                if (neighborPrefab != null && neighborPrefab == targetPrefab)
                {
                    neighbors.Add(neighborPrefab);
                }
            }
        }

        return neighbors;
    }

    public bool AreAllNeighborsSame(int x, int z)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int neighborX = x + i;
                int neighborZ = z + j;

                // Skip the center tile
                if (i == 0 && j == 0)
                    continue;

                if (!IsInBounds(neighborX, neighborZ))
                    continue;

                // Check if the neighboring tile has a different terrain type
                if (GetExactTilePrefabAt(neighborX, neighborZ) != GetExactTilePrefabAt(x, z))
                    return false;
            }
        }

        return true;
    }

    // Helper function to get the exact tile prefab without blending
    GameObject GetExactTilePrefab(float noiseValue)
    {
        if (noiseValue < _waterThreshold)
        {
            return waterPrefab;
        }
        else if (noiseValue < _desertThreshold)
        {
            return desertPrefab;
        }
        else if (noiseValue < _lowlandThreshold)
        {
            return lowlandPrefab;
        }
        else if (noiseValue < _grasslandThreshold)
        {
            return grasslandPrefab;
        }
        else if (noiseValue < _highlandThreshold)
        {
            return highlandPrefab;
        }
        else
        {
            return mountainPrefab;
        }
    }

    GameObject GetExactTilePrefabAt(int x, int z)
    {
        Vector3 hexCoords = GetHexCoords(x, z);

        if (_noiseSeed == -1)
        {
            _noiseSeed = Random.Range(0, 99999999);
        }

        float noiseValue = Mathf.PerlinNoise((hexCoords.x + _noiseSeed) / _noiseFrequency, (hexCoords.z + _noiseSeed) / _noiseFrequency);
        return GetExactTilePrefab(noiseValue);
    }


    public bool FindBaseLocation(out int baseX, out int baseZ)
    {
        // Initialize deterministic random number generator with noise seed
        System.Random random = new System.Random((int)_noiseSeed);

        // Loop until a suitable location is found
        for (int attempt = 0; attempt < 100; attempt++)
        {
            baseX = random.Next(3, _MapWidth - 3);
            baseZ = random.Next(3, _MapHeight - 3);

            if (IsValidBaseLocation(baseX, baseZ) && IsInBounds(baseX, baseZ))
            {
                return true;
            }
        }

        baseX = baseZ = -1;
        return false;
    }

    public float GetNoiseSeed()
    {
        return _noiseSeed;
    }
    public void SetNoiseSeed(int n)
    {
        _noiseSeed = n;
    }

    public bool IsValidBaseLocation(int baseX, int baseZ)
    {
        // Check if the base location is not on water, mountain, or crystal tiles
        if (IsWaterOrMountainOrCrystalTile(baseX, baseZ))
        {
            return false;
        }

        for (int i = -3; i <= 3; i++)
        {
            for (int j = -3; j <= 3; j++)
            {
                int x = baseX + i;
                int z = baseZ + j;

                if (!IsInBounds(x, z) || IsMountainTile(x, z) || IsWaterTile(x, z))
                {
                    return false;
                }
            }
        }

        return true;
    }



    public bool IsWaterOrMountainOrCrystalTile(int x, int z)
    {
        if (!IsInBounds(x, z))
            return false;

        GameObject tilePrefab = GetTilePrefabAtPosition(x, z);

        // Check if the tile is water, mountain, or crystal
        return tilePrefab == waterPrefab || tilePrefab == mountainPrefab || tilePrefab == crystalPrefab;
    }

    public bool IsCloseToBases(Vector2Int crystalLocation, Vector2Int baseLocation1, Vector2Int baseLocation2, int minDistance)
    {
        if (Vector2Int.Distance(crystalLocation, baseLocation1) < minDistance || Vector2Int.Distance(crystalLocation, baseLocation2) < minDistance)
        {
            return true;  // Crystal is too close to one of the bases
        }
        return false;
    }

  
}


