using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    public GunFire gun;

	public virtual void Awake ()
	{
		gameObject.SetActive (false);
	}

    public GameObject[] AnimationOnImpactPrefabs;

    public bool PlayAnimationOnWallHit = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            other.gameObject.GetComponent<Wall>().DestroyWall(true);
            InitDamageAnimation(other);
            gun.ReturnBullet(gameObject);
        }
        if (other.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<Wall>().DestroyWall(true);
            InitDamageAnimation(other);
            gun.ReturnBullet(gameObject);
        }
        else if (other.CompareTag("Ground")) {
            InitDamageAnimation(other);
            gun.ReturnBullet(gameObject);
        }
    }

    protected void InitDamageAnimation(Collider2D other)
    {
        if (ImpactAnimationPresent())
        {
            var dir = transform.up.normalized;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            Instantiate(AnimationOnImpactPrefabs[Random.Range(0, AnimationOnImpactPrefabs.Length)],
                            transform.position, Quaternion.AngleAxis(angle, Vector3.forward));
        }
    }

    private bool ImpactAnimationPresent()
    {
        return AnimationOnImpactPrefabs != null && AnimationOnImpactPrefabs.Length > 0;
    }
}
