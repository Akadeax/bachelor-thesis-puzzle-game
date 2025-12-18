using UnityEngine;

public class TAnimationPreviewer : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private TNode _node;

    private float _currentTimeUntilNextFrame;
    private int _currentFrame;
    
    private void Awake()
    {
        _node = transform.parent.GetComponent<TNode>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (_node.Node is not TreeNodeAnimation tAnim)
        {
            _spriteRenderer.sprite = null;
            return;
        }
        
        if (_currentFrame >= tAnim.Animation.sprites.Count) _currentFrame = 0;
        _spriteRenderer.sprite = tAnim.Animation.sprites[_currentFrame];
        
        if (_currentTimeUntilNextFrame <= 0f)
        {
            NextFrame();
            _currentTimeUntilNextFrame = tAnim.Animation.timeBetweenFrames;
            return;
        }
    
        _currentTimeUntilNextFrame -= Time.deltaTime;
    }

    private void NextFrame()
    {
        if (_node.Node is not TreeNodeAnimation tAnim) return;
        
        if (tAnim.Animation.sprites.Count == 0) return;
        
        _currentFrame++;
        if (_currentFrame >= tAnim.Animation.sprites.Count) _currentFrame = 0;
    }
}
