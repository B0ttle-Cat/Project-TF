using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System.Collections;
using System.Text;
using TFSystem;

using UnityEngine;
using UnityEngine.Networking;

namespace TFContent.Playspace
{
	public class WorldMapSystem : SystemState
	{
		protected override void AwakeOnSystem()
		{
		}

		protected override void DestroyOnSystems()
		{
		}

		protected override async Awaitable StartWaitSystem()
		{
		}

		protected override async Awaitable EndedWaitSystem()
		{
		}

		[Button]
		void TestCreateWorldMapRawData()
		{
			if(!ThisContainer.TryGetData<WorldMapUserSettingData>(out var mapUserSetting)) return;
			if(!ThisContainer.TryGetData<WorldMapBuildInfo>(out var mapBuildInfo)) return;

			var worldMapRawData = WorldMapRawData.CreateSample(mapUserSetting);
			mapBuildInfo.worldMapRawData = worldMapRawData;
		}

		class MapData
		{
			public MapData(WorldMapBuildInfo info)
			{
				this.info = info;
				mapSize = info.worldMapRawData.mapSize;
				nodes = info.worldMapRawData.roomNodeArray;
				variantDatas = info.worldMapRawData.roomVariationDataArray;
            }
			private WorldMapBuildInfo info;
			public Vector2Int mapSize;
			public WorldMapRawData.RoomNodeData[] nodes;
			public WorldMapRawData.RoomVariationData[] variantDatas;
        }

		[SerializeField]
		public string ip = "0.0.0.0";
        [SerializeField]
        public string port = "0";

		[Button]
		void SaveWorldMapRawData()
		{
            if (!ThisContainer.TryGetData<WorldMapBuildInfo>(out var mapBuildInfo)) return;
			MapData mapData = new MapData(mapBuildInfo);
			string jsonData = JsonUtility.ToJson(mapData);

			if (ip == "0.0.0.0" || port == "0")
			{
				Debug.LogAssertion("ip or port is null. enter valid endpoint first.");
				return;
			}

			StartCoroutine(PostJsonToEndpoint($"http://{ip}:{port}/v1/map", jsonData));
        }

        private IEnumerator PostJsonToEndpoint(string url, string jsonData)
        {
            // Create the UnityWebRequest for POST
            using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
            {
                // Set the JSON body
                byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);
                webRequest.uploadHandler = new UploadHandlerRaw(jsonBytes);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");

                // Send the request
                yield return webRequest.SendWebRequest();

                // Handle the response
                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"Response: {webRequest.downloadHandler.text}");
                }
                else
                {
                    Debug.LogError($"Error: {webRequest.error}");
                }
            }
        }

        public void OnDrawGizmos()
		{
			if(!ThisContainer.TryGetData<WorldMapBuildInfo>(out var mapBuildInfo)) return;
			mapBuildInfo.worldMapRawData.DrawGizmos(ThisTransform.position);
		}
	}
}