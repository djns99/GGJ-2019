//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class DestructionHandler : MonoBehaviour
//{
//    public int poolSize = 100;
//    public int maxPerSecond = 100;
//    public GameObject failedWallPrefab;
//    Queue<GameObject> failedPrefabPool = new Queue<GameObject>();
//    List<Wall> crumblingRegions = new List<Wall>();

//    private void Start()
//    {
//        for (int i = 0; i < poolSize; i++) {
//            GameObject instance = Instantiate(failedWallPrefab, new Vector3(0, 0, 0), Quaternion.identity);
//            instance.SetActive(false);
//            failedPrefabPool.Enqueue(instance);
//        }
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        for (int i = 0; i < Mathf.CeilToInt(maxPerSecond * Time.deltaTime) && failedPrefabPool.Count > 0; i++) {
//            if (crumblingRegions.Count == 0) {
//                return;
//            }
//            int idx = Random.Range(0, crumblingRegions.Count);
//            Wall wall = crumblingRegions[idx];
//            wall.ReplaceWithFailed(failedPrefabPool.Dequeue());
//            crumblingRegions.RemoveAt(idx);
//        }
//    }

//    public void RequeueDestroyedWall(GameObject destroyed) {
//        failedPrefabPool.Enqueue(destroyed);
//    }

//    public void NewRegion(Wall[] walls) {
//        foreach (Wall wall in walls)
//        {
//            wall.scheduledForDeath = true;
//            crumblingRegions.Add(wall);
//        }
//    }

//    public void NewSingle(Wall wall)
//    {
//        wall.scheduledForDeath = true;
//        crumblingRegions.Add(wall);
//    }
//}
