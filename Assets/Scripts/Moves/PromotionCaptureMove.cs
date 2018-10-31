using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromotionCaptureMove : CaptureMove, IPromotionMove {
    public Piece PromotionPiece { get; set; }
    public PromotionCaptureMove(Piece piece, int x, int y, Piece capturedPiece = null) : base(piece, x, y, capturedPiece) {
    }
    public PromotionCaptureMove(Piece piece, Vector3Int point, Piece capturedPiece = null) : base(piece, point, capturedPiece) {
    }

    public PromotionCaptureMove(CaptureMove move) : base(move.Piece, move.To, move.CapturedPiece) {
    }
    public override void Execute() {
        Piece.MovingTo(To, false);
        if (CapturedPiece != null)
            CapturedPiece.CanBeCaptured = true;
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