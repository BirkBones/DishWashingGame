using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

struct Routines {
    
};

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;
    [SerializeField] private AudioSource soundFXObject;
    
    static int AmountOfSounds = 100;

    public Coroutine[] SoundRoutines; // Let each index correspond to a certan coroutine for a certain sound. 

    public bool[] SoundPlaying;

    private bool[] cancelSound; // Track cancellation flags

    // Just so we can more easily instantiate an audiosource later (less code).
    // Also, since the audiosource is copied, all settings will be transferred from
    // the original to the copy.
    void Awake() {
    if (Instance == null) {
        Instance = this;
    }

    if (SoundRoutines == null || SoundRoutines.Length < AmountOfSounds) { //dont want to overwrite values if we call soundfxmanager from a different script.
        SoundRoutines = new Coroutine[AmountOfSounds];
    }

    if (SoundPlaying == null || SoundPlaying.Length < AmountOfSounds) {
        SoundPlaying = new bool[AmountOfSounds];
    }
    if (cancelSound == null || cancelSound.Length < AmountOfSounds){
        cancelSound = new bool[AmountOfSounds];
    }
}

public IEnumerator PlaySoundFXClip(AudioClip audioClip, Transform audioLocation, float Volume, int SoundIndex)
{
    SoundPlaying[SoundIndex] = true;
    cancelSound[SoundIndex] = false;

    AudioSource audioSource = Instantiate(soundFXObject, audioLocation.position, quaternion.identity);
    audioSource.clip = audioClip;
    audioSource.volume = Volume;
    audioSource.Play();

    yield return new WaitUntil(() => !audioSource.isPlaying || cancelSound[SoundIndex]);

    Destroy(audioSource.gameObject);
    cancelSound[SoundIndex] = false;
    SoundPlaying[SoundIndex] = false;
}

public void StopPlaySoundFXClip(int SoundIndex)
{
    cancelSound[SoundIndex] = true; // Signal the coroutine to stop
}

    

}
