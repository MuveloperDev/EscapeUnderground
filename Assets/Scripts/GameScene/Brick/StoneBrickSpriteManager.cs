using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoneBrickSpriteManager : MonoBehaviour
{
    [SerializeField] private Sprite broken;

    [SerializeField] private SpriteRenderer[] spriteRenderer;
    [SerializeField] private Brick brick = null;

    private void Awake()
    {
        brick = GetComponentInParent<Brick>();
        spriteRenderer = GetComponentsInParent<SpriteRenderer>();
    }
    
    void Update()
    {
        if (brick.CurHP <= 10)
        {
            spriteRenderer[1].sprite = broken;
        }
        if (brick.CurHP <= 0)
        {
            spriteRenderer[1].sprite = null;
        }
    }
}
