using System;
using System.Collections.Generic;
using System.Linq;

using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;
namespace TFContent.Playspace
{
	public class WorldRoomObjectGroup : ComponentBehaviour//, IOdccUpdate
	{
		#region ODCCFunction
		///Awake 대신 사용.
		[ShowInInspector, ReadOnly]
		private QuerySystem roomObjectQuery;
		[ShowInInspector, ReadOnly]
		private OdccQueryCollector roomObjectCollector;

		public WorldMapBuilder Builder { get; internal set; }

		[SerializeField]
		[Range(0, 5)]
		private int neighborCreateDepth = 0;

		private HashSet<int> createNodeHashList;
		private Dictionary<int, RoomObject> createAllRoomList;

		[SerializeField,ReadOnly]
		private int currentRoomIndex;
		[SerializeField,ReadOnly]
		private RoomObject currentRoom;

		private int changeCurrentRoomIndex;
		private RoomObject changeCurrentRoom;

		protected override void BaseAwake()
		{
			createNodeHashList = new HashSet<int>();
			createAllRoomList = new Dictionary<int, RoomObject>();

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

			if(changeCurrentRoomIndex == roomNodeData.nodeIndex)
			{
				changeCurrentRoom = roomObject;
			}

			if(createNodeHashList.Add(roomNodeData.nodeIndex))
			{
				createAllRoomList.Add(roomNodeData.nodeIndex, roomObject);
			}
		}
		private void DestroyRoomObject(RoomObject roomObject)
		{
			if(!roomObject.ThisContainer.TryGetData<RoomNodeData>(out var roomNodeData)) return;

			if(currentRoomIndex == roomNodeData.nodeIndex)
			{
				currentRoomIndex = -1;
				currentRoom = null;
			}

			if(createNodeHashList.Remove(roomNodeData.nodeIndex))
			{
				createAllRoomList.Remove(roomNodeData.nodeIndex);
			}
		}

		///OnEnable 대신 사용.
		protected override void BaseEnable()
		{

		}
		///Start 대신 사용.
		protected override void BaseStart()
		{

		}
		///OnDisable 대신 사용.
		protected override void BaseDisable()
		{

		}
		///OnDestroy 대신 사용
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

			currentRoom = null;
			changeCurrentRoom = null;

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

		internal void ChangeCurrentNode(int nodeIndex, Action<IRoomObject> callback)
		{
			changeCurrentRoom = null;
			changeCurrentRoomIndex = nodeIndex;
			Builder.CreateRoom(nodeIndex, neighborCreateDepth, createNodeHashList, () => {
				if(changeCurrentRoom == null)
				{
					currentRoomIndex = changeCurrentRoomIndex;
					currentRoom = changeCurrentRoom;
				}
				changeCurrentRoomIndex = -1;
				changeCurrentRoom = null;

				callback?.Invoke(currentRoom);
			}, (List<int> neighborNodeList) => {
				IEnumerable<int> onlyInHashSet = createNodeHashList.Except(neighborNodeList);
				foreach(var item in onlyInHashSet)
				{
					if(TryGetRoomObject(item, out var iRoomObject))
					{
						iRoomObject.DestroyThis(true);
					}
				}
			});
		}
		internal bool TryGetRoomObject(int nodeIndex, out IRoomObject iRoomObject)
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
		internal void SetNeighborCreateDepth(int nodeDepth)
		{
			neighborCreateDepth = nodeDepth;
		}

		#endregion
	}
}