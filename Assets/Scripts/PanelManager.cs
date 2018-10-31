using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour {
	public PieceType promotionPiece;

	public void SetPromotionPiece(int type) {
		promotionPiece = (PieceType)type;
	}
	private void OnEnable() {
		promotionPiece = PieceType.None;
	}
}
public enum PieceType : int {
	Queen = 0,
	Rook = 1,  
	Bishop = 2, 
	Knight = 3,
	None = -1
}
