using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;

public class GameController : MonoBehaviour
{
    [Header("Hook Settings")]
    public float moveSpeed = 1.3f;
    public float moveRange = 0.8f;
    
    [Header("Level Configuration")]
    public BuildingType currentBuildingData;
    public int blocksLeft;
    public GameObject blockPrefab;

    private float blockHeight = 1.5f; 
    
    private Rigidbody2D rb;
    private LineRenderer line;

    private Block currentBlock = null;
    private Block newBlock = null;
    public List<Block> activeBlocks = new List<Block>();
    public float highestY = -0.27f;
    
    public bool isGameOver = false;
    public int lives = 3;
    public int score = 0;
    public int blockCount = 0;

    [Header("UI Settings")]
    public TextMeshProUGUI scoreText;
    public GameObject heartPrefab;
    public Transform heartsParent;
    private List<GameObject> heartIcons = new List<GameObject>();

    void Start(){
        if (currentBuildingData != null) blocksLeft = currentBuildingData.totalBlocks;
        
        InitializeHearts();

        rb = GetComponent<Rigidbody2D>();
        line = GetComponent<LineRenderer>();
        SpawnNewBlock();
    }

    void Update(){
        // if (isGameOver) return;

        UpdateHighestPoint();
        UpdateScore();
        MoveHook();

        CheckForBlockDrop();
    }

    public void onSuccessfulLanding(){
        SpawnNewBlock();
    }

    public void onMissedLanding(){
        currentBlock.isCurrent = false;
        currentBlock = null;
        lives -= 1;

        UpdateHeartUI();

        if (lives <= 0){
            GameOver();
        }
        SpawnNewBlock();
    }

    void SpawnNewBlock(){
        if (isGameOver) return;
        if (newBlock != null) return;

        Vector3 spawnPos = transform.position + Vector3.down * 2f;
        GameObject go = Instantiate(blockPrefab, spawnPos, Quaternion.identity);
        newBlock = go.GetComponent<Block>();
        
        newBlock.SetupVisuals(currentBuildingData, blocksLeft == 1);

        newBlock.SetupBlock(rb, ++blockCount);
    }

    void GameOver()
    {
        isGameOver = true;
        Debug.Log("Koniec gry! Wynik: " + score);
        // Tutaj możesz aktywować Panel Game Over w UI
    }
    
    void Drop(){
        if (currentBlock) currentBlock.isCurrent = false;

        newBlock.Release();
        currentBlock = newBlock;
        currentBlock.isCurrent = true;
        newBlock = null;

        blocksLeft--;
    }

    void UpdateHighestPoint(){
        float currentMaxY = -0.27f;
        for (int i = activeBlocks.Count - 1; i >= 0; i--)
        {
            if (activeBlocks[i] == null) {
                activeBlocks.RemoveAt(i);
                continue;
            }

            if (activeBlocks[i].transform.position.y < -2f)
            {
                Destroy(activeBlocks[i].gameObject);
                activeBlocks.RemoveAt(i);
                continue;
            }

            if (activeBlocks[i].transform.position.y > currentMaxY)
            {
                currentMaxY = activeBlocks[i].transform.position.y;
            }
        }
        highestY = currentMaxY;
    }
    
    private void UpdateScore(){
        score = (int)Math.Round(highestY / blockHeight);

        if (scoreText != null){
            scoreText.text = score.ToString();
        }
    }

    private void MoveHook(){
        float x = Mathf.Sin(Time.time * moveSpeed) * moveRange;
        float targetHeight = Camera.main.transform.position.y + 7f;
        transform.position = new Vector3(x, targetHeight, 0);

        if (newBlock != null){
            line.enabled = true;
            line.SetPosition(0, transform.position);
            line.SetPosition(1, newBlock.transform.position);
        } else {
            line.enabled = false;
        }

    }
    
    private void CheckForBlockDrop(){
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && newBlock != null){
            Drop();
        }
    }

    public void StartStabilityCheck(Block block) {
        if (!block.isCurrent) return; 
        
        StartCoroutine(StabilityRoutine(block));
    }

    private System.Collections.IEnumerator StabilityRoutine(Block block){
        yield return new WaitForSeconds(1f);

        if (block == null) yield break;

        if (TestStability(block)) {
            activeBlocks.Add(block);
            onSuccessfulLanding();
        } else {
            onMissedLanding();
        }
    }

    private bool TestStability(Block block) {
        Rigidbody2D rb2d = block.GetComponent<Rigidbody2D>();
        
        if (rb2d.linearVelocity.y < -0.1f) return false;

        if (block.transform.position.y < (highestY - 2.0f)) return false;

        return true;
    }

    void InitializeHearts() {
        foreach (GameObject h in heartIcons) Destroy(h);
        heartIcons.Clear();

        for (int i = 0; i < lives; i++) {
            GameObject heart = Instantiate(heartPrefab, heartsParent);
            heartIcons.Add(heart);
        }
    }

    public void UpdateHeartUI() {
        for (int i = 0; i < heartIcons.Count; i++) {
            heartIcons[i].SetActive(i < lives);
        }
    }
}