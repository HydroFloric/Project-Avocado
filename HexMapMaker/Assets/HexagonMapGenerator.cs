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

    void Start()
    {
        GenerateHexagonGrid();
        GenerateCrystals();
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

                    // Spawn tree on a grassland tile with a 1% chance for treePrefab and 1% chance for treePrefab2
                    if (selectedTreePrefab != null)
                    {
                        Instantiate(selectedTreePrefab, hexCoords, Quaternion.Euler(0, rotationAngle, 0), instantiatedTile.transform);
                    }
                }
                else if (tilePrefab == mountainPrefab && Random.value < _mountainExtraSpawnChance)
                {
                    // Spawn tree on a mountain tile with a low percent chance
                    Instantiate(extraMountainPrefab, hexCoords, Quaternion.Euler(0, rotationAngle, 0), instantiatedTile.transform);
                }
            }
        }
    }

    void GenerateCrystals()
    {
        for (int i = 0; i < _numCrystals;)
        {
            int x = Random.Range(0, _MapWidth);
            int z = Random.Range(0, _MapHeight);

            if (IsCrystalTile(x, z) && !IsMountainTile(x, z))
            {
                Vector3 crystalCoords = GetHexCoords(x, z);

                for (int j = 0; j < 3; j++)
                {
                    float yOffset = j * 0.2f;  // Adjust yOffset to the tile height
                    Instantiate(crystalPrefab, new Vector3(crystalCoords.x, yOffset, crystalCoords.z), Quaternion.Euler(0, 90f, 0));
                }

                Instantiate(crystalPrefab, new Vector3(crystalCoords.x, 0.6f, crystalCoords.z), Quaternion.Euler(0, 90f, 0));

                i++;
            }
        }
    }

    bool IsMountainTile(int x, int z)
    {
        Vector3 hexCoords = GetHexCoords(x, z);
        float noiseValue = Mathf.PerlinNoise((hexCoords.x + _noiseSeed) / _noiseFrequency, (hexCoords.z + _noiseSeed) / _noiseFrequency);

        // Adjusted condition to identify mountain tiles
        return noiseValue >= _highlandThreshold;
    }

    bool IsCrystalTile(int x, int z)
    {
        Vector3 hexCoords = GetHexCoords(x, z);
        float noiseValue = Mathf.PerlinNoise((hexCoords.x + _noiseSeed) / _noiseFrequency, (hexCoords.z + _noiseSeed) / _noiseFrequency);

        // Adjusted condition to allow crystals in both lowland and highland, excluding water and mountains
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
            if (HasWaterNeighbor(x, z))
                return waterPrefab;

            return waterPrefab; // Default to water if no blending
        }
        else if (noiseValue < _desertThreshold)
        {
            if (HasWaterNeighbor(x, z) && randomValue < 0.05f)
                return waterPrefab;

            return desertPrefab;
        }
        else if (noiseValue < _lowlandThreshold)
        {
            if (HasWaterNeighbor(x, z) && randomValue < 0.05f)
                return waterPrefab;

            if (HasSameTypeNeighbor(x, z, desertPrefab))
                return desertPrefab;

            if (HasSameTypeNeighbor(x, z, lowlandPrefab))
                return lowlandPrefab;

            return lowlandPrefab; // Default to lowland
        }
        else if (noiseValue < _grasslandThreshold)
        {
            if (randomValue < 0.1f)
                return grasslandPrefab;

            return grasslandPrefab; // Default to grassland
        }
        else if (noiseValue < _highlandThreshold)
        {
            if (randomValue < 0.05f)
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

    bool HasWaterNeighbor(int x, int z)
    {
        List<GameObject> waterNeighbors = GetSameTypeNeighbors(x, z, waterPrefab);

        return waterNeighbors.Count > 0;
    }

    bool HasSameTypeNeighbor(int x, int z, GameObject targetPrefab)
    {
        List<GameObject> neighbors = GetSameTypeNeighbors(x, z, targetPrefab);

        return neighbors.Count > 0;
    }

    // Helper function to get the same type neighbors of a specific prefab
    List<GameObject> GetSameTypeNeighbors(int x, int z, GameObject targetPrefab)
    {
        List<GameObject> neighbors = new List<GameObject>();

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

    // Helper function to get the exact tile prefab without blending at a specific position
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

    // Helper function to check if a position is within map bounds
    bool IsWithinMapBounds(int x, int z)
    {
        return x >= 0 && x < _MapWidth && z >= 0 && z < _MapHeight;
    }
}