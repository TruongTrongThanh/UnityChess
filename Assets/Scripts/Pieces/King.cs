using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class King : Piece {
    public override int CalculateMoves(List<Move> moveList, bool isSimulate) {
        int beforeAddCount = moveList.Count;

        var currentPos = GridPosition;
        if (isSimulate)
        {
            if (SimulatePosition.z == -1)
                return 0;

            currentPos = SimulatePosition;
        }
        AddCaptureMove(moveList, currentPos);
        if (!isSimulate)
        {
            AddCastlingMove(moveList, currentPos);
        }
 
        return moveList.Count - beforeAddCount;
    }

    private int AddCaptureMove(List<Move> moveList, Vector3Int atPosition) {
        int beforeAddCount = moveList.Count;
        var pts = new Vector3Int[] {
            atPosition + Vector3Int.up,
            atPosition + Vector3Int.down,
            atPosition + Vector3Int.left,
            atPosition + Vector3Int.right
        };
        GeneratePointSet(moveList, pts);
        return moveList.Count - beforeAddCount;
    }
    private int AddCastlingMove(List<Move> moveList, Vector3Int atPosition) {
        int beforeAddCount = moveList.Count;

        if (!IsFirstMove()) return 0;

        var directions = new Vector2[] {
            Vector2.left,
            Vector2.right
        };
        foreach (var direction in directions) {
            var box = GetComponent<BoxCollider2D>();
            var mask = LayerMask.GetMask("Piece");
            var results = new RaycastHit2D[1];
            var amount = box.Raycast(direction, results, Mathf.Infinity, mask);

            if (amount > 0) {
                var encounterPiece = results[0].collider.gameObject.GetComponent<Piece>() as Rook;
                var lastPos = new List<Vector3Int> {
                    new Vector3Int(0, atPosition.y, 0),
                    new Vector3Int(7, atPosition.y, 0)
                };
                if (encounterPiece != null &&
                    lastPos.Contains(encounterPiece.GridPosition) && 
                    encounterPiece.team == this.team &&
                    encounterPiece.IsFirstMove()) 
                {
                    Vector3Int convertDirection = Vector3Int.RoundToInt(direction);
                    var dest = atPosition + convertDirection * 2;

                    moveList.Add(new CastlingMove(this, encounterPiece, convertDirection * -1, dest));
                }
            }
        }
        return moveList.Count - beforeAddCount;
    }
}
