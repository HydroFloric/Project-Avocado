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

    public GameObject basePrefab;
    public GameObject baseWallPrefab;

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
    private int _noiseSeed = -1;

    private float _palmTreeSpawnChance = 0.05f; // 5% chance
    private float _mountainExtraSpawnChance = 0.1f; // 10% chance


    public GameObject[,] tileMap;
    void Start()
    {
        tileMap = new GameObject[_MapWidth, _MapHeight];
        GenerateHexagonGrid();
        GenerateCrystals();
        GenerateBase();
    }

    private Vector3 GetHexCoords(int x, int z)
    {
        float xPos = x * _tileSize * Mathf.Cos(Mathf.Deg2Rad * 30);
        float zPos = z * _tileSize + (x % 2 == 1 ? _tileSize * 0.5f : 0);

        return new Vector3(xPos, 0, zPos);
    }

    void GenerateHexagonGrid()
    {
        for (int z = 0; z < _MapHeight; z++)
        {
            for (int x = 0; x < _MapWidth; x++)
            {
                Vector3 hexCoords = GetHexCoords(x, z);

                if (_noiseSeed == -1)
                {
                    _noiseSeed = Random.Range(0, 99999999);
                }

                float noiseValue = Mathf.PerlinNoise((hexCoords.x + _noiseSeed) / _noiseFrequency, (hexCoords.z + _noiseSeed) / _noiseFrequency);

                float rotationAngle = 90f;

                GameObject tilePrefab = GetTilePrefab(noiseValue, x, z);
                GameObject instantiatedTile = Instantiate(tilePrefab, hexCoords, Quaternion.Euler(0, rotationAngle, 0));

                // Check for special cases
                if (tilePrefab == desertPrefab && Random.value < _palmTreeSpawnChance)
                {
                    // Spawn palm tree on a desert
                    Instantiate(palmTreePrefab, hexCoords, Quaternion.Euler(0, rotationAngle, 0), instantiatedTile.transform);
                }
                else if (tilePrefab == grasslandPrefab)
                {
                    GameObject selectedTreePrefab;
                    float randomValue = Random.value;

                    if (randomValue < 0.01f)
                    {
                        selectedTreePrefab = treePrefab; // 1% chance for treePrefab
                    }
                    else if (randomValue < 0.02f)
                    {
                        selectedTreePrefab = treePrefab2; // 1% chance for treePrefab2
                    }
                    else
                    {
                        // Default to no tree 
                        selectedTreePrefab = null;
                    }

                    if (selectedTreePrefab != null)
                    {
                        Instantiate(selectedTreePrefab, hexCoords, Quaternion.Euler(0, rotationAngle, 0), instantiatedTile.transform);
                    }
                }
                else if (tilePrefab == mountainPrefab && Random.value < _mountainExtraSpawnChance)
                {
                    Instantiate(extraMountainPrefab, hexCoords, Quaternion.Euler(0, rotationAngle, 0), instantiatedTile.transform);
                }
            }
        }
    }

    void GenerateCrystals()
    {
        List<Vector2Int> crystalLocations = new List<Vector2Int>();

        for (int i = 0; i < _numCrystals;)
        {
            int x = Random.Range(2, _MapWidth - 2);  // Ensure at least 2 tiles edges
            int z = Random.Range(2, _MapHeight - 2); 

            Vector2Int crystalLocation = new Vector2Int(x, z);

            
            if (IsCrystalTile(x, z) && !IsMountainTile(x, z) && IsFarFromOtherCrystals(crystalLocation, crystalLocations, 5))
            {
                Vector3 crystalCoords = GetHexCoords(x, z);

                for (int j = 0; j < 3; j++)
                {
                    float yOffset = j * 0.2f;  // Adjust yOffset to the tile height
                    Instantiate(crystalPrefab, new Vector3(crystalCoords.x, yOffset, crystalCoords.z), Quaternion.Euler(0, 90f, 0));
                }

                Instantiate(crystalPrefab, new Vector3(crystalCoords.x, 0.6f, crystalCoords.z), Quaternion.Euler(0, 90f, 0));

                // Add the crystal location to the list
                crystalLocations.Add(crystalLocation);

                i++;
            }
        }
    }

    bool IsFarFromOtherCrystals(Vector2Int currentLocation, List<Vector2Int> otherLocations, int minDistance)
    {
        foreach (Vector2Int location in otherLocations)
        {
            if (Vector2Int.Distance(currentLocation, location) < minDistance)
            {
                return false;  // Current location is too close to another crystal
            }
        }
        return true;  
    }

    bool IsMountainTile(int x, int z)
    {
        Vector3 hexCoords = GetHexCoords(x, z);
        float noiseValue = Mathf.PerlinNoise((hexCoords.x + _noiseSeed) / _noiseFrequency, (hexCoords.z + _noiseSeed) / _noiseFrequency);

        return noiseValue >= _highlandThreshold;
    }

    bool IsCrystalTile(int x, int z)
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

            return lowlandPrefab; // Default to lowland
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

    bool HasSameTypeNeighbor(int x, int z, GameObject prefab)
    {
        // Check neighbors in all directions
        if (IsSameType(x - 1, z, prefab) || // Left
            IsSameType(x + 1, z, prefab) || // Right
            IsSameType(x, z - 1, prefab) || // Down
            IsSameType(x, z + 1, prefab))   // Up
        {
            return true;
        }

        return false;
    }

    bool IsSameType(int x, int z, GameObject prefab)
    {
        // Check if the position is within bounds
        if (IsInBounds(x, z))
        {
            return GetTilePrefabAtPosition(x, z) == prefab;
        }

        return false;
    }

    bool IsInBounds(int x, int z)
    {
        return x >= 0 && x < _MapWidth && z >= 0 && z < _MapHeight;
    }

    GameObject GetTilePrefabAtPosition(int x, int z)
    {
        

        if (IsInBounds(x, z))
        {
            return tileMap[x, z];
        }

        // Return a default value or handle out-of-bounds accordingly
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

                if (!IsWithinMapBounds(neighborX, neighborZ))
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

    bool AreAllNeighborsSame(int x, int z)
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

                if (!IsWithinMapBounds(neighborX, neighborZ))
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



    // Hcheck if a position is within map bounds
    bool IsWithinMapBounds(int x, int z)
    {
        return x >= 0 && x < _MapWidth && z >= 0 && z < _MapHeight;
    }



    void GenerateBase()
    {
        int baseX, baseZ;

        if (FindBaseLocation(out baseX, out baseZ))
        {
            GenerateBaseTile(baseX, baseZ);

            GenerateWallTiles(baseX, baseZ);
        }
        else
        {
            Debug.LogWarning("Unable to find a suitable location for the base.");
        }
    }

    bool FindBaseLocation(out int baseX, out int baseZ)
    {
        // Loop until a suitable location is found
        for (int attempt = 0; attempt < 100; attempt++)
        {
            baseX = Random.Range(3, _MapWidth - 3);
            baseZ = Random.Range(3, _MapHeight - 3);

            if (IsValidBaseLocation(baseX, baseZ))
            {
                return true;
            }
        }

        baseX = baseZ = -1;
        return false;
    }

    bool IsValidBaseLocation(int baseX, int baseZ)
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

                if (!IsWithinMapBounds(x, z) || IsWaterOrMountainOrCrystalTile(x, z))
                {
                    return false;
                }
            }
        }

        return true;
    }


    void GenerateBaseTile(int baseX, int baseZ)
    {
        Vector3 baseCoords = GetHexCoords(baseX, baseZ);

        // Check if a tile already exists at the location
        if (tileMap[baseX, baseZ] != null)
        {
            Destroy(tileMap[baseX, baseZ]);
        }

        GameObject baseTile = Instantiate(basePrefab, baseCoords, Quaternion.Euler(0, 90f, 0));
        tileMap[baseX, baseZ] = baseTile;
    }

    void GenerateWallTiles(int baseX, int baseZ)
    {
        for (int i = 0; i < 6; i++)
        {
            int direction = (i + 1) % 6;

            int wallX, wallZ;
            GetAdjacentTileCoordinates(baseX, baseZ, direction, out wallX, out wallZ);

            // Check if the position is valid for a wall tile
            if (IsWithinMapBounds(wallX, wallZ) && !IsWaterOrMountainOrCrystalTile(wallX, wallZ))
            {
                // Delete existing tile, if any
                if (tileMap[wallX, wallZ] != null)
                {
                    Destroy(tileMap[wallX, wallZ]);
                }

                Vector3 wallCoords = GetHexCoords(wallX, wallZ);

                GameObject wallTile = Instantiate(baseWallPrefab, wallCoords, Quaternion.Euler(0, 90f, 0));
                tileMap[wallX, wallZ] = wallTile;
            }
        }
    }

    void GetAdjacentTileCoordinates(int baseX, int baseZ, int direction, out int adjacentX, out int adjacentZ)
        {
            int[] deltaX = { 1, 1, 0, -1, -1, 0};
            int[] deltaZ = { 0, -1, -1, 0, 1, 1};

            adjacentX = baseX + deltaX[direction];
            adjacentZ = baseZ + deltaZ[direction];
        }

        bool IsWaterOrMountainOrCrystalTile(int x, int z)
        {
            if (!IsWithinMapBounds(x, z))
                return false;

            GameObject tilePrefab = GetTilePrefabAtPosition(x, z);

            // Check if the tile is water, mountain, or crystal
            return tilePrefab == waterPrefab || tilePrefab == mountainPrefab || tilePrefab == crystalPrefab;
        }

}