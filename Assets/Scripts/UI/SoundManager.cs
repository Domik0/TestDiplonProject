using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Unity.Mathematics;

namespace Assets.Scripts.UI
{
    class SoundManager:MonoBehaviour
    {
        [SerializeField] 
        Slider volumeSlider;
        [SerializeField]
        AudioMixer Mixer;

        private void Start()
        {
            if (!PlayerPrefs.HasKey("MasterVolume"))
            {
                PlayerPrefs.SetFloat("MasterVolume", 1);
                Load();
            }
            else
            {
                Load();
            }
        }

        private void Load()
        {
            volumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        }

        public void ChangeVolume(float volume)
        {
           Mixer.SetFloat("MasterVolume", Mathf.Lerp(-80,0,volume));
           Save();
        }

        private void Save()
        {
            PlayerPrefs.SetFloat("MasterVolume", volumeSlider.value);
        }

    }
}
