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
        // Cr�e une instance de PlayerTank pour les tests
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
    /// Test unitaire de la m�thode TankMovement</br>
    /// V�rifie que le tank se d�place dans la bonne direction en simulant des inputs � partir d'un jeu de test
    /// </summary>
    /// <returns>Retourne une assertion de r�ussite ou non selon si le test s'est bien d�roul�</returns>
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

            // R�ussite du test si le tank se d�place bien dans la direction voulue
            NUnit.Framework.Assert.IsTrue(velocity.magnitude > 0.0f, "TankMovement failed to move the tank");
        }
    }

    /// <summary>
    /// Test unitaire de la m�thode TurretMovement</br>
    /// V�rifie que la tourelle s'oriente dans la bonne direction selon un jeu de donn�es de vecteurs
    /// </summary>
    /// <returns>Retourne une assertion de r�ussite ou non selon si le test s'est bien d�roul�</returns>
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

            // R�ussite du test si la tourelle est bien orient� dans la direction souhait�e
            NUnit.Framework.Assert.IsTrue(angle != 0.0f, "TurretMovement failed to rotate the turret");
        }
    }

    /// <summary>
    /// Test unitaire de la m�thode Shoot</br>
    /// V�rifie qu'un missile suppl�mentaire est bien instanci� � l'appel de la m�thode
    /// </summary>
    /// <returns>Retourne une assertion de r�ussite ou non selon si le test s'est bien d�roul�Retourne une assertion de r�ussite ou non selon si le test s'est bien d�roul�</returns>
    [UnityTest]
    public IEnumerator TestShoot()
    {
        int initialMissileCount = GameObject.FindGameObjectsWithTag("Missile").Length;

        playerTank.Shoot();
        yield return new WaitForSeconds(0.1f);
        int finalMissileCount = GameObject.FindGameObjectsWithTag("Missile").Length;

        // R�ussite du test s'il y a bien un objet suppl�mentaire missile qui est cr�� apr�s le tir
        NUnit.Framework.Assert.AreEqual(initialMissileCount + 1, finalMissileCount, "Shoot failed to instantiate a new missile");
    }
}
