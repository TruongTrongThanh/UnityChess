using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleForwardMove : Move {
    public DoubleForwardMove(Piece piece, int x, int y) : base(piece, x, y) {
    }
    public DoubleForwardMove(Piece piece, Vector3Int point) : base(piece, point) {
    }
    public DoubleForwardMove(Move move) : base(move.Piece, move.To) {
    }
}