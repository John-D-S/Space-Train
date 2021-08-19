// Property of TUNACORN STUDIOS PTY LTD 2018
// 
// Creator: John Stevenson
// Creation Time: 2021/08/19 9:09 PM

using System.Collections.Generic;

using UnityEngine;

namespace LevelGeneration
{
	public class Room
	{
		public Room(int _roomID, ref List<LevelTile> _occupiedTiles, RoomStyle _roomStyle, LevelGenerator _levelGenerator)
		{
			levelGenerator = _levelGenerator;
			RoomID = _roomID;
			foreach(LevelTile tile in _occupiedTiles)
			{
				tile.room = this;
			}
			OccupiedTiles = _occupiedTiles;
			roomStyle = _roomStyle;
		}

		public readonly LevelGenerator levelGenerator;
		public readonly RoomStyle roomStyle;
		public int RoomID { get; private set; }
		public List<LevelTile> OccupiedTiles { get; private set; }
		
		public List<Vector2Int> OccupiedTilePositions
		{
			get
			{
				List<Vector2Int> returnValue = new List<Vector2Int>();
				foreach(LevelTile tile in OccupiedTiles)	
				{
					returnValue.Add(tile.PositionInGrid);
				}

				return returnValue;
			}
		}
	}
}