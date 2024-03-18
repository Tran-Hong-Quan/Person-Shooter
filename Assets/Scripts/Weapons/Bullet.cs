using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] protected ParticleSystem hitEff;

    protected float speed = 0;
    protected Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public virtual void Init(float speed,Vector3 pos,Vector3 dir)
    {
        StartCoroutine(AutoDestroy());
        transform.position = pos;
        transform.rotation = Quaternion.LookRotation(dir, transform.up);
        rb.velocity = dir.normalized * speed;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        Instantiate(hitEff);
        hitEff.transform.position = transform.position;
        SimplePool.Despawn(gameObject);
        StopAllCoroutines();
    }

    IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(10);
        SimplePool.Despawn(gameObject);
    }
}
