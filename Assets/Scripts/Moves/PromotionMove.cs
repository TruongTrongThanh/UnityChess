using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromotionMove : Move, IPromotionMove {

    public Piece PromotionPiece { get; set; }

    public PromotionMove(Piece piece, int x, int y) : base(piece, x, y) {
    }
    public PromotionMove(Piece piece, Vector3Int point) : base(piece, point) {
    }
    public PromotionMove(Move move) : base(move.Piece, move.To) {
    }
    public override void Execute() {
        Piece.MovingTo(To, false);
        ((Pawn)Piece).Promotion();
    }
    public override void ExecuteBackward() {
        Piece.Reborn();
        PromotionPiece.DestroySelf();
        base.ExecuteBackward();
    }

    public override string ToString() {
        return base.ToString();
    }
}