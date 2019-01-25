using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject wallFailedPrefab;
    public Transform background;

    private Wall[,] grid;
    private int xOff;
    private int yOff;

    private void Start()
    {
        grid = new Wall[(int)background.localScale.x, (int)background.localScale.y];
        xOff = (int)background.localScale.x / 2;
        yOff = (int)background.localScale.y / 2 - (int)background.localPosition.y;

        Debug.Assert(grid.GetLength(0) == background.localScale.x);
        Debug.Assert(grid.GetLength(1) == background.localScale.y);
    }

    void Update()
    {
        MoveForward(); // Player Movement
    }

    void FixedUpdate()
    {
        Vector3 pos = transform.localPosition;
        pos.x = Mathf.Round(pos.x - 0.5f) + 0.5f;
        transform.localPosition = pos;
    }

    void MoveForward()
    {
        if (Input.GetKeyDown("up"))
        {
            transform.Translate(0, 1, 0);
            CheckWall();
        }
        else if (Input.GetKeyDown("down"))
        {
            // TODO Jump down entire level
            transform.Translate(0, -1, 0);
            // No wall going down
        }
        else if (Input.GetKeyDown("left"))
        {
            transform.Translate(-1, 0, 0);
            CheckWall();
        }
        else if (Input.GetKeyDown("right"))
        {
            transform.Translate(1, 0, 0);
            CheckWall();
        }
    }

    private List<Wall> GetNeighbours(int x, int y)
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

                if (grid[x + i, y + j] != null)
                {
                    neighbours.Add( grid[x + i, y + j] );
                }
            }
        }

        return neighbours;
    }

    void CheckWall() {
        if (Input.GetKey("space"))
        {
            int x = Mathf.RoundToInt(transform.localPosition.x - 0.5f);
            int y = Mathf.RoundToInt(transform.localPosition.y - 1.5f);

            int xIdx = x + xOff;
            int yIdx = y + yOff;

            if (xIdx < 0 || yIdx < 0 || xIdx >= grid.GetLength(0) || yIdx >= grid.GetLength(1))
            {
                return;
            }


            Debug.Log("x:" + xIdx.ToString());
            Debug.Log(yIdx);

            // Only add if no existing wall
            if (grid[xIdx, yIdx] == null)
            {
                List<Wall> neighbours = GetNeighbours(xIdx, yIdx);
                if (neighbours.Count > 0 || yIdx == 0)
                {
                    Debug.Log("Num neighbours: " + neighbours.Count.ToString());
                    // Add 0.5f to offsets to centre cube in middle of 1x1
                    GameObject wallObject = Instantiate(wallPrefab, new Vector3(x + 0.5f, y + 0.5f), Quaternion.identity);
                    Wall wall = wallObject.GetComponent<Wall>();

                    foreach (Wall neighbour in neighbours) {
                        neighbour.UpdateNeighbours(wall);
                    }

                    wall.SetupWall(neighbours, yIdx == 0);
                    grid[xIdx, yIdx] = wall;
                }
                else
                {
                    Instantiate(wallFailedPrefab, new Vector3(x + 0.5f, y + 0.5f), Quaternion.identity);
                }
            }
        }
    }
}
