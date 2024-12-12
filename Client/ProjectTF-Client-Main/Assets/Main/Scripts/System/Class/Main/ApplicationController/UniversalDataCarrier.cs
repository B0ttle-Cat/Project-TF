using System.Collections.Generic;

using BC.ODCC;

using Sirenix.OdinInspector;

using UnityEngine;
namespace TFSystem
{
	public class UniversalDataCarrier : DataObject, IDataCarrier
	{
		private string lastKeyHandle;

		[ShowInInspector]
		private bool ShowLog { get; set; } = true;

		[ShowInInspector, ReadOnly]
		private Dictionary<string, object> lockDataCarrier;
		[ShowInInspector, ReadOnly]
		private Dictionary<string, object> dataCarrier;
		public UniversalDataCarrier() : base()
		{
			lastKeyHandle = null;
			lockDataCarrier = new Dictionary<string, object>();
			dataCarrier = new Dictionary<string, object>();
		}

		protected override void Disposing()
		{
			if(lockDataCarrier != null)
			{
				lockDataCarrier.Clear();
				lockDataCarrier = null;
			}
			if(dataCarrier != null)
			{
				dataCarrier.Clear();
				dataCarrier = null;
			}
		}
		private void Log(string log)
		{
			if(ShowLog) Debug.Log($"NetworkLog: {log}");
		}

