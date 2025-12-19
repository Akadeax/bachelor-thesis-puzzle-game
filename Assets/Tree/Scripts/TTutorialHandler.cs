using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class TTutorialStep
{
    [Header("Text")]
    [TextArea]
    public string displayedText;
    
    [Header("Player Allowance")]
    public bool interactable;
    public bool darkBackground;
    public bool nodesVisible;
    public bool blackboardVisible;
    public bool convertible;
    public int playerBehaviorIndex;
}

public class TTutorialHandler : MonoBehaviour
{
    public static TTutorialHandler Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    private GameObject _blackboard;
    private void Start()
    {
        _blackboard = GameObject.Find("Blackboard");
        EnterNextStep();
    }

    [SerializeField] private Image darkBackground;
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private List<TTutorialStep> steps = new();

    private int currentStepIndex = -1;

    [CanBeNull]
    public TTutorialStep CurrentStep => currentStepIndex == -1 ? null : steps[currentStepIndex];

    public void EnterNextStep()
    {
        if (currentStepIndex == steps.Count - 1)
        {
            SceneManager.LoadScene("TMainScene");
            return;
        }
        
        currentStepIndex++;
        InitializeCurrentStep();
    }
    
    public void EnterPreviousStep()
    {
        if (currentStepIndex == 0) return;
        
        currentStepIndex--;
        InitializeCurrentStep();
    }
    
    public void InitializeCurrentStep()
    {
        Assert.IsNotNull(CurrentStep);
        
        _blackboard.SetActive(CurrentStep.blackboardVisible);
        
        FindObjectOfType<TPlayerAnimator>().behaviorIndex = CurrentStep.playerBehaviorIndex;
        FindObjectOfType<TPlayerAnimator>().Initialize();
        
        displayText.text = CurrentStep.displayedText;
        darkBackground.enabled = CurrentStep.darkBackground;
        
        foreach (TNode node in FindObjectsOfType<TNode>(true))
        {
            node.gameObject.SetActive(CurrentStep.nodesVisible);
            node.Interactable = CurrentStep.interactable;
        }
        foreach (TFieldDisplay blackboardField in FindObjectsOfType<TFieldDisplay>(true))
        {
            blackboardField.gameObject.SetActive(CurrentStep.blackboardVisible);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            EnterPreviousStep();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            EnterNextStep();
        }
    }
}
