﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Erasable : MonoBehaviour
{
    private Texture2D m_Texture;
    private Color[] m_Colors;
    private SpriteRenderer spriteRend;
    Color zeroAlpha = Color.clear;
    public Vector2Int lastPos;
    private int erSize;
    private Collider2D myCollider;
    private RubberEraser rubberEraser;

    // Start is called before the first frame update
    void Start()
    {
        rubberEraser = FindObjectOfType<RubberEraser>();
        spriteRend = gameObject.GetComponent<SpriteRenderer>();
        myCollider = gameObject.GetComponent<Collider2D>();
        var tex = spriteRend.sprite.texture;
        m_Texture = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);
        m_Texture.filterMode = FilterMode.Bilinear;
        m_Texture.wrapMode = TextureWrapMode.Clamp;
        m_Colors = tex.GetPixels();
        m_Texture.SetPixels(m_Colors);
        m_Texture.Apply();
        spriteRend.sprite = Sprite.Create(m_Texture, spriteRend.sprite.rect, new Vector2(0.5f, 0.5f));
    }

    public void UpdateTexture(Vector2 hitPoint)
    {
        if(myCollider==null)myCollider = gameObject.GetComponent<Collider2D>();

        erSize = rubberEraser.erSize;
        int w = m_Texture.width;
        int h = m_Texture.height;
        var mousePos = hitPoint - (Vector2)myCollider.bounds.min;
        mousePos.x *= w / myCollider.bounds.size.x;
        mousePos.y *= h / myCollider.bounds.size.y;
        Vector2Int p = new Vector2Int((int)mousePos.x, (int)mousePos.y);
        Vector2Int start = new Vector2Int();
        Vector2Int end = new Vector2Int();
        if (!rubberEraser.Drawing)
            lastPos = p;
        start.x = Mathf.Clamp(Mathf.Min(p.x, lastPos.x) - erSize, 0, w);
        start.y = Mathf.Clamp(Mathf.Min(p.y, lastPos.y) - erSize, 0, h);
        end.x = Mathf.Clamp(Mathf.Max(p.x, lastPos.x) + erSize, 0, w);
        end.y = Mathf.Clamp(Mathf.Max(p.y, lastPos.y) + erSize, 0, h);
        Vector2 dir = p - lastPos;
        for (int x = start.x; x < end.x; x++)
        {
            for (int y = start.y; y < end.y; y++)
            {
                Vector2 pixel = new Vector2(x, y);
                Vector2 linePos = p;
                if (rubberEraser.Drawing)
                {
                    float d = Vector2.Dot(pixel - lastPos, dir) / dir.sqrMagnitude;
                    d = Mathf.Clamp01(d);
                    linePos = Vector2.Lerp(lastPos, p, d);
                }
                if ((pixel - linePos).sqrMagnitude <= erSize * erSize)
                {
                    m_Colors[x + y * w] = zeroAlpha;
                }
            }
        }
        lastPos = p;
        m_Texture.SetPixels(m_Colors);
        m_Texture.Apply();
        spriteRend.sprite = Sprite.Create(m_Texture, spriteRend.sprite.rect, new Vector2(0.5f, 0.5f));
    }
}
