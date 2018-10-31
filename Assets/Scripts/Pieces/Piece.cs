using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System;

public abstract class Piece : MonoBehaviour {
	public Team team;
	public Team OppositeTeam {
		get {
			return this.team == Team.Black ? Team.White : Team.Black;
		}
	}
	public Sprite[] sprites;
	public float gameSpeed;

	protected BoardManager boardManager;
	protected Grid grid;
	protected SpriteRenderer sRender;
	public bool CanBeCaptured { get; set; }
	public bool IsMoving { get; set; }
    public Vector3Int GridPosition
    {
        get
        {
            return grid.WorldToCell(transform.position);
        }
    }
    public Vector3Int SimulatePosition
    {
        get
        {
            return boardManager.GetPieceLocationInSimulateBoard(GetInstanceID());
        }
    }

    protected virtual void Awake() {
		grid = GetComponentInParent<Grid>();
		boardManager = GetComponentInParent<BoardManager>();
		sRender = GetComponent<SpriteRenderer>();
		CanBeCaptured = false;
		IsMoving = false;
	}

	protected virtual void Start() {
		PieceRender();
		gameSpeed = gameSpeed == 0 ? 0.4f : gameSpeed;
	}

	void PieceRender() {
		try {
        	sRender.sprite = sprites[(int)team];
		}
		catch (IndexOutOfRangeException) {
			Debug.LogWarning("Sprite's not available");
		}
	}
	protected void Focus() {
		sRender.sortingLayerName = "FocusPiece";
	}
	protected void UnFocus() {
		sRender.sortingLayerName = "Piece";
	}
	public bool IsFirstMove() {
		return boardManager.IsFirstMove(this);
	}
	public bool IsInDanger(List<Piece> attackers, IEnumerable<Move> captureMoves) {
		return captureMoves.Cast<CaptureMove>().Where(m => {
			if (m.CapturedPiece != null && m.CapturedPiece.IsSameId(this)) {
				attackers.Add(m.Piece);
				return true;
			}
			return false;
		}).Count() > 0;
	}
	public bool IsInDanger(IEnumerable<Move> captureMoves) {
		return captureMoves.OfType<CaptureMove>()
			.Where(m => m.CapturedPiece != null && m.CapturedPiece.IsSameId(this))
			.Count() > 0;
	}
	private void OnCollisionStay2D(Collision2D other) {
		if (CanBeCaptured)
			this.Captured();
	}

	public void Reborn() {
		CanBeCaptured = false;
		this.gameObject.SetActive(true);
	}
	public void Captured() {
		StartCoroutine(WaitAndDisable());
	}
	private IEnumerator WaitAndDisable() {
		yield return new WaitForSeconds(0.2f);
		this.gameObject.SetActive(false);
	}
	public void DestroySelf() {
		Destroy(gameObject);
	}

	public bool IsSame(Piece other) {
		return this.team == other.team && this.GetType() == other.GetType();
	}
	public bool IsSameId(Piece other) {
		return this.gameObject.GetInstanceID() == other.gameObject.GetInstanceID();
	}
	public override string ToString() {
		return string.Format("{0} {1}", team, this.GetType().Name);
	}

	protected void GeneratePointSet(List<Move> moveList, IEnumerable<Vector3Int> pts) {
		foreach (var point in pts) {
			Piece piece = null;
			if (IsInsideBoard(point) && IsValidCaptureMove(point, ref piece)) {
				moveList.Add(new CaptureMove(this, point, piece));
			}
		}
	}
	protected void GenerateStraightLine(List<Move> moveList, Vector3Int currentPos, Vector3Int direction) {
		for (var i = currentPos + direction; IsInsideBoard(i); i += direction) {
            Piece piece = null;
            if (IsValidCaptureMove(i, ref piece))
            {
                if (piece == null)
                    moveList.Add(new CaptureMove(this, i));
                else if (piece != null)
                {
                    moveList.Add(new CaptureMove(this, i, piece));
                    break;
                }
            }
            else break;
		}
	}
	protected bool IsInsideBoard(int i, int j) {
		return i > -1 && i < 8 && j > -1 && j < 8;
	}
	protected bool IsInsideBoard(Vector3Int point) {
		return IsInsideBoard(point.x, point.y);
	}
	protected bool IsValidCaptureMove(Vector3Int point, ref Piece enCounterPiece) {
		enCounterPiece = boardManager.GetPieceAt(point);
		return (enCounterPiece == null) || (enCounterPiece != null && enCounterPiece.team != this.team);
	}

	public abstract int CalculateMoves(List<Move> moveList, bool isSimulate);

	void OnMouseDown() {
		if (boardManager.CurrentPlayerTurn == this.team)
			SendMessageUpwards("SelectPiece", this);
	}

	public virtual void MovingTo(Vector3Int destination, bool shouldEndTurn = true, bool isForward = true) {
		Vector2 dest = grid.GetCellCenterWorld(destination);
		IEnumerator coroutine = MoveAnimation(dest, shouldEndTurn, isForward);
		StartCoroutine(coroutine);
	}

	protected virtual IEnumerator MoveAnimation(Vector2 end, bool shouldEndTurn = true, bool isForward = true) {
		IsMoving = true;
		Focus();
		float delta = 0f;
		var startPos = transform.position;
		while (Vector2.Distance(transform.position, end) > float.Epsilon) {
			delta += Time.deltaTime / gameSpeed;
			transform.position = Vector2.Lerp(startPos, end, delta);
			yield return null;
		}
		IsMoving = false;
		if (shouldEndTurn) {
			UnFocus();
			SendMessageUpwards("EndTurn", isForward);
		}
	}
}
public enum Team : int {
	Black = 1,
	White = 0
}