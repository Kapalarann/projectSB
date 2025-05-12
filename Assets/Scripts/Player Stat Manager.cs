using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerStatManager : MonoBehaviour
{
    public static PlayerStatManager instance;

    public CharacterData[] selectedCharacters;
    public InputDevice[] playerDevices;

    public int downedPlayers = 0;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        int maxPlayers = 4;
        selectedCharacters = new CharacterData[maxPlayers];
        playerDevices = new InputDevice[maxPlayers];

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void SetCharacter(int playerIndex, CharacterData character, InputDevice device)
    {
        if (playerIndex >= 0 && playerIndex < selectedCharacters.Length)
        {
            selectedCharacters[playerIndex] = character;
            selectedCharacters[playerIndex].locked = true;
            playerDevices[playerIndex] = device;
        }
    }

    public void UnsetCharacter(int playerIndex)
    {
        selectedCharacters[playerIndex] = null;
        selectedCharacters[playerIndex].locked = false;
        playerDevices[playerIndex] = null;
    }

    public CharacterData GetCharacter(int playerIndex)
    {
        if (playerIndex >= 0 && playerIndex < selectedCharacters.Length)
        {
            return selectedCharacters[playerIndex];
        }
        return null;
    }

    public void LoadScene(string sceneName)
    {
        bool proceed = true;
        int i = 0;
        foreach (CharacterData cd in selectedCharacters)
        {
            if (i + 1 > SelectorController.selectorControllers.Count) continue;
            if (!cd.locked) proceed = false;
            i++;
        }
        if(!proceed) return;
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Only spawn characters in levels that need them
        if (scene.name.Contains("Level") || scene.name.Contains("Battle")) // Customize your rules
        {
            SpawnPlayers();
        }
    }

    private void SpawnPlayers()
    {
        for (int i = 0; i < selectedCharacters.Length; i++)
        {
            GameObject prefab = selectedCharacters[i].characterPrefab;
            if (prefab == null) continue;

            InputDevice device = playerDevices[i];
            Vector3 spawnPos = new Vector3(i * 2f, 0, 0);
            PlayerInput.Instantiate(
            prefab,
            playerIndex: i,
            controlScheme: null,
            splitScreenIndex: -1,
            pairWithDevice: device
            );
        }
    }
}
