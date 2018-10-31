using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastlingMove : Move {

    public Rook CastlingPiece { get; set; }
    public Vector3Int Direction { get; private set; }

    public CastlingMove(Piece piece, Rook castlingPiece, Vector3Int direction, int x, int y) : base(piece, x, y) {
        this.CastlingPiece = castlingPiece;
        this.Direction = direction;
    }
    public CastlingMove(Piece piece, Rook castlingPiece, Vector3Int direction, Vector3Int point) : base(piece, point) {
        this.CastlingPiece = castlingPiece;
        this.Direction = direction;
    }
    public override void Execute() {
        base.Execute();
        CastlingPiece.MovingTo(To + Direction, false);
    }
    public override void SimulateExecute(Piece[,] simulateBoard)
    {
        base.SimulateExecute(simulateBoard);
        simulateBoard[CastlingPiece.GridPosition.x, CastlingPiece.GridPosition.y] = null;
        int x = Direction.x > 0 ? 0 : 7;
        simulateBoard[x, From.y] = CastlingPiece;
    }
    public override void ExecuteBackward() {
        base.ExecuteBackward();
        int x = Direction.x > 0 ? 0 : 7;
        CastlingPiece.MovingTo(new Vector3Int(x, From.y, 0), false, false);
    }

    public override string ToString() {
        return base.ToString();
    }
}