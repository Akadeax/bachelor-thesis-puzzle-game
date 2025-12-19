using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TLevelHandler : MonoBehaviour
{

    #region SINGLETON
    public static TLevelHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    private void Start()
    {
        if (FindObjectOfType<SMTutorialHandler>() is null) DontDestroyOnLoad(gameObject);
    }

    [SerializeField] private GameObject winOverlay;
    [SerializeField] private GameObject wrongOverlay;
    [SerializeField] private GameObject finalWinOverlay;
    
    [SerializeField] List<TLevelData> levels = new();
    private int _currentLevelIndex;
    
    [HideInInspector] public bool winOverride;

    private readonly List<float> _trackedTimes = new();
    private readonly List<int> _trackedEdits = new();

    private float _currentTrackedTime;
    public int CurrentTrackedEdits { get; set; }
    
    private void Update()
    {
        _currentTrackedTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.L))
        {
            winOverride = true;
            Check();
        }
    }
    
    public TLevelData GetCurrentLevel()
    {
        return levels[_currentLevelIndex];
    }
    
    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void Check()
    {
        if (THandler.Instance.CheckSolution() || winOverride)
        {
            winOverride = false;
            
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
                File.WriteAllText($"{exeFolder}/YOUR_T_DATA.grad", fileText.ToString());
                
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
        Application.OpenURL("https://forms.gle/FL499Jvrh373Twsj9");
    }

    private void AdvanceLevel()
    {
        // if (isTutorial)
        // {
        //     SceneManager.LoadScene("SMMainScene");
        //     return;
        // }

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
}
