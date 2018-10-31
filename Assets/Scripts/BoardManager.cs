using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Linq;
using System.Runtime.CompilerServices;

public class BoardManager : MonoBehaviour {
	private Tilemap board;
	private RedmapManager redmapManager;
	private Piece selectedPiece;
	private List<Move> selectedMoves;
	private HistoryManager historyManager;

	public Team CurrentPlayerTurn { get; private set; }
	public int Turn { get; private set; }
    public Piece[,] SimulateBoard { get; set; }
    public bool IsSimulate { get; set; }

	void Awake() {
		board = GetComponent<Tilemap>();
		redmapManager = GameObject.Find("Redmap").GetComponent<RedmapManager>();
		historyManager = GameObject.Find("HistoryManager").GetComponent<HistoryManager>();
		selectedMoves = new List<Move>();
        SimulateBoard = new Piece[8, 8];
        IsSimulate = false;
    }

	void Start() {
		Turn = 1;
		CurrentPlayerTurn = Team.White;
    }

    public void UpdateSimulateBoard()
    {
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                SimulateBoard[i, j] = GetPieceAt(i, j);
    }
    public void PrintSimulateBoard()
    {
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                print(SimulateBoard[i, j] + string.Format("- {0},{1} -", i, j));
    }

    public Team OppositeTeam {
		get {
			return CurrentPlayerTurn == Team.Black ? Team.White : Team.Black;
		}
	}
	
	void SelectPiece(Piece piece) {
		if (selectedPiece != null && selectedPiece.IsSameId(piece)) {
			DeselectPiece();
		}
		else {
			DeselectPiece();
            selectedPiece = piece;
            piece.CalculateMoves(selectedMoves, false);
            CastlingFilter();
            CheckIfKingIsNotProtectedFilter(selectedPiece, selectedMoves);

            foreach (var move in selectedMoves)
                redmapManager.Set(move.To);
        }
	}
	void DeselectPiece() {
		redmapManager.Clear();
		selectedPiece = null;
		selectedMoves.Clear();
	}

	public Move LastMove { get; private set; }
	public void EndTurn(bool isForward) {
		if (isForward) {
			historyManager.Write(LastMove);
			Turn++;
            BeforeStartTurn();
        }
		else
			Turn--;

		SwapPlayer();
	}
	public void BeforeStartTurn() {
        var allEnemyPieces = GetAllPieces(typeof(Piece)).Where(p => p.team == OppositeTeam);
        bool isCheckmate = true;
        foreach (Piece p in allEnemyPieces)
        {
            var calculatedMoves = new List<Move>();
            p.CalculateMoves(calculatedMoves, false);
            CheckIfKingIsNotProtectedFilter(p, calculatedMoves);
            if (calculatedMoves.Any())
                isCheckmate = false;
        }
        if (isCheckmate)
            EndGame();
    }

    public void CastlingFilter()
    {
        if (selectedPiece is King) {
            var restrictMoves = new List<Move>();
            var captureMoves = new List<Move>();
            
            UpdateSimulateBoard();
            IsSimulate = true;
            CalculateAllSimulateMoves(captureMoves, OppositeTeam);

            foreach (var m in selectedMoves)
            {
                IsSimulate = false;
                UpdateSimulateBoard();
                IsSimulate = true;

                var move = m as CastlingMove;
                if (move != null)
                {
                    var direction = move.Direction * -1;
                    var middlePoint = move.To - direction;
                    if (HasAttackAt(middlePoint, captureMoves))
                        restrictMoves.Add(m);
                }
            }
            selectedMoves.RemoveAll(m => restrictMoves.Contains(m));
        }
        IsSimulate = false;
    }

	public void CheckIfKingIsNotProtectedFilter(Piece checkedPiece, List<Move> calculatedMoves) {

        King king = (King)GetPiece(checkedPiece.team, typeof(King));
        var restrictMoves = new List<Move>();
  
        for (int i = 0; i < calculatedMoves.Count; i++)
        {
            IsSimulate = false;
            UpdateSimulateBoard();
            IsSimulate = true;

            calculatedMoves[i].SimulateExecute(SimulateBoard);

            var moveList = new List<Move>();
            CalculateAllSimulateMoves(moveList, checkedPiece.OppositeTeam);

            if (king.IsInDanger(moveList))
                restrictMoves.Add(calculatedMoves[i]);
        }
        calculatedMoves.RemoveAll(m => restrictMoves.Contains(m));

        IsSimulate = false;
	}

