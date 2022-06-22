using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    class SoundManager : MonoBehaviour
    {
        [SerializeField]
        Slider volumeSlider;
        [SerializeField]
        AudioMixer Mixer;

        private void Awake()
        {
            if (!PlayerPrefs.HasKey("musicVolume"))
            {
                PlayerPrefs.SetFloat("musicVolume", 1);
                Load();
            }
            else
            {
                Load();
            }
        }

        private void Load()
        {
            volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
        }

        public void ChangeVolume()
        {
            Mixer.SetFloat("MasterVolume", Mathf.Lerp(-80, 0, volumeSlider.value));
            Save();
        }

        private void Save()
        {
            PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
        }
    }
}
