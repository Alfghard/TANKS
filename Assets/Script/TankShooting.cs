using UnityEngine;

public class TankShooting : MonoBehaviour
{

    [SerializeField] public Transform firePoint;
    [SerializeField] public GameObject missilePrefab;
    [SerializeField] public int button = 350;
    [SerializeField] public Transform turret;
    [SerializeField] public float turretSmoothAngle = 0.05f;
    [SerializeField] public float shootThreshold = 0.5f;  // Seuil d'activation du tir du bouton entre 0 compris et 1 non compris
    private float turretSpeed = 0f;
    private float shootAxis = 0f;

    void Start() { }     
    
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Joystick1Button7))
        {
            Shoot();
        }*/
        RotationTurret();
        // Debug.Log($"RJoystickX: {Input.GetAxis("RTrigger")}, pressed: {Input.GetButton("Fire")}");
        ShootMecanism();
        
    }

    private void RotationTurret()
    {
        float rJoyX = -Input.GetAxis("RJoyX");
        float rJoyY = Input.GetAxis("RJoyY");

        Vector3 shootDir = new Vector3(rJoyX, 0f, rJoyY);

        if (shootDir.magnitude >= 0.1f)
        {
            // Calcule l'angle cible basé sur la direction du joystick
            float targetAngle = Mathf.Atan2(shootDir.x, shootDir.z) * Mathf.Rad2Deg;
            // Obtient l'angle actuel de la tourelle
            float turretAngle = Mathf.SmoothDampAngle(turret.eulerAngles.y, targetAngle, ref turretSpeed, turretSmoothAngle);

            // Applique la rotation vers l'angle cible
            turret.rotation = Quaternion.Euler(0f, turretAngle, 0f);
        }
    }

    private void ShootMecanism()
    {
        // Si le bouton RTrigger est enclenché
        if (Input.GetButton("Fire"))
        {
            float normShootAxis = (Input.GetAxis("RTrigger") + 1) / 2;
            Debug.Log($"normShootAxis: {normShootAxis}");
            // Tire si l'axe du bouton dépasse 
            if (shootAxis <= shootThreshold && normShootAxis > shootThreshold)
            {
                Shoot();
            }
            shootAxis = normShootAxis;
        }
    }

    private void Shoot()
    {
        GameObject missile = Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
        missile.transform.Rotate(180f, 0, 0);
    }
}