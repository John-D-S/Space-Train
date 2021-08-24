using UnityEngine;

namespace Willow
{
	/// <summary> The Movement State Willow is in.</summary>
	public enum WillowMovementState
	{
		Idle,Walking,Running,Dead
	}

	/// <summary> If Willow is Alive or Dead. </summary>
	public enum WillowAliveStatus
	{
		Alive, Dead
	}
	
	/// <summary> Enum for Visibility. </summary>
	public enum WillowVisibility
	{
		Visible,
		InVisible
	}
	
	/// <summary> Will keep track on the State, Animator and Input Controller of Willow. </summary>
	public class WillowInputHandler : MonoBehaviour
	{
		/// <summary> This will take in the horizontal and vertical input. </summary>
		public Vector2 InputVector { get; private set; }
		
		/// <summary> The mouse position. ONLY FOR if you are using rotate towards mouse direction. </summary>
		public Vector3 MousePosition { get; private set; }
		
		/// <summary> The Movement State the player is in. This will change the movement speed of the player.</summary>
		public WillowMovementState myWillowMovementState = WillowMovementState.Idle;
		
		/// <summary> The Player Status if they are Alive/Dead. </summary>
		public WillowAliveStatus myWillowAliveStatus = WillowAliveStatus.Alive;
		
		/// <summary> The Animator, Grabs on Awake - !!! (Make Sure it has the Willow Character Controller) !!! </summary>
		public Animator myAnim;
		
		/// <summary> WillowTopDownCharacterMover, grabs in awake. </summary>
		public WillowTopDownCharacterMover myWillowTopDownCharacterMover;
		
		/// <summary> Run timer. </summary>
		private float turningTimer;
		
		/// <summary>
		/// This will be how long the player has to turn before they are considered Idle.
		/// Basically if you are holding "Run" and you are running 'left' and then you
		/// let go of 'left' and still holding "Run" and press 'right' how long of a
		/// delay do you have before you are considered idle.
		/// </summary>
		[SerializeField, Range(0, 0.6f)] private float maxTurningTimer = 0.1f;

		/// <summary>The Visibility of Willow.</summary>
		public WillowVisibility myWillowVisibility = WillowVisibility.Visible;

		
		/// <summary> Gets the components in the Game Object.</summary>
		private void Awake()
		{
			myAnim = GetComponentInChildren<Animator>();
			myWillowTopDownCharacterMover = GetComponentInChildren<WillowTopDownCharacterMover>();
		}

		/// <summary>
		/// Sets the player to "Idle" and "Alive".
		/// Can't see why this needs to be changed but it might.
		/// Also resets the turning timer, e.g turning timer = its max;
		/// </summary>
		private void Start()
		{
			IdleState();
			myWillowAliveStatus = WillowAliveStatus.Alive;
			turningTimer = maxTurningTimer;
			myWillowVisibility = WillowVisibility.Visible;
		}

		/// <summary> This will be called in Animations at the end of stealth animation.</summary>
		public void MakePlayerInVisible()
		{
			myWillowVisibility = WillowVisibility.InVisible;
		}
		
		// These are all the scripts that change the player states.
		#region Player States
		
		/// <summary> This is the non moving idle state.</summary>
		private void IdleState()
		{
			myAnim.SetBool("Idle", true);
			myAnim.SetBool("Walking", false);
			myAnim.SetBool("Running", false);
			myWillowMovementState = WillowMovementState.Idle;
		}

		/// <summary> This is the moving walking state.</summary>
		private void WalkingState()
		{
			myAnim.SetBool("Idle", false);
			myAnim.SetBool("Walking", true);
			myAnim.SetBool("Running", false);
			myWillowMovementState = WillowMovementState.Walking;
		}

		/// <summary> This is the moving running state.</summary>
		private void RunningState()
		{
			myAnim.SetBool("Idle", false);
			myAnim.SetBool("Walking", false);
			myAnim.SetBool("Running", true);
			myWillowMovementState = WillowMovementState.Running;
		}
		
		#endregion

		private void Update()
		{
			// If player not dead.
			if(myWillowAliveStatus != WillowAliveStatus.Dead)
			{
				// This will check the movement and state of movement of Willow.
				WillowMovement();
				
				// Checks if they activate invibility.
				WillowGoInVisible();

				MousePosition = Input.mousePosition;
			}
		}

		/// <summary> This will check the movement and state of movement of Willow.</summary>
		private void WillowMovement()
		{
				// Get the Vertical and Horizontal Axis.
				float h = Input.GetAxis("Horizontal");
				float v = Input.GetAxis("Vertical");

				// Makes a new Vector 2 based on the above inputs
				InputVector = new Vector2(h, v);

				// This is for the player movement.
				if(InputVector.magnitude != 0 // If there is no movement input.
				   || myWillowMovementState == WillowMovementState.Running // OR the last input was not running.
				   || myWillowMovementState == WillowMovementState.Walking) // OR the last input was not walking.
				{
					// If the player was in a 'running' or 'walking' state last frame
					// BUT the directional input is 0.
					if(InputVector.magnitude == 0)
					{
						// Ticks down the timer of the player turning before they will enter an idle state.
						turningTimer -= Time.deltaTime;
					}
					// If there is a directional input.
					else
					{
						// The turning timer resets.
						turningTimer = maxTurningTimer;
					}
					
					// If the player is sprinting and the timer for them to turn is not 0.
					if(Input.GetKey(KeyCode.LeftShift) && turningTimer > 0)
					{
						// If they are not already Running.
						if(myWillowMovementState != WillowMovementState.Running)
						{
							// Start Running.
							RunningState();
						}
					}
					// Else they are moving but not running.
					else if(turningTimer > 0)
					{
						// If they are not already Walking.
						if(myWillowMovementState != WillowMovementState.Walking)
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
					// To note this "shouldn't" hit
					IdleState();
				}
			
		}

		/// <summary> Checks if they activate invibility.</summary>
		private void WillowGoInVisible()
		{
			// If press space will turn inVisible.
			if(Input.GetButtonDown("Jump")) // For some reason "space" didn't work so we had to add "Jump" ~ John.
			{
				// If Willow is currently Visible.
				if(myWillowVisibility == WillowVisibility.Visible)
				{
					// Activate Stealth trigger.
					myAnim.SetTrigger("Activate Stealth");
				}
				
				// If Willow is currently already Stealthed.
				if(myWillowVisibility == WillowVisibility.InVisible)
				{
					// Will reset the stealth trigger.
					myAnim.ResetTrigger("Activate Stealth");
					// And make her Visible.
					myWillowVisibility = WillowVisibility.Visible;
				}
			}
		}
	}
}
