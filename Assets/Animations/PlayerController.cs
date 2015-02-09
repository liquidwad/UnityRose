using UnityEngine;
using System.Collections;
using Network.Packets;
using Network;


namespace UnityRose
{

    public class PlayerController : MonoBehaviour
    {
    	public bool isMainPlayer = false;
        public float speed = 10f;
        public float rotateSpeed = 10f;

		public string name = "wadii";
		
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
        	gameObject.name = name;
        	
            playerMachine = new PlayerState(States.STANDING,"Player State Machine", this.gameObject);
            playerMachine.Entry();

            floorMask = LayerMask.GetMask("Floor");

            controller = this.gameObject.GetComponent<CharacterController>();

            destinationPosition = transform.position;
            
            if( isMainPlayer )
            {
	            // Send an instantiate char packet to server
				NetworkManager.Send( new InstantiateChar( gameObject.name, gameObject.transform.position, gameObject.transform.rotation ) );
			}
            // Add definitions for all packet received delegates
            NetworkManager.groundClickDelegate += (GroundClick gc) => 
            {
				// TODO: add checking for player ID
				if(gc.clientID == name)
					destinationPosition = gc.pos;
				// set destinationPosition = position received
            };

        }

        // Update is called once per frame
        void Update()
        {
			
			// Only take input if this player is the main player
			if( this.isMainPlayer )
			{
				// TODO: remove this after debugging is over
				if( Input.GetKeyDown( KeyCode.J ) )
				{
					NetworkManager.Send( new InstantiateChar( gameObject.name, gameObject.transform.position, gameObject.transform.rotation ));
				}
				
				
				bool locate = false;
				switch (Application.platform)
				{
					case RuntimePlatform.IPhonePlayer:
					case RuntimePlatform.Android:
					case RuntimePlatform.WP8Player:
						locate = Input.touchCount > 0;
					break;
					default:
						locate = Input.GetMouseButton(0);
					break;
					
				}
				
				if ( locate )
	                LocatePosition();
        	}	
            

            MoveToPosition();

			// TODO: use character state packets to control state
            if (isWalking)
                state = States.RUN;
            else
                state = States.STANDING;


            if (playerMachine != null)
                playerMachine.Evaluate(state);

        }


        void LocatePosition()
        {
			Vector2 screenPoint;
			bool fire = false;
			switch (Application.platform)
        	{
        		case RuntimePlatform.IPhonePlayer:
        		case RuntimePlatform.Android:
        		case RuntimePlatform.WP8Player:
					screenPoint = Input.GetTouch(0).position;
					fire = (Input.GetTouch(0).tapCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Ended);
					break;
				default:
					screenPoint = Input.mousePosition;
					fire = Input.GetMouseButtonDown(0);
					break;
			
			}
			
             Ray camRay = Camera.main.ScreenPointToRay( screenPoint );
             RaycastHit floorHit;

			if( fire )
			{
				// Perform the raycast and if it hits something on the floor layer...
				if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask))
				{
				 	//destinationPosition = floorHit.point;
				 	// Send a clicked on ground packet
					NetworkManager.Send( new GroundClick( gameObject.name, floorHit.point ));
				 	
				}
			
            }
        }


		
        void MoveToPosition() 
        {

            if ( Vector3.Distance( transform.position , destinationPosition ) > 0.5f )
            {
                Vector3 playerToMouse = destinationPosition - transform.position;
                playerToMouse.y = 0;

                // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
                Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
                transform.rotation = newRotation; //Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * rotateSpeed);

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

