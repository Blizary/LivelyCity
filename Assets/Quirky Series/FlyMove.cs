using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FlyMove : MonoBehaviour
{
    [SerializeField] XRController rightHand;
    [SerializeField] private GameObject forwarOBJ; // Obj that determines the forward movement
    [SerializeField] InputHelpers.Button button;
    [SerializeField] GameObject cameraMain;
    public float moveSpeed = 5;
    bool pressed;
    Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 fordi = forwarOBJ.transform.position - cameraMain.transform.position;
        Debug.DrawRay(cameraMain.transform.position, fordi, Color.black);

        rightHand.inputDevice.IsPressed(button, out pressed);
        if (pressed)
        {
            Vector3 forwardPos = transform.position + fordi.normalized;
            transform.position = Vector3.MoveTowards(transform.position, forwardPos, Time.deltaTime * moveSpeed);
        }
        else
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
 
    }



}
