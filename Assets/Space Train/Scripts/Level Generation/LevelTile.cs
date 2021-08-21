using System.Collections.Generic;

using UnityEngine;

namespace LevelGeneration
{
	public class LevelTile
	{
		public TileType tileType;
		public Room room;
		public int RoomID
		{
			get
			{
				if(room != null)
				{
					return room.RoomID;
				}
				else
				{
					return 0;
				}
			}
		}
		public Vector2Int PositionInGrid { get; private set; }
		public TileEdge[] tileEdges = new TileEdge[4];

		public RoomStyle RoomStyle => room.roomStyle;

		public LevelTile(TileType _tileType, Vector2Int _positionInGrid)
		{
			tileType = _tileType;
			PositionInGrid = _positionInGrid;
		}
		
		public enum TileEdge
		{
			Empty,
			Door,
			Wall
		}

		private bool occupiedByProp = false;
		/// <summary>
		/// returns true if there is a prop on this tile.
		/// </summary>
		public bool OccupiedByProp
		{
			get
			{
				if(occupiedByProp)
					return true;
				foreach(LevelTile.TileEdge tileEdge in tileEdges)
					if(tileEdge == LevelTile.TileEdge.Door)
					{
						return true;
					}
				return false;
			}
			set
			{
				occupiedByProp = value;
			}
		}

		public bool HasWall
		{
			get
			{
				foreach(LevelTile.TileEdge tileEdge in tileEdges)
				{
					if(tileEdge == LevelTile.TileEdge.Wall)
					{
						return true;
					}
				}

				return false;
			}
		}
		
		public bool doorAlreadyInstantiated = false;
		public int RoomIDOnOtherSideOfDoor()
		{
			for(int i = 0; i < 0; i++)
			{
				if(tileEdges[i] == TileEdge.Door)
				{
					Vector2Int posOnOtherSideOfDoor = PosInOneUnitInDirection((Direction) i);
					return room.levelGenerator.levelTiles[posOnOtherSideOfDoor.x][posOnOtherSideOfDoor.y].RoomID;
				}
			}
			return RoomID;
		}
		
		public Vector2Int PosInOneUnitInDirection(Direction _direction)
		{
			switch(_direction)
			{
				case Direction.Zpos:
					return PositionInGrid + Vector2Int.up;
				case Direction.Xpos:
					return PositionInGrid + Vector2Int.right;
				case Direction.Zneg:
					return PositionInGrid + Vector2Int.down;
				case Direction.Xneg:
					return PositionInGrid + Vector2Int.left;
			}
			return PositionInGrid;
		}
		
		public bool TrySetDoor(Direction _direction)
		{
			if(tileEdges[(int) _direction] == TileEdge.Wall)
			{
				Vector2Int connectedDoorTilePos = PosInOneUnitInDirection(_direction);
				if(room.levelGenerator.PositionIsWithinLevel(connectedDoorTilePos))
				{
					SetEdge(TileEdge.Door, _direction);				
					//this is the opposite of _direction. for example, if _direction is Xneg, otherDoorDirection will be Xpos;
					Direction otherDoorDirection = (Direction)(((int)_direction + 2) % 4);
					room.levelGenerator.levelTiles[connectedDoorTilePos.x][connectedDoorTilePos.y].SetEdge(TileEdge.Door, otherDoorDirection);
					return true;
				}
			}
			return false;
		}
		
		public void SetEdge(TileEdge _edge, Direction _direction)
		{
			tileEdges[(int)_direction] = _edge;
		}
		
		public void InstantiateTileObjects(Vector3 _offset, ref List<GameObject> _instantiatedGameObjects, RoomStyle _defaultStyle)
		{
			if(RoomStyle == null)
			{
				Debug.Log("roomstyle is null");
			}
			RoomStyle usedRoomstyle = RoomStyle != null
				? RoomStyle
				: _defaultStyle;
			Vector3 positionToInstantiate = _offset + new Vector3(PositionInGrid.x * room.levelGenerator.TileSize, 0, PositionInGrid.y * room.levelGenerator.TileSize);
			_instantiatedGameObjects.Add(Object.Instantiate(usedRoomstyle.Floor, positionToInstantiate, Quaternion.identity));
			//instantiating the edgeObjects;
			for(int i = 0; i < 4; i ++)
			{
				switch(tileEdges[i])
				{
					case TileEdge.Door:
						//only instantiate a door if it has not already been instantiated from the other side.
						Vector2Int doorNeighborPosition = PosInOneUnitInDirection((Direction)i);
						//LevelTile doorNeighborTile = room.levelGenerator.levelTiles[doorNeighborPosition.x][doorNeighborPosition.y];
						if(!room.levelGenerator.levelTiles[doorNeighborPosition.x][doorNeighborPosition.y].doorAlreadyInstantiated)
						{
							_instantiatedGameObjects.Add(Object.Instantiate(usedRoomstyle.Door, positionToInstantiate, Quaternion.AngleAxis(i * 90, Vector3.up)));
							room.levelGenerator.levelTiles[doorNeighborPosition.x][doorNeighborPosition.y].doorAlreadyInstantiated = true;
							doorAlreadyInstantiated = true;
						}
						break;
					case TileEdge.Wall:
						_instantiatedGameObjects.Add(Object.Instantiate(usedRoomstyle.Wall, positionToInstantiate, Quaternion.AngleAxis(i * 90, Vector3.up)));
						break;
					case TileEdge.Empty:
						break;
				}
			}
		}
	}
}