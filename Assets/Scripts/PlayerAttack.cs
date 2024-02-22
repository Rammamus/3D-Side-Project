using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange;
    public new Camera camera;
    Vector3 lookDirection;
    public LayerMask sliceAble;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lookDirection = camera.transform.forward;
        Physics.Raycast(camera.transform.position, lookDirection, attackRange, sliceAble);
    }
}
