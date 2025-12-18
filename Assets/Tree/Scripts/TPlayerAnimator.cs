using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class TPlayerAnimator : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;


    private float _animTimeInState;
    private int _frame;
    private Vector3 startPos;

    [HideInInspector] public int behaviorIndex = -1;

    private GAnimation currentAnimation;
    private GAnimation lastFrameAnimation;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        startPos = transform.position;

    }

    private void Start()
    {
        behaviorIndex = THandler.Instance.levelData.playerBehaviorIndex;
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
            3 => PlayerBehaviorRunBackForthJump(),
            4 => PlayerBehaviorRunBackForthJumpPunch(),
            _ => null
        };
        StartCoroutine(coroutine);
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
    
    IEnumerator PlayerBehaviorRunBackForthJump()
    {
        while (true)
        {
            yield return StartCoroutine(DoIdle(1f));
            yield return StartCoroutine(DoWalk(1.5f, 1));
            yield return StartCoroutine(DoIdle(1f));
            yield return StartCoroutine(DoJump());
            yield return StartCoroutine(DoIdle(0.3f));
            yield return StartCoroutine(DoRun(0.5f, -1));
            StartCoroutine(DoJump());
            yield return StartCoroutine(DoRun(1.0f, -1));
            yield return StartCoroutine(DoIdle(1.8f));
            yield return StartCoroutine(DoWalk(1.5f, 1));
            yield return StartCoroutine(DoRun(0.75f, 1));
            yield return StartCoroutine(DoIdle(0.3f));
            yield return StartCoroutine(DoWalk(0.2f, -1));
            StartCoroutine(DoJump());
            yield return StartCoroutine(DoWalk(1.3f, -1));
            
        }
        // ReSharper disable once IteratorNeverReturns
    }
    
    IEnumerator PlayerBehaviorRunBackForthJumpPunch()
    {
        while (true)
        {
            yield return StartCoroutine(DoIdle(1f));
            yield return StartCoroutine(DoPunch());
            yield return StartCoroutine(DoIdle(0.5f));
            yield return StartCoroutine(DoWalk(1.5f, 1));
            yield return StartCoroutine(DoIdle(1f));
            yield return StartCoroutine(DoJump());
            yield return StartCoroutine(DoIdle(0.3f));
            yield return StartCoroutine(DoRun(0.5f, -1));
            StartCoroutine(DoJump());
            yield return StartCoroutine(DoRun(1.0f, -1));
            yield return StartCoroutine(DoIdle(1.8f));
            yield return StartCoroutine(DoPunch());
            yield return StartCoroutine(DoIdle(0.5f));
            yield return StartCoroutine(DoWalk(1.5f, 1));
            yield return StartCoroutine(DoRun(0.75f, 1));
            yield return StartCoroutine(DoIdle(0.3f));
            yield return StartCoroutine(DoPunch());
            yield return StartCoroutine(DoIdle(0.5f));
            yield return StartCoroutine(DoWalk(0.2f, -1));
            StartCoroutine(DoJump());
            yield return StartCoroutine(DoWalk(1.3f, -1));
            
        }
        // ReSharper disable once IteratorNeverReturns
    }

    private IEnumerator DoIdle(float time)
    {
        THandler.Instance.Blackboard.GetField("Is Walking").value = false;
        yield return new WaitForSeconds(time);
    }

    private IEnumerator DoWalk(float time, int dir, float speed = 1.5f)
    {
        _spriteRenderer.flipX = dir == -1;
        THandler.Instance.Blackboard.GetField("Is Walking").value = true;
        float timer = 0f;
        while (timer < time)
        {
            timer += Time.deltaTime;
            yield return null;
            transform.Translate(Vector3.right * (dir * speed * Time.deltaTime));
        }

        THandler.Instance.Blackboard.GetField("Is Walking").value = false;
    }
    
    private IEnumerator DoRun(float time, int dir, float speed = 3f)
    {
        _spriteRenderer.flipX = dir == -1;
        THandler.Instance.Blackboard.GetField("Is Running").value = true;
        THandler.Instance.Blackboard.GetField("Is Walking").value = true;
        float timer = 0f;
        while (timer < time)
        {
            timer += Time.deltaTime;
            yield return null;
            transform.Translate(Vector3.right * (dir * speed * Time.deltaTime));
        }

        THandler.Instance.Blackboard.GetField("Is Running").value = false;
        THandler.Instance.Blackboard.GetField("Is Walking").value = false;
    }
    
    private IEnumerator DoJump(float jumpTime = 0.7f)
    {
        THandler.Instance.Blackboard.GetField("Is Jumping").value = true;
        float timer = 0f;
        float startingY = transform.position.y;
        while (timer < jumpTime)
        {
            timer += Time.deltaTime;
            transform.position = new Vector3(transform.position.x, startingY + JumpFunction(timer / jumpTime) * 3f, 0);
            yield return null;
        }
        THandler.Instance.Blackboard.GetField("Is Jumping").value = false;
        transform.position = new Vector3(transform.position.x, startingY, 0);
    }

    private static float JumpFunction(float x)
    {
        float inner = 2 * x - 1;
        float outer = inner * inner * -1 + 1;
        return outer;
        // f(x) = -(2x - 1)^2 + 1
    }
    
    private IEnumerator DoPunch(float punchTime = 0.75f)
    {
        THandler.Instance.Blackboard.GetField("Is Punching").value = true;
        yield return new WaitForSeconds(punchTime);
        THandler.Instance.Blackboard.GetField("Is Punching").value = false;
    }

    #endregion

    private void Update()
    {
        if (THandler.Instance is null || THandler.Instance.RootNode is null || THandler.Instance.RootNode.Node is null) return;
        
        currentAnimation = THandler.Instance.RootNode.Node.Resolve();
        lastFrameAnimation = currentAnimation;

        if (currentAnimation != lastFrameAnimation)
        {
            _animTimeInState = 0f;
            _frame = 0;
        }
        
        DisplayCurrentFrame();
    }
    
    private void DisplayCurrentFrame()
    {
        if (currentAnimation is null) return;
        
        _animTimeInState += Time.deltaTime;

        if (_animTimeInState >= currentAnimation.timeBetweenFrames)
        {
            _animTimeInState = 0;
            _frame++;
            if (_frame >= currentAnimation.sprites.Count)
            {
                _frame = 0;
            }
        }

        if (_frame >= currentAnimation.sprites.Count) _frame = 0;
        _spriteRenderer.sprite = currentAnimation.sprites[_frame];
    }
}