using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class KeyDisplay : MonoBehaviour
{
	[SerializeField, Tooltip("The Text which tells the player how many keys they have and need.")] private TextMeshProUGUI keysRemainingTextDisplay;
	[SerializeField, Tooltip("The bar that displays the amount of keys still needed visually")] private Image keyBar;

	private void Update()
	{
		// update the text and bar to match the remaining number of keys.
		if(keysRemainingTextDisplay)
		{
			keysRemainingTextDisplay.text = $"{Key.numberOfKeysCollected}/{Key.requiredNumberOfKeys} Keys";
		}
		keyBar.fillAmount = (float)Key.numberOfKeysCollected / (float)Key.requiredNumberOfKeys;
	}
}
