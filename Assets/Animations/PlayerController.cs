using UnityEngine;
using System.Collections;


namespace UnityRose
{

    public class PlayerController : MonoBehaviour
    {


        int floorMask;
        float camRayLength = 500f;


        public float speed = 3.0f;
        float rotateSpeed = 3.0f;

        float jumpSpeed = 8.0f;
        float gravity   = 20.0f;



        Vector3 moveDirection = Vector3.zero;

        private PlayerState playerMachine;
        States state;
       // public StateParams stateParams;
        // Use this for initialization
        void Start()
        {
            playerMachine = new PlayerState(States.STANDING,"Player State Machine", this.gameObject);
            playerMachine.Entry();

            floorMask = LayerMask.GetMask("Floor");

        }

        // Update is called once per frame
        void Update()
        {

           ////using character controller

           // CharacterController controller = this.gameObject.GetComponent<CharacterController>();

           //// if (controller.isGrounded)
           //// {
           ////     moveDirection.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
           ////     moveDirection = transform.TransformDirection(moveDirection);
           ////     moveDirection *= speed;
 
           ////     if (Input.GetButton("Jump"))
           ////     {
           ////         moveDirection.y = jumpSpeed;
           ////     }

           //// }

           //// // Apply gravity
           //// moveDirection.y -= gravity * Time.deltaTime;

           //// // Move the controller
           //// controller.Move(moveDirection * Time.deltaTime);



           // using simple character controller
            CharacterController controller = this.gameObject.GetComponent<CharacterController>();


            transform.Rotate(0, 0, Input.GetAxis("Horizontal") * rotateSpeed);
            Vector3 forward = transform.TransformDirection(0.0f, -1.0f, 1.0f);
            float curSpeed = speed * Input.GetAxis("Horizontal");
            controller.SimpleMove(forward * speed);



            //code we need to work on previous things are just for testing

            //if ( Input.GetButton("Fire1"))
            //{
            //    Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            //    // Create a RaycastHit variable to store information about what was hit by the ray.
            //    RaycastHit floorHit;

            //    // Perform the raycast and if it hits something on the floor layer...
            //    if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask))
            //    {

            //        ///floorHit.point  is the Point that we need to go to

            //            /*
            //         // Create a vector from the player to the point on the floor the raycast from the mouse hit.  direction ?
            //         Vector3 playerToMouse = floorHit.point - transform.position;



                 
            //         Vector3 movement = new Vector3();

            //         movement.Set( floorHit.point.x , 0f,  floorHit.point.z);

            //         // Normalise the movement vector and make it proportional to the speed per second.
            //         movement = movement.normalized * 0.6f * Time.deltaTime;

            //         // Move the player to it's current position plus the movement.
            //         gameObject.rigidbody.MovePosition(floorHit.point * 0.6f);

            //         */
                    

            //    }
            //}
            //else if (Input.GetKey(KeyCode.S))
            //{
            //    state = States.SITTING;
                
            //}
            //else if (Input.GetKey(KeyCode.Z))
            //{
            //    state = States.STANDING;
            //}
            //else if (Input.GetKey(KeyCode.W))
            //{
            //    state = States.WALK;
            //}
            //else if (Input.GetKey(KeyCode.Q))
            //{
            //    state = States.RUN;
            //}
            


            if ( Input.GetKey(KeyCode.S))
            {
                state = States.RUN;
            }


            if (playerMachine != null)
                playerMachine.Evaluate(state);

        }
    }
}

