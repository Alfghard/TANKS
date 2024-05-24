using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;


public class TestPlayerTank : MonoBehaviour
{

    [SerializeField] private Rigidbody rb;                      // RigidBody du tank
    [SerializeField] private Transform turret;                  // objet Tourelle
    [SerializeField] private Transform firePoint;               // Point a la sortie du canon
    [SerializeField] private GameObject missilePrefab;          // Missile a tirer

    private PlayerTank playerTank;
    private GameObject tankObject;

    [SetUp]
    public void Setup()
    {
        // Crée une instance de PlayerTank pour les tests
        tankObject = new GameObject();
        playerTank = tankObject.AddComponent<PlayerTank>();

        // Configure les composants nécessaires
        playerTank.rb = rb;
        playerTank.turret = turret;
        playerTank.firePoint = new GameObject().transform;
        playerTank.missilePrefab = new GameObject();
    }

    public IEnumerator TestTankMovement()
    {
        // Valeurs de test
        Vector3[] testDirections = {
            new Vector3(1, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(-1, 0, -1)
        };

        foreach (var direction in testDirections)
        {
            var initialPosition = playerTank.transform.position;
            var expectedRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            var expectedSpeed = 1f;

            List<Vector3> realRotation = new List<Vector3>();

            for (int i = 0; i < 200; i++)
            {
                realRotation = playerTank.TankMovement(direction);
                yield return new WaitForSeconds(0.01f);
            }

            var actualRotation = Mathf.Atan2(realRotation[0].x, realRotation[0].z) * Mathf.Rad2Deg;
            var actualSpeed = realRotation[1].magnitude;

            var delta = Mathf.Abs(expectedRotation - actualRotation);
            var deltaSpeed = Mathf.Abs(expectedSpeed - actualSpeed);

            if (delta > 5f)
            {
                Debug.Log($"TankMovement's test successful for direction {direction} (rotation)");
            }
            else
            {
                Debug.LogAssertion($"TankMovement's test failed for direction {direction}: expected angle {expectedRotation}, got angle {actualRotation}");
            }

            if (deltaSpeed > 0.05f)
            {
                Debug.Log($"TankMovement's test successful for direction {direction} (speed)");
            }
            else
            {
                Debug.LogAssertion($"TankMovement's test failed for direction {direction}: expected speed {expectedSpeed}, got speed {actualSpeed}");
            }
            yield return null;
        }
    }

    public IEnumerator TestTurretMovement()
    {
        // Valeurs de test
        Vector3[] testDirections = {
            new Vector3(1, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(-1, 0, -1)
        };

        foreach (var direction in testDirections)
        {
            var expectedRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            Vector3 realRotation = Vector3.zero;

            for (int i=0; i<200; i++)
            {
                realRotation = playerTank.TurretMovement(direction);
                yield return new WaitForSeconds(0.01f);
            }

            var actualRotation = Mathf.Atan2(realRotation.x, realRotation.z) * Mathf.Rad2Deg;
            var delta = Mathf.Abs(expectedRotation - actualRotation);

            if (delta > 5f)
            {
                Debug.LogAssertion($"TurretMovement's test failed for direction {direction}: expected {expectedRotation}, got {actualRotation}");
            }
            else
            {
                Debug.Log($"TurretMovement's test successful for direction {direction}");
            }
            yield return null;
        }
    }

    public IEnumerator TestShoot()
    {
        // Compte initial des missiles
        int initialMissileCount = GameObject.FindGameObjectsWithTag("Missile").Length;

        playerTank.Shoot();

        
        yield return new WaitForSeconds(0.1f);  // Attend un moment pour que le missile soit instantié

        // Compte final des missiles
        int finalMissileCount = GameObject.FindGameObjectsWithTag("Missile").Length;

        if (initialMissileCount + 1 == finalMissileCount)
        {
            Debug.Log("TestShoot successful");
        }
        else
        {
            Debug.LogAssertion("TestShoot failed: failed to instantiate a new missile");
        }
        yield return null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("Z'est partiiii !");
            StartCoroutine(RunTests());
        }
    }

    private IEnumerator RunTests()
    {
        Setup();
        yield return TestTankMovement();
        yield return TestTurretMovement();
        yield return TestShoot();

        Object.DestroyImmediate(tankObject);
    }
}
