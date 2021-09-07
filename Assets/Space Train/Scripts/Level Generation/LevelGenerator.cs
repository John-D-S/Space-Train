using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Unity.AI.Navigation;

// Needed to comment this out for the build for some reason :)
// ~Kieran 6:03 Tuesday 31/8/2021
//using UnityEditor.AI;

using UnityEngine;


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

	public class LevelGenerator : MonoBehaviour
	{
		[SerializeField, Tooltip("How far apart each tile is spaced")] private float tileSize = 2f;
		public float TileSize => tileSize;
		[SerializeField, Tooltip("How many tiles are generated in the positive z direction")] private int levelLength = 32;
		[SerializeField, Tooltip("How many tiles are generated in the positive x direction")] private int levelWidth = 8;
		
		[SerializeField, Tooltip("The minimum distance between corridors. This in part determines the size of the rooms.")] private int minDistanceBetweenCorridors = 2;
		[SerializeField, Tooltip("The minimum distance between corridors and the edge of the level. This helps to determine the size of the rooms which are adjacent to the edge.")] private int minDistanceBetweenCorridorAndLevelEdge = 2;
		[SerializeField, Tooltip("The Maximum length of corridors.")] private int maxCorridorLength = 6;
		[SerializeField, Tooltip("How close can doors of the same room be together.")] private int minDistanceBetweenDoors = 6;

		[SerializeField, Tooltip("How many corridors to try to create.")] private int targetNumberOfCorridors = 8;

		[SerializeField, Tooltip("What room style should corridors have?")] private RoomStyle corridorRoomStyle;
		[SerializeField, Tooltip("What room styles should be included in the generation of rooms.")] private List<RoomStyle> roomStyles;
		[System.NonSerialized] public List<List<LevelTile>> levelTiles = new List<List<LevelTile>>();
		
		[System.NonSerialized] public List<Room> rooms = new List<Room>();

		[SerializeField, Tooltip("The Navmesh Surface that will need to update when the level has generated.")] private NavMeshSurface navMeshSurface;
		
		private List<GameObject> instantiatedLevelObjects = new List<GameObject>();

		
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
		
		/// <summary>
		/// gets all the tiles connected to the tile at the given position which share the same tyleType and roomID
		/// It does this like flood filling
		/// </summary>
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

		/// <summary>
		/// set the room of each tile in levelTiles.
		/// </summary>
		private void SetRooms()
		{
			//clear any already set rooms
			rooms.Clear();
			int currentRoomID = 2;
			// shuffle the room styles so that the order of the rooms are randomized without the frequency of them being randomized,
			// i.e. there should be the same number of each room style in the level but shuffled.
			List<RoomStyle> shuffledRoomStyles = roomStyles.OrderBy( x => Random.value ).ToList( ); 
			int i = 0;
			for(int x = 0; x < levelWidth; x++)
			{
				for(int z = 0; z < levelLength; z++)
				{
					LevelTile levelTile = levelTiles[x][z];
					if(levelTile.RoomID == 0)
					{
						List<LevelTile> connectedLevelTiles = new List<LevelTile>();
						foreach(Vector2Int position in ConnectedLevelTilePositions(new Vector2Int(x, z)))
						{
							connectedLevelTiles.Add(levelTiles[position.x][position.y]);	
						}

						RoomStyle roomStyle;
						if(levelTile.tileType == TileType.Corridor)
						{
							roomStyle = corridorRoomStyle;
						}
						else
						{
							int roomStyleIndex = i % roomStyles.Count;
							if(roomStyleIndex == 0)
							{
								shuffledRoomStyles = roomStyles.OrderBy( x => Random.value ).ToList( );
							}
							roomStyle = shuffledRoomStyles[roomStyleIndex % roomStyles.Count];
							i += 1;
						}
						Room newRoom = new Room(currentRoomID, ref connectedLevelTiles, roomStyle, this);
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
				levelTiles[position.x][position.y].entrancePositions[2] = true;
				GenerateCorridor(position, Axis.X);
			}
			foreach(Vector2Int position in frontEntrancePostions)
			{
				levelTiles[position.x][position.y].entrancePositions[0] = true;
				GenerateCorridor(position, Axis.X);
			}
		}

		/// <summary>
		/// returns true if the postition is inside of the level and false if it is outside
		/// </summary>
		public bool PositionIsWithinLevel(Vector2Int _postion)
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

		/// <summary>
		/// rotatest the given position clockwise about 0,0 in 90 increments _rotAmount times
		/// </summary>
		private Vector2Int RotateVector2Int(Vector2Int _pos, int _rotAmount)
		{
			int actualRotAmount = _rotAmount % 4;
			if(actualRotAmount == 0)
				return _pos;
			if(actualRotAmount == 1)
				return new Vector2Int(_pos.y, -_pos.x);
			if(actualRotAmount == 2)
				return -_pos;
			if(actualRotAmount == 3)
				return new Vector2Int(-_pos.y, _pos.x);
			return _pos;
		}
		
		/// <summary>
		/// try to place a prop in a given room.
		/// </summary>
		/// <returns>returns true if the placement was successful</returns>
		private bool TryPlaceProp(ref Room _room, PropSpawningInfo _propSpawningInfo)
		{
			List<LevelTile> roomTiles = _room.OccupiedTiles.ToArray().ToList();
			roomTiles = roomTiles.OrderBy( x => Random.value ).ToList();
			
			foreach(LevelTile tile in roomTiles)
			{
				int randRotOffset = Random.Range(0, 4);
				
				//try to see if the prop fits with each rotation
				//i represents how many 90 degree increments of rotation we are dealing with.
				for(int i = 0; i < 4; i++)
				{
					int rot = (i + randRotOffset) % 4;

					if(TryPlacePropInPos(_room, _propSpawningInfo, rot, tile, false))
						return true;
				}
			}
			return false;
		}

		/// <summary>
		/// try to place a prop in a given position.
		/// </summary>
		/// <returns>returns true if the placement was successful</returns>
		private bool TryPlacePropInPos(Room _room, PropSpawningInfo _propSpawningInfo, int rot, LevelTile tile, bool _ignoreWallPlacement)
		{
			int roomID = _room.RoomID;
			
			Vector2Int roomSmallestCorner = new Vector2Int(levelWidth, levelLength);
			Vector2Int roomLargestCorner = Vector2Int.zero;

			Vector2Int _propSize = _propSpawningInfo.PropComponent.PropSize;
			
			List<Vector2Int> rotationMultipliers = new List<Vector2Int>()
			{
				new Vector2Int(1, 1),
				new Vector2Int(1, -1),
				new Vector2Int(-1, -1),
				new Vector2Int(-1, 1)
			};
			
			//if the prop is marked to be placed next to a wall, these are all the positions that must have a wall on them
			List<Vector2Int> requiredWallPositions = new List<Vector2Int>();

			if((_propSpawningInfo.PropComponent.ZPosWallPlacement || _propSpawningInfo.PropComponent.XPosWallPlacement || _propSpawningInfo.PropComponent.ZNegWallPlacement || _propSpawningInfo.PropComponent.XNegWallPlacement) && !_ignoreWallPlacement)
			{
				//the list of positions walls must be in relation to the prop.
				List<Vector2Int> localWallPositions = new List<Vector2Int>();

				Vector2Int usedPropSize = _propSize - Vector2Int.one;

				//jeez :/
				if(_propSpawningInfo.PropComponent.ZPosWallPlacement)
					foreach(Vector2Int pos in PositionsInBox(new Vector2Int(0, usedPropSize.y), usedPropSize))
						localWallPositions.Add(pos);
				if(_propSpawningInfo.PropComponent.XPosWallPlacement)
					foreach(Vector2Int pos in PositionsInBox(usedPropSize, new Vector2Int(usedPropSize.x, 0)))
						localWallPositions.Add(pos);
				if(_propSpawningInfo.PropComponent.ZNegWallPlacement)
					foreach(Vector2Int pos in PositionsInBox(new Vector2Int(usedPropSize.x, 0), Vector2Int.zero))
						localWallPositions.Add(pos);
				if(_propSpawningInfo.PropComponent.XNegWallPlacement)
					foreach(Vector2Int pos in PositionsInBox(Vector2Int.zero, new Vector2Int(0, usedPropSize.y)))
						localWallPositions.Add(pos);

				foreach(Vector2Int position in localWallPositions)
					requiredWallPositions.Add(RotateVector2Int(position, rot) + tile.PositionInGrid);
			}

			List<Vector2Int> positionsInRotatedProp = PositionsInBox(tile.PositionInGrid, tile.PositionInGrid + RotateVector2Int((_propSize - Vector2Int.one), rot));
			bool canPlacePropHere = true;
			foreach(Vector2Int posInRotatedProp in positionsInRotatedProp)
			{
				if(!PositionIsWithinLevel(posInRotatedProp) || levelTiles[posInRotatedProp.x][posInRotatedProp.y].room.RoomID != roomID)
				{
					canPlacePropHere = false;
					break;
				}

				if(levelTiles[posInRotatedProp.x][posInRotatedProp.y].OccupiedByProp)
				{
					canPlacePropHere = false;
					break;
				}

				if(requiredWallPositions.Count > 0)
				{
					bool allWallPositionsHaveHalls = true;
					foreach(Vector2Int wallPostion in requiredWallPositions)
					{
						if(!PositionIsWithinLevel(wallPostion) || !levelTiles[wallPostion.x][wallPostion.y].HasWall)
						{
							allWallPositionsHaveHalls = false;
							break;
						}
					}

					if(!allWallPositionsHaveHalls)
					{
						canPlacePropHere = false;
						break;
					}
					//check each of the positions in required wallpositions and disallow placement if any of the tiles at those positions do not have a wall on them.
				}
			}
			if(canPlacePropHere)
			{
				foreach(Vector2Int wallPosition in requiredWallPositions)
				{
					allUsedWallPositions.Add(wallPosition);
				}

				foreach(Vector2Int pos in positionsInRotatedProp)
				{
					//Debug.Log($"{_propSpawningInfo.PropGameObject.name}: {pos * 2}");
					levelTiles[pos.x][pos.y].OccupiedByProp = true;
				}

				//find a way to add the prop's instantiation rotation and position to a list to be instantiated later.
				_room.RoomProps.Add(new Room.InstantiationInfo(_propSpawningInfo.PropGameObject, new Vector3(tile.PositionInGrid.x - rotationMultipliers[rot].x * .5f, 0, tile.PositionInGrid.y - rotationMultipliers[rot].y * .5f), Quaternion.AngleAxis(rot * 90, Vector3.up)));
				if(_propSpawningInfo.PropComponent.Repeat)
				{
					Direction repeatDirection = _propSpawningInfo.PropComponent.RepeatDirection;
					Vector2Int nextTileDirection = PointsAroundPosition(Vector2Int.zero)[(int) repeatDirection];
					int nextTileDistance = repeatDirection == Direction.Zpos || repeatDirection == Direction.Zneg
						? _propSize.y
						: _propSize.x;
					Vector2Int nextTilePosition = tile.PositionInGrid + nextTileDistance * nextTileDirection;
					if(PositionIsWithinLevel(new Vector2Int(nextTilePosition.x, nextTilePosition.y)))
					{
						LevelTile nextTile = levelTiles[nextTilePosition.x][nextTilePosition.y];						
						TryPlacePropInPos(_room, _propSpawningInfo, rot, nextTile, true);
					}
				}
				return true;
			}
			return false;
		}

		private List<Vector2Int> allUsedWallPositions = new List<Vector2Int>();
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;
			foreach(Vector2Int position in allUsedWallPositions)
			{
				Gizmos.DrawSphere(new Vector3(position.x * tileSize, 1, position.y * tileSize), 1);
			}
		}

		private void SetProps()
		{
			allUsedWallPositions.Clear();
			//foreach(Room room in rooms) cannot be used here since the variable being iterated on cannot be passed into a funciton as a ref which is what is needed for TryPlaceProp()
			for(int i = 0; i < rooms.Count; i++)
			{
				Room room = rooms[i];
				List<LevelTile> roomTiles = room.OccupiedTiles.ToArray().ToList();
				roomTiles = roomTiles.OrderBy( x => Random.value ).ToList();
				//place the props associated with room's roomstyle until there is no room left or the maximum number of each prop has been used
				
				//a dictionary to contain the number of each prop already placed
				Dictionary<PropSpawningInfo, int> numberOfPropAdded = new Dictionary<PropSpawningInfo, int>();
				//a dictionary to contain whether it is possible to place any more of each prop in room.roomStyle.Props
				Dictionary<PropSpawningInfo, bool> propCanBeAdded = new Dictionary<PropSpawningInfo, bool>();
				//set the initial values of the above dictionaries
				foreach(PropSpawningInfo prop in room.roomStyle.Props)
				{
					numberOfPropAdded[prop] = 0;
					propCanBeAdded[prop] = true;
				}
				
				bool morePropsCanBeAdded = true;
				RoomStyle roomRoomStyle = room.roomStyle;
				while(morePropsCanBeAdded)
				{
					foreach(PropSpawningInfo prop in roomRoomStyle.Props)
					{
						if(propCanBeAdded[prop] && numberOfPropAdded[prop] < prop.MaxNumberInRoom)
						{
							propCanBeAdded[prop] = TryPlaceProp(ref room, prop);
							if(propCanBeAdded[prop])
							{
								numberOfPropAdded[prop]++;
							}
						}
					}

					morePropsCanBeAdded = false;
					foreach(PropSpawningInfo prop in roomRoomStyle.Props)
					{
						if(propCanBeAdded[prop] && numberOfPropAdded[prop] < prop.MaxNumberInRoom)
						{
							morePropsCanBeAdded = true;
							break;
						}
					}
				}
			}
		}

		private void SetWalls()
		{
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
		}

		private void InstantiateLevelObjects()
		{
			foreach(GameObject thing in instantiatedLevelObjects)
			{
				Destroy(thing);
			}
			instantiatedLevelObjects.Clear();
			foreach(Room room in rooms)
			{
				room.InstantiateRoomObjects(gameObject.transform.position,  ref instantiatedLevelObjects, corridorRoomStyle);
			}
		}

		private IEnumerator BuildNavMesh()
		{
			yield return null;
			navMeshSurface.BuildNavMesh();
		}
		
		/// <summary>
		/// sequentially do all the things that generates the level
		/// </summary>
		public void GenerateLevel()
		{
			InitializeLevel();
			GenerateCorridors();
			AddEntrances();
			SetRooms();
			SetWalls();
			SetProps();
			InstantiateLevelObjects();
			StartCoroutine(BuildNavMesh());
		}
		
		private void Start()
		{
			// Its just that easy!
			GenerateLevel();
		}	
	}
}