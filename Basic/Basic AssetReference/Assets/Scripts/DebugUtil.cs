
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.SceneManagement;

public class DebugUtil
{
    public static void UnloadScene()
    {
        SceneManager.LoadScene("EmptyScene0");
    }
}