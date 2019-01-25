using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private class WallComparer : Comparer<Wall> {

        public override int Compare(Wall l, Wall r) {
            if (l.transform.localPosition.x < r.transform.localPosition.x) {
                return -1;
            }
            if (l.transform.localPosition.x > r.transform.localPosition.x)
            {
                return 1;
            }

            if (l.transform.localPosition.y < r.transform.localPosition.y)
            {
                return -1;
            }

            if (l.transform.localPosition.y > r.transform.localPosition.y)
            {
                return 1;
            }

            return 0;
        }

    }

    //[HideInInspector]
    public enum Neighbour {
        UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3
    }

    //[HideInInspector]
    public List<Wall> neighbours;
    //[HideInInspector]
    public SortedSet<Wall> dependants = new SortedSet<Wall>(new WallComparer()); // List of all nodes that rely on this node existing
    //[HideInInspector]
    public Vector3 dependantLocSum = new Vector2(); // Average position of connected mass

    //[HideInInspector]
    public SortedSet<Wall> criticalSupports = new SortedSet<Wall>(new WallComparer()); // Set of all supports that are critical to this node existing
    //[HideInInspector]
    public bool dead = false;
    public float countdown = float.NaN;
    //[HideInInspector]
    public bool scheduledForDeath = false;
    //[HideInInspector]
    public bool root = false;

    public float maxTorque = 100.0f;
    public float deathCountdown = 1.0f;

    public GameObject failedPrefab;

    public void Update()
    {
        if (dependants.Count > 0 && criticalSupports.Count == 0)
        {
            // Root
            GetComponent<Renderer>().material.color = Color.black;
        }
        else if (dependants.Count > 0)
        {
            // Under tension
            GetComponent<Renderer>().material.color = Color.red;
        }
        else if(criticalSupports.Count > 0) {
            // Unstable
            GetComponent<Renderer>().material.color = Color.yellow;
        } else {
            // Secure
            GetComponent<Renderer>().material.color = Color.white;
        }

        if (dead)
        {
            foreach (Wall neighbour in neighbours)
            {
                if (dependants.Contains(neighbour))
                {
                    neighbour.countdown = deathCountdown;
                }

                neighbour.neighbours.Remove(this);
            }

            foreach (Wall dependant in dependants)
            {
                //Debug.Log("Removing dependant: " + dependant.transform.localPosition.ToString());
                dependant.criticalSupports.Remove(this);
                scheduledForDeath = true;
            }

            Instantiate(failedPrefab, transform.localPosition, Quaternion.identity);
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }
        else if (countdown != float.NaN) {
            countdown -= Time.deltaTime;
            if (countdown <= 0)
                CheckCollapse();
            countdown = float.NaN;
        }
    }

    public void SetupWall(List<Wall> neighbours, bool root) {
        this.neighbours = neighbours;
        foreach (Wall wall in neighbours) {
            Debug.Log("Created With: " + wall.transform.localPosition.ToString());
        }
        this.root = root;
        this.dead = false;
        CalcCriticalSupports();
    }

    public void UpdateNeighbours(Wall newNeighbour) {
        Debug.Assert(!neighbours.Contains(newNeighbour));
        Debug.Log("Updated: " + newNeighbour.transform.localPosition.ToString());
        neighbours.Add(newNeighbour);
    }

    public void AddDependant(Wall dependant) {
        dependants.Add(dependant);
        dependantLocSum += dependant.transform.localPosition;

        Debug.Assert(!criticalSupports.Contains(dependant));

        CheckCollapse();
    }

    public void RemoveDependant(Wall dependant) {
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

    private void CalcCriticalSupports(Wall callee = null) {
        // Roots never have critcal supports
        if (root)
            return;

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

        if (neighbours.Count > 1 && ( oldSupports > criticalSupports.Count || callee == null) ) {
            // Recalculate neighbours since we may have joint two roots
            foreach (Wall neighbour in neighbours) {
                if (neighbour != callee)
                {
                    // Don't call same neighbour again (in the trivial case)
                    neighbour.CalcCriticalSupports(this);
                }
            }
        }
    }

}
