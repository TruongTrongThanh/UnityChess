using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class RedmapManager : MonoBehaviour {

	public Tile redDot;

	private Tilemap redmap;
	void Start() {
		redmap = GetComponent<Tilemap>();
	}
	public void Set(Vector3Int pos) {
		redmap.SetTile(pos, redDot);
	}
	public void Clear() {
		redmap.ClearAllTiles();
	}

	void OnMouseDown() {
		var currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		SendMessageUpwards("ExecuteMove", redmap.WorldToCell(currentPos));
	}
}
