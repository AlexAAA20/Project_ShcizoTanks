using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    [Serializable]
    public class BeltPart       // an item that allows the tank to move
    {
        public Collider collider;   // lel
        public float maxSpeed;      // speed at 1 general speed
        public float maxTorque;     // speed at 1 turn
        public int leftMultiplier;
        public int rightMultiplier;

    }

    // belts. listing of all belts that are
    // on this tank.
    public List<BeltPart> belts;

    // input general speed. the speed of which the
    // belt parts should move.
    public float inputGeneralSpeed;

    // input turn. how much does the L/R side gets
    // boosted and R/L side decreased.
    // 1 is right, -1 is left.
    public float inputTurn;

    // rigidbody.
    public Rigidbody rb;

    // jumpPower. how much power is stored in a zero-velocity jump
    public float jumpPower;

    // jumpMultiplier. how much the power increases per unit of horizontal velocity.
    public float jumpMultiplier;

    // jumpCap. what is the max vertical velocity can you achieve with the
    // jumpMultiplier extra bonus.
    public float jumpCap;

    // secondsTillFlip. how much seconds will be waited
    // until the tank flips itself
    public float secondsTillFlip;

    // speedForFlip. how much maximum speed is allowed
    // to do a flip. stops midair flipping.
    public float speedForFlip;

    // root. ig?
    public Transform root;

    // speedometer. selfexplanatory.
    public float speedometer;

    bool flipped;

    int groundedCount;

    public void Start ( )
    {
        root = root == null ? transform.parent : root;
    }

    public void Update ( )
    {
        HandleInputs( );
        ProcessTouches( );
        speedometer = +rb.linearVelocity.magnitude;
    }
    public void HandleInputs ( )
    {
        if ( Input.GetKey( KeyCode.W ) ) inputGeneralSpeed = 1;
        else if ( Input.GetKey( KeyCode.S ) ) inputGeneralSpeed = -1;
        else inputGeneralSpeed = 0;

        if ( Input.GetKey( KeyCode.D ) )
        {
            inputGeneralSpeed *= .75f;
            inputTurn = 1;
        }
        else if ( Input.GetKey( KeyCode.A ) )
        {
            inputGeneralSpeed *= .75f;
            inputTurn = -1;
        }
        else
        {
            inputTurn = 0;
        }
    }

    public void ProcessTouches( )
    {
        groundedCount = 0;
        foreach (var item in belts)
        {
            RaycastHit hit;
            bool grounded = Physics.BoxCast(
                center: item.collider.bounds.center + Vector3.up * 0.1f,
                halfExtents: item.collider.bounds.extents * .8f, // slightly bigger j4f (outdated BY THE WAY)
                direction:  item.collider.transform.rotation * Vector3.down,
                out hit,
                orientation: item.collider.transform.rotation,
                maxDistance: .5f // loads of DISTANCE

                // so uhhh
                // it takes the position of the collider
                // and then raycasts five microunits (unit = m, microunit = dm)
                // (also 0.5 units) (or .5 idk) (or also .5f if you like)
                // with the rotation of the current collider rotation
                // also moves downwards
            );
            if ( grounded )
            {
                groundedCount++;
                // MOVE
                rb.AddRelativeForce( Vector3.forward * item.maxSpeed * inputGeneralSpeed * Time.deltaTime );
                Vector3 velocity = (rb.rotation * Vector3.forward) * item.maxTorque * Time.deltaTime;
                if ( inputTurn > 0 )
                    velocity *= item.rightMultiplier;
                else if ( inputTurn < 0 )
                    velocity *= item.leftMultiplier;
                else
                    velocity *= 0;
                rb.AddForceAtPosition( velocity, item.collider.transform.position );
                item.collider.GetComponent<Renderer>( ).material.color = Color.green;
            }
            else
            {
                // DONT MOVE
                item.collider.GetComponent<Renderer>( ).material.color = Color.red;
            }
        }
        // jump key pressed
        if ( Input.GetKeyDown( KeyCode.Space ) )
        {
            if ( groundedCount > 0 )
            {
                float power = Mathf.Clamp( jumpPower + ( +rb.linearVelocity.magnitude ) * jumpMultiplier, 0, jumpCap );
                float horizPower = Mathf.Clamp( +rb.linearVelocity.magnitude * power / 6, 0, jumpCap / 2 );
                Debug.Log( $"Jumped with {power} vertical power ({+rb.linearVelocity.magnitude * jumpMultiplier} speed ({+rb.linearVelocity.magnitude} actual speed), {jumpPower} default, {jumpCap} cap)" );
                rb.AddForce( Vector3.up * power + (transform.rotation * Vector3.forward * horizPower) );
            }
            else
            {
                Debug.Log( $"Couldn't jump, no belts are on the ground." );
            }
        }
        if ( groundedCount == 0 && +rb.linearVelocity.magnitude < speedForFlip && !flipped )
        {
            flipped = true;
            StartCoroutine( Flip( ) );
        }
    }

    IEnumerator Flip()
    {
        for (int i = 0; i < secondsTillFlip / .01f; i++)
        {
            yield return new WaitForSeconds( 0.01f );
            if ( groundedCount > 0 )
            {
                flipped = false;
                yield break;
            }
        }
        flipped = false;
        root.position = root.position + Vector3.up * 0.25f;
        root.rotation = Quaternion.Lerp( root.rotation, Quaternion.Euler(0, root.rotation.eulerAngles.y, 0), 0.8f);
    }
}
