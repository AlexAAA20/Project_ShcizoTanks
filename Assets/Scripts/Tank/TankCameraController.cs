using UnityEngine;

public class TankCameraController : MonoBehaviour
{
    // this is a script that allows you to turn your camera
    // relative to the tank's camera pivot.
    // it also allows you to singularly cast a camera-ray
    // and allows the camera to not pass through objects that
    // have Default layer.

    // current angles in degrees.
    public float horizontal;
    public float vertical;

    // sensitivity of inputs.
    public float hSensitivity;
    public float vSensitivity;

    // if you don't wanna turn sensitivity ugly, but
    // you still have to invert it either way.
    // also for masochist players.
    public bool invertVertical;
    public bool invertHorizontal;

    // pivot. i mean not exactly a pivot, but just a point
    // from where the camera will be cast from.
    public Transform pivot;

    // center. i mean not exactly a center but just a point
    // to which the camera should point.
    // pivot if none is selected.
    public Transform center;

    // max zoomout, or how far can the camera reach out.
    public float maxZoomout;

    // min zoomout, or how close can the camera reach out by custom controls.
    public float minZoomout;

    // currzoom, how much you wanna the camera to be far.
    public float currZoom;

    // zoomsens, how fast does the currzoom change.
    public float zoomSens;

    // camera. if none set, uses the main one.
    // if no main one, use the one displaying.
    public Camera selectedCamera;

    Vector3 pivotPosition;
    Vector3 centerPosition;

    private void Start ( )
    {
        if ( selectedCamera == null )
        {
            // my inner python-coder is happy
            if ( Camera.main == null )
                selectedCamera = Camera.current;
            else
                selectedCamera = Camera.main;
        }
        if ( center == null )
            center = pivot;
        pivotPosition = pivot.position;
        centerPosition = center.position;
    }


    void Update()
    {
        // i love the functions in functions
        pivot.position = pivotPosition + transform.position;
        center.position = centerPosition + transform.position;
        HandleInputs( );
        SingularCast( );
    }

    void HandleInputs( )
    {
        // turn only on right-mouse-button holding
        if ( Input.GetMouseButton( 1 ) )
        {
            if ( Input.mouseScrollDelta.magnitude > 0.1f)
            {
                currZoom = Mathf.Lerp( currZoom, currZoom + Input.mouseScrollDelta.y * zoomSens, 0.17f );
                currZoom = Mathf.Clamp( currZoom, minZoomout, maxZoomout );
            }
            Cursor.lockState = CursorLockMode.Locked;
            // 100 symbols in one string :)
            // or :( idk
            horizontal += Input.GetAxisRaw( "Mouse X" ) * hSensitivity * ( invertHorizontal ? -1 : 1 );
            vertical += Input.GetAxisRaw( "Mouse Y" ) * vSensitivity * ( invertVertical ? -1 : 1 );
        }
        else
            Cursor.lockState = CursorLockMode.Confined;
    }

    private void OnDrawGizmosSelected ( )
    {
        Quaternion rotation = Quaternion.Euler( vertical, horizontal, 0 );
        Vector3 direction = rotation * Vector3.forward;

        Gizmos.color = Color.yellow; 
        Gizmos.DrawLine( pivot.position, pivot.position + direction );
    }

    void SingularCast( )
    {
        // hardest thing ever - find a vector out of a rotation
        Quaternion rotation = Quaternion.Euler( vertical, horizontal, 0 );
        Vector3 direction = rotation * Vector3.forward;

        // nevermind its pretty easy
        // ok so now what
        // oh yeah raycasting

        RaycastHit hitInfo;
        bool didHit = Physics.Raycast( new Ray (pivot.position, direction), out hitInfo, currZoom );

        // current position of the camera
        Vector3 at;
        if ( didHit )
            // if we hit, then put the position at the point of collision
            at = hitInfo.point - direction * 0.02f;
        else
            // if we did not hit, then scale the distance and put the camera there
            at = pivot.position + direction * currZoom;

        // now the turn yeah
        Quaternion turn = Quaternion.LookRotation( center.position - at, Vector3.up );

        // and now we put the values in
        selectedCamera.transform.position = at;
        selectedCamera.transform.rotation = turn;
    }
}
