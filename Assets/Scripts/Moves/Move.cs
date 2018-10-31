using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move {
    public Piece Piece { get; private set; }
    public Vector3Int From { get; private set; }
    public Vector3Int To { get; private set; }
    public bool IsSimulate { get; set; }
    public GameObject HistoryText { get; set; }

    public Move(Piece piece, Vector3Int to, bool isSimulate = false) {
        this.Piece = piece;
        this.From = piece.GridPosition;
        this.To = to;
        this.IsSimulate = isSimulate;
    }
    public Move(Piece piece, int x, int y, bool isSimulate = false) : this(piece, new Vector3Int(x, y, 0), isSimulate) {}
    public virtual void Execute() {
        Piece.MovingTo(To);
    }
    public virtual void SimulateExecute(Piece[,] simulateBoard)
    {
        if (Piece.SimulatePosition.z == -1) return;

        var from = Piece.SimulatePosition;

        simulateBoard[from.x, from.y] = null;
        simulateBoard[To.x, To.y] = Piece;
    }
    public virtual void ExecuteBackward() {
        Piece.MovingTo(From, true, false);
    }

    public override string ToString() {
        return string.Format("{0}: {1} - {2}", this.GetType().Name, Piece, new Vector2Int(To.x, To.y));
    }
}