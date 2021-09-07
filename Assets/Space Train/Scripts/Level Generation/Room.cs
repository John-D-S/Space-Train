// Property of TUNACORN STUDIOS PTY LTD 2018
// 
// Creator: John Stevenson
// Creation Time: 2021/08/19 9:09 PM

using System.Collections.Generic;

using UnityEngine;

namespace LevelGeneration
{
	/// <summary>
	/// contains information common to all LevelTiles that belong to a given room.
	/// </summary>
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

		public class InstantiationInfo
		{
			private GameObject propGameObject;
			private Vector3 instantiationPosition;
			private Quaternion instantiationRotation;

			public InstantiationInfo(GameObject _propGameObject, Vector3 _instantiationPosition, Quaternion _instantiationRotation)
			{
				propGameObject = _propGameObject;
				instantiationPosition = _instantiationPosition;
				instantiationRotation = _instantiationRotation;
			}

			public void InstantiateProp(Vector3 _offset, float levelScale, ref List<GameObject> _instantiatedGameObjects)
			{
				_instantiatedGameObjects.Add(Object.Instantiate(propGameObject, instantiationPosition * levelScale + _offset, instantiationRotation));
			}
		}

		/// <summary>
		/// all positions occupied by this room.
		/// </summary>
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

		public List<InstantiationInfo> RoomProps = new List<InstantiationInfo>();

		/// <summary>
		/// Instantiate all the level tiles and props in the room.
		/// </summary>
		public void InstantiateRoomObjects(Vector3 _offset, ref List<GameObject> _instantiatedGameObjects, RoomStyle _defaultStyle)
		{
			foreach(LevelTile occupiedTile in OccupiedTiles)
				occupiedTile.InstantiateTileObjects(_offset, ref _instantiatedGameObjects, _defaultStyle);
			foreach(InstantiationInfo propInstantiationInfo in RoomProps)
				propInstantiationInfo.InstantiateProp(_offset, levelGenerator.TileSize, ref _instantiatedGameObjects);
		}
	}
}