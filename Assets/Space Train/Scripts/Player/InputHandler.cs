using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceTrain.Player
{
	/// <summary> The Movement State the player is in.</summary>
	public enum PlayerState
	{
		Idle,
		Walking,
		Running,
		Dead
	}

	/// <summary> If the player is Alive or Dead. </summary>
	public enum PlayerStatus
	{
		Alive,
		Dead
	}

	/// <summary> Will keep track on the State, Animator and Input Controller of the player. </summary>
	public class InputHandler : MonoBehaviour
	{
		/// <summary> This will take in the horizontal and vertical input. </summary>
		public Vector2 InputVector { get; private set; }

		/// <summary> The mouse position. ONLY FOR if you are using rotate towards mouse direction. </summary>
		public Vector3 MousePosition { get; private set; }

		/// <summary>The Movement State the player is in. This will change the movement speed of the player. </summary>
		public PlayerState myPlayerState;

		// The Player Status of the Player.
		// This will change if the player is, Alive, or Dead.
		// Will Hold Visability in the InputHandler.
		public PlayerStatus myPlayerStatus;

		[Header("Player Components")]
		// This will be the render that will change with the player.
		public Renderer characterRenderer;

		// The Animator.
		public Animator myAnim;
		
		/// <summary> Run timer. </summary>
		private float turningTimer;
		
		/// <summary>
		/// This will be how long the player has to turn before they are considered Idle.
		/// Basically if you are holding "Run" and you are running 'left' and then you
		/// let go of 'left' and still holding "Run" and press 'right' how long of a
		/// delay do you have before you are considered idle.
		/// </summary>
		[SerializeField, Range(0, 0.6f)] private float maxTurningTimer = 0.1f;


		// TopDownCharacterMover.
		public TopDownCharacterMover myTopDownCharacterMover;

		private void Awake()
		{
			myAnim = GetComponentInChildren<Animator>();
			myTopDownCharacterMover = GetComponentInChildren<TopDownCharacterMover>();
		}

		private void Start()
		{
			IdleState();
			myPlayerStatus = PlayerStatus.Alive;
			turningTimer = maxTurningTimer;
		}

	#region Player States

		// This is the non moving idle state.
		private void IdleState()
		{
			myAnim.SetBool("Idle", true);
			myAnim.SetBool("Walking", false);
			myAnim.SetBool("Running", false);
			myPlayerState = PlayerState.Idle;
		}

		// This is the moving walking state.
		private void WalkingState()
		{
			myAnim.SetBool("Idle", false);
			myAnim.SetBool("Walking", true);
			myAnim.SetBool("Running", false);
			myPlayerState = PlayerState.Walking;
		}

		// This is the moving running state.
		private void RunningState()
		{
			myAnim.SetBool("Idle", false);
			myAnim.SetBool("Walking", false);
			myAnim.SetBool("Running", true);
			myPlayerState = PlayerState.Running;
		}

	#endregion

		void Update()
		{
			// If player not dead.
			if(myPlayerStatus != PlayerStatus.Dead)
			{
				// This will check the movement and state of movement of the player.
				PlayerMovement();
				
				MousePosition = Input.mousePosition;
			}
		}

		/// <summary> This will check the movement and state of movement of the player.</summary>
		private void PlayerMovement()
		{
			float h = Input.GetAxis("Horizontal");
			float v = Input.GetAxis("Vertical");

			InputVector = new Vector2(h, v);

			// This is for the player movement.
			if(InputVector.magnitude != 0 // If there is no movement input.
				|| myPlayerState == PlayerState.Running // OR the last input was not running.
				|| myPlayerState == PlayerState.Walking) // OR the last input was not walking.
			{
				//Debug.Log("InputVector" + InputVector + "|| myPlayerState" + myPlayerState.ToString() 
				//          + "|| turningTimer" + turningTimer);
				
				// If the player was in a 'running' or 'walking' state last frame
				// BUT the directional input is 0.
				if(InputVector.magnitude == 0)
				{
					// Ticks down the timer of the player turning before they will enter an idle state.
					turningTimer -= Time.deltaTime;
				}
				else
				{
					// If there is a directional input.
					turningTimer = maxTurningTimer;
				}

				// If the player is sprinting and the timer for them to turn is not 0.
				if(Input.GetKey(KeyCode.LeftShift) && turningTimer > 0)
				{
					// If they are not already Running.
					if(myPlayerState != PlayerState.Running)
					{
						// Start Running.
						RunningState();
					}
				}
				// Else they are moving but not running.
				else if(turningTimer > 0)
				{
					// If they are not already Walking.
					if(myPlayerState != PlayerState.Walking)
					{
						// Start Walking.
						WalkingState();
					}
				}
				// Else the turning timer has reached 0.
				else
				{
					// Go to Idle state.
					IdleState();
				}
			}
			else
			{
				// Go into an Idle State.
				// To note this "shouldn't" hit.
				IdleState();
			}
		}
	}
}