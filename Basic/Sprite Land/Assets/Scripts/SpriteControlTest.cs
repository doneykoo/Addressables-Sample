using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AddressableAssets.GUI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;

public class SpriteControlTest : MonoBehaviour
{
    public AssetReferenceSprite singleSpriteReference;
    
    public AssetReference spriteSheetReference;
    
    
    public AssetReferenceAtlas spriteAtlasReference;
    public AssetReference spriteSubAssetReference;
    public AssetReference atlasSubAssetReference;
    
    // @note: AssetReferenceT<T>泛型 不能直接被 Serialize!
    //     若必要使用，可以做一个自定义类型： [Serializable] class 继承 AssetReferenceT<T>。
    public AssetReferenceT<Sprite> spriteRefT;
    
    // @note: CANNOT COMPILE AssetReferenceT<IList<Sprite>>:
    //     The type 'System.Collections.Generic.IList<UnityEngine.Sprite>' must be convertible to 'UnityEngine.Object'
    // public AssetReferenceT<IList<Sprite>> spriteSheetRefT;
    
    // @note: AssetReferenceT<T>泛型 不能直接被 Serialize!
    public AssetReferenceT<SpriteAtlas> spriteAtlasRefT;
    
    public List<SpriteRenderer> spritesToChange;
    public List<SpriteRenderer> spritesToChange2;

    public Button button;
    public Text buttonText;

    int m_ClickCount = 0;
    private const int ClickMax = 14;

    private void Awake() {
        // start from
        m_ClickCount = 7;
    }

    public void OnButtonClick()
    {
        button.interactable = false;
        m_ClickCount++;
        if (m_ClickCount > ClickMax) {
            m_ClickCount = 1;
        }
        Debug.Log($"Test for click: #{m_ClickCount}");
        switch (m_ClickCount)
        {
            case 1:
                Debug.Log("singleSpriteReference.LoadAssetAsync<Sprite>()");
                singleSpriteReference.LoadAssetAsync<Sprite>().Completed += SingleDone;
                break;
            case 2:
                Debug.Log("spriteSheetReference.LoadAssetAsync<IList<Sprite>>()");
                spriteSheetReference.LoadAssetAsync<IList<Sprite>>().Completed += SheetDone;
                break;
            case 3:
                Debug.Log("spriteAtlasReference.LoadAssetAsync()");
                spriteAtlasReference.LoadAssetAsync().Completed += AtlasDone;
                break;
            case 4:
                Debug.Log("spriteSubAssetReference.LoadAssetAsync<Sprite>()");
                spriteSubAssetReference.LoadAssetAsync<Sprite>().Completed += SheetSubDone;
                break;
            case 5:
                Debug.Log("atlasSubAssetReference.LoadAssetAsync<Sprite>()");
                atlasSubAssetReference.LoadAssetAsync<Sprite>().Completed += AtlasSubDone;
                break;
            case 6:
                Debug.Log("Addressables.LoadAssetAsync<Sprite>(\"sheet[sprite_sheet_4]\")");
                Addressables.LoadAssetAsync<Sprite>("sheet[sprite_sheet_4]").Completed += SheetNameSubDone;
                break;
            case 7:
                Debug.Log("Addressables.LoadAssetAsync<Sprite>(\"Atlas[u7]\")");
                Addressables.LoadAssetAsync<Sprite>("Atlas[u7]").Completed += AtlasNameSubDone;
                break;
            default:
                Addressables.LoadAssetAsync<IList<Sprite>>("sheet").Completed += SheetAddressDone;
                Addressables.LoadAssetsAsync<Sprite>("sheet", null).Completed += SheetAssetsAddressDone;
                break;
        }
    }
    
    

    void SingleDone(AsyncOperationHandle<Sprite> op)
    {
        if (op.Result == null)
        {
            Debug.LogError("no sprites here.");
            return;
        }
        
        spritesToChange[0].sprite = op.Result;
        
        button.interactable = true;
        buttonText.text = "Change with sheet list";
    }

    void SheetDone(AsyncOperationHandle<IList<Sprite>> op)
    {
        if (op.Result == null)
        {
            Debug.LogError("no sheets here.");
            return;
        }

        spritesToChange[1].sprite = op.Result[5];
        
        button.interactable = true;
        buttonText.text = "Change with atlas";
    }

    void AtlasDone(AsyncOperationHandle<SpriteAtlas> op)
    {
        if (op.Result == null)
        {
            Debug.LogError("no atlases here.");
            return;
        }

        spritesToChange[2].sprite = op.Result.GetSprite("u3");
        
        button.interactable = true;
        buttonText.text = "Change with sprite sub-object ref";
    }

    void SheetSubDone(AsyncOperationHandle<Sprite> op)
    {
        if (op.Result == null)
        {
            Debug.LogError("no sprite in sheet here.");
            return;
        }

        spritesToChange[3].sprite = op.Result;
        
        button.interactable = true;
        buttonText.text = "Change with atlas sub-object ref";
    }

    void AtlasSubDone(AsyncOperationHandle<Sprite> op)
    {
        if (op.Result == null)
        {
            Debug.LogError("no sprite in atlas here.");
            return;
        }

        spritesToChange[4].sprite = op.Result;
        
        button.interactable = true;
        buttonText.text = "Change with sprite[name]";
    }

    void SheetNameSubDone(AsyncOperationHandle<Sprite> op)
    {
        if (op.Result == null)
        {
            Debug.LogError("no sprite in sheet here.");
            return;
        }

        spritesToChange[5].sprite = op.Result;
        
        button.interactable = true;
        buttonText.text = "Change with atlas[name]";
    }

    void AtlasNameSubDone(AsyncOperationHandle<Sprite> op)
    {
        if (op.Result == null)
        {
            Debug.LogError("no sprite in atlas here.");
            return;
        }

        button.interactable = true;
        spritesToChange[6].sprite = op.Result;
        
        buttonText.text = "Change with sheet address";
    }

    void SheetAddressDone(AsyncOperationHandle<IList<Sprite>> op)
    {
        if (op.Result == null)
        {
            Debug.LogError("no sheets here.");
            return;
        }

        Debug.Log($"Addressables.LoadAssetAsync<IList<Sprite>>(\"sheet\"), return list: {op.Result.GetType().Name} count: {op.Result.Count}");

        button.interactable = true;
        var index = m_ClickCount - 7;
        switch (index) {
            case 1:
                spritesToChange2[0].sprite = op.Result[3];
                break;
            case 2:
                spritesToChange2[1].sprite = op.Result[5];
                break;
            case 3:
                spritesToChange2[2].sprite = op.Result[2];
                break;
            case 4:
                spritesToChange2[3].sprite = op.Result[1];
                break;
            case 5:
                spritesToChange2[4].sprite = op.Result[3];
                break;
            case 6:
                spritesToChange2[5].sprite = op.Result[4];
                break;
            case 7:
                spritesToChange2[6].sprite = op.Result[0];
                break;
            default:
                buttonText.text = "The End";
                break;
        }
    }

    void SheetAssetsAddressDone(AsyncOperationHandle<IList<Sprite>> op) {
        if (op.Result == null)
        {
            Debug.LogError("no sheets here.");
            return;
        }

        Debug.Log($"Addressables.LoadAssetsAsync<Sprite>(\"sheet\", null), return list: {op.Result.GetType().Name} count: {op.Result.Count}");
    }

    void Start()
    {
        Addressables.InitializeAsync();
    }

}

[Serializable]
public class AssetReferenceAtlas : AssetReferenceT<SpriteAtlas>
{
    public AssetReferenceAtlas(string guid) : base(guid) { }
}
