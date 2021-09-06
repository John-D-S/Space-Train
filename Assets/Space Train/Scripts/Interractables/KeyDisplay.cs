using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class KeyDisplay : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI keysRemainingTextDisplay;
	[SerializeField] private Image keyBar;

	private void Update()
	{
		if(keysRemainingTextDisplay)
		{
			keysRemainingTextDisplay.text = $"{Key.numberOfKeysCollected}/{Key.requiredNumberOfKeys} Keys";
		}
		keyBar.fillAmount = (float)Key.numberOfKeysCollected / (float)Key.requiredNumberOfKeys;
	}
}
