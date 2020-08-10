using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EmptySceneMenu : MonoBehaviour
{
    public UnityEngine.UI.Button Button_BasicReference;
    public UnityEngine.UI.Button Button_ListOfReferences;
    public UnityEngine.UI.Button Button_FilteredReferences;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("EmptySceneMenu.Start");
        Button_BasicReference.onClick.AddListener(() => {
            LoadScene("BasicReference");
        });
        Button_ListOfReferences.onClick.AddListener(() => {
            LoadScene("ListOfReferences");
        });
        Button_FilteredReferences.onClick.AddListener(() => {
            LoadScene("FilteredReferences");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }
    
    private void OnDestroy() {
        Debug.Log("EmptySceneMenu.OnDestroy");
        Button_BasicReference.onClick.RemoveAllListeners();
        Button_ListOfReferences.onClick.RemoveAllListeners();
        Button_FilteredReferences.onClick.RemoveAllListeners();
    }
}
