using UnityEngine;
using UnityEngine.Audio;

//Soundmixer manager handles all treatment of sound
public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void SetMasterVolume(float level){
        audioMixer.SetFloat("MasterVolume",ConvertFromLinearToDecibels(level));
    }

    public void SetSoundFXVolume(float level){
        audioMixer.SetFloat("SoundFXVolume",ConvertFromLinearToDecibels(level));

    }

    public void SetMusicVolume(float level){
        audioMixer.SetFloat("MusicVolume",ConvertFromLinearToDecibels(level));

    }
    private float ConvertFromLinearToDecibels(float linear){
        if (linear == 0){
            return -80;
        }
        return 20 * Mathf.Log10(linear);
    }
}
