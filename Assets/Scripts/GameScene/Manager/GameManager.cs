using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingleTon<GameManager>
{
    [SerializeField] private bool isGameOver;

    private void Awake()
    {
        isGameOver = false;
    }
    // 게임 종료를 Ball에게 알려줌
    public bool GetGameOver()
    {
        return isGameOver;
    }
    // FullHP가 0이되면 true를 받음
    public void SetGameOver(bool isgame)
    {
        isGameOver=isgame;
    }
}
