using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanArrow : MonoBehaviour
{
    private Transform owner;
    private Transform target;
    private float duration;

    public void Initialize(Transform owner, Transform target, float duration)
    {
        this.owner = owner;
        this.target = target;
        this.duration = duration;
        StartCoroutine(EffectEnd());
    }

    public IEnumerator EffectEnd()
    {
        yield return new WaitForSeconds(this.duration);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (owner != null && target != null)
        {
            var lookPos = target.position - owner.transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 1f);
        }
    }
}
