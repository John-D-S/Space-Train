using System;
using UnityEngine;

namespace SpaceTrain.Player
{
    /// <summary> This will hold all the Identity Information.</summary>
    public class IdentityHandler : MonoBehaviour
    {
	    // OK THIS IS SO ANNOYING WITHOUT ODIN AHHHHH!!!
	    // FFS HERE YOU GO

	    // The renderer of the player.
	    public Renderer myPlayerRenderer;
	    
	    // This is all the current player values.
	    [Header("Current Player Values")]
	    // This is the current Player Identity.
	    public CharacterIdentity currentPlayerIdentity;
	    // This is the UI of the current selected Identity.
	    public CharacterIdentity selectedIdentiy;


	    [Header("Currently Changed Identity")]
	    // Has the player recently changed identites.
	    public bool recentlyChangedIdentities;
	    // How long people will be identified after the player changes Identites
	    [Range(0f,2f)] public float timeToChangeIdenities = 0.2f;
	    private float changedIdentitesTimer = 0;
	    
	    [Header("None Identity")]
	    // The Material of the none identity.
	    public Texture2D nonePlayerMaterial;
	    // Is this the UI game object of the current identity of the player; 
	    public GameObject noneCurrentIdentityUI;
	    // This will be the UI Game Object for the identity when it is selected.
	    public GameObject noneSelectedIdentiyUI;
	    
	    [Header("Passenger Identity")]
	    // The Material of the Passenger identity.
	    public Texture2D passengerPlayerMaterial;
	    // Is this the UI game object of the current identity of the player; 
	    public GameObject passengerCurrentIdentityUI;
	    // This will be the UI Game Object for the identity when it is selected.
	    public GameObject passengerSelectedIdentiyUI;
	    
	    [Header("Worker Identity")]
	    // The Material of the Worker identity.
	    public Texture2D workerPlayerMaterial;
	    // Is this the UI game object of the current identity of the player; 
	    public GameObject workerCurrentIdentityUI;
	    // This will be the UI Game Object for the identity when it is selected.
	    public GameObject workerSelectedIdentiyUI;
	    
	    [Header("Guard Identity")]
	    // The Material of the Guard identity.
	    public Texture2D guardPlayerMaterial;
	    // Is this the UI game object of the current identity of the player; 
	    public GameObject guardCurrentIdentityUI;
	    // This will be the UI Game Object for the identity when it is selected.
	    public GameObject guardSelectedIdentiyUI;
	    
	    private void Awake()
	    {
		    // Basically Change the player to none identity.
		    ChangeToNoneIdentity();
		    // Select No Idenetity.
		    SelectingNoIdentity();
	    }

	    private void Update()
	    {
		    // If press space select that identity.
		    if(Input.GetKey(KeyCode.Space))
		    {
			    ChangeIdentity(selectedIdentiy);
		    }

		    // Basically if they have just changed identities it resets the timer and makes
		    // RecentlyChangedIdentities = true.
		    if(changedIdentitesTimer > 0)
		    {
			    recentlyChangedIdentities = true;
			    changedIdentitesTimer -= Time.deltaTime;
		    }
		    else
		    {
			    recentlyChangedIdentities = false;
		    }
		    
		    // Go up a selected identity.
		    if(Input.GetKeyDown(KeyCode.UpArrow))
		    {
			    switch(selectedIdentiy)
			    {
				    case CharacterIdentity.None:
					    SelectingGuardIdentity();
					    break;
				    case CharacterIdentity.Passenger:
					    SelectingNoIdentity();
					    break;
				    case CharacterIdentity.Worker:
					    SelectingPassengerIdentity();
					    break;
				    case CharacterIdentity.Guard:
					    SelectingWorkerIdentity();
					    break;
				    default:
					    throw new ArgumentOutOfRangeException();
			    }
		    }
		    
		    // Go down selecting an identity.
		    if(Input.GetKeyDown(KeyCode.DownArrow))
		    {
			    switch(selectedIdentiy)
			    {
				    case CharacterIdentity.None:
					    SelectingPassengerIdentity();
					    break;
				    case CharacterIdentity.Passenger:
					    SelectingWorkerIdentity();
					    break;
				    case CharacterIdentity.Worker:
					    SelectingGuardIdentity();
					    break;
				    case CharacterIdentity.Guard:
					    SelectingNoIdentity();
					    break;
				    default:
					    throw new ArgumentOutOfRangeException();
			    }
		    }
	    }


	    // Turns on and off the selection.
	    #region Selecting Identities

		    // Select No Identity.
		    private void SelectingNoIdentity()
		    {
			    selectedIdentiy = CharacterIdentity.None;
			    noneSelectedIdentiyUI.gameObject.SetActive(true);
			    passengerSelectedIdentiyUI.gameObject.SetActive(false);
			    guardSelectedIdentiyUI.gameObject.SetActive(false);
			    workerSelectedIdentiyUI.gameObject.SetActive(false);
		    }
		    
		    // Select Passenger Identity.
		    private void SelectingPassengerIdentity()
		    {
			    selectedIdentiy = CharacterIdentity.Passenger;
			    noneSelectedIdentiyUI.gameObject.SetActive(false);
			    passengerSelectedIdentiyUI.gameObject.SetActive(true);
			    guardSelectedIdentiyUI.gameObject.SetActive(false);
			    workerSelectedIdentiyUI.gameObject.SetActive(false);
		    }
		    
