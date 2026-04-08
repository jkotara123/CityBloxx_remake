using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;

public class GameController : MonoBehaviour
{
    public GameObject blockPrefab;
    public float moveSpeed = 2f;
    public float moveRange = 3f;

    public float blockHeight = 1f; 
    
    private Rigidbody2D rb;
    private LineRenderer line;

    private Block currentBlock = null;
    private Block newBlock = null;
    public List<Block> activeBlocks = new List<Block>();
    public float highestY = 0f;
    
    public bool isGameOver = false;
    public int lives = 3;
    public int score = 0;
    public int blockCount = 0;

    [Header("UI Settings")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        line = GetComponent<LineRenderer>();
        SpawnNewBlock();
    }

    void Update(){
        if (isGameOver) return;

        UpdateHighestPoint();
        UpdateStats();
        MoveHook();

        CheckForBlockDrop();
    }

    public void onSuccessfulLanding(){
        currentBlock.isCurrent = false;
        currentBlock = null;

        SpawnNewBlock();
    }

    public void onMissedLanding(){
        currentBlock.isCurrent = false;
        currentBlock = null;
        lives -= 1;

        if (lives <= 0){
            GameOver();
        }
        SpawnNewBlock();
    }

    void SpawnNewBlock(){
        if (isGameOver) return;

        Vector3 spawnPos = transform.position + Vector3.down * 2f;
        GameObject go = Instantiate(blockPrefab, spawnPos, Quaternion.identity);
        newBlock = go.GetComponent<Block>();
        newBlock.SetupBlock(rb, ++blockCount);
    }

    void GameOver()
    {
        isGameOver = true;
        Debug.Log("Koniec gry! Wynik: " + score);
        // Tutaj możesz aktywować Panel Game Over w UI
    }
    
    void Drop(){
        newBlock.Release();

        currentBlock = newBlock;
        currentBlock.isCurrent = true;
        newBlock = null;
    }

    void UpdateHighestPoint(){
        float currentMaxY = 0.3f;
        for (int i = activeBlocks.Count - 1; i >= 0; i--)
        {
            if (activeBlocks[i] == null) {
                activeBlocks.RemoveAt(i);
                continue;
            }

            if (activeBlocks[i].transform.position.y < -1f) 
            {
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
    
    private void UpdateStats(){
        score = (int)Math.Round(highestY / blockHeight);

        if (scoreText != null){
            scoreText.text = score.ToString();
        }
        if (livesText != null){
            livesText.text = "Lives: " + lives.ToString();
        }
    }

    private void MoveHook(){
        float x = Mathf.Sin(Time.time * moveSpeed) * moveRange;
        float targetHeight = highestY + 8f; 
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
        yield return new WaitForSeconds(0.5f);

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
}