using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject wallFailedPrefab;
    public Transform background;

    public float speed = 20.0f;

    public bool player1 = true;

    public PlayerMovement other;

    private Wall[,] grid;
    private int xOff;
    private int yOff;

    
    private const int NUM_IN_POOL = 30;
    private GameObject[] failedPool = new GameObject[NUM_IN_POOL];
    private int nextIndex = 0;
    private SpriteRenderer spriteRenderer;


    public bool gameStopped = false;

    private string upKey, downKey, leftKey, rightKey;

    private float left, right, up, down;

    private void Awake()
    {
        float aspect = Camera.main.aspect;
        float cameraHeight = Camera.main.orthographicSize * 2;
        float cameraWidth = cameraHeight * aspect;

        left = -cameraWidth / 2;
        right = cameraWidth / 2;
        up = cameraHeight / 2 + Camera.main.transform.position.y;
        down = background.localPosition.y - background.localScale.y / 2;

        grid = new Wall[(int)cameraWidth, (int)up - (int)down];
        xOff = (int)cameraWidth / 2;
        yOff = -Mathf.FloorToInt(down);
    }

    private void Start()
    {
        for (int i = 0; i < NUM_IN_POOL; i++) {
            GameObject failed = Instantiate(wallFailedPrefab);
            failed.SetActive(false);
            failed.GetComponent<DespawnDestroyedWall>().persist = true;
            failedPool[i] = failed;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (player1)
        {
            upKey = "w";
            downKey = "s";
            leftKey = "a";
            rightKey = "d";
        }
        else {
            upKey = "up";
            downKey = "down";
            leftKey = "left";
            rightKey = "right";
        }

        for (int x = 0; x < grid.GetLength(0); x++) {
            CheckWall(x - xOff, 0 - yOff);
        }
    }

    void Update()
    {
        if (!gameStopped)
        {
            MoveForward(); // Player Movement
        }
    }

    void FixedUpdate()
    {
        Vector3 pos = transform.localPosition;
        pos.x = Mathf.Round(pos.x - 0.5f) + 0.5f;
        transform.localPosition = pos;
    }

    public int GetPlayerScoreTotalTiles() {
        int count = 0;
        for (int x = 0; x < grid.GetLength(0); x++) {
            for (int y = 1; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] != null)
                    count++;
            }
        }
        return count;
    }

    private void TraverseFalse(Vector2Int vec, HashSet<Vector2Int> invalidSquares) {

        Wall[] neighbours = new Wall[4];
        Stack<Vector2Int> toProcess = new Stack<Vector2Int>();
        toProcess.Push(vec);
        invalidSquares.Add(vec);
        while (toProcess.Count > 0) {
            Vector2Int top = toProcess.Pop();

            int x = top.x;
            int y = top.y;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (Math.Abs(i) == Math.Abs(j) || x + i < 0 || y + j < 0 || x + i >= grid.GetLength(0) || y + j >= grid.GetLength(1))
                    {
                        continue;
                    }

                    Vector2Int newVec = new Vector2Int(x + i, y + j);
                    if (grid[x + i, y + j] == null && !invalidSquares.Contains(newVec)) {
                        toProcess.Push(newVec);
                        invalidSquares.Add(newVec);
                    }

                }
            }
        }
    }

    public int GetPlayerScoreTotalArea()
    {
        HashSet<Vector2Int> invalidSquares = new HashSet<Vector2Int>();
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (x == 1 && y != 0 && y != grid.GetLength(1) - 1) {
                    x = grid.GetLength(0) - 1;
                }

                if (y == 1 && x != 0 && x != grid.GetLength(0) - 1) {
                    y = grid.GetLength(1) - 1;
                }

                // Not covered and not already traversed
                Vector2Int vec = new Vector2Int(x, y);
                if (grid[x, y] == null && !invalidSquares.Contains(vec)) 
                    TraverseFalse(vec, invalidSquares);
            }
        }
        return TotalGridsquares() - invalidSquares.Count;
    }

    public int GetPlayerScoreSurrounded()
    {
        // Surrounded area
        return GetPlayerScoreTotalArea() - GetPlayerScoreTotalTiles();
    }

    public int TotalGridsquares() {
        return grid.GetLength(0) * grid.GetLength(1);
    }

    void MoveForward()
    {
        float step = 1;// Mathf.Clamp01(speed * Time.deltaTime);

        int xLast = Mathf.RoundToInt(transform.localPosition.x - 0.5f);
        int yLast = Mathf.RoundToInt(transform.localPosition.y - 0.5f);

        if (Input.GetKey(upKey) && transform.localPosition.y + step < up)
        {
            transform.Translate(0, step, 0);
        }
        else if (Input.GetKey(downKey) && transform.localPosition.y - step > down)
        {
            transform.Translate(0, -step, 0);
        }

        int xNew = Mathf.RoundToInt(transform.localPosition.x - 0.5f);
        int yNew = Mathf.RoundToInt(transform.localPosition.y - 0.5f);

        if (xNew != xLast || yNew != yLast)
        {
            CheckWall(xLast, yLast);
        }

        if (Input.GetKey(leftKey) && transform.localPosition.x - step > left)
        {
            transform.Translate(-step, 0, 0);
            spriteRenderer.flipX = true;
        }
        else if (Input.GetKey(rightKey) && transform.localPosition.x + step < right)
        {
            transform.Translate(step, 0, 0);
            spriteRenderer.flipX = true;
        }

        xNew = Mathf.RoundToInt(transform.localPosition.x - 0.5f);
        yNew = Mathf.RoundToInt(transform.localPosition.y - 0.5f);

        if (xNew != xLast || yNew != yLast) {
            CheckWall(xLast, yLast);
        }

        //if (Input.GetMouseButtonDown(0))
        //{
        //    // Testing to stop 
        //    Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    int x = Mathf.RoundToInt(pos.x - 0.5f) + xOff;
        //    int y = Mathf.RoundToInt(pos.y - 0.5f) + yOff;
        //    if (!(x < 0 || y < 0 || x >= grid.GetLength(0) || y >= grid.GetLength(1)))
        //    {
        //        if (grid[x, y] != null)
        //        {
        //            grid[x, y].DestroyWall(true);
        //        }
        //    }
        //}

    }

    public static List<Wall> GetNeighbours(Wall[,] grid, int x, int y)
    {
        List<Wall> neighbours = new List<Wall>();

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {

                if (Math.Abs(i) == Math.Abs(j) || x + i < 0 || y + j < 0 || x + i >= grid.GetLength(0) || y + j >= grid.GetLength(1))
                {
                    continue;
                }

                if (grid[x + i, y + j] != null && !grid[x + i, y + j].scheduledForDeath)
                {
                    neighbours.Add( grid[x + i, y + j] );
                }
            }
        }

        return neighbours;
    }

    GameObject getFailed() {
        nextIndex = (nextIndex + 1) % NUM_IN_POOL;
        failedPool[nextIndex].SetActive(false); // Disable if not already
        return failedPool[nextIndex];
    }

    void CheckWall(int x, int y) {
        int xIdx = x + xOff;
        int yIdx = y + yOff;

        if (xIdx < 0 || yIdx < 0 || xIdx >= grid.GetLength(0) || yIdx >= grid.GetLength(1))
        {
            return;
        }

        int half = player1 ? 1 : 0; // Half they are not allowed in
        int quarterSize = Mathf.CeilToInt(grid.GetLength(0) / 4.0f);
        if (yIdx == 0 && (xIdx >= quarterSize * half) && xIdx < (quarterSize * half + quarterSize * 3)) {
            return;
        }

        //Debug.Log("x:" + xIdx.ToString());
        //Debug.Log(yIdx);

        // Only add if no existing wall
        if (grid[xIdx, yIdx] == null)
        {
            List<Wall> neighbours = GetNeighbours(grid, xIdx, yIdx);
            if (neighbours.Count > 0 || yIdx == 0)
            {
                //Debug.Log("Num neighbours: " + neighbours.Count.ToString());
                // Add 0.5f to offsets to centre cube in middle of 1x1
                GameObject wallObject = Instantiate(wallPrefab, new Vector3(x + 0.5f, y + 0.5f, -(((float)yIdx / 10.0f) + ((float)xIdx / 1000.0f))), Quaternion.identity);
                Wall wall = wallObject.GetComponent<Wall>();

                foreach (Wall neighbour in neighbours) {
                    neighbour.UpdateNeighbours(wall);
                }

                //Debug.Log("Neighbours: " + neighbours.Count.ToString());
                wall.SetupWall(neighbours, yIdx == 0, grid, xIdx, yIdx);
                grid[xIdx, yIdx] = wall;

                if (other.grid[xIdx, yIdx] != null) {
                    other.grid[xIdx, yIdx].DestroyWall(true);
                }
            }
            else
            {
                // Fail to place here
                GameObject prefab = getFailed();
                // Reset position
                prefab.transform.position = new Vector3(x + 0.5f, y + 0.5f);
                prefab.SetActive(true);
            }
        }
    }
}
