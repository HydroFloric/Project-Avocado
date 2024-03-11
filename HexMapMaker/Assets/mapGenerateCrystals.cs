using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapGenerateCrystals : MonoBehaviour
{
    [SerializeField]
    HexagonMapGenerator hexagonMapGenerator;
    [SerializeField]
    MapBaseGenerator mapBaseGenerator;

    public GameObject crystalPrefab;
    private int _numCrystals = 15; 

    void Start()
    {
        GenerateCrystals();
    }

    void GenerateCrystals()
    {
        List<Vector2Int> crystalLocations = new List<Vector2Int>();

        for (int i = 0; i < _numCrystals;)
        {
            int x = Random.Range(2, hexagonMapGenerator._MapWidth - 2);
            int z = Random.Range(2, hexagonMapGenerator._MapHeight - 2);

            Vector2Int crystalLocation = new Vector2Int(x, z);

            // Ensure crystals are not on bases or too close to them, have a buffer from walls and are far from other crystals
            if (!IsCloseToBases(crystalLocation, 3) &&
                hexagonMapGenerator.IsCrystalTile(x, z) &&
                !hexagonMapGenerator.IsMountainTile(x, z) &&
                IsBufferFromWalls(crystalLocation) &&
                IsFarFromOtherCrystals(crystalLocation, crystalLocations, 5) &&
                !IsOnCrystalBase(crystalLocation))
            {
                Vector3 crystalCoords = hexagonMapGenerator.GetHexCoords(x, z);

                for (int j = 0; j < 3; j++)
                {
                    float yOffset = j * 0.2f;  
                    Instantiate(crystalPrefab, new Vector3(crystalCoords.x, yOffset, crystalCoords.z), Quaternion.Euler(0, 90f, 0));
                }

                Instantiate(crystalPrefab, new Vector3(crystalCoords.x, 0.6f, crystalCoords.z), Quaternion.Euler(0, 90f, 0));

                // Add the crystal location to the list
                crystalLocations.Add(crystalLocation);

                i++;
            }
        }
    }

    bool IsOnCrystalBase(Vector2Int crystalLocation)
{
    
    int bufferDistance = 4;  

    for (int baseIndex = 0; baseIndex < 2; baseIndex++)
    {
        Vector2Int crystalBaseLocation = mapBaseGenerator.GetBaseLocation(baseIndex);

        if (Mathf.Abs(crystalLocation.x - crystalBaseLocation.x) < bufferDistance &&
            Mathf.Abs(crystalLocation.y - crystalBaseLocation.y) < bufferDistance)
        {
            return true;  
        }
    }

    return false;
}

    bool IsBufferFromWalls(Vector2Int crystalLocation)
    {
        // Define the buffer distance from walls
        int bufferDistance = 4;  

    
        for (int baseIndex = 0; baseIndex < 2; baseIndex++)
        {
            Vector2Int baseLocation = mapBaseGenerator.GetBaseLocation(baseIndex);

            if (Mathf.Abs(crystalLocation.x - baseLocation.x) < bufferDistance &&
                Mathf.Abs(crystalLocation.y - baseLocation.y) < bufferDistance)
            {
                return false;  
            }
        }

        return true;
    }

    bool IsFarFromOtherCrystals(Vector2Int currentLocation, List<Vector2Int> otherLocations, int minDistance)
    {
        foreach (Vector2Int location in otherLocations)
        {
            if (Vector2Int.Distance(currentLocation, location) < minDistance)
            {
                return false;  
            }
        }
        return true;
    }

    bool IsCloseToBases(Vector2Int crystalLocation, int minDistance)
    {
        Vector2Int baseLocation1 = mapBaseGenerator.GetBaseLocation(0);
        Vector2Int baseLocation2 = mapBaseGenerator.GetBaseLocation(1);

        if (Vector2Int.Distance(crystalLocation, baseLocation1) < minDistance || Vector2Int.Distance(crystalLocation, baseLocation2) < minDistance)
        {
            return true;  
        }
        return false;
    }
}