		IDataCarrier IDataCarrier.AddKey(string key)
		{
			key = key.Trim();
			lastKeyHandle = key;
			if(string.IsNullOrWhiteSpace(key))
			{

			}
			else if(lockDataCarrier.ContainsKey(key) || dataCarrier.ContainsKey(key))
			{

			}
			else
			{
				dataCarrier.Add(key, null);
			}
			return this;
		}
		IDataCarrier IDataCarrier.AddData(string key, object value)
		{
			key = key.Trim();
			lastKeyHandle = key;
			if(string.IsNullOrWhiteSpace(key))
			{

			}
			else if(lockDataCarrier.ContainsKey(key))
			{

			}
			else if(dataCarrier.ContainsKey(key))
			{
				dataCarrier[key] = value;
			}
			else
			{
				dataCarrier.Add(key, value);
			}
			return this;
		}
		IDataCarrier IDataCarrier.RemoveData(string key, bool includeLockData)
		{
			key = key.Trim();
			lastKeyHandle = "";

			if(dataCarrier.Remove(key))
			{
				Log($"DataCarrier: Remove Data {key}");
			}
			else if(includeLockData && lockDataCarrier.Remove(key))
			{
				Log($"DataCarrier: Remove Data(Lock) {key}");
			}
			return this;
		}
		IDataCarrier IDataCarrier.ClearData(bool includeLockData)
		{
			Log($"DataCarrier: ClearData. Include Lock:{includeLockData}");
			lastKeyHandle = "";
			if(includeLockData) lockDataCarrier.Clear();
			dataCarrier.Clear();
			return this;
		}
		IDataCarrier IDataCarrier.HasKey(string key, out bool hasKey)
		{
			key = key.Trim();
			lastKeyHandle = key;
			if(string.IsNullOrWhiteSpace(key))
			{
				hasKey = false;
			}
			else if(lockDataCarrier.ContainsKey(key))
			{
				hasKey = true;
			}
			else if(dataCarrier.ContainsKey(key))
			{
				hasKey = true;
			}
			else
			{
				hasKey = false;
			}
			return this;
		}
		IDataCarrier IDataCarrier.PopKey(string key, out bool hasKey)
		{
			key = key.Trim();
			lastKeyHandle = "";
			if(string.IsNullOrWhiteSpace(key))
			{
				hasKey = false;
			}
			else if(lockDataCarrier.ContainsKey(key))
			{
				hasKey = true;
				Log($"DataCarrier: Can't Pop. Is Lock Data. {key}");
			}
			else if(dataCarrier.ContainsKey(key))
			{
				if(dataCarrier.Remove(key))
				{
					Log($"DataCarrier: PopData {key}");
				}
				hasKey = true;
			}
			else
			{
				hasKey = false;
			}
			return this;
		}
		IDataCarrier IDataCarrier.GetData<T>(string key, out T value, T defaultValue)
		{
			key = key.Trim();
			lastKeyHandle = key;
			if(string.IsNullOrWhiteSpace(key))
			{
				value = defaultValue;
			}
			else if(lockDataCarrier.TryGetValue(key, out var _lockData) && _lockData is T result1)
			{
				value = result1;
			}
			else if(dataCarrier.TryGetValue(key, out var _unlockData) && _unlockData is T result2)
			{
				value = result2;
			}
			else
			{
				value = defaultValue;
			}
			return this;
		}
		IDataCarrier IDataCarrier.PopData<T>(string key, out T value, T defaultValue)
		{
			key = key.Trim();
			lastKeyHandle = "";
			if(string.IsNullOrWhiteSpace(key))
			{
				value = defaultValue;
			}
			else if(lockDataCarrier.TryGetValue(key, out var _lockData) && _lockData is T result1)
			{
				value = result1;
				Log($"DataCarrier: Can't Pop. Is Lock Data. {key}");
			}
			else if(dataCarrier.TryGetValue(key, out var _unlockData) && _unlockData is T result2)
			{
				if(dataCarrier.Remove(key))
				{
					Log($"DataCarrier: PopData {key}");
				}
				value = result2;
			}
			else
			{
				value = defaultValue;
			}
			return this;
		}
		IDataCarrier IDataCarrier.Lock()
		{
			string key = lastKeyHandle;
			if(string.IsNullOrWhiteSpace(key))
			{

			}
			else if(!lockDataCarrier.ContainsKey(key) && dataCarrier.ContainsKey(key))
			{
				lockDataCarrier.Add(key, dataCarrier[key]);
				dataCarrier.Remove(key);
			}
			return this;
		}
		IDataCarrier IDataCarrier.Unlock()
		{
			string key = lastKeyHandle;
			if(string.IsNullOrWhiteSpace(key))
			{

			}
			else if(lockDataCarrier.ContainsKey(key) && !dataCarrier.ContainsKey(key))
			{
				dataCarrier.Add(key, lockDataCarrier[key]);
				lockDataCarrier.Remove(key);
			}
			return this;
		}
		IDataCarrier IDataCarrier.Lock(string key)
		{
			key = key.Trim();
			lastKeyHandle = key;
			if(string.IsNullOrWhiteSpace(key))
			{

			}
			else if(!lockDataCarrier.ContainsKey(key) && dataCarrier.ContainsKey(key))
			{
				lockDataCarrier.Add(key, dataCarrier[key]);
				dataCarrier.Remove(key);
			}
			return this;
		}
		IDataCarrier IDataCarrier.Unlock(string key)
		{
			key = key.Trim();
			lastKeyHandle = key;
			if(string.IsNullOrWhiteSpace(key))
			{

			}
			else if(lockDataCarrier.ContainsKey(key) && !dataCarrier.ContainsKey(key))
			{
				dataCarrier.Add(key, lockDataCarrier[key]);
				lockDataCarrier.Remove(key);
			}
			return this;
		}
		bool IDataCarrier.Contains(string key)
		{
			key = key.Trim();
			lastKeyHandle = key;
			if(string.IsNullOrWhiteSpace(key))
			{
				return false;
			}
			else
			{
				return lockDataCarrier.ContainsKey(key) || dataCarrier.ContainsKey(key);
			}
		}
		bool IDataCarrier.IsLock(string key)
		{
			key = key.Trim();
			lastKeyHandle = key;
			if(string.IsNullOrWhiteSpace(key))
			{
				return false;
			}
			else
			{
				return lockDataCarrier.ContainsKey(key) && !dataCarrier.ContainsKey(key);
			}
		}
	}
}