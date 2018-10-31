using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece {
	public override int CalculateMoves(List<Move> moveList, bool isSimulate) {
		int beforeAddCount = moveList.Count;
        var currentPos = GridPosition;

        if (isSimulate)
        {
            if (SimulatePosition.z == -1)
                return 0;

            currentPos = SimulatePosition;
        }
	
        var pts = new Vector3Int[] {
            new Vector3Int(currentPos.x + 1, currentPos.y + 2, 0),
			new Vector3Int(currentPos.x + 1, currentPos.y - 2, 0),
			new Vector3Int(currentPos.x - 1, currentPos.y + 2, 0),
			new Vector3Int(currentPos.x - 1, currentPos.y - 2, 0),

			new Vector3Int(currentPos.x + 2, currentPos.y + 1, 0),
			new Vector3Int(currentPos.x - 2, currentPos.y + 1, 0),
			new Vector3Int(currentPos.x + 2, currentPos.y - 1, 0),
			new Vector3Int(currentPos.x - 2, currentPos.y - 1, 0),
        };

		GeneratePointSet(moveList, pts);
        return moveList.Count - beforeAddCount;
    }
}
