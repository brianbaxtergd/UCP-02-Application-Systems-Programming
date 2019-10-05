using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(OffScreenWrapper))]
public class Bullet : MonoBehaviour {
    static private Transform _BULLET_ANCHOR;
    static Transform BULLET_ANCHOR {
        get {
            if (_BULLET_ANCHOR == null) {
                GameObject go = new GameObject("BulletAnchor");
                _BULLET_ANCHOR = go.transform;
            }
            return _BULLET_ANCHOR;
        }
    }

    public float    bulletSpeed = 20;
    public float    lifeTime = 2;
    public bool     bDidWrap = false;
    public GameObject bulletParticlePrefab;

    private GameObject partGO;

    void Start()
    {
        transform.SetParent(BULLET_ANCHOR, true);

        // Set Bullet to self-destruct in lifeTime seconds
        Invoke("DestroyMe", lifeTime);

        // Set the velocity of the Bullet
        GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;

        // Instantiate the bullet's particle system prefab object.
        partGO = Instantiate(bulletParticlePrefab);
        partGO.transform.SetParent(transform);
        partGO.transform.position = transform.position;
    }

    private void Update()
    {
        // Update partGO's position to match bullet.
        //partGO.transform.position = transform.position;
    }

    void DestroyMe()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // Destroy partGO.
        Destroy(partGO);
    }
}
