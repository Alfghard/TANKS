using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] public GameObject wallPrefab;
    [SerializeField] public List<GameObject> wallPrefabs = new List<GameObject>();
    public GameObject playerTankPrefab;
    public GameObject enemyTankPrefab;
    public Camera mainCamera;

    // Taille de la map : 22x16
    public int rows = 16;
    public int cols = 22;

    private int xOrigin = -21;  // Coordonnée en X d'origine
    private int zOrigin = 13;   // Coordonnée en Z d'origine
    private int xStep = 2;      // Pas sur X
    private int zStep = -2;     // Pas sur Z
    private int yHeight = 1;    // Hauteur en Y des éléments à placer

    private void Start()
    {
        GenerateLevel("../Maps/level1.txt");
    }

    void GenerateLevel(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("Level file not found");
            return;
        }

        string[] lines = File.ReadAllLines(filePath);
        if (lines.Length != rows)
        {
            Debug.LogError("Level file does not match expected number of rows.");
            return;
        }

        for (int i = 0; i < rows; i++)
        {
            string line = lines[i];
            if (line.Length != cols)
            {
                Debug.LogError("One or more lines in the level file does not match the expected number of columns.");
                return;
            }

            for (int j = 0; j < cols; j++)
            {
                char element = line[j];
                Vector3 position = new Vector3(xOrigin + j * xStep, yHeight, zOrigin + i * zStep); // Adjust y-axis if needed

                switch (element)
                {
                    case '#': // Wall
                        Instantiate(wallPrefab, position, Quaternion.identity);
                        break;
                    case 'a': // Player's tank
                        Instantiate(playerTankPrefab, position, Quaternion.identity);
                        break;
                    case '1': // Enemy tank type 1 (expand with '2', '3', etc. for other types)
                        Instantiate(enemyTankPrefab, position, Quaternion.identity);
                        break;
                    case '.': // Empty space
                        break;
                    default:
                        Debug.LogWarning($"Unsupported type: {element} at row {i} column {j}");
                        break;
                }
            }
        }

        // set up the camera
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(0, 16.6f, -17.3f); // Positionnement de la caméra
            mainCamera.transform.eulerAngles = new Vector3(45, 0, 0);      // Caméra en contre-plongée
        }
    }
}