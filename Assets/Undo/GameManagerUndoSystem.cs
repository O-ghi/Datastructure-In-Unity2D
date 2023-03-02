using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerUndoSystem : MonoBehaviour
{
    [SerializeField]
    public static Stack<GameState> savedStates = new Stack<GameState>();

    void Start()
    {
        SaveGameState();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            UndoMove();
        }
    }

    public static void SaveGameState()
    {
        savedStates.Push(GameState.GetCurrentState());
    }

    public static void UndoMove()
    {
        if (savedStates.Count <= 1)
        {
            Debug.Log("No moves to undo");
        }
        else
        {
            savedStates.Pop();
            savedStates.Peek().LoadGameState();
            // savedStates.Pop();
        }
    }
}
