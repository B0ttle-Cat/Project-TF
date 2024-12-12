using System;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Networking;

namespace TFContent
{
	[Serializable]
	public class MapManageAPI
	{
		[BoxGroup("MapData Endpoint"), ShowInInspector]
		public string MapIp => "20.41.121.220";
		[BoxGroup("MapData Endpoint"), ShowInInspector]
		public string MapPort => "35000";
		[BoxGroup("MapData Endpoint"), ShowInInspector]
		public string MapAPIURL => $"http://{MapIp}:{MapPort}/v1/map";
		private bool EndpointValidity => !(string.IsNullOrWhiteSpace(MapIp) || string.IsNullOrWhiteSpace(MapPort) || MapIp == "0.0.0.0" || MapPort == "0");

		[Serializable]
		private struct UploadData
		{
			public string id;
		}
		[Serializable]
		private struct DownloadData
		{
			public string data;
			public string key;
		}

		public async Awaitable<string> Editor_UploadWorldMapRawData(WorldMapRawData worldMapRawData)
		{
			if(!EndpointValidity)
			{
				Debug.LogAssertion("ip or port is null. enter valid endpoint first.");
				return "";
			}
			string worldMapJsonData = JsonUtility.ToJson(worldMapRawData);

			UploadData uploadData = await UploadMap(MapAPIURL, worldMapJsonData);
			return uploadData.id;

			static async Awaitable<UploadData> UploadMap(string url, string jsonData)
			{
				// Create the UnityWebRequest for POST
				Debug.Log($"Post:{url}");
				using(UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
				{
					// Set the JSON body
					byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);
					webRequest.uploadHandler = new UploadHandlerRaw(jsonBytes);
					webRequest.downloadHandler = new DownloadHandlerBuffer();
					webRequest.SetRequestHeader("Content-Type", "application/json");

					// Send the request
					await webRequest.SendWebRequest();

					// Handle the response
					if(webRequest.result == UnityWebRequest.Result.Success)
					{
						string download = webRequest.downloadHandler.text;
						Debug.Log($"Response: {download}");
						try
						{
							var uploadData = JsonConvert.DeserializeObject<UploadData>(download);
							return uploadData;
						}
						catch(Exception ex)
						{
							Debug.LogException(ex);
						}
					}
					else
					{
						Debug.LogError($"Error: {webRequest.error}");
					}
				}
				return default;
			}
		}
		public async Awaitable<(WorldMapRawData data, string key)[]> Editor_GetWorldMapRawDataList()
		{
			if(!EndpointValidity)
			{
				Debug.LogAssertion("ip or port is null. enter valid endpoint first.");
				return null;
			}

			var downloadMaps = await DownloadMap($"{MapAPIURL}");
			if(downloadMaps == null) return new (WorldMapRawData data, string key)[0];
			else return downloadMaps.Select(i => (JsonUtility.FromJson<WorldMapRawData>(i.data), i.key)).ToArray();

			static async Awaitable<DownloadData[]> DownloadMap(string url)
			{
				Debug.Log($"Get:{url}");
				using(UnityWebRequest webRequest = new UnityWebRequest(url, "Get"))
				{
					webRequest.downloadHandler = new DownloadHandlerBuffer();
					webRequest.SetRequestHeader("Content-Type", "application/json");

					// Send the request
					await webRequest.SendWebRequest();

					// Handle the response
					if(webRequest.result == UnityWebRequest.Result.Success)
					{
						string download = webRequest.downloadHandler.text;
						Debug.Log($"Response: {download}");

						try
						{
							var downloadData = JsonConvert.DeserializeObject<DownloadData[]>(download);
							return downloadData;
						}
						catch(Exception ex)
						{
							Debug.LogException(ex);
						}
					}
					else
					{
						Debug.LogError($"Error: {webRequest.error}");
					}
				}
				return null;
			}
		}
		public async Awaitable<(WorldMapRawData data, string key)> Editor_DownloadWorldMapRawData(string mapHaskey)
		{
			if(!EndpointValidity)
			{
				Debug.LogAssertion("ip or port is null. enter valid endpoint first.");
				return default;
			}

			var downloadMap = await DownloadMap($"{MapAPIURL}?key={mapHaskey}");
			return (JsonUtility.FromJson<WorldMapRawData>(downloadMap.data), downloadMap.key);

			static async Awaitable<DownloadData> DownloadMap(string url)
			{
				Debug.Log($"Get:{url}");
				using(UnityWebRequest webRequest = new UnityWebRequest(url, "Get"))
				{
					webRequest.downloadHandler = new DownloadHandlerBuffer();
					webRequest.SetRequestHeader("Content-Type", "application/json");

					// Send the request
					await webRequest.SendWebRequest();

					// Handle the response
					if(webRequest.result == UnityWebRequest.Result.Success)
					{
						string download = webRequest.downloadHandler.text;
						Debug.Log($"Response: {download}");

						try
						{
							var downloadData = JsonConvert.DeserializeObject<DownloadData[]>(download);
							if(downloadData != null && downloadData.Length > 0) return downloadData[0];
						}
						catch(Exception ex)
						{
							Debug.LogException(ex);
						}
					}
					else
					{
						Debug.LogError($"Error: {webRequest.error}");
					}
				}

				return default;
			}
		}

	}
}
