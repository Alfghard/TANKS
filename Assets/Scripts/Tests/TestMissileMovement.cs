using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestMissileMovement : MonoBehaviour
{
    private MissileMovement missile;
    private GameObject missileObject;

    [SetUp]
    public void Setup()
    {
        // Crée une instance de MissileMovement pour les tests
        missileObject = new GameObject("Missile");
        missile = missileObject.AddComponent<MissileMovement>();

        // Configure les composants nécessaires
        missile.speed = 10f;
        missile.nbRebond = 1;
        missileObject.transform.forward = Vector3.forward;
    }

    [TearDown]
    public void Teardown()
    {
        // Détruire les objets créés pour les tests
        Object.DestroyImmediate(missileObject);
    }

    [UnityTest]
    public IEnumerator TestMissileInitialDirection()
    {
        // Vérifie si la direction initiale est correcte
        yield return null; // Attendre un frame pour que Start() soit appelé
        Vector3 initialDirection = missile.currentDirection;
        Assert.AreEqual(Vector3.forward, initialDirection, "Initial direction should be forward.");
    }

    [UnityTest]
    public IEnumerator TestBaseMissileMovement()
    {
        // Vérifie si le missile se déplace correctement
        Vector3 initialPosition = missileObject.transform.position;
        yield return new WaitForSeconds(1f); // Attend 1 seconde
        Vector3 newPosition = missileObject.transform.position;
        Assert.AreNotEqual(initialPosition, newPosition, "Missile should have moved.");
    }
}
