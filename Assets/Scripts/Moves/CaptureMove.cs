using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureMove : Move {
    public Piece CapturedPiece { get; set; }
    
    public CaptureMove(Piece piece, int x, int y, Piece capturedPiece = null, bool isSimulate = false) : base(piece, x, y, isSimulate) {
        this.CapturedPiece = capturedPiece;
    }
    public CaptureMove(Piece piece, Vector3Int point, Piece capturedPiece = null, bool isSimulate = false) : base(piece, point, isSimulate) {
        this.CapturedPiece = capturedPiece;
    }
    public override void Execute() {
        base.Execute();
        if (CapturedPiece != null)
            CapturedPiece.CanBeCaptured = true;
    }
    public override void SimulateExecute(Piece[,] simulateBoard)
    {
        if (CapturedPiece != null)
        {
            simulateBoard[CapturedPiece.GridPosition.x, CapturedPiece.GridPosition.y] = null;
        }
        base.SimulateExecute(simulateBoard);
    }
    public override void ExecuteBackward() {
        if (CapturedPiece != null)
            CapturedPiece.Reborn();
        base.ExecuteBackward();
    }

    public override string ToString() {
        if (CapturedPiece != null)
            return string.Format("{0} - Captured: {1}", base.ToString(), CapturedPiece);
        return base.ToString();
    }
}