using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class SMPlayerAnimator : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    [CanBeNull] private SMNode _currentNode;
    [CanBeNull] private SMAnimation CurrentAnimation => _currentNode?.NodeAnimation;

    private float _animTimeInState;
    private int _frame;
    private Vector3 startPos;

    [HideInInspector] public int behaviorIndex = -1;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        startPos = transform.position;

    }

    private void Start()
    {
        behaviorIndex = SMHandler.Instance.smLevelData.playerBehaviorIndex;
        Initialize();
    }

    public void Initialize()
    {
        
        StopAllCoroutines();
        transform.position = startPos;

        // Add new behaviors here
        IEnumerator coroutine = behaviorIndex switch
        {
            0 => PlayerBehaviorIdle(),
            1 => PlayerBehaviorWalkBackForth(),
            2 => PlayerBehaviorRunBackForth(),
            _ => null
        };
        StartCoroutine(coroutine);

        if (SMHandler.Instance.Nodes.Count == 0) return;
        
        TransitionState(SMHandler.Instance.Nodes[0]);
    }

    #region BEHAVIOR

    IEnumerator PlayerBehaviorIdle()
    {
        while (true)
        {
            yield return StartCoroutine(DoIdle(2.5f));
        }
        // ReSharper disable once IteratorNeverReturns
    }
    
    IEnumerator PlayerBehaviorWalkBackForth()
    {
        while (true)
        {
            yield return StartCoroutine(DoIdle(2.5f));
            yield return StartCoroutine(DoWalk(1.5f, 1));
            yield return StartCoroutine(DoIdle(1.7f));
            yield return StartCoroutine(DoWalk(2.5f, -1));
            yield return StartCoroutine(DoIdle(0.8f));
            yield return StartCoroutine(DoWalk(1f, 1));
            
        }
        // ReSharper disable once IteratorNeverReturns
    }
    
    IEnumerator PlayerBehaviorRunBackForth()
    {
        while (true)
        {
            yield return StartCoroutine(DoIdle(1f));
            yield return StartCoroutine(DoWalk(1.5f, 1));
            yield return StartCoroutine(DoIdle(1f));
            yield return StartCoroutine(DoRun(1.5f, -1));
            yield return StartCoroutine(DoIdle(1.8f));
            yield return StartCoroutine(DoWalk(1.5f, 1));
            yield return StartCoroutine(DoRun(0.75f, 1));
            yield return StartCoroutine(DoIdle(0.3f));
            yield return StartCoroutine(DoWalk(1.5f, -1));
            
        }
        // ReSharper disable once IteratorNeverReturns
    }

    private IEnumerator DoIdle(float time)
    {
        SMHandler.Instance.Blackboard.GetField("Is Walking").value = false;
        yield return new WaitForSeconds(time);
    }

    private IEnumerator DoWalk(float time, int dir, float speed = 1.5f)
    {
        _spriteRenderer.flipX = dir == -1;
        SMHandler.Instance.Blackboard.GetField("Is Walking").value = true;
        float timer = 0f;
        while (timer < time)
        {
            timer += Time.deltaTime;
            yield return null;
            transform.Translate(Vector3.right * (dir * speed * Time.deltaTime));
        }

        SMHandler.Instance.Blackboard.GetField("Is Walking").value = false;
    }
    
    private IEnumerator DoRun(float time, int dir, float speed = 3f)
    {
        _spriteRenderer.flipX = dir == -1;
        SMHandler.Instance.Blackboard.GetField("Is Running").value = true;
        SMHandler.Instance.Blackboard.GetField("Is Walking").value = true;
        float timer = 0f;
        while (timer < time)
        {
            timer += Time.deltaTime;
            yield return null;
            transform.Translate(Vector3.right * (dir * speed * Time.deltaTime));
        }

        SMHandler.Instance.Blackboard.GetField("Is Running").value = false;
        SMHandler.Instance.Blackboard.GetField("Is Walking").value = false;
    }

    #endregion

    private void Update()
    {
        if (SMHandler.Instance.Nodes.Count == 0)
        {
            Initialize();
            return;
            
        }
        HandleTransitions();
        DisplayCurrentFrame();
    }

    
    private void HandleTransitions()
    {
        foreach (var trans in _currentNode!.transitions)
        {
            if (trans.associatedField == null) continue;

            bool isInCorrectState = _currentNode == trans.From;
            bool isCorrectValue = trans.associatedField.value == trans.associatedValue;

            if (!isInCorrectState || !isCorrectValue) continue;
            TransitionState(trans.To);
            trans.MarkAsUsed();
            return;
        }
    }

    private void DisplayCurrentFrame()
    {
        _animTimeInState += Time.deltaTime;

        if (_animTimeInState >= CurrentAnimation!.timeBetweenFrames)
        {
            _animTimeInState = 0;
            _frame++;
            if (_frame >= CurrentAnimation.sprites.Count)
            {
                _frame = 0;
            }
        }

        _spriteRenderer.sprite = CurrentAnimation.sprites[_frame];
    }

    private void TransitionState(SMNode newState)
    {
        _currentNode?.DeactivateNode();
        _currentNode = newState;
        _currentNode!.ActivateNode();

        _animTimeInState = 0;
        _frame = 0;
    }
}