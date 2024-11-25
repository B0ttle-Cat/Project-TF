using System;

using BC.ODCC;
using BC.Sequence;

using UnityEngine;

namespace BC.Sequence
{
	public static partial class NodeBuilder
	{
		internal static NodeGraph _CallAction(NodeGraph parent, System.Action action)
		{
			var newBuilder = new NodeGraph(new CallAction(action));
			newBuilder.SetParent(parent);
			return newBuilder;
		}
		public static NodeGraph CallAction(System.Action action)
		{
			return _CallAction(null, action);
		}
		public static NodeGraph CallAction(this NodeGraph parent, System.Action action)
		{
			_CallAction(parent, action);
			return parent;
		}
	}
	public class CallAction : BC.Sequence.Action
	{
		public System.Action action;

		public CallAction(System.Action action)
		{
			this.action=action;
		}

		protected override State OnStart()
		{
			try
			{
				action?.Invoke();
				return State.Success;
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
				return State.Failure;
			}
		}
	}
}
namespace TFContent
{
	public class WorldMapBuilder : ComponentBehaviour
	{
		SequencePlayer mapBuildPlayer;
		WorldMapBuildData worldMapBuildData;
		protected override void BaseAwake()
		{
			var mapBuildGraph = SequenceBuilder.Create("MapBuilder")
				.Sequence("MapBuilder")
					.Logger("Start - MapBuilder")
					.CallAction(WorldMapBuilder_GetBuilderData)
					.Condition(() => worldMapBuildData != null)
						.True(NodeBuilder.CallAction(null))
					.Logger("End - MapBuilder")
				.Return();
			mapBuildPlayer = SequenceBuilder.Build(mapBuildGraph);
		}

		protected override void BaseStart()
		{
			base.BaseStart();
		}

		private void WorldMapBuilder_GetBuilderData()
		{
			ThisContainer.TryGetData(out worldMapBuildData);
		}
	}
}
