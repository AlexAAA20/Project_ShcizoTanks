using UnityEngine;

public class TankCannonAimer : MonoBehaviour
{
    public TankShooterModulo shootingController;
    public TankCameraController cameraController;
    public Transform turretPivot;
    public Transform cannonPivot;

    // If not aiming, aim at the direction where the camera looks (hopefully not into another object)
    // If is aiming, aim at the position of aiming.
    // (a shape is pressed into a shape with the shape
    // presser that presses the shape into a pressed shape)

    void Update()
    {
        // code
        Quaternion to = Quaternion.LookRotation( (shootingController.pointer - turretPivot.position).normalized, Vector3.up );
        turretPivot.rotation = Quaternion.Euler( 0, to.eulerAngles.y, 0 );
        cannonPivot.rotation = Quaternion.Euler( to.eulerAngles.x, turretPivot.eulerAngles.y, turretPivot.eulerAngles.z );
    }
}
