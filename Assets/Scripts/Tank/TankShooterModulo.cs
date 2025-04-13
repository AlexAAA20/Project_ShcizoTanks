using System;
using UnityEngine;
using UnityEngine.UIElements;

public class TankShooterModulo : MonoBehaviour
{
    public class CannonAction
    {
        public string name;
        public float kickback;
        public float explosionPower;
        public float explosionSpeed;
        public float upwardhit;
    }

    public LineRenderer display;
    public float knockback;
    public bool ready;
    public Vector3 pointer;

    // alr so
    // course of actions:
    /*
     * #1: get the position of where you wanna point
     * #2: check if no own objects are touched
     * #3: detect the thing that got shot
     * #4: KAPAWCH
     * #5: gon
     * #6: yay :D
     */
    void Update()
    {
        Vector3 position = GetPointingAt( );
        pointer = position;
        if ( Input.GetMouseButtonDown( 0 ) )
        {
            ready = true;
        }
        if ( ready )
        {
            display.enabled = true;
            display.SetPosition( 0, transform.position );
            Idle( );
            if ( Input.GetMouseButtonUp( 0 ) )
            {
                ready = false;
                Kick( );
                display.enabled = false;
            }
        }
        
    }

    void Idle( )
    {
        Ray r = GetRayAt( );
        RaycastHit hit;
        bool hitSomething = Physics.Raycast( r, out hit );
        if ( hitSomething )
        {
            Debug.Log( $"Hit {hit.transform.name}" );
            if ( !hit.transform.CompareTag("Player Tank") )
            {
                if ( hit.transform.GetComponent<Rigidbody>( ) != null )
                    display.material.color = Color.red;
                else
                    display.material.color = Color.black;
            }    
            else
            {
                display.material.color = Color.white;
            }
            display.SetPosition( 1, hit.point );
        }
        else
        {
            display.material.color = Color.black;
            display.SetPosition( 1, r.GetPoint( 5000 ) );
        }
    }

    void Kick ( )
    {
        Ray r = GetRayAt( );
        RaycastHit hit;
        bool hitSomething = Physics.Raycast( r, out hit );
        display.SetPosition( 1, transform.position );
        if ( hitSomething )
        {
            if ( !hit.transform.CompareTag( "Player Tank" ) )
            {
                if ( hit.transform.GetComponent<Rigidbody>( ) != null )
                    hit.transform.GetComponent<Rigidbody>( ).AddForceAtPosition( r.direction * knockback, hit.point, ForceMode.Impulse );
                else
                    return;
            }
            else
            {
                return;
            }
        }
    }

    Vector3 GetPointingAt( )
    {
        Ray r = GetRayAt( );
        RaycastHit h;
        bool got = Physics.Raycast( r, out h );
        if ( got )
            return h.point;
        else
            return r.GetPoint( 5000 );
    }

    Ray GetRayAt( )
    {
        return Camera.main.ScreenPointToRay( Input.mousePosition );
    }
}
