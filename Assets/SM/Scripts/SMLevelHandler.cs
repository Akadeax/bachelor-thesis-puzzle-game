using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SMLevelHandler : MonoBehaviour
{
    [SerializeField] private bool isTutorial;
    [SerializeField] private SMLevelData tutorialLevelData;
    public bool IsTutorial => isTutorial;
    
    #region SINGLETON

    public static SMLevelHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if(!isTutorial) DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    [SerializeField] List<SMLevelData> levels = new();
    private int _currentLevelIndex;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AdvanceLevel();
        }
    }

    public SMLevelData GetCurrentLevel()
    {
        return isTutorial ? tutorialLevelData : levels[_currentLevelIndex];
    }

    public void AdvanceLevel()
    {
        if (isTutorial)
        {
            SceneManager.LoadScene("SMMainScene");
            return;
        }
        
        if (_currentLevelIndex == levels.Count - 1)
        {
            print("WIN!");
            return;
        }

        _currentLevelIndex++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}