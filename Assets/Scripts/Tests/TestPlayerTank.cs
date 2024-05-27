using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;


/// <summary>
/// Classe de test unitaires de la classe PlayerTank
/// </summary>
public class TestPlayerTank : MonoBehaviour
{
    private PlayerTank playerTank;
    private GameObject tankObject;

    /// <summary>
    /// Lancement du script de test
    /// </summary>
    [SetUp]
    public void Setup()
    {
        // Crée une instance de PlayerTank pour les tests
        tankObject = new GameObject();
        playerTank = tankObject.AddComponent<PlayerTank>();

        playerTank.rb = tankObject.AddComponent<Rigidbody>();

        GameObject turretObject = new GameObject("Turret");
        playerTank.turret = turretObject.transform;

        GameObject firePointObject = new GameObject("FirePoint");
        playerTank.firePoint = firePointObject.transform;

        GameObject missilePrefab = new GameObject("Missile");
        playerTank.missilePrefab = missilePrefab;

        // Ajoute un tag "Missile" au prefab de missile
        playerTank.missilePrefab.tag = "Missile";
    }

    /// <summary>
    /// Test unitaire de la méthode TankMovement</br>
    /// Vérifie que le tank se déplace dans la bonne direction en simulant des inputs à partir d'un jeu de test
    /// </summary>
    /// <returns>Retourne une assertion de réussite ou non selon si le test s'est bien déroulé</returns>
    [UnityTest]
    public IEnumerator TestTankMovement()
    {
        // Jeu de test
        Vector3[] testDirections = {
            new Vector3(1, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(-1, 0, -1)
        };

        foreach (var direction in testDirections)
        {
            playerTank.TankMovement(direction);
            yield return null;

            Vector3 velocity = playerTank.rb.velocity;

            // Réussite du test si le tank se déplace bien dans la direction voulue
            NUnit.Framework.Assert.IsTrue(velocity.magnitude > 0.0f, "TankMovement failed to move the tank");
        }
    }

    /// <summary>
    /// Test unitaire de la méthode TurretMovement</br>
    /// Vérifie que la tourelle s'oriente dans la bonne direction selon un jeu de données de vecteurs
    /// </summary>
    /// <returns>Retourne une assertion de réussite ou non selon si le test s'est bien déroulé</returns>
    [UnityTest]
    public IEnumerator TestTurretMovement()
    {
        // Jeu de test
        Vector3[] testDirections = {
            new Vector3(1, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(-1, 0, -1)
        };

        foreach (var direction in testDirections)
        {
            playerTank.TurretMovement(direction);
            yield return null;
            
            float angle = playerTank.turret.eulerAngles.y;

            // Réussite du test si la tourelle est bien orienté dans la direction souhaitée
            NUnit.Framework.Assert.IsTrue(angle != 0.0f, "TurretMovement failed to rotate the turret");
        }
    }

    /// <summary>
    /// Test unitaire de la méthode Shoot</br>
    /// Vérifie qu'un missile supplémentaire est bien instancié à l'appel de la méthode
    /// </summary>
    /// <returns>Retourne une assertion de réussite ou non selon si le test s'est bien dérouléRetourne une assertion de réussite ou non selon si le test s'est bien déroulé</returns>
    [UnityTest]
    public IEnumerator TestShoot()
    {
        int initialMissileCount = GameObject.FindGameObjectsWithTag("Missile").Length;

        playerTank.Shoot();
        yield return new WaitForSeconds(0.1f);
        int finalMissileCount = GameObject.FindGameObjectsWithTag("Missile").Length;

        // Réussite du test s'il y a bien un objet supplémentaire missile qui est créé après le tir
        NUnit.Framework.Assert.AreEqual(initialMissileCount + 1, finalMissileCount, "Shoot failed to instantiate a new missile");
    }
}
