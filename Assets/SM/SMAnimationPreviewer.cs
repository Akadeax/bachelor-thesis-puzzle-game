using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMAnimationPreviewer : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private SMNode _node;

    private float _currentTimeUntilNextFrame;
    private int _currentFrame;
    
    private void Awake()
    {
        _node = transform.parent.GetComponent<SMNode>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        _spriteRenderer.sprite = _node.NodeAnimation.sprites[_currentFrame];
        
        if (_currentTimeUntilNextFrame <= 0f)
        {
            NextFrame();
            _currentTimeUntilNextFrame = _node.NodeAnimation.timeBetweenFrames;
            return;
        }
    
        _currentTimeUntilNextFrame -= Time.deltaTime;
    }

    private void NextFrame()
    {
        if (_node.NodeAnimation.sprites.Count == 0) return;
        
        _currentFrame++;
        if (_currentFrame >= _node.NodeAnimation.sprites.Count) _currentFrame = 0;
    }
}
