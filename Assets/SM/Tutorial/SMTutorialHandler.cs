using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[System.Serializable]
public class SMTutorialStep
{
    [Header("Text")]
    [TextArea]
    public string displayedText;
    
    [Header("Player Allowance")]
    public bool interactable;
    public bool darkBackground;
    public bool nodesVisible;
    public bool blackboardVisible;
    public bool transitionsVisible;

    [Header("Elements")]
    public bool keepPreviousElements = true;
    public List<SMInitialNode> nodes = new();
    public List<SMInitialTransition> transitions = new();
}

public class SMTutorialHandler : MonoBehaviour
{
    public static SMTutorialHandler Instance { get; private set; }
    public static bool IsInTutorial => Instance is not null;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    private void Start()
    {
        EnterNextStep();
    }

    [SerializeField] private Image darkBackground;
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private List<SMTutorialStep> steps = new();

    private int currentStepIndex = -1;

    [CanBeNull]
    public SMTutorialStep CurrentStep => currentStepIndex == -1 ? null : steps[currentStepIndex];

    public void EnterNextStep()
    {
        if (currentStepIndex == steps.Count - 1) return;
        
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
        
        if (!CurrentStep.keepPreviousElements)
        {
            SMHandler.Instance.Restart();
            SMHandler.Instance.SpawnFromLevelData(CurrentStep.nodes, CurrentStep.transitions);
            FindObjectOfType<SMPlayerAnimator>().Initialize();
        }
        
        displayText.text = CurrentStep.displayedText;
        darkBackground.enabled = CurrentStep.darkBackground;
        
        foreach (SMNode node in FindObjectsOfType<SMNode>(true))
        {
            node.gameObject.SetActive(CurrentStep.nodesVisible);
            node.Interactable = CurrentStep.interactable;
        }
        foreach (SMTransition transition in FindObjectsOfType<SMTransition>(true))
        {
            transition.gameObject.SetActive(CurrentStep.transitionsVisible);
            transition.Interactable = CurrentStep.interactable;
        }
        foreach (SMFieldDisplay blackboardField in FindObjectsOfType<SMFieldDisplay>(true))
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
