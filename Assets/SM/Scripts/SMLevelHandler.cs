using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
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

    private List<float> _trackedTimes = new();
    private List<int> _trackedEdits = new();

    private float _currentTrackedTime;
    public int CurrentTrackedEdits { get; set; } = 0;
    
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

    private void Update()
    {
        _currentTrackedTime += Time.deltaTime;
    }

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

        _trackedTimes.Add(_currentTrackedTime);
        _currentTrackedTime = 0f;
        
        _trackedEdits.Add(CurrentTrackedEdits);
        CurrentTrackedEdits = 0;
        
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
                _trackedTimes.Add(_currentTrackedTime);
                _trackedEdits.Add(CurrentTrackedEdits);
                
                string exeFolder = Path.GetDirectoryName(Application.dataPath);
                StringBuilder fileText = new();
                for (int i = 1; i < _trackedTimes.Count + 1; i++)
                {
                    fileText.AppendLine($"Level {i}: {_trackedTimes[i - 1]} seconds & {_trackedEdits[i - 1]} edits");
                }
                File.WriteAllText($"{exeFolder}/YOUR_DATA.grad", fileText.ToString());
                
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
        Time.timeScale = 1f;
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
        yield return new WaitForSecondsRealtime(5f);
        Application.OpenURL("https://bit.ly/grad-fsm");
        
        
    }
}