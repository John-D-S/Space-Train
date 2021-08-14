using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UIElements;

using Random = UnityEngine.Random;

namespace LevelGeneration
{
	public enum Axis
	{
		X,
		Z
	}

	public enum Direction
	{
		Zpos,
		Xpos,
		Zneg,
		Xneg
	}
	
	public enum TileType
	{
		Corridor,
		Room
	}

	public class LevelGenerator : MonoBehaviour
	{
		[SerializeField] private int levelLength = 32;
		[SerializeField] private int levelWidth = 8;

		[SerializeField] private int minDistanceBetweenCorridors = 2;
		[SerializeField] private int minDistanceBetweenCorridorAndLevelEdge = 2;
		[SerializeField] private int maxCorridorLength = 6;
		[SerializeField] private int minDistanceBetweenDoors = 6;

		[SerializeField] private int targetNumberOfCorridors = 8;

		[SerializeField] private RoomStyle PlaceHolderRoomStyle;
		[System.NonSerialized] public List<List<LevelTile>> levelTiles = new List<List<LevelTile>>();
		
		[System.NonSerialized] public List<Room> rooms = new List<Room>();
		
		private List<GameObject> instantiatedLevelObjects = new List<GameObject>();

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
				Vector3 positionToInstantiate = _offset + new Vector3(PositionInGrid.x, 0, PositionInGrid.y);
				_instantiatedGameObjects.Add(Instantiate(usedRoomstyle.Floor, positionToInstantiate, Quaternion.identity));
				//instantiating the edgeObjects;
				for(int i = 0; i < 4; i ++)
				{
					switch(tileEdges[i])
					{
						case TileEdge.Door:
							_instantiatedGameObjects.Add(Instantiate(usedRoomstyle.Door, positionToInstantiate, Quaternion.AngleAxis(i * 90, Vector3.up)));
							break;
						case TileEdge.Wall:
							_instantiatedGameObjects.Add(Instantiate(usedRoomstyle.Wall, positionToInstantiate, Quaternion.AngleAxis(i * 90, Vector3.up)));
							break;
						case TileEdge.Empty:
							break;
					}
				}
			}
		}

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

			public LevelGenerator levelGenerator;
			public RoomStyle roomStyle;
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
		
		/// <summary>
		/// returns a list of all the Vector2s contained within the box defined by _from and _to. (inclusive)
		/// </summary>
		private List<Vector2Int> PositionsInBox(Vector2Int _from, Vector2Int _to)
		{
			//set the starting x and y values for the for loop to be the smallest from _from and _to
			int startingX = _from.x < _to.x ? _from.x : _to.x;
			int startingY = _from.y < _to.y ? _from.y : _to.y;
			//set the max x and y values for the for loop to be the largest from _from and _to
			int maxX = _from.x > _to.x ? _from.x : _to.x;
			int maxY = _from.y > _to.y ? _from.y : _to.y;
			//initialise the return value
			List<Vector2Int> returnValue = new List<Vector2Int>();
			//add all the positions within the box to the return value.
			for(int y = startingY; y < maxY + 1; y++)
			{
				for(int x = startingX; x < maxX + 1; x++)
				{
					returnValue.Add(new Vector2Int(x, y));
				}
			}
			return returnValue;
		} 
		
		private List<Vector2Int> PointsAroundPosition(Vector2Int _position) => new List<Vector2Int>() {Vector2Int.up + _position, Vector2Int.right + _position, Vector2Int.down + _position, Vector2Int.left + _position};
		
		void BoxSetTileType(Vector2Int _from, Vector2Int _to, TileType _type)
		{
			List<Vector2Int> tilePositions = PositionsInBox(_from, _to);
			for(int i = 0; i < tilePositions.Count; i++)
			{
				int x = tilePositions[i].x;
				int z = tilePositions[i].y;
				//check if the tilePosition is within the bounds of levelTiles
				if(levelTiles.Count > x && levelTiles[x].Count > z && levelTiles[x][z].tileType != _type)
				{
					levelTiles[x][z].tileType = _type;
				}
			}
		}
		
		private List<Vector2Int> ConnectedLevelTilePositions(Vector2Int _position)
		{
			TileType targetTileType = levelTiles[_position.x][_position.y].tileType;
			int targetRoomID = levelTiles[_position.x][_position.y].RoomID;
			
			Queue<Vector2Int> uncheckedPositions = new Queue<Vector2Int>();
			List<Vector2Int> connectedLevelTilePositions = new List<Vector2Int>();
			uncheckedPositions.Enqueue(_position);
			connectedLevelTilePositions.Add(_position);
			while(uncheckedPositions.Count > 0)
			{
				Vector2Int positionToCheck = uncheckedPositions.Dequeue();
				List<Vector2Int> pointsAroundPosToCheck = PointsAroundPosition(positionToCheck);
				foreach(Vector2Int position in pointsAroundPosToCheck)
				{
					if(PositionIsWithinLevel(position))
					{
						LevelTile tileAtPos = levelTiles[position.x][position.y];
						if(!connectedLevelTilePositions.Contains(position) && tileAtPos.tileType == targetTileType && tileAtPos.RoomID == targetRoomID)
						{
							uncheckedPositions.Enqueue(position);
							connectedLevelTilePositions.Add(position);
						}
					}
				}
			}

			return connectedLevelTilePositions;
		}

		private void SetRooms()
		{
			//clear any already set rooms
			rooms.Clear();
			int currentRoomID = 2;
			for(int x = 0; x < levelWidth; x++)
			{
				for(int z = 0; z < levelLength; z++)
				{
					if(levelTiles[x][z].RoomID == 0)
					{
						List<LevelTile> connectedLevelTiles = new List<LevelTile>();
						foreach(Vector2Int position in ConnectedLevelTilePositions(new Vector2Int(x, z)))
						{
							connectedLevelTiles.Add(levelTiles[position.x][position.y]);	
						}

						Room newRoom = new Room(currentRoomID, ref connectedLevelTiles, PlaceHolderRoomStyle, this);
						rooms.Add(newRoom);
						currentRoomID++;
					}
				}
			}
		}
		
		/// <summary>
		/// add the entrances and exits to the sides, back and front of the level.
		/// </summary>
		private void AddEntrances()
		{
			//if the level's length is odd, add an entrance to the sides that is 3 tiles long,
			//if its even, add an entrance that is 2 tiles long
			List<Vector2Int> leftEntrancePositions = new List<Vector2Int>();
			List<Vector2Int> rightEntrancePositions = new List<Vector2Int>();
			if(levelLength % 2 != 0)
			{
				int halfPostition = Mathf.FloorToInt(levelLength * 0.5f);
				//set the entrance of the left and right side of the level.
				leftEntrancePositions = PositionsInBox(new Vector2Int(0, halfPostition - 1), new Vector2Int(0, halfPostition + 1));
				rightEntrancePositions = PositionsInBox(new Vector2Int(levelWidth - 1, halfPostition - 1), new Vector2Int(levelWidth - 1, halfPostition + 1));
				//BoxSetTileType(new Vector2Int(levelWidth - 1, halfPostition - 1), new Vector2Int(levelWidth - 1, halfPostition + 1), TileType.Corridor);
				//BoxSetTileType(new Vector2Int(0, halfPostition - 1), new Vector2Int(0, halfPostition + 1), TileType.Corridor);
				
			}
			else
			{
				int halfPostition = Mathf.FloorToInt(levelLength * 0.5f);
				//set the entrance of the left and right of the level.
				leftEntrancePositions = PositionsInBox(new Vector2Int(0, halfPostition - 1), new Vector2Int(0, halfPostition));
				rightEntrancePositions = PositionsInBox(new Vector2Int(levelWidth - 1, halfPostition - 1), new Vector2Int(levelWidth - 1, halfPostition));
				//BoxSetTileType(new Vector2Int(levelWidth - 1, halfPostition -1), new Vector2Int(levelWidth - 1, halfPostition), TileType.Corridor);
				//BoxSetTileType(new Vector2Int(0, halfPostition - 1), new Vector2Int(0, halfPostition), TileType.Corridor);
			}
			foreach(Vector2Int position in leftEntrancePositions)
			{
				GenerateCorridor(position, Axis.Z);
			}
			foreach(Vector2Int position in rightEntrancePositions)
			{
				GenerateCorridor(position, Axis.Z);
			}
			
			//if the level's width is odd, add an entrance to the front and back that is 3 tiles long,
			//if its even, add an entrance that is 2 tiles long
			List<Vector2Int> rearEntrancePostions = new List<Vector2Int>();
			List<Vector2Int> frontEntrancePostions = new List<Vector2Int>();
			if(levelWidth % 2 != 0)
			{
				int halfPosition = Mathf.FloorToInt(levelWidth * 0.5f);
				//set the entrance of the front and back of the level
				rearEntrancePostions = PositionsInBox(new Vector2Int(halfPosition - 1, 0), new Vector2Int(halfPosition + 1, 0));
				frontEntrancePostions = PositionsInBox(new Vector2Int(halfPosition - 1, levelLength - 1), new Vector2Int(halfPosition + 1, levelLength - 1));
				//BoxSetTileType(new Vector2Int(halfPosition - 1, levelLength - 1), new Vector2Int(halfPosition + 1, levelLength - 1), TileType.Corridor);
				//BoxSetTileType(new Vector2Int(halfPosition - 1, 0), new Vector2Int(halfPosition + 1, 0), TileType.Corridor);
			}
			else
			{
				int halfPosition = Mathf.FloorToInt(levelWidth * 0.5f);
				//set the entrance of the front and back of the level
				rearEntrancePostions = PositionsInBox(new Vector2Int(halfPosition - 1, 0), new Vector2Int(halfPosition, 0));
				frontEntrancePostions = PositionsInBox(new Vector2Int(halfPosition - 1, levelLength - 1), new Vector2Int(halfPosition, levelLength - 1));
				//BoxSetTileType(new Vector2Int(halfPosition - 1, levelLength - 1), new Vector2Int(halfPosition, levelLength - 1), TileType.Corridor);
				//BoxSetTileType(new Vector2Int(halfPosition - 1, 0), new Vector2Int(halfPosition, 0), TileType.Corridor);
			}
			foreach(Vector2Int position in rearEntrancePostions)
			{
				GenerateCorridor(position, Axis.X);
			}
			foreach(Vector2Int position in frontEntrancePostions)
			{
				GenerateCorridor(position, Axis.X);
			}
		}

		/// <summary>
		/// returns true if the postition is inside of the level and false if it is outside
		/// </summary>
		private bool PositionIsWithinLevel(Vector2Int _postion)
		{
			if(_postion.x < 0 || _postion.x > levelWidth - 1 || _postion.y < 0 || _postion.y > levelLength - 1)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Generates a corridor along the given axis either side of the start position
		/// until it is obstructed by the edge of the level or another corridor
		/// </summary>
		private void GenerateCorridor(Vector2Int _startPosition, Axis _axis)
		{
			Vector2Int positiveDirection = _axis == Axis.X
				? Vector2Int.right
				: Vector2Int.up;
			bool roomInPositiveDirection = true;
			bool roomInNegativeDirection = true;
			int currentCorridorLength = 1;
			//set the tile at _start position to be a Corridor.
			levelTiles[_startPosition.x][_startPosition.y].tileType = TileType.Corridor;
			//initialize the pointer and set it to _startPosition.
			//this will be pointing to the position of tiles that are in the path
			Vector2Int pointer = _startPosition;
			//generate tiles in the positive direction until the pointer is obstructed by the edge of the level or another corridor
			while(roomInPositiveDirection)
			{
				pointer += positiveDirection;
				if(PositionIsWithinLevel(pointer) && levelTiles[pointer.x][pointer.y].tileType != TileType.Corridor && currentCorridorLength < maxCorridorLength)
				{
					levelTiles[pointer.x][pointer.y].tileType = TileType.Corridor;
					currentCorridorLength++;
				}
				else
				{
					roomInPositiveDirection = false;
				}
			}
			// reset the position fo the pointer back to _startPosition;
			//generate tiles in the negative direction until the pointer is obstructed by the edge of the level or another corridor
			pointer = _startPosition;
			while(roomInNegativeDirection)
			{
				pointer -= positiveDirection;
				if(PositionIsWithinLevel(pointer) && levelTiles[pointer.x][pointer.y].tileType != TileType.Corridor && currentCorridorLength < maxCorridorLength)
				{
					levelTiles[pointer.x][pointer.y].tileType = TileType.Corridor;
					currentCorridorLength++;
				}
				else
				{
					roomInNegativeDirection = false;
				}
			}
		}
		
		/// <summary>
		/// sets levelTiles to be a grid of empty tiles with width levelWidth and length levelLength
		/// </summary>
		private void InitializeLevel()
		{
			//initialize a grid of empty level tiles according to length and width. 
			levelTiles.Clear();
			for(int x = 0; x < levelWidth; x++)
			{
				//add another row of level tiles until levelLength is reached
				levelTiles.Add(new List<LevelTile>());
				for(int z = 0; z < levelLength; z++)
				{
					//add another tile to each row of level tiles until levelWidth is reached.
					levelTiles[x].Add(new LevelTile(TileType.Room, new Vector2Int(x, z)));
				}
			}
		}

		/// <summary>
		/// returns a list of positions where corridors along the given axis are allowed to be generated.
		/// </summary>
		private List<Vector2Int> AllowedCorridorPositions(Axis _axis)
		{
			// a list of positions that corridors cannot generate.
			List<Vector2Int> disallowedCorridorPositions = new List<Vector2Int>();
			// the vector2 in the direction perpandicular to _axis
			Vector2Int perpandicularAxisPosDirection = _axis == Axis.X
				? Vector2Int.up
				: Vector2Int.right;
			// 
			for(int x = 0; x < levelWidth; x++)
			{
				for(int z = 0; z < levelLength; z++)
				{
					Vector2Int currentPosition = new Vector2Int(x, z);
					if(levelTiles[x][z].tileType == TileType.Corridor)
					{
						disallowedCorridorPositions.Add(currentPosition);
						for(int i = 0; i < minDistanceBetweenCorridors + 1; i++)
						{
							Vector2Int leftPos = currentPosition + perpandicularAxisPosDirection * i;
							Vector2Int rightPos = currentPosition - perpandicularAxisPosDirection * i;
							if(PositionIsWithinLevel(leftPos) && levelTiles[leftPos.x][leftPos.y].tileType != TileType.Corridor)
							{
								disallowedCorridorPositions.Add(leftPos);
							}
							if(PositionIsWithinLevel(rightPos) && levelTiles[rightPos.x][rightPos.y].tileType != TileType.Corridor)
							{
								disallowedCorridorPositions.Add(rightPos);
							}
						}
					}
					else
					{
						bool positionTooCloseToNearSide = !PositionIsWithinLevel(currentPosition + perpandicularAxisPosDirection * minDistanceBetweenCorridorAndLevelEdge);
						bool positionTooCloseToFarSide = !PositionIsWithinLevel(currentPosition - perpandicularAxisPosDirection * minDistanceBetweenCorridorAndLevelEdge);
						if(positionTooCloseToNearSide || positionTooCloseToFarSide)
						{
							disallowedCorridorPositions.Add(currentPosition);
						}
					}
				}
			}
			
			List<Vector2Int> allowedCorridorPositions = new List<Vector2Int>();
			for(int x = 0; x < levelWidth; x++)
			{
				for(int z = 0; z < levelLength; z++)
				{
					Vector2Int currentPosition = new Vector2Int(x, z);
					if(!disallowedCorridorPositions.Contains(currentPosition))
					{
						allowedCorridorPositions.Add(currentPosition);
					}
				}
			}
			return allowedCorridorPositions;
		}
		
		/// <summary>
		/// generates a network of corridors across the level
		/// </summary>
		private void GenerateCorridors()
		{
			int i = 0;
			while(i < targetNumberOfCorridors)
			{
				//a list of positions where x axis coridors are allowed to be generated.
				List<Vector2Int> allowedXAxisCorridorPositions = AllowedCorridorPositions(Axis.X);
				if(allowedXAxisCorridorPositions.Count > 0)
				{
					GenerateCorridor(allowedXAxisCorridorPositions[Random.Range(0, allowedXAxisCorridorPositions.Count)], Axis.X);
				}
				i++;
				if(i < targetNumberOfCorridors)
				{
					//a list of positions where z axis coridors are allowed to be generated.
					List<Vector2Int> allowedZAxisCorridorPositions = AllowedCorridorPositions(Axis.Z);
					if(allowedZAxisCorridorPositions.Count > 0)
					{
						GenerateCorridor(allowedZAxisCorridorPositions[Random.Range(0, allowedZAxisCorridorPositions.Count)], Axis.Z);
					}
					i++;
				}
			}
		}
		
		/// <summary>
		/// returns the taxicab distance between _from and _to
		/// </summary>
		private int TaxiCabDistance(Vector2Int _from, Vector2Int _to) => Mathf.Abs(_from.x - _to.x) + Mathf.Abs(_from.y - _to.y);
		
		/// <summary>
		/// will replace a wall at the position with a door. if the position is
		/// </summary>
		private void SetDoorTiles()
		{
			foreach(Room room in rooms)
			{
				//Debug.Log(room.RoomID);
				//the int in this dictionary refers to the id of the room on the other side of the door on this tile.
				List<LevelTile> tilesWithDoors = new List<LevelTile>();
				List<LevelTile> tilesWithWalls = new List<LevelTile>();
				foreach(LevelTile tile in room.OccupiedTiles)
				{
					foreach(LevelTile.TileEdge tileEdge in tile.tileEdges)
					{
						if(tileEdge == LevelTile.TileEdge.Door && !tilesWithDoors.Contains(tile))
						{
							tilesWithDoors.Add(tile);
						}
						if(tileEdge == LevelTile.TileEdge.Wall && !tilesWithWalls.Contains(tile))
						{
							tilesWithWalls.Add(tile);	
						}
					}
				}
				
				//this shuffles the list
				tilesWithWalls = tilesWithWalls.OrderBy( x => Random.value ).ToList( );
				foreach(LevelTile tile in tilesWithWalls)
				{
					List<Direction> possibleDirectionsToPlaceDoor = new List<Direction>();
					for(int i = 0; i < 4; i++)
					{
						if(tile.tileEdges[i] == LevelTile.TileEdge.Wall)
						{
							possibleDirectionsToPlaceDoor.Add((Direction)i);
						}
					}
					
					Direction direcitonToPlaceDoor = possibleDirectionsToPlaceDoor[Random.Range(0, possibleDirectionsToPlaceDoor.Count)];
					Vector2Int posOnOtherSideOfNewDoor = tile.PosInOneUnitInDirection(direcitonToPlaceDoor);
					if(PositionIsWithinLevel(posOnOtherSideOfNewDoor))
					{
						bool farEnoughFromAllDoors = true; 
						foreach(LevelTile tileWithDoor in tilesWithDoors)
						{
							if(TaxiCabDistance(tile.PositionInGrid, tileWithDoor.PositionInGrid) < minDistanceBetweenDoors)
							{
								//Debug.Log($"From: {tile.PositionInGrid}, To: {tileWithDoor.PositionInGrid}, Distance: {TaxiCabDistance(tile.PositionInGrid, tileWithDoor.PositionInGrid)}");
								farEnoughFromAllDoors = false;
								break;
							}
						}
						if(farEnoughFromAllDoors)
						{
							if(tile.TrySetDoor(direcitonToPlaceDoor))
							{
								tilesWithDoors.Add(tile);
							}
						}
					}
				}
			}
		}
		
		private void InstantiateLevelObjects()
		{
			foreach(GameObject levelTile in instantiatedLevelObjects)
			{
				Destroy(levelTile);
			}
			instantiatedLevelObjects.Clear();
			
			foreach(Room room in rooms)
			{
				foreach(Vector2Int position in room.OccupiedTilePositions)
				{
					Vector3 floorPosition = new Vector3(position.x, 0, position.y);
					//instantiatedLevelObjects.Add(Instantiate(floorPlaceHolder, floorPosition, Quaternion.identity));
					List<Vector2Int> pointsAroundPosition = PointsAroundPosition(position);
					for(int i = 0; i < 4; i++)
					{
						Vector2Int point = pointsAroundPosition[i];
						if(PositionIsWithinLevel(point))
						{
							if(levelTiles[point.x][point.y].RoomID != levelTiles[position.x][position.y].RoomID)
							{
								levelTiles[position.x][position.y].tileEdges[i] = LevelTile.TileEdge.Wall;
								//instantiatedLevelObjects.Add(Instantiate(wallPlaceHolder, floorPosition, Quaternion.FromToRotation(Vector3.forward, pointPosition - floorPosition)));
							}
						}
						else
						{
							levelTiles[position.x][position.y].tileEdges[i] = LevelTile.TileEdge.Wall;
							//instantiatedLevelObjects.Add(Instantiate(wallPlaceHolder, floorPosition, Quaternion.FromToRotation(Vector3.forward, pointPosition - floorPosition)));
						}
					}
				}
			}

			SetDoorTiles();
			
			foreach(List<LevelTile> levelTile in levelTiles)
			{
				foreach(LevelTile tile in levelTile)
				{
					tile.InstantiateTileObjects(gameObject.transform.position,  ref instantiatedLevelObjects, PlaceHolderRoomStyle);
				}
			}
		}

		public void GenerateLevel()
		{
			InitializeLevel();
			GenerateCorridors();
			AddEntrances();
			SetRooms();
			InstantiateLevelObjects();
		}
		
		private void Start()
		{
			GenerateLevel();
		}	
	}
}