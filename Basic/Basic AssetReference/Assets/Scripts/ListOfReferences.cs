using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ListOfReferences : MonoBehaviour {

	public List<AssetReference> shapes;
	public List<GameObject> savedAssets;

	bool m_IsReady = false;
	int m_ToLoadCount;

	int currentIndex = 0;
	// Use this for initialization
	void Start ()
	{
		m_ToLoadCount = shapes.Count;
		foreach (var shape in shapes)
		{
			shape.LoadAssetAsync<GameObject>().Completed += OnShapeLoaded;
		}
	}

	void OnShapeLoaded(AsyncOperationHandle<GameObject> obj)
	{
		m_ToLoadCount--;
		if (m_ToLoadCount <= 0)
			m_IsReady = true;
		// debug code of save loaded asset to variable, and release loaded asset from addressable.
		if (m_IsReady && savedAssets.Count == 0) {
            savedAssets.Clear();
			for (int i = 0; i < shapes.Count; i++)
			{
                var shape = shapes[i];
                var go = shape.Asset as GameObject;
                Debug.Log($"save asset: #{i} {go}");
                savedAssets.Add(go);
                Debug.Log($"release addressable shape.Asset: #{i} {shape}");
                shape.ReleaseAsset();
            }
        }
	}

	public void SpawnAThing()
	{
		if (m_IsReady && shapes[currentIndex].Asset != null)
		{
			Debug.Log($"using AssetReference shapes to spawn: #{currentIndex}");
			for(int count = 0; count <= currentIndex; count++)
				GameObject.Instantiate(shapes[currentIndex].Asset);
			currentIndex++;
			if (currentIndex >= shapes.Count)
				currentIndex = 0;

		}
		// debug code of use saved loaded assets, whose original AssetReference was released.
		if (m_IsReady && savedAssets != null && savedAssets.Count > 0 && savedAssets[currentIndex] != null)
		{
			Debug.Log($"using savedAssets to spawn: #{currentIndex}");
			for(int count = 0; count <= currentIndex; count++) {
				GameObject.Instantiate(savedAssets[currentIndex]);
			}
			currentIndex++;
			if (currentIndex >= savedAssets.Count)
				currentIndex = 0;
		}
	}

	void OnDestroy()
	{
		// foreach (var shape in shapes)
		// {
		// 	shape.ReleaseAsset();
		// }
	}
	public void UnloadScene()
	{
		DebugUtil.UnloadScene();
	}
}
