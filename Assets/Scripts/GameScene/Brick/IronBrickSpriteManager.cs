using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IronBrickSpriteManager : MonoBehaviour
{
    [SerializeField] private Sprite broken1;
    [SerializeField] private Sprite broken2;

    [SerializeField] private SpriteRenderer[] spriteRenderer;

    [SerializeField] private Brick brick = null;

    private void OnEnable()
    {
        brick = GetComponentInParent<Brick>();
        spriteRenderer = GetComponentsInParent<SpriteRenderer>();
    }

    private void Update()
    {
        if (brick.CurHP <= 20)
        {
            spriteRenderer[1].sprite = broken1;
        }
        if (brick.CurHP <= 10)
        {
            spriteRenderer[1].sprite = broken2;
        }
        if (brick.CurHP <= 0)
        {
            spriteRenderer[1].sprite = null;
        }
    }

}
