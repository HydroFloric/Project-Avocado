using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBaseGenerator : MonoBehaviour
{
    [SerializeField]
    HexagonMapGenerator hexagonMapGenerator;
    MapManager mapManager;
    public GameObject baseWallPrefab;
    public GameObject basePrefab;

    public GameObject crystalBasePrefab;
    public GameObject crystalBaseWall;

    private int baseX1, baseZ1;
    private int baseX2, baseZ2;

    private bool centerBaseSpawned = false;
    private System.Random random;

    public void init()
    {
        hexagonMapGenerator = GetComponent<HexagonMapGenerator>();
        mapManager = GameObject.Find("Manager").GetComponent<MapManager>();
        // Initialize deterministic random number generator with noise seed
        random = new System.Random((int)hexagonMapGenerator.GetNoiseSeed());
        GenerateBases();
    }

    void GenerateBases()
    {
        // Try to find valid base locations
        if (TryFindValidBaseLocations())
        {
            // Spawn the first normal base
            SpawnNormalBase(baseX1, baseZ1);

            // Symmetrically find the location for the second normal base
            baseX2 = hexagonMapGenerator._MapWidth - 1 - baseX1;
            baseZ2 = hexagonMapGenerator._MapHeight - 1 - baseZ1;

            SpawnBaseWalls(baseX1, baseZ1);
            SpawnCrystalBaseWalls(baseX2, baseZ2);
        }
    }

    bool TryFindValidBaseLocations()
    {
        // Try to find a valid location for the first base
        if (hexagonMapGenerator.FindBaseLocation(out baseX1, out baseZ1))
        {
            // Check if the symmetrical location is also valid
            int symmetricalX = hexagonMapGenerator._MapWidth - 1 - baseX1;
            int symmetricalZ = hexagonMapGenerator._MapHeight - 1 - baseZ1;

            if (hexagonMapGenerator.IsValidBaseLocation(symmetricalX, symmetricalZ))
            {
                // Both locations are valid, return true
                return true;
            }
        }

        // If either location is not valid, return false
        return false;
    }


    void SpawnNormalBase(int x, int z)
    {
        // Check if the base location is valid
        if (hexagonMapGenerator.IsValidBaseLocation(x, z))
        {
            //RemoveTileAtLocation(x, z);

            // Spawn base walls using the same random instance
            SpawnBaseWalls(x, z);

            Vector3 baseCoords = hexagonMapGenerator.GetHexCoords(x, z);
            mapManager.setNewDiff(x,z, 100);
            float elevation = hexagonMapGenerator.CalculateElevation(Mathf.PerlinNoise((baseCoords.x + hexagonMapGenerator.GetNoiseSeed()) / hexagonMapGenerator._noiseFrequency, (baseCoords.z + hexagonMapGenerator.GetNoiseSeed()) / hexagonMapGenerator._noiseFrequency));

            GameObject baseObject = Instantiate(basePrefab, new Vector3(baseCoords.x, elevation + 0.1f, baseCoords.z), Quaternion.Euler(0, 90f, 0));

            centerBaseSpawned = true;

            // Calculate the symmetric location for the crystal base
            int symmetricalX = hexagonMapGenerator._MapWidth - 1 - x;
            int symmetricalZ = hexagonMapGenerator._MapHeight - 1 - z;
            Vector3 crystalBaseCoords = hexagonMapGenerator.GetHexCoords(symmetricalX, symmetricalZ);

            // Use the same method to calculate elevation for crystal base as for crystal
            float crystalElevation = hexagonMapGenerator.CalculateElevation(Mathf.PerlinNoise((crystalBaseCoords.x + hexagonMapGenerator.GetNoiseSeed()) / hexagonMapGenerator._noiseFrequency, (crystalBaseCoords.z + hexagonMapGenerator.GetNoiseSeed()) / hexagonMapGenerator._noiseFrequency));

            GameObject crystalBaseObject = Instantiate(crystalBasePrefab, new Vector3(crystalBaseCoords.x, crystalElevation + 0.1f, crystalBaseCoords.z), Quaternion.Euler(0, 90f, 0));
        }
    }

    void SpawnBaseWalls(int baseX, int baseZ)
    {
        // Calculate elevation for the crystal base
        Vector3 crystalBaseCoords = hexagonMapGenerator.GetHexCoords(baseX, baseZ);
        float crystalElevation = hexagonMapGenerator.CalculateElevation(Mathf.PerlinNoise((crystalBaseCoords.x + hexagonMapGenerator.GetNoiseSeed()) / hexagonMapGenerator._noiseFrequency, (crystalBaseCoords.z + hexagonMapGenerator.GetNoiseSeed()) / hexagonMapGenerator._noiseFrequency));

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                int wallX = baseX + i;
                int wallZ = baseZ + j;

                // Check if the wall location is valid
                if (hexagonMapGenerator.IsValidBaseLocation(wallX, wallZ))
                {
                    // Remove the existing tile at the wall location
                    //RemoveTileAtLocation(wallX, wallZ);

                    // Adjusted position to center the base wall prefab
                    Vector3 wallCoords = hexagonMapGenerator.GetHexCoords(wallX, wallZ);

                    // Set the elevation of the base wall to match the crystal base
                    float elevation = crystalElevation;

                    Instantiate(baseWallPrefab, new Vector3(wallCoords.x, elevation + 0.1f, wallCoords.z), Quaternion.Euler(0, 90f, 0));
                    mapManager.setNewDiff(wallX, wallZ, 100);
                }
            }
        }
    }


    void SpawnCrystalBaseWalls(int baseX, int baseZ)
    {
        // Calculate elevation for the crystal base
        Vector3 crystalBaseCoords = hexagonMapGenerator.GetHexCoords(baseX, baseZ);
        float crystalElevation = hexagonMapGenerator.CalculateElevation(Mathf.PerlinNoise((crystalBaseCoords.x + hexagonMapGenerator.GetNoiseSeed()) / hexagonMapGenerator._noiseFrequency, (crystalBaseCoords.z + hexagonMapGenerator.GetNoiseSeed()) / hexagonMapGenerator._noiseFrequency));

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                int wallX = baseX + i;
                int wallZ = baseZ + j;

                // Check if the wall location is valid
                if (hexagonMapGenerator.IsValidBaseLocation(wallX, wallZ))
                {
                    // Remove the existing tile at the wall location
                    //RemoveTileAtLocation(wallX, wallZ);

                    // Adjusted position to center the crystalBaseWall prefab
                    Vector3 wallCoords = hexagonMapGenerator.GetHexCoords(wallX, wallZ);

                    // Set the elevation of the base wall to match the crystal base
                    float elevation = crystalElevation;

                    Instantiate(crystalBaseWall, new Vector3(wallCoords.x, elevation + 0.1f, wallCoords.z), Quaternion.Euler(0, 90f, 0));
                    mapManager.setNewDiff(wallX, wallZ, 100);
                }
            }
        }
        // Remove the existing tile at the crystal base location
        //RemoveTileAtLocation(baseX, baseZ);
    }



    void RemoveTileAtLocation(int x, int z)
    {
        if (hexagonMapGenerator.IsInBounds(x, z) && hexagonMapGenerator.tileMap[x, z] != null)
        {
            Destroy(hexagonMapGenerator.tileMap[x, z]);
            hexagonMapGenerator.tileMap[x, z] = null;
        }
    }

    public Vector2Int GetBaseLocation(int index)
    {
        if (index == 0)
            return new Vector2Int(baseX1, baseZ1);
        else if (index == 1)
            return new Vector2Int(baseX2, baseZ2);
        else
            return Vector2Int.zero;
    }
}