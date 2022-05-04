using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPlace : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 fixpos = new Vector3(transform.position.x, 0, 0);
        transform.position = fixpos;
    }
}
