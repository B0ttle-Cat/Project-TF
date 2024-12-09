using BC.ODCC;
namespace TFSystem
{
	public interface IDataCarrier : IOdccData
	{
		IDataCarrier AddKey(string key);
		IDataCarrier AddData(string key, object value);
		IDataCarrier RemoveData(string key, bool includeLockData = false);
		IDataCarrier ClearData(bool includeLockData = false);
		IDataCarrier HasKey(string key, out bool hasKey);
		IDataCarrier PopKey(string key, out bool hasKey);
		IDataCarrier GetData<T>(string key, out T value, T defaultValue = default);
		IDataCarrier PopData<T>(string key, out T value, T defaultValue = default);
		IDataCarrier Lock();
		IDataCarrier Unlock();
		IDataCarrier Lock(string key);
		IDataCarrier Unlock(string key);
		bool Contains(string key);
		bool IsLock(string key);
		bool HasKey(string key)
		{
			HasKey(key, out var hasKey);
			return hasKey;
		}
		bool PopKey(string key)
		{
			PopKey(key, out var hasKey);
			return hasKey;
		}
		T GetData<T>(string key, T defaultValue = default)
		{
			GetData<T>(key, out T value, defaultValue);
			return value;
		}
		T PopData<T>(string key, T defaultValue = default)
		{
			PopData<T>(key, out T value, defaultValue);
			return value;
		}
	}
}