	public void EndGame() {
		print("End game");
	}
	private int GeneratePointBetween(Vector3Int pointA, Vector3Int pointB, List<Vector3Int> results) {
		int beforeAddCount = results.Count;
		Vector3 heading = pointB - pointA;
		Vector3 directionVec3 = heading.normalized;
		Vector3Int direction = Vector3Int.RoundToInt(directionVec3);

		int limitNumber = 0;
		for (var i = pointA + direction; i != pointB; i += direction) {
			results.Add(i);

			if (limitNumber++ > 99)
				throw new Exception("Something's wrong in GeneratePointBetween");
		}
		return results.Count - beforeAddCount;
	}
	public void TurnBack() {
		historyManager.TurnBack();
		DeselectPiece();
	}

	private void SwapPlayer() {
		CurrentPlayerTurn = CurrentPlayerTurn == Team.White ? Team.Black : Team.White;
	}

	public IEnumerable<Piece> GetAllPieces(Type pieceType) {
        if (IsSimulate)
        {
            var list = new List<Piece>();
            foreach (var p in SimulateBoard)
            {
                if (p != null)
                    list.Add(p);
            }
            return list;
        }
		UnityEngine.Object[] allPieces = FindObjectsOfType(pieceType);
		return allPieces.Cast<Piece>();
	}
	public Piece GetPiece(Team team, Type pieceType) {
		return GetAllPieces(pieceType).Where(p => p.team == team).SingleOrDefault();
	}
    public Vector3Int GetPieceLocationInSimulateBoard(int id)
    {
        if (IsSimulate)
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (SimulateBoard[i, j] != null && SimulateBoard[i, j].GetInstanceID() == id)
                        return new Vector3Int(i, j, 0);

            return new Vector3Int(-1, -1, -1);
        }
        else
            throw new Exception("Can only call this function in simulate mode");
    }

    private bool IsInsideBoard(int i, int j)
    {
        return i > -1 && i < 8 && j > -1 && j < 8;
    }

    public Piece GetPieceAt(Vector3Int point) {
        if (IsSimulate)
        {
            if (IsInsideBoard(point.x, point.y))
                return SimulateBoard[point.x, point.y];
            return null;
        }
		Vector3 worldPoint = board.GetCellCenterWorld(point);
		var mask = LayerMask.GetMask("Piece");
		Collider2D box = Physics2D.OverlapPoint(worldPoint, mask);
		if (box != null)
			return box.gameObject.GetComponent<Piece>();
		return null;
	}
	public Piece GetPieceAt(int x, int y) {
		return GetPieceAt(new Vector3Int(x, y, 0));
	}
	public bool HasPieceAt(int x, int y) {
		return GetPieceAt(new Vector3Int(x, y, 0)) != null;
	}
	public bool HasPieceAt(Vector3Int point) {
		return GetPieceAt(point) != null;
	}

	public bool HasAttackAt(Vector3Int point, IEnumerable<Move> captureMoves) {
		foreach (Move move in captureMoves) {
			if (move.To == point)
				return true;
		}
		return false;
	}
	
	public void CalculateAllSimulateMoves(List<Move> moveList, Team filterTeam) {
		var tmp = GetAllPieces(typeof(Piece)).Where(p => p.team == filterTeam);
		foreach (var p in tmp) {
			p.CalculateMoves(moveList, true);
		}
	}

	public void ExecuteMove(Vector3Int pos) {
		var move = selectedMoves.Where(p => p.To == pos).Single();
		move.Execute();
		LastMove = move;
		DeselectPiece();
	}

	public Move GetPreviousMove() {
		return historyManager.Previous;
	}
	public bool IsFirstMove(Piece piece) {
		return historyManager.IsFirstMove(piece);
	}
}