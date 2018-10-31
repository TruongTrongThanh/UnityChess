using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece {
    public override int CalculateMoves(List<Move> moveList, bool isSimulate) {
        int beforeAddCount = moveList.Count;

        var currentPos = GridPosition;
        if (isSimulate)
        {
            if (SimulatePosition.z == -1)
                return 0;

            currentPos = SimulatePosition;
        }

        GenerateStraightLine(moveList, currentPos, Vector3Int.up);
        GenerateStraightLine(moveList, currentPos, Vector3Int.down);
        GenerateStraightLine(moveList, currentPos, Vector3Int.left);
        GenerateStraightLine(moveList, currentPos, Vector3Int.right);

        return moveList.Count - beforeAddCount;
    }
}
