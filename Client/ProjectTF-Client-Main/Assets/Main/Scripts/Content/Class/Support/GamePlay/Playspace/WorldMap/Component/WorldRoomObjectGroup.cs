using System;
using System.Collections.Generic;
using System.Linq;

using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;
namespace TFContent.Playspace
{
	public class WorldRoomObjectGroup : ComponentBehaviour, IRoomObjectGroup
	{
		///Awake 대신 사용.
		[ShowInInspector, ReadOnly]
		private QuerySystem roomObjectQuery;
		[ShowInInspector, ReadOnly]
		private OdccQueryCollector roomObjectCollector;

		public IWorldMapBuilder Builder { get; internal set; }

		[SerializeField]
		[Range(0, 5)]
		private int createNeighborDepth = 0;
		public int CreateNeighborDepth { get => createNeighborDepth; set => createNeighborDepth = value; }

		private HashSet<int> createNodeHashList;
		private Dictionary<int, RoomObject> createAllRoomList;

		[SerializeField,ReadOnly]
		private int currentRoomNodeIndex;
		public int CurrentRoomNodeIndex => currentRoomNodeIndex;
		[SerializeField,ReadOnly]
		private RoomObject currentRoomObject;
		public IRoomObject CurrentRoomObject => currentRoomObject;

		[SerializeField,ReadOnly]
		private bool readyCurrentRoom = false;
		public bool ReadyCurrentRoom => readyCurrentRoom && currentRoomObject != null;

		protected override void BaseAwake()
		{
			readyCurrentRoom = false;
			currentRoomNodeIndex = -1;
			currentRoomObject = null;

			createNodeHashList = new HashSet<int>();
			createAllRoomList = new Dictionary<int, RoomObject>();

			Builder = ThisContainer.GetComponent<WorldMapBuilder>();

			roomObjectQuery = QuerySystemBuilder.CreateQuery().WithAll<RoomObject>().Build();
			roomObjectCollector = OdccQueryCollector.CreateQueryCollector(roomObjectQuery)
				.CreateChangeListEvent(UpdateRoomObject)
				.GetCollector();
		}
		private void UpdateRoomObject(ObjectBehaviour item, bool isAdd)
		{
			if(item is not RoomObject roomObject) return;
			if(roomObject == null) return;
			if(isAdd)
			{
				CreateRoomObject(roomObject);
			}
			else
			{
				DestroyRoomObject(roomObject);
			}
		}
		private void CreateRoomObject(RoomObject roomObject)
		{
			if(!roomObject.ThisContainer.TryGetData<RoomNodeData>(out var roomNodeData)) return;

			if(createNodeHashList.Add(roomNodeData.nodeIndex))
			{
				createAllRoomList.Add(roomNodeData.nodeIndex, roomObject);
			}
		}
		private void DestroyRoomObject(RoomObject roomObject)
		{
			if(!roomObject.ThisContainer.TryGetData<RoomNodeData>(out var roomNodeData)) return;

			if(currentRoomNodeIndex == roomNodeData.nodeIndex)
			{
				currentRoomNodeIndex = -1;
				currentRoomObject = null;
			}

			if(createNodeHashList.Remove(roomNodeData.nodeIndex))
			{
				createAllRoomList.Remove(roomNodeData.nodeIndex);
			}
		}
		protected override void BaseDestroy()
		{
			if(roomObjectCollector != null)
			{
				roomObjectCollector.DeleteChangeListEvent(UpdateRoomObject);
				OdccQueryCollector.DeleteQueryCollector(roomObjectQuery);
				roomObjectCollector = null;
			}
			roomObjectQuery = null;

			Builder = null;

			readyCurrentRoom = false;
			currentRoomNodeIndex = -1;
			currentRoomObject = null;

			if(createNodeHashList != null)
			{
				createNodeHashList.Clear();
				createNodeHashList = null;
			}
			if(createAllRoomList != null)
			{
				createAllRoomList.Clear();
				createAllRoomList = null;
			}
		}


		async Awaitable<IRoomObject> CreateRoom(int createNodeIndex, Action<List<IRoomObject>> createNeighborRoom)
		{
			readyCurrentRoom = false;
			await Builder.CreateRoom(createNodeIndex, createNeighborDepth, createNodeHashList, true, (List<int> neighborNodeList) => {
				IEnumerable<int> onlyInHashSet = createNodeHashList.Except(neighborNodeList);
				foreach(var item in onlyInHashSet)
				{
					if(item != createNodeIndex && TryGetCreateRoom(item, out var iRoomObject))
					{
						iRoomObject.DestroyThis(true);
					}
				}
				if(createNeighborRoom != null)
				{
					List<IRoomObject> neighborRooms = new List<IRoomObject>();
					int length = neighborNodeList.Count;
					for(int i = 0 ; i < length ; i++)
					{
						if(createAllRoomList.TryGetValue(neighborNodeList[i], out var neighborRoom))
						{
							neighborRooms.Add(neighborRoom);
						}
					}
					createNeighborRoom.Invoke(neighborRooms);
				}
			});

			if(createAllRoomList.TryGetValue(createNodeIndex, out var roomObject))
			{
				currentRoomNodeIndex = createNodeIndex;
				currentRoomObject = roomObject;
			}
			else
			{
				currentRoomNodeIndex = -1;
				currentRoomObject = null;
			}
			readyCurrentRoom = true;
			return currentRoomObject;
		}
		bool TryGetCreateRoom(int nodeIndex, out IRoomObject iRoomObject)
		{
			if(createAllRoomList.TryGetValue(nodeIndex, out var roomObject))
			{
				iRoomObject = roomObject;
			}
			else
			{
				iRoomObject = null;
			}
			return iRoomObject != null;
		}
		void ClearAllCreateRoom()
		{
			readyCurrentRoom = false;
			currentRoomNodeIndex = -1;
			currentRoomObject = null;
			createNodeHashList.Clear();
			foreach(var item in createAllRoomList)
			{
				item.Value.DestroyThis(true);
			}
		}



		async Awaitable<IRoomObject> IRoomObjectGroup.CreateRoom(int createNodeIndex, Action<List<IRoomObject>> createNeighborRoom) => await CreateRoom(createNodeIndex, createNeighborRoom);
		bool IRoomObjectGroup.TryGetCreateRoom(int nodeIndex, out IRoomObject iRoomObject) => TryGetCreateRoom(nodeIndex, out iRoomObject);
		void IRoomObjectGroup.ClearAllCreateRoom() => ClearAllCreateRoom();
	}
}