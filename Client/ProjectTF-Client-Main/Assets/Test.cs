using UnityEngine;

public class Test : MonoBehaviour
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	public async void Start()
	{
		await Awaitable.WaitForSecondsAsync(1f);
		Debug.Log(gameObject.name);
	}

	public async void OnDestroy()
	{
		await Awaitable.WaitForSecondsAsync(1f);
		Debug.Log(gameObject.name);
	}


}
