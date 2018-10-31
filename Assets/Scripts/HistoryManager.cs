using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HistoryManager : MonoBehaviour {

	private Stack<Move> historyBook = new Stack<Move>();
	public GameObject historyText;
	public Scrollbar scrollbar;

	public void Write(Move move) {
		GameObject obj = Instantiate(historyText, this.transform);
		var component = obj.GetComponent<Text>();
		component.text = move.ToString();
		move.HistoryText = obj;
		historyBook.Push(move);
	}
	public Move Previous {
		get {
			if (historyBook.Count != 0)
				return historyBook.Peek();
			return null;
		}
	}
	public void TurnBack() {
		if (historyBook.Count != 0) {
			var pre = historyBook.Pop();
			pre.ExecuteBackward();
			Destroy(pre.HistoryText);
		}
	}
	public bool IsFirstMove(Piece piece) {
		return historyBook.Where(m => m.Piece.IsSameId(piece)).Count() == 0;
	}
}
