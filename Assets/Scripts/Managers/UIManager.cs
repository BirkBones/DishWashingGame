using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] BrushMovement brushScript;
    [SerializeField] GameObject settings;
    [SerializeField] GameObject Startingmenu;
    [SerializeField] Button StartButton;
    [SerializeField] Button QuitButton;
    [SerializeField] Button SettingsButton;
    [SerializeField] Button BackButton;
    [SerializeField] Slider MasterVolume;
    [SerializeField] Slider SoundFXVolume;
    [SerializeField] Slider MusicVolume;
    PlayerInput playerInput;
    private bool GameHasStarted = false;
    private int BackGroundMusicIndex;
    void Awake(){
        
        playerInput = FindAnyObjectByType<PlayerInput>();

    }
    void OnEnable(){
        brushScript.enabled = false;
        settings.SetActive(false);
        Startingmenu.SetActive(true);
        
        StartButton.onClick.AddListener(PlayGame);
        QuitButton.onClick.AddListener(QuitGame);
        SettingsButton.onClick.AddListener(ToggleSettings);
        BackButton.onClick.AddListener(ToggleSettings);

        playerInput.actions["OpenOptionsMenu"].canceled += _ => ToggleSettings();
        MusicManager.Instance.PlayBackgroundMusic(MusicManager.Instance.Pieces[0], 1, out BackGroundMusicIndex);

    }
    void Update(){

    }
    private void PlayGame(){
        brushScript.enabled = true;
        GameHasStarted = true;
        Startingmenu.SetActive(false);
        MusicManager.Instance.StopBackgroundMusic(BackGroundMusicIndex);
    }
    private void QuitGame(){
        Application.Quit();
    }
    private void ToggleSettings(){
        settings.SetActive(!settings.activeSelf); // toggle settings menu.
        Startingmenu.SetActive(!(GameHasStarted || settings.activeSelf)); //if game has not started, and settings menu is not active, reactivate the starting menu. 
        brushScript.enabled = !(settings.activeSelf || Startingmenu.activeSelf); //disable the brush movement correctly
    }
}
