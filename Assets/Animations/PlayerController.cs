using UnityEngine;
using System.Collections;


namespace UnityRose
{

    public class PlayerController : MonoBehaviour
    {

        private PlayerState playerMachine;
        States state;
       // public StateParams stateParams;
        // Use this for initialization
        void Start()
        {
            playerMachine = new PlayerState(States.STANDING,"Player State Machine", this.gameObject);
            playerMachine.Entry();
        }

        // Update is called once per frame
        void Update()
        {

            if (Input.GetKey(KeyCode.S))
            {
                state = States.SIT;
            }
            else if (Input.GetKey(KeyCode.Z))
            {
                state = States.STANDING;
            }
            else if (Input.GetKey(KeyCode.W))
            {
                state = States.WALK;
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                
            }
            


            if (playerMachine != null)
                playerMachine.Evaluate(state);

        }
    }
}

