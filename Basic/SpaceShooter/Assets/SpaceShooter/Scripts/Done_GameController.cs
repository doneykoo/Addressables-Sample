using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Done_GameController : MonoBehaviour
{
    // ADDRESSABLES UPDATES
    public AssetReference player;
    public AssetLabelReference hazardsLabel;
    List<IResourceLocation> hazardLocations;

    public Vector3 spawnValues;
    public int hazardCount;
    public float spawnWait;
    public float startWait;
    public float waveWait;

    public Text scoreText;
    public Text restartText;
    public Text gameOverText;
    public Text loadingText;

    public string nextSceneAddress;

    bool gameOver;
    bool restart;
    int score;

    AsyncOperationHandle preloadOp;
    bool baseDateTimeInitialized = false;
    System.DateTime baseDateTime;
    double ElpasedTime
    {
        get {
            if (!baseDateTimeInitialized)
            {
                baseDateTime = System.DateTime.UtcNow;
                baseDateTimeInitialized = true;
            }
            return (System.DateTime.UtcNow - baseDateTime).TotalMilliseconds / 1000.0; 
        }
    }
    bool frameCountInitialized = false;
    int baseFrameCount;
    int ElapsedFrame
    {
        get
        {
            if (!frameCountInitialized)
            {
                baseFrameCount = Time.frameCount;
                frameCountInitialized = true;
            }
            return Time.frameCount - baseFrameCount;
        }
    }

    void Awake()
    {
        Debug.Log($"Awake - {ElpasedTime:f2}s {ElapsedFrame}f");
    }

    void Start()
    {
        // ADDRESSABLES UPDATES
        loadingText.text = string.Format("Loading: {0}%", 0);
        Debug.Log($"Start - {ElpasedTime:f2}s {ElapsedFrame}f");
        preloadOp = Addressables.DownloadDependenciesAsync("preload");
        LoadHazards();
    }

    void LoadHazards()
    {
        Debug.Log($"LoadHazards - {ElpasedTime:f2}s {ElapsedFrame}f");
        Addressables.LoadResourceLocationsAsync(hazardsLabel.labelString).Completed += OnHazardsLoaded;
    }

    // ADDRESSABLES UPDATES
    void OnHazardsLoaded(AsyncOperationHandle<IList<IResourceLocation>> op)
    {
        Debug.Log($"OnHazardsLoaded - {ElpasedTime:f2}s {ElapsedFrame}f");
        if (op.Status == AsyncOperationStatus.Failed)
        {
            Debug.Log("Failed to load hazards, retrying in 1 second...");
            Invoke("LoadHazards", 1);
            return;
        }
        hazardLocations = new List<IResourceLocation>(op.Result);
        Debug.Log($"OnHazardsLoaded - player.InstantiateAsync  {ElpasedTime:f2}s {ElapsedFrame}f");
        player.InstantiateAsync().Completed += op2 =>
        {
            Debug.Log($"OnHazardsLoaded - player.InstantiateAsync Completed  {ElpasedTime:f2}s {ElapsedFrame}f");
            if (op2.Status == AsyncOperationStatus.Failed)
            {
                gameOverText.text = "Failed to load player prefab. Check console for errors.";
                Invoke("LoadHazards", 1);
            }
            else
            {
                gameOver = false;
                restart = false;
                restartText.text = "";
                gameOverText.text = "";
                score = 0;
                UpdateScore();
                StartCoroutine(SpawnWaves());
            }
        };
    }

    void Update()
    {
        if (preloadOp.IsValid())
        {
            loadingText.text = string.Format("Loading: {0}%", (int)(preloadOp.PercentComplete * 100));
            Debug.Log($"Update - loadingText: \"{loadingText.text}\"  {ElpasedTime:f2}s {ElapsedFrame}f");
            if (preloadOp.PercentComplete == 1)
            {
                Debug.Log($"Update - preloadOp.PercentComplete == 1  {ElpasedTime:f2}s {ElapsedFrame}f");
                Addressables.Release(preloadOp);
                preloadOp = new AsyncOperationHandle();
                loadingText.text = "";
                //LoadHazards();
            }
        }
        if (restart)
        {
            if (Input.GetKeyDown(KeyCode.R) || Input.GetButton("Fire1"))
            {
                // ADDRESSABLES UPDATES
                Addressables.LoadSceneAsync(nextSceneAddress);
            }
        }
    }


    IEnumerator SpawnWaves()
    {
        Debug.Log($"SpawnWaves - WaitForSeconds startWait {startWait:f2} ...  {ElpasedTime:f2}s {ElapsedFrame}f");
        yield return new WaitForSeconds(startWait);
        Debug.Log($"SpawnWaves - WaitForSeconds startWait {startWait:f2} finish.  {ElpasedTime:f2}s {ElapsedFrame}f");
        while (true)
        {
            Debug.Log($"SpawnWaves - WaitForSeconds wave start ...  {ElpasedTime:f2}s {ElapsedFrame}f");
            for (int i = 0; i < hazardCount; i++)
            {
                var hazardAddress = hazardLocations[Random.Range(0, hazardLocations.Count)];
                Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
                Quaternion spawnRotation = Quaternion.identity;

                // ADDRESSABLES UPDATES
                Addressables.InstantiateAsync(hazardAddress, spawnPosition, spawnRotation);

                yield return new WaitForSeconds(spawnWait);
            }
            Debug.Log($"SpawnWaves - WaitForSeconds wave finish.  {ElpasedTime:f2}s {ElapsedFrame}f");
            yield return new WaitForSeconds(waveWait);

            if (gameOver)
            {
                restartText.text = "Press 'R' for Restart";
                restart = true;
                break;
            }
        }
    }

    public void AddScore(int newScoreValue)
    {
        score += newScoreValue;
        UpdateScore();
    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + score;
    }

    public void GameOver()
    {
        gameOverText.text = "Game Over!";
        gameOver = true;
    }
}