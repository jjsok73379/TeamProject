using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Arrow : MonoBehaviour
{
    public LayerMask crashMask;
    public float MoveSpeed = 10.0f;
    float totalDist = 0.0f;
    public Transform myTarget;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("ArrowDestroy", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        float delta = MoveSpeed * 100.0f * Time.deltaTime;
        Ray ray = new Ray(transform.position, transform.up);
        if (Physics.Raycast(ray, out RaycastHit hit, delta, crashMask))
        {
            hit.transform.GetComponent<Host>()?.AttackTarget();
        }

        transform.Translate(Vector3.up * delta);

        totalDist += delta;
        if (totalDist > 1000.0f)
        {
            totalDist = 0.0f;
            ObjectPool.Instance.ReleaseObject<Arrow>(gameObject);
        }
    }

    public void OnFire()
    {
        transform.SetParent(null);
    }

    public void ArrowDestroy()
    {
        Destroy(gameObject);
    }
}