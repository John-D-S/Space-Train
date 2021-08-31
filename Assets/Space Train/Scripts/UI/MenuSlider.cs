using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace SpaceTrain.UI
{

    public enum MenuSliderType
    {

        Music,
        Sound_Effects

    }

    public class MenuSlider : MonoBehaviour
    {
        [SerializeField, Tooltip("What The Slider controls")]
        private MenuSliderType sliderType = MenuSliderType.Music;

        [SerializeField, HideInInspector]
        private Slider slider;
        [SerializeField, HideInInspector]
        private TextMeshProUGUI sliderText;

        private MenuHandler menuHandler;

        private void OnValidate()
        {
            
            if (!slider)
                slider = GetComponent<Slider>();
            if (!sliderText)
                sliderText = GetComponentInChildren<TextMeshProUGUI>();
            slider.maxValue = 1;
            slider.minValue = 0;

            //set the text of the button to be the selected MenuSliderType text
            sliderText.text = sliderType.ToString().Replace("_", " ");
            //remove all the listeners of the slider because they will cause an error
            slider.onValueChanged.RemoveAllListeners();
        }

        private void Start()
        {
            //set the menu handler
            menuHandler = TheMenuHandler.theMenuHandler;
            //set the onvaluchanged listener
            slider.onValueChanged.AddListener(PerformFunction);
            //set the initial slider position to its respective position according to playerprefs
            switch (sliderType)
            {
                case MenuSliderType.Music:
                    slider.value = Mathf.Pow((PlayerPrefs.GetFloat("MusicVolume") + 80) / 80, 2);
                    break;
                case MenuSliderType.Sound_Effects:
                    slider.value = Mathf.Pow((PlayerPrefs.GetFloat("SFXVolume") + 80) / 80, 2);
                    break;
                default:
                    break;
            }

        }

        private void PerformFunction(float _value)
        {
            //perform the appropriate function of the slider according to the selected SliderType
            //the maths in here are to turn a value from 0-1 to -80-0
            switch (sliderType)
            {
                case MenuSliderType.Music:
                    menuHandler.ChangeMusicVolume(Mathf.Sqrt(_value) * 80 - 80);
                    break;
                case MenuSliderType.Sound_Effects:
                    menuHandler.ChangeSFXVolume(Mathf.Sqrt(_value) * 80 - 80);
                    break;
                default:
                    break;
            }
        }
    }
}