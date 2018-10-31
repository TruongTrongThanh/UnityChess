using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece {
	public override int CalculateMoves(List<Move> moveList, bool isSimulate) {
        int beforeAddCount = moveList.Count;

        var currentPos = GridPosition;
        if (isSimulate)
        {
            if (SimulatePosition.z == -1)
                return 0;

            currentPos = SimulatePosition;
        }

        GenerateStraightLine(moveList, currentPos, Vector3Int.up + Vector3Int.right);
        GenerateStraightLine(moveList, currentPos, Vector3Int.up + Vector3Int.left);
        GenerateStraightLine(moveList, currentPos, Vector3Int.down + Vector3Int.right);
        GenerateStraightLine(moveList, currentPos, Vector3Int.down + Vector3Int.left);
        return moveList.Count - beforeAddCount;
    }
}
