using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBaseGenerator : MonoBehaviour
{
    [SerializeField]
    HexagonMapGenerator hexagonMapGenerator;

    public GameObject baseWallPrefab;
    public GameObject basePrefab;

    public GameObject crystalBasePrefab;
    public GameObject crystalBaseWall;

    private int baseX1, baseZ1;
    private int baseX2, baseZ2;

    private bool centerBaseSpawned = false;

    void Start()
    {
        StartCoroutine(GenerateBasesCoroutine());
    }

    IEnumerator GenerateBasesCoroutine()
    {
        while (true)
        {
            // Try to find valid base locations
            if (TryFindValidBaseLocations())
            {
                // Spawn the first normal base
                yield return StartCoroutine(SpawnNormalBaseCoroutine(baseX1, baseZ1));

                // Symmetrically find the location for the second normal base
                baseX2 = hexagonMapGenerator._MapWidth - 1 - baseX1;
                baseZ2 = hexagonMapGenerator._MapHeight - 1 - baseZ1;

                yield return StartCoroutine(SpawnBaseWallsCoroutine(baseX1, baseZ1));
                yield return StartCoroutine(SpawnCrystalBaseWallsCoroutine(baseX2, baseZ2));


                break;
            }
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

    IEnumerator SpawnNormalBaseCoroutine(int x, int z)
    {
        // Check if the base location is valid
        if (hexagonMapGenerator.IsValidBaseLocation(x, z))
        {
            // Spawn base walls
            yield return StartCoroutine(SpawnBaseWallsCoroutine(x, z));

            // Spawn the base at the center
            Vector3 baseCoords = hexagonMapGenerator.GetHexCoords(x, z);
            GameObject baseObject = Instantiate(basePrefab, new Vector3(baseCoords.x, 0f, baseCoords.z), Quaternion.Euler(0, 90f, 0));

            // Set the flag to indicate that the center base has been spawned
            centerBaseSpawned = true;

            // Spawn the crystal base at the center
            Vector3 crystalBaseCoords = hexagonMapGenerator.GetHexCoords(x, z);
            GameObject crystalBaseObject = Instantiate(crystalBasePrefab, new Vector3(crystalBaseCoords.x, 0f, crystalBaseCoords.z), Quaternion.Euler(0, 90f, 0));
        }
    }


    IEnumerator SpawnBaseWallsCoroutine(int baseX, int baseZ)
    {
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
                    // Adjusted position to center the base prefab
                    Vector3 wallCoords = hexagonMapGenerator.GetHexCoords(wallX, wallZ);
                    Instantiate(baseWallPrefab, new Vector3(wallCoords.x, 0f, wallCoords.z), Quaternion.Euler(0, 90f, 0));

                    yield return new WaitForSeconds(0.01f);
                }
            }
        }
    }

   
    IEnumerator SpawnCrystalBaseWallsCoroutine(int baseX, int baseZ)
    {

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
                    // Adjusted position to center the crystalBasePrefab
                    Vector3 wallCoords = hexagonMapGenerator.GetHexCoords(wallX, wallZ);
                    Instantiate(crystalBaseWall, new Vector3(wallCoords.x, 0f, wallCoords.z), Quaternion.Euler(0, 90f, 0));

                    yield return new WaitForSeconds(0.01f);
                }
            }
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
