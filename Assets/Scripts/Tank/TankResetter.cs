using UnityEngine;

public class TankResetter : MonoBehaviour
{
    // where do we start from?
    Vector3 startingPosition;
    // when do we return back?
    public float returnYLevel = -3;
    private void Start ( ) => startingPosition = transform.position;
    void Update()
    {
        if (transform.position.y < returnYLevel)
            transform.position = startingPosition;
    }
}
