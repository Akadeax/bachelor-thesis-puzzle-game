using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SMLevelHandler : MonoBehaviour
{
    [SerializeField] private bool isTutorial;
    [SerializeField] private SMLevelData tutorialLevelData;
    [SerializeField] private GameObject winOverlay;
    [SerializeField] private GameObject wrongOverlay;
    [SerializeField] private GameObject finalWinOverlay;
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

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Check()
    {
        if (SMHandler.Instance.CheckSolution())
        {
            if (_currentLevelIndex == levels.Count - 1)
            {
                StartCoroutine(FinalWinCoroutine());
                return;
            }
            StartCoroutine(WinCoroutine());
            return;
        }
        StartCoroutine(WrongCoroutine());
    }

    IEnumerator WinCoroutine()
    {
        winOverlay.SetActive(true);
        Time.timeScale = 0.01f;
        yield return new WaitForSecondsRealtime(1f);
        AdvanceLevel();
        winOverlay.SetActive(false);
    }
    
    IEnumerator WrongCoroutine()
    {
        wrongOverlay.SetActive(true);
        yield return new WaitForSeconds(1f);
        wrongOverlay.SetActive(false);
    }
    
    IEnumerator FinalWinCoroutine()
    {
        finalWinOverlay.SetActive(true);
        yield return new WaitForSecondsRealtime(6f);
        Application.OpenURL("https://google.com");
    }
}