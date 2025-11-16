using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TMPAnimDriver : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private Image image;
    [SerializeField] private Image background;

    public void SetText(string text)
    {
        textMesh.text = text;
    }

    public void SetImage(Sprite sprite)
    {
        image.enabled = sprite != null;
        image.sprite = sprite;
    }

    public void DisableBackgroundBlur()
    {
        background.enabled = false;
    }
}
