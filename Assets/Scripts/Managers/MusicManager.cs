using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

public class MusicManager : MonoBehaviour
{
    static MusicManager instance;
    [SerializeField] private AudioSource MusicObject; 

    void Awake(){
        if (instance!=null){
            instance = this;
        }
    }
    public void PlaySoundFXClip(AudioClip audioClip, Transform audioLocation, float Volume){
        AudioSource audioSource = Instantiate(MusicObject, audioLocation.position, quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = Volume;
        audioSource.Play();
        float _clipLength = audioClip.length;
        Destroy(audioSource.gameObject, _clipLength);

    }
}
