using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    //private class WallComparer : Comparer<Wall> {

    //    public override int Compare(Wall l, Wall r) {

    //        if (l == null && r == null) {
    //            return 0;
    //        }

    //        if (l == null || r == null) {
    //            return l == null ? 1 : -1;
    //        }

    //        if (l.transform.localPosition.x < r.transform.localPosition.x) {
    //            return -1;
    //        }
    //        if (l.transform.localPosition.x > r.transform.localPosition.x)
    //        {
    //            return 1;
    //        }

    //        if (l.transform.localPosition.y < r.transform.localPosition.y)
    //        {
    //            return -1;
    //        }

    //        if (l.transform.localPosition.y > r.transform.localPosition.y)
    //        {
    //            return 1;
    //        }

    //        return 0;
    //    }

    //}

    [HideInInspector]
    public enum Neighbour {
        UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3
    }

    public Wall[,] grid;
    public int x;
    public int y;

    [HideInInspector]
    public List<Wall> neighbours;
    [HideInInspector]
    public HashSet<Wall> dependants = new HashSet<Wall>(); // List of all nodes that rely on this node existing
    [HideInInspector]
    public Vector3 dependantLocSum = new Vector2(); // Average position of connected mass

    [HideInInspector]
    public HashSet<Wall> criticalSupports = new HashSet<Wall>(); // Set of all supports that are critical to this node existing
    [HideInInspector]
    public bool dead = false;
    [HideInInspector]
    public bool scheduledForDeath = false;
    [HideInInspector]
    public bool root = false;

    public float maxTorque = 100.0f;

    public GameObject failedPrefab;

    public void Update()
    {
        //if (dependants.Count > 0 && criticalSupports.Count == 0)
        //{
        //    // Root
        //    GetComponent<Renderer>().material.color = Color.black;
        //}
        //else if (dependants.Count > 0)
        //{
        //    // Under tension
        //    GetComponent<Renderer>().material.color = Color.red;
        //}
        //else if (criticalSupports.Count > 0) {
        //    // Unstable
        //    GetComponent<Renderer>().material.color = Color.yellow;
        //} else {
        //    // Secure
        //    GetComponent<Renderer>().material.color = Color.white;
        //}

        if (dead)
        {
            DestroyWall(false);
            dead = false;
            return;

            //foreach (Wall neighbour in neighbours)
            //{
            //    if (dependants.Contains(neighbour))
            //    {
            //        neighbour.countdown = deathCountdown;
            //    }

            //    neighbour.neighbours.Remove(this);
            //}

            //foreach (Wall dependant in dependants)
            //{
            //    //Debug.Log("Removing dependant: " + dependant.transform.localPosition.ToString());
            //    dependant.criticalSupports.Remove(this);
            //    scheduledForDeath = true;
            //}

            //Instantiate(failedPrefab, transform.localPosition, Quaternion.identity);
            //gameObject.SetActive(false);
            //Destroy(gameObject);
            //return;
        }
    }

    public void SetupWall(List<Wall> neighbours, bool root, Wall[,] grid, int x, int y) {

        

        this.neighbours = neighbours;
        //foreach (Wall wall in neighbours) {
        //    Debug.Log("Created With: " + wall.transform.localPosition.ToString());
        //}
        this.root = root;
        this.dead = false;
        this.grid = grid;
        this.x = x;
        this.y = y;

        dependants.Clear();
        criticalSupports.Clear();
        dependantLocSum.Set(0, 0, 0);

        CalcCriticalSupports();
    }

    public void UpdateNeighbours(Wall newNeighbour) {
        Debug.Assert(!neighbours.Contains(newNeighbour));
        //Debug.Log("Updated: " + newNeighbour.transform.localPosition.ToString());
        neighbours.Add(newNeighbour);
    }

    public void AddDependant(Wall dependant) {
        

        Debug.Assert(dependant != null);
        dependants.Add(dependant);
        dependantLocSum += dependant.transform.localPosition;

        Debug.Assert(!criticalSupports.Contains(dependant) && !dependant.dependants.Contains(this));

        

        CheckCollapse();

        
    }

    public void RemoveDependant(Wall dependant) {
        Debug.Assert(dependant != null);
        dependants.Remove(dependant);
        dependantLocSum -= dependant.transform.localPosition;

        CheckCollapse();
    }

    private void CheckCollapse()
    {
        float dist = Mathf.Abs((dependantLocSum / dependants.Count).x - transform.localPosition.x);
        float mass = dependants.Count;
        if (dist * mass >= maxTorque)
        {
            dead = true;
        }
    }

    private void InheritNeighbourSupports(Wall wall) {

        

        foreach (Wall support in wall.criticalSupports) {
            criticalSupports.Add(support);
        }

        
    }

    private void CalcCriticalSupports(Wall callee = null, bool print = false) {
        // Roots never have critcal supports
        if (print)
            Debug.Log("New level at: " + transform.localPosition.ToString());

        if (root)
        {
            if (callee == null)
            {
                // Neighbours got new root 
                foreach (Wall neighbour in neighbours)
                {
                    Debug.Log("Root Calling: " + neighbour.transform.localPosition.ToString());
                    neighbour.CalcCriticalSupports(this);
                }
                Debug.Log("Root with neighbours: " + neighbours.Count.ToString());
            }
            return;
        }

        

        int oldSupports = criticalSupports.Count;

        

        foreach (Wall support in criticalSupports) {
            support.RemoveDependant(this);
        }

        

        criticalSupports.Clear();

        

        Debug.Assert(neighbours.Count > 0);

        // Inherit all our first neighbours critical supports
        Wall sole = neighbours[0];
        InheritNeighbourSupports(sole);
        criticalSupports.Add(sole);

        

        // Intersect with all other neighbours to find common critical supports
        for (int i = 1; i < neighbours.Count; i++) {
            if (!neighbours[i].criticalSupports.Contains(this))
            {
                criticalSupports.Add(neighbours[i]);
                criticalSupports.IntersectWith(neighbours[i].criticalSupports);
            }
        }

        

        // Tell all dependant children they are dependant
        foreach (Wall support in criticalSupports) {
            // Debug.Log("Dependant on: " + support.transform.localPosition.ToString());
            support.AddDependant(this);
        }

        if (neighbours.Count > 1 && (oldSupports > criticalSupports.Count || callee == null))
        {
            if (print)
                Debug.Log("Critical supports: " + criticalSupports.Count + " < " + oldSupports.ToString());
            // Recalculate neighbours since we may have joint two roots
            foreach (Wall neighbour in neighbours)
            {
                if (print)
                    Debug.Log("Calling: " + neighbour.transform.localPosition.ToString());
                neighbour.CalcCriticalSupports(this);
            }

            
        }
        else {
            if (print)
                Debug.Log("Critical supports: " + criticalSupports.Count + " >= " + oldSupports.ToString());
        }
    }

    private void RemoveFromNeighbours() {
        foreach (Wall neighbour in neighbours)
        {
            if (neighbour.neighbours.Contains(this))
            {
                neighbour.neighbours.Remove(this);
            }
        }
    }

    public void ReplaceWithFailed()
    {
        

        foreach (Wall criticalSupport in criticalSupports) {
            if (criticalSupport.dependants.Contains(this))
            {
                Debug.Assert(!criticalSupport.dependants.Contains(null));
                criticalSupport.dependants.Remove(this);
                Debug.Assert(!criticalSupport.dependants.Contains(null));
            }
        }
        criticalSupports.Clear();
        grid[x, y] = null;

        

        RemoveFromNeighbours();
        neighbours.Clear();

        

        dependants.Clear();

        Instantiate(failedPrefab, transform.localPosition, Quaternion.identity);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private void DestroyDependants() {
        Debug.Assert(dependants != null);
        // Copy since replace mutates dependants
        Wall[] backups = new Wall[dependants.Count];

        dependants.CopyTo(backups);
        foreach (Wall backup in backups) {
            backup.ReplaceWithFailed();
        }
        
        dependants.Clear();
        dependantLocSum.Set(0, 0, 0);

        
    }

    private List<Wall> FindRoots() {
        List<Wall> roots = new List<Wall>();

        for (int x = 0; x < grid.GetLength(0); x++) {
            if (grid[x, 0] != null) {
                roots.Add(grid[x, 0]);
            }
        }

        return roots;
    }

    private void UpdateFromRoots(List<Wall> roots) {
        // Rebuild grid as we go
        Wall[,] workingGrid = new Wall[grid.GetLength(0), grid.GetLength(1)];

        HashSet<Wall> visited = new HashSet<Wall>();
        Queue<Wall> walls = new Queue<Wall>();
        foreach (Wall root in roots)
        {
            walls.Enqueue(root);
            visited.Add(root);
        }
        while (walls.Count > 0)
        {
            Wall top = walls.Dequeue();
            workingGrid[top.x, top.y] = top;

            List<Wall> newNeighbours = PlayerMovement.GetNeighbours(workingGrid, top.x, top.y);

            foreach(Wall neighbour in newNeighbours) {
                neighbour.UpdateNeighbours(top);
            }

            top.SetupWall(newNeighbours, top.root, top.grid, top.x, top.y);

            // Get all neighbours
            foreach (Wall neighbour in PlayerMovement.GetNeighbours(top.grid, top.x, top.y))
            {
                if (!visited.Contains(neighbour))
                {
                    walls.Enqueue(neighbour);
                    visited.Add(neighbour);
                }
            }
        }
    }

    private void UpdateSupported() {
        List<Wall> roots = FindRoots();
        UpdateFromRoots(roots);
    }

    public void DestroyWall(bool inclusive) {
        DestroyDependants();

        if (inclusive) {
            RemoveFromNeighbours();
            grid[x, y] = null;

            UpdateSupported();

            ReplaceWithFailed();
        }

    }

}
