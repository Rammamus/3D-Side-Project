using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericCode : MonoBehaviour
{
    void test<t>() where t : Component
    {
        t x = GetComponent<t>();
        x.gameObject.AddComponent<Rigidbody>();
    }
}
