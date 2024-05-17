using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> staticObjects;
    [SerializeField] private List<GameObject> wallPrefabs;
    public GameObject playerTankPrefab;
    public GameObject enemyTankPrefab;
    public Camera mainCamera;

    // Taille de la map : 22x16
    private int rows = 16;
    private int cols = 22;

    private int xOrigin = -21;  // Coordonnée en X d'origine
    private int zOrigin = 13;   // Coordonnée en Z d'origine
    private int xStep = 2;      // Pas sur X
    private int zStep = -2;     // Pas sur Z
    private int yHeight = 1;    // Hauteur en Y des éléments à placer

    // Seed statique de la sélection des murs afin qu'un même niveau soit toujours généré de manière identique
    private int seed = 42;

    private void Start()
    {
        // Création d'une nouvelle scène
        //Scene level1 = SceneManager.CreateScene("Level1");
        //SceneManager.LoadScene("Levels");

        // Basculer vers la nouvelle scène
        //SceneManager.SetActiveScene(level1);

        Random.InitState(seed);

        GenerateLevel("level1");
    }

    private void CreateAndSwitchScene()
    {
        // Création d'une nouvelle scène
        Scene level1 = SceneManager.CreateScene("Level1");

        // Basculer vers la nouvelle scène
        SceneManager.SetActiveScene(level1);

        // Ajout d'un objet de base pour démonstration
        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube.transform.position = Vector3.zero;

        // Vous pouvez ici ajouter votre logique de génération de niveau
        //GenerateLevel();
    }

    private void GenerateLevel(string filePath)
    {
        //foreach (GameObject obj in staticObjects)
        //{
        //    Instantiate(obj);
        //}

        TextAsset levelData = Resources.Load<TextAsset>(filePath);
        if (levelData == null)
        {
            Debug.LogError("Level file not found");
            return;
        }

        string levelText = levelData.text;
        Debug.Log(levelText);
        string[] lines = levelText.Split('\n');
        Debug.Log($"'{lines[0]}', {lines[0].Length} caractères");
        if (lines.Length != rows)
        {
            Debug.LogError($"Level file does not match expected number of rows. ({lines.Length} found, {rows} expected)");
            return;
        }

        for (int i = 0; i < rows; i++)
        {
            string line = lines[i].Substring(0, cols);
            if (line.Length != cols)
            {
                Debug.LogError($"Line {i} in the level file does not match the expected number of columns. ({line.Length} found, {cols} expected)");
                return;
            }

            for (int j = 0; j < cols; j++)
            {
                char element = line[j];
                Vector3 position = new Vector3(xOrigin + j * xStep, yHeight, zOrigin + i * zStep); // Adjust y-axis if needed
                Quaternion rotation = Quaternion.Euler(-90, 0, 0);

                switch (element)
                {
                    case '#': // Wall
                        Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Count)], position, rotation);
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
        //if (mainCamera != null)
        //{
        //    mainCamera.transform.position = new Vector3(0, 16.6f, -17.3f); // Positionnement de la caméra
        //    mainCamera.transform.eulerAngles = new Vector3(45, 0, 0);      // Caméra en contre-plongée
        //}
    }
}