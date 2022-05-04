using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SillyMove : MonoBehaviour
{
    [SerializeField] private GameObject bread;
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float landOnGround;
    [SerializeField] private float proximitybreakVertical;
    [SerializeField] private float proximitybreakHorizontal;
    [SerializeField] private GameObject raycastOBJ;
    [SerializeField] private bool grounded;
    [SerializeField] private GameObject camOBJ;
    public LayerMask ignoreLayer;


    public Vector3 projectionOfBread;
    private bool belowGround;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {


       grounded = (Physics.Raycast(raycastOBJ.transform.position, Vector3.down, landOnGround, ~ignoreLayer));
        if(!grounded)
        {
            if (bread.transform.position.y < projectionOfBread.y)
            {
                belowGround = true;
                grounded = true;
                Debug.Log("bread is below ground!");
            }
            else
            {
                grounded = false;
                belowGround = false;
                projectionOfBread = new Vector3(0, -20, 0);
            }
        }
        

        anim.SetBool("Grounded", grounded);
       GroundAndLand();
    }

    private void FixedUpdate()
    {
        
        CheckMove();
        FollowBread();
    }

    private void FollowBread()
    {
        
        if(grounded)
        {
            Vector3 breadPosOnFloor = new Vector3(projectionOfBread.x, projectionOfBread.y-0.2f, projectionOfBread.z);
            transform.position = Vector3.MoveTowards(transform.position, breadPosOnFloor, speed * Time.deltaTime);
            
        }
        else
        {
            Vector3 breadPosOnFloor = new Vector3(bread.transform.position.x, bread.transform.position.y, bread.transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, breadPosOnFloor, speed * Time.deltaTime);
        }
       
       
    }

    private void CheckMove()
    {
        if (grounded)
        {
            Vector3 breadPosOnFloor = new Vector3(projectionOfBread.x, projectionOfBread.y, projectionOfBread.z);
      
            if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(projectionOfBread.x, 0, projectionOfBread.z)) > proximitybreakVertical)
            {
                Vector3 breadPosOnSameLevel = new Vector3(projectionOfBread.x, transform.position.y, projectionOfBread.z);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(projectionOfBread - transform.position), rotationSpeed * Time.deltaTime);
                anim.SetBool("Moving", true);
            }
            
            else
            {
              
                Vector3 breadPosOnSameLevel = new Vector3(0, projectionOfBread.y, 0);
                //Vector3 camOnGround = new Vector3(camOBJ.transform.position.x, camOBJ.transform.position.y, camOBJ.transform.position.z);
                //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.position - camOBJ.transform.position), rotationSpeed * Time.deltaTime);
                anim.SetBool("Moving", false);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(breadPosOnSameLevel - transform.position), rotationSpeed * Time.deltaTime);
                
            }
           
        }
        
        else
        {
            anim.SetBool("Moving", true);// flying
            Vector3 breadPosOnFloor = new Vector3(bread.transform.position.x, bread.transform.position.y, bread.transform.position.z);
            if (Vector3.Distance(transform.position, breadPosOnFloor) > proximitybreakHorizontal)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(breadPosOnFloor - transform.position), rotationSpeed * Time.deltaTime);

            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.position-camOBJ.transform.position), rotationSpeed * Time.deltaTime);
            }

           

        }
        


    }

    private void GroundAndLand()
    {
        if(!belowGround)
        {
            if (grounded)
            {
                RaycastHit hit;
                Ray ray = new Ray(raycastOBJ.transform.position, Vector3.down);
                Debug.DrawRay(raycastOBJ.transform.position, Vector3.down, Color.white);

                if (Physics.Raycast(ray, out hit, 1000f, ~ignoreLayer))
                {

                    projectionOfBread = hit.point;
                }
            }

        }
        else
        {
            if(grounded)
            {
                projectionOfBread = new Vector3(bread.transform.position.x, projectionOfBread.y, bread.transform.position.z);
            }
        }
       
    }

    private void OnDrawGizmos()
    {
        if(projectionOfBread!=null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(projectionOfBread,0.1f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(raycastOBJ.transform.position, 0.1f);
        }
    }
}
