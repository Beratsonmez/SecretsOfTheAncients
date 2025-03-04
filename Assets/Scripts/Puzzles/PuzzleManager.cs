using UnityEngine;
using System.Collections.Generic;
using Mirror;

public class PuzzleManager : NetworkBehaviour
{
    public static PuzzleManager instance {  get; private set; }

    private Dictionary<string, bool> puzzleStates = new Dictionary<string, bool>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Server]
    public void SetPuzzleState(string puzzleID, bool isSolved)
    {
        if (puzzleStates.ContainsKey(puzzleID))
        {
            puzzleStates[puzzleID] = isSolved;
        }
        else
        {
            puzzleStates.Add(puzzleID, isSolved);
        }

        RpcUpdatePuzzleState(puzzleID, isSolved);

    }

    [ClientRpc]
    private void RpcUpdatePuzzleState(string puzzleID, bool isSolved)
    {
        Debug.Log($"Puzzle {puzzleID} durumu: {(isSolved ? "Çözüldü" : "Çözülmedi")}");
    }

    public bool GetPuzzleState(string puzzleID )
    {
        return puzzleStates.TryGetValue(puzzleID, out bool isSolved) && isSolved;
    }
}