		    // Select Worker Identity.
		    private void SelectingWorkerIdentity()
		    {
			    selectedIdentiy = CharacterIdentity.Worker;
			    noneSelectedIdentiyUI.gameObject.SetActive(false);
			    passengerSelectedIdentiyUI.gameObject.SetActive(false);
			    guardSelectedIdentiyUI.gameObject.SetActive(false);
			    workerSelectedIdentiyUI.gameObject.SetActive(true);
		    }
		    
		    // Select Guard Identity.
		    private void SelectingGuardIdentity()
		    {
			    selectedIdentiy = CharacterIdentity.Guard;
			    noneSelectedIdentiyUI.gameObject.SetActive(false);
			    passengerSelectedIdentiyUI.gameObject.SetActive(false);
			    guardSelectedIdentiyUI.gameObject.SetActive(true);
			    workerSelectedIdentiyUI.gameObject.SetActive(false);
		    }

	    #endregion

	    #region Change Player Identities

		    private void ChangeIdentity(CharacterIdentity _identityToChangeTo)
		    {
			    // If the same Identity as the current one, return.
			    if(_identityToChangeTo == currentPlayerIdentity)
			    {
				    return;
			    }
			    
			    // Will change the the selected identity.
			    switch(_identityToChangeTo)
			    {
				    case CharacterIdentity.None:
					    ChangeToNoneIdentity();
					    break;
				    case CharacterIdentity.Passenger:
					    ChangeToPassengerIdentity();
					    break;
				    case CharacterIdentity.Worker:
					    ChangeToWorkerIdentity();
					    break;
				    case CharacterIdentity.Guard:
					    ChangeToGuardIdentity();
					    break;
				    default:
					    throw new ArgumentOutOfRangeException(nameof(_identityToChangeTo), _identityToChangeTo, null);
			    }
			    
			    // Will reset the identity change timer.
			    changedIdentitesTimer = timeToChangeIdenities;
		    }
		    // This is for when you change to no identity.
		    public void ChangeToNoneIdentity()
		    {
			    // Make the player None Identitity.
			    currentPlayerIdentity = CharacterIdentity.None;
			    // Make the players current material = none;
			    myPlayerRenderer.material.mainTexture = nonePlayerMaterial;
			    // Make the UI of the Selected Material = On;
			    noneCurrentIdentityUI.gameObject.SetActive(true);
			    passengerCurrentIdentityUI.gameObject.SetActive(false);
			    workerCurrentIdentityUI.gameObject.SetActive(false);
			    guardCurrentIdentityUI.gameObject.SetActive(false);
		    }
		    
		    
		    // This is for when you change to Passenger identity.
		    public void ChangeToPassengerIdentity()
		    {
			    // Make the player None Identitity.
			    currentPlayerIdentity = CharacterIdentity.Passenger;
			    // Make the players current material = none;
			    myPlayerRenderer.material.mainTexture = passengerPlayerMaterial;
			    // Make the UI of the Selected Material = On;
			    noneCurrentIdentityUI.gameObject.SetActive(false);
			    passengerCurrentIdentityUI.gameObject.SetActive(true);
			    workerCurrentIdentityUI.gameObject.SetActive(false);
			    guardCurrentIdentityUI.gameObject.SetActive(false);
		    }
		    
		    // This is for when you change to worker identity.
		    public void ChangeToWorkerIdentity()
		    {
			    // Make the player None Identitity.
			    currentPlayerIdentity = CharacterIdentity.Worker;
			    // Make the players current material = none;
			    myPlayerRenderer.material.mainTexture = workerPlayerMaterial;
			    // Make the UI of the Selected Material = On;
			    noneCurrentIdentityUI.gameObject.SetActive(false);
			    passengerCurrentIdentityUI.gameObject.SetActive(false);
			    workerCurrentIdentityUI.gameObject.SetActive(true);
			    guardCurrentIdentityUI.gameObject.SetActive(false);
		    }
		    
		    // This is for when you change to guard identity.
		    public void ChangeToGuardIdentity()
		    {
			    // Make the player None Identitity.
			    currentPlayerIdentity = CharacterIdentity.Guard;
			    // Make the players current material = none;
			    myPlayerRenderer.material.mainTexture = guardPlayerMaterial;
			    // Make the UI of the Selected Material = On;
			    noneCurrentIdentityUI.gameObject.SetActive(false);
			    passengerCurrentIdentityUI.gameObject.SetActive(false);
			    workerCurrentIdentityUI.gameObject.SetActive(false);
			    guardCurrentIdentityUI.gameObject.SetActive(true);
		    }
		    
	    #endregion
    }
    
    public enum CharacterIdentity
    {
	    None, Passenger, Worker, Guard
    }
    
    // ODIN NOT HERE!!
    // UNITY GO BRR
     /// <summary> This will be a class to hold all the different Player Identites.</summary>
     public class PlayerIdentitys
     {
	     [Header("Identity Name")]
	     // This is the name of the Identity.
	     public CharacterIdentity playerIdentityName;
	     [Header("Identity Material")]
	     // The Material of this identity.
	     public Material identityMaterial;
	     // This will be the UI Game Object for the identity when it is selected.
	     [Header("Selected Identity")]
	     public GameObject selectedIdentiyUI;
	     public bool isSelectedIdentity;
	     // Is this the UI game object of the current identity of the player; 
	     [Header("Current Identity")]
	     public GameObject currentIdentityUI;
	     public bool isCurrentIdentity;
     }
}