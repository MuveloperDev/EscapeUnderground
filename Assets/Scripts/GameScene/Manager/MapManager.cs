using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private bool[,] isPlaced = null;
    Vector2[,] board = null;
    private float scale = 0.5f; // Brick의 Scale

    // Brick을 설치할 전체 board와 Brick을 생성시킬 위치bool
    private void Awake()
    {
        board = new Vector2[14, 12];
        isPlaced = new bool[14, 12];
    }
    // 랜덤한 맵을 생성
    private void OnEnable()
    {

        // 배열의 Vector2 값들을 설정
        for (int y = 0; y < 12; y++)
        {
            for (int x = 0; x < 14; x++)
            {
                board[x, y] = new Vector2((-8.25f + (x * scale)), -1 + (y * scale));
            }
        }

        // 맵의 조건

        CreateMap1(14, 12);

        for (int y = 0; y < 12; y++)
        {
            for (int x = 0; x < 14; x++)
            {
                if (isPlaced[x, y] == false)
                {
                    BrickPool.Instance.GetBrick(board[x, y]);
                }
            }
        }
    }

    // 첫번째 맵 (T자 맵)
    void CreateMap1(int x1, int y1)
    {
        for (int x = 0; x < x1; x++)
        {
            isPlaced[x, 3] = true;
        }
        for (int y = 1; y < y1 - 1; y++)
        {
            isPlaced[3, y] = true;
        }
    }

    void CreateMap2(int x1, int y1)
    {
        for (int x = 0; x < x1; x++)
        {
            isPlaced[x, 0] = true;
        }
        for (int y = 0; y < y1; y++)
        {
            isPlaced[3, y] = true;
        }
    }

    void CreateMap3(int x1, int y1)
    {
        for (int x = 0; x < x1; x++)
        {
            isPlaced[x, 2] = true;
        }
        for (int y = 0; y < y1; y++)
        {
            isPlaced[1, y] = true;
        }
    }
}
