using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pawn : Piece {
	private GameObject promotionPanel;
	public GameObject[] pieceType;
	private Vector3Int oneStep;
    protected override void Awake() {
		base.Awake();
		oneStep = team == Team.White ? Vector3Int.up : Vector3Int.down;
		promotionPanel = GameObject.Find("FrontCavas").transform.Find("PromotionPanel").gameObject;
    }
	public override int CalculateMoves(List<Move> moveList, bool isSimulate) {
		int beforeAddCount = moveList.Count;

        var currentPos = GridPosition;

        if (isSimulate)
        {
            var simulatePos = SimulatePosition;
            if (simulatePos.z == -1)
                return 0;

            currentPos = simulatePos;
        }

        AddForwardMoves(moveList, currentPos);
        AddCaptureMoves(moveList, currentPos); 
		AddEnPassantMove(moveList, currentPos);

        if (!isSimulate)
        {
            ReplacePromotionMove(moveList);
        }
		

        return moveList.Count - beforeAddCount;
    }

	private int AddForwardMoves(List<Move> moveList, Vector3Int atPosition) {
		int beforeAddCount = moveList.Count;

		var pts = new Vector3Int[] {
            atPosition + oneStep,
            atPosition + oneStep * 2
		};
		bool notHavePieceAt0 = !boardManager.HasPieceAt(pts[0]);
		bool notHavePieceAt1 = !boardManager.HasPieceAt(pts[1]);
		if (IsInsideBoard(pts[0]) && notHavePieceAt0)
			moveList.Add(new Move(this, pts[0]));
		if (IsInsideBoard(pts[1]) && IsFirstMove() && notHavePieceAt0 && notHavePieceAt1)
			moveList.Add(new DoubleForwardMove(this, pts[1]));

		return moveList.Count - beforeAddCount;
	}
	private int AddCaptureMoves(List<Move> moveList, Vector3Int atPosition) {
		int beforeAddCount = moveList.Count;

		var capturedPts = new Vector3Int[] {
            atPosition + oneStep + Vector3Int.left,
            atPosition + oneStep + Vector3Int.right
		};
		foreach (var point in capturedPts) {
			var p = boardManager.GetPieceAt(point);

			if (IsInsideBoard(point))  {
				if (p != null && p.team != team)
					moveList.Add(new CaptureMove(this, point, p));
			}
		}

		return moveList.Count - beforeAddCount;
	}
	private int AddEnPassantMove(List<Move> moveList, Vector3Int atPosition) {
		int beforeAddCount = moveList.Count;

		var lastMove = boardManager.GetPreviousMove();
		if (lastMove != null && lastMove is DoubleForwardMove) {
			var pts = new Vector3Int[] {
                atPosition + Vector3Int.left,
                atPosition + Vector3Int.right
			};
			foreach (var point in pts) {
				var p = boardManager.GetPieceAt(point);
				if (p == lastMove.Piece)
					moveList.Add(new CaptureMove(this, point + oneStep, p));
			}
		}
		return moveList.Count - beforeAddCount;
	}
	private int ReplacePromotionMove(List<Move> moveList) {
		int replaceCount = 0;

		int lastRow = team == Team.White ? 7 : 0;
		for (int i = 0; i < moveList.Count; i++) {
			if (moveList[i].To.y == lastRow && moveList[i].Piece is Pawn) {
				replaceCount++;
				if (moveList[i] is CaptureMove) {
					moveList[i] = new PromotionCaptureMove((CaptureMove)moveList[i]);
				}
				else
					moveList[i] = new PromotionMove(moveList[i]);
			}
		}
		return replaceCount;
	}
	public void Promotion() {
		StartCoroutine(PromotionProcess());
	}
	private IEnumerator PromotionProcess() {
		yield return new WaitUntil(() => !IsMoving);

		promotionPanel.SetActive(true);
		PanelManager panelManager = promotionPanel.GetComponent<PanelManager>();
	
		yield return new WaitUntil(() => panelManager.promotionPiece != PieceType.None);

		promotionPanel.SetActive(false);
		GameObject obj = Instantiate(pieceType[(int)panelManager.promotionPiece], boardManager.transform);
		Piece piece = obj.GetComponent<Piece>();
		piece.team = this.team;
		obj.transform.position = this.transform.position;

		((IPromotionMove)boardManager.LastMove).PromotionPiece = piece;
		this.gameObject.SetActive(false);

		UnFocus();
		SendMessageUpwards("EndTurn", true);
	}
}
