using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceTrain.Player
{
	public enum PlayerState
	{
		Idle,Walking,Running,Dead
	}

	public enum PlayerStatus
	{
		Alive, Dead
	}

	public class InputHandler : MonoBehaviour
	{
		public Vector2 InputVector { get; private set; }
		public Vector3 MousePosition { get; private set; }

		public bool isChangeMenuOpen = false;
    
		// Will be to pause the game but taking out for now 
		public bool isInGamePaused = false;

		// The Movement State the player is in.
		// This will change the movement speed of the player.
		public PlayerState myPlayerState;

		// The Player Status of the Player.
		// This will change if the player is, Alive, or Dead.
		// Will Hold Visability in the InputHandler.
		public PlayerStatus myPlayerStatus;

		[Header("Player Identities")]
		// The current identity of the player.
		public PlayerIdentity myPlayerIdentity = PlayerIdentity.None;
		
		// A list of all the textures for each Identity.
		public List<Texture> identityTexture = new List<Texture>();
		
		[Header("Currently Changed Identity")]
		// Has the player recently changed identites.
		public bool recentlyChangedIdentities = false;
		// How long people will be identified after the player changes Identites
		[Range(0f,2f)] public float timeToChangeIdenities = 0.2f;
		private float changedIdentitesTimer = 0;

		[Header("Player Components")]
		// This will be the render that will change with the player.
		public Renderer characterRenderer;
		
		// The Animator.
		public Animator myAnim;
		// Run timer.
		private float turningTimer;
		[SerializeField] private float maxTurningTimer;
    
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
			NoIdentity();
			myPlayerStatus = PlayerStatus.Alive;
			turningTimer = maxTurningTimer;
		}

	#region Player Identites

		// This is called when the player is back to their base Idenity.
		private void NoIdentity()
		{
			
		}

	#endregion

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
				float h = Input.GetAxis("Horizontal");
				float v = Input.GetAxis("Vertical");

				InputVector = new Vector2(h, v);

				// This is for the player movement.
				if(InputVector.magnitude != 0 || myPlayerState == PlayerState.Running || myPlayerState == PlayerState.Walking)
				{
					// This will not put the player into an idle state if they just turn side to side.
					if (InputVector.magnitude == 0)
					{
						turningTimer -= Time.deltaTime;
					}
					else
					{
						turningTimer = maxTurningTimer;
					}
			    
					if(Input.GetKey(KeyCode.LeftShift) && turningTimer > 0)
					{
						// Starts Running.
						RunningState();
					}
					else if (turningTimer > 0)
					{
						// If not Starts Walking.
						WalkingState();
					}
					else
					{
						// Go to Idle state.
						IdleState();
					}
				}
				else
				{
					IdleState();
				}
		    
				// If press space will turn invisable.
				if(Input.GetButtonDown("Jump"))
				{
					// Basically Activate stealth or not.
					if(myTopDownCharacterMover.myPlayerVisability == PlayerVisability.Visable)
					{
						myAnim.SetTrigger("Activate Stealth");
					}

					if(myTopDownCharacterMover.myPlayerVisability == PlayerVisability.Invisable)
					{
						myAnim.ResetTrigger("Activate Stealth");
						myTopDownCharacterMover.myPlayerVisability = PlayerVisability.Visable;
					}
				}
				MousePosition = Input.mousePosition;
			}
		}

	}
}