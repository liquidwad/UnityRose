using UnityEngine;
using System.Collections;


namespace UnityRose
{

    public class PlayerController : MonoBehaviour
    {


        public float speed = 10f;
        public float rotateSpeed = 10f;

        int floorMask;
        float camRayLength = 500f;


     

        private Vector3 destinationPosition;
        private CharacterController controller;


       
        Vector3 moveDirection = Vector3.zero;

        private PlayerState playerMachine;


        bool isWalking = false;
        States state = States.STANDING;

       // public StateParams stateParams;
        // Use this for initialization
        void Start()
        {
            playerMachine = new PlayerState(States.STANDING,"Player State Machine", this.gameObject);
            playerMachine.Entry();

            floorMask = LayerMask.GetMask("Floor");

            controller = this.gameObject.GetComponent<CharacterController>();

            destinationPosition = transform.position;

        }

        // Update is called once per frame
        void Update()
        {

            destinationPosition.y = transform.position.y;

            if (Input.GetMouseButton(0))
            {
                LocatePosition();
            }

            MoveToPosition();

            //if (Input.GetKey(KeyCode.S))
            //{
            //    state = States.RUN;
            //}

            if (isWalking)
                state = States.RUN;
            else
                state = States.STANDING;


            if (playerMachine != null)
                playerMachine.Evaluate(state);

        }


        void LocatePosition()
        {
             Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
             RaycastHit floorHit;


                // Perform the raycast and if it hits something on the floor layer...
             if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask))
             {
                 destinationPosition = floorHit.point;

                 
             }
        }


        void MoveToPosition()
        {



            if ( Vector3.Distance( transform.position , destinationPosition ) > 0.1f )
            {
                //direction ?
                Vector3 playerToMouse = destinationPosition - transform.position;


                playerToMouse.y = 0;



                //u see that quaternion ? to get that i checked the initial quaternion rotatopn of char when loaded
                //and then i multiplied the angle to that to get the right rotation cus its fkd up

                // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
                Quaternion newRotation = Quaternion.LookRotation(playerToMouse);// *new Quaternion(0.7f, 0f, 0f, -0.7f); // initial rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * rotateSpeed);

                //check hangout


                //now here transformDirecttion should have  Vector3.Forward and not this values 
                //if u use vector3forward the char try to go on Y axis and not the right one cus forward is vector3(0,0,1)
                //so basically i did many tets to get the one that works with this model , am thikning its all rotation problem ok 
               // float curSpeed = speed * Input.GetAxis("Horizontal");
                controller.SimpleMove(transform.forward * speed);
                isWalking = true;
            }
            else
            {
                isWalking = false;
            }
             
           
             
        }




    }
}

