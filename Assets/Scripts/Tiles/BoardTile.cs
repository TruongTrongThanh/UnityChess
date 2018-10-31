using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "BoardTile")]
public class BoardTile : Tile {
	public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData) {
		tileData.sprite = sprite;
		tileData.color = color;
	}
}
