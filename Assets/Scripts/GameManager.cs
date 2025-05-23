using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnPause();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            RestartLevel();
        }
    }

    public void OnPause()
    {
        isPaused = !isPaused;
        Time.timeScale = (isPaused) ? 0 : 1;
    }

    public void RestartLevel()
    {
        PlayerMovement.Players.Clear();
        EnemyStateManager.Enemies.Clear();
        PlayerStatManager.instance.downedPlayers = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

