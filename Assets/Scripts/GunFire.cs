using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFire : MonoBehaviour
{
    public float secondsPerRound = 0.75f;
    public GameObject bulletPrefab;
    public float bulletVelocity = 40.0f;
    public bool playerBullet = true;
    private float charge = 0.0f;

    private Queue<GameObject> pooledBullets = new Queue<GameObject>();

    private void Start()
    {
        for (int i = 0; i < 20; i++) {
            ReturnBullet(Instantiate(bulletPrefab));
        }
    }

    // Update is called once per frame
    void Update()
    {
        charge += Time.deltaTime;
        if (charge > secondsPerRound) {
            charge = 0.0f;
            if (pooledBullets.Count > 0) {
                float angle = Random.Range(-2.5f, 2.5f);
                float power = Random.Range(bulletVelocity - 5.0f, bulletVelocity + 5.0f);

                GameObject bullet = GetBullet();
                bullet.transform.position = transform.position;
                bullet.transform.rotation = transform.rotation;
                bullet.transform.Rotate(new Vector3(0, 0, 90 * (playerBullet ? -1 : 1)));
                bullet.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                bullet.GetComponent<Projectile>().gun = this;
                bullet.layer = playerBullet ? 8 : 9;
                bullet.SetActive(true);
                bullet.GetComponent<Rigidbody2D>().velocity = transform.right * bulletVelocity * (playerBullet ? 1 : -1);
            }
        }
    }

    public void ReturnBullet(GameObject bullet) {
        Debug.Log("Bullet Returned");
        bullet.SetActive(false);
        pooledBullets.Enqueue(bullet);
    }

    GameObject GetBullet() {
        Debug.Log("Bullet Spawned");
        return pooledBullets.Dequeue();
    }
}
