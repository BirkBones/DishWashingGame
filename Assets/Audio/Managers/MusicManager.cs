using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Serialization;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

//MusicManager is used for playing sounds that have no location (aka positioned at the same place as the camera)
public class MusicManager : MonoBehaviour
{   
    public static MusicManager Instance;
    [SerializeField] private AudioSource soundFXObject;
    public AudioClip[] Pieces;
    [SerializeField] private Transform PlayerTransform;

    private Dictionary<int, SoundData> soundDataMap = new Dictionary<int, SoundData>();
    private Queue<int> availableSoundIndices = new Queue<int>();
    private int nextIndex = 0;

    void Awake() {
    if (Instance == null) {
        Instance = this;
        Subscribe();
        }
    }
    void Update (){
       
    }
    private void Subscribe (){
    }

    private int GetNextAvailableSoundIndex()
    {
        if (availableSoundIndices.Count > 0)
        {
            return availableSoundIndices.Dequeue();
        }
        return nextIndex++;
    }

    private void RecycleSoundIndex(int soundIndex)
    {
        availableSoundIndices.Enqueue(soundIndex);
    }

     AudioClip ChooseRandomSoundFromArray(AudioClip[] soundArray, out int RandomSoundIndex){
        RandomSoundIndex = Random.Range(0,soundArray.Length);
        AudioClip randomSound = soundArray[RandomSoundIndex];
        return randomSound;

    }

    public void StopPlaySoundFXClip(int SoundIndex)
    {
        soundDataMap[SoundIndex].CancelSound = true;
    }
    public void PlaySoundFXClip(AudioClip audioClip, Transform audioLocation, float Volume, out int GeneratedSoundIndex){
        SoundData inst = new SoundData(true,false);
        int SoundIndex = GetNextAvailableSoundIndex();
        GeneratedSoundIndex = SoundIndex;
        soundDataMap.Add(SoundIndex,inst);
        StartCoroutine(PlaySoundFXClipCoroutine(audioClip, audioLocation, Volume, SoundIndex));

    }

    public IEnumerator PlaySoundFXClipCoroutine(AudioClip audioClip, Transform audioLocation, float Volume, int SoundIndex)
    {
        AudioSource audioSource = Instantiate(soundFXObject, audioLocation.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = Volume;
        audioSource.Play();

        yield return new WaitUntil(() => !audioSource.isPlaying || soundDataMap[SoundIndex].CancelSound);

        Destroy(audioSource.gameObject);
        soundDataMap.Remove(SoundIndex);
        RecycleSoundIndex(SoundIndex);
    }

    public void PlayContinuousRandomSoundFromArrayFX(AudioClip[] soundArray, Transform audioLocation, float volume, out int GeneratedSoundIndex){
        SoundData inst = new SoundData(true,false);
        int SoundIndex = GetNextAvailableSoundIndex();
        GeneratedSoundIndex = SoundIndex;
        soundDataMap.Add(SoundIndex,inst);
        StartCoroutine(PlayContinuousRandomSoundFromArrayFXCoroutine(soundArray, audioLocation, volume, SoundIndex));
    }
    public IEnumerator PlayContinuousRandomSoundFromArrayFXCoroutine(AudioClip[] soundArray, Transform audioLocation, float volume, int SoundIndex)
{
    AudioSource audioSource = Instantiate(soundFXObject, audioLocation.position, Quaternion.identity);
    while (!soundDataMap[SoundIndex].CancelSound) 
    {
        AudioClip currentClip = ChooseRandomSoundFromArray(soundArray, out _);
        audioSource.clip = currentClip;
        audioSource.volume = volume;
        audioSource.Play();

        yield return new WaitUntil(() => soundDataMap[SoundIndex].CancelSound || !audioSource.isPlaying);

    }
        Destroy(audioSource.gameObject);
        soundDataMap.Remove(SoundIndex);
        RecycleSoundIndex(SoundIndex);
    
}
    public void PlayContinuousSoundFX(AudioClip audioClip, Transform audioLocation, float volume, out int GeneratedSoundIndex){
        SoundData inst = new SoundData(true,false);
        int SoundIndex = GetNextAvailableSoundIndex();
        GeneratedSoundIndex = SoundIndex;
        soundDataMap.Add(SoundIndex,inst);
        StartCoroutine(PlayContinuousSoundFXCoroutine(audioClip, audioLocation, volume, SoundIndex));
    }

    public IEnumerator PlayContinuousSoundFXCoroutine(AudioClip audioClip, Transform audioLocation, float volume, int SoundIndex)
    {
        AudioSource audioSource = Instantiate(soundFXObject, audioLocation.position, Quaternion.identity);

        while (!soundDataMap[SoundIndex].CancelSound)
        {
            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.Play();
            yield return new WaitUntil(() => !audioSource.isPlaying || soundDataMap[SoundIndex].CancelSound);
        }
        Destroy(audioSource.gameObject);
        soundDataMap.Remove(SoundIndex);
        RecycleSoundIndex(SoundIndex);
    }

    public void PlayBackgroundMusic(AudioClip audioClip, float volume, out int GeneratedSoundIndex){
        PlayContinuousSoundFX(audioClip, MusicManager.Instance.PlayerTransform, volume, out GeneratedSoundIndex);

    }
    public void StopBackgroundMusic(int cancelSoundIndex){
        StopPlaySoundFXClip(cancelSoundIndex);
    }

}
