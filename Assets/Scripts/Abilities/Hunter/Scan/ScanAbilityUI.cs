using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanAbilityUI : MonoBehaviour
{
    public ScanAbility parent;
    public Transform player;
    public Transform target;
    public GameObject imageObj;
    public float duration;

    private GameObject arrowUI = null;
    private float damping = 1f;
    private LineRenderer r = null;

    public void Initialize(ScanAbility parent, Transform player, Transform target, float duration)
    {
        this.parent = parent;
        this.player = player;
        this.target = target;
        this.duration = duration;

        r = gameObject.AddComponent<LineRenderer>();
        r.startWidth = 0.2f;
        r.endWidth = 0.2f;
        StartCoroutine(End());
    }

    public void Display()
    {
        //Canvas mainCanvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
        //arrowUI = Instantiate(parent.scanArrowUI, mainCanvas.transform);
    }

    public IEnumerator End()
    {
        yield return new WaitForSeconds(this.duration);
        //Destroy(arrowUI);
        Destroy(r);
        Destroy(this);
    }

    private void Update()
    {
        if (r != null)
        {
            var lookPos = (target.position - player.transform.position).normalized;
            //lookPos.y = 0;
            //var rotation = Quaternion.LookRotation(lookPos, Vector3.forward);
            //arrowUI.transform.rotation = Quaternion.Slerp(arrowUI.transform.rotation, rotation, Time.deltaTime * damping);
            r.startColor = Color.red;
            r.endColor = Color.red;
            Vector3[] positions = new Vector3[2];
            positions[0] = player.transform.position + 1f * Vector3.up;
            positions[1] = 2f * lookPos + 1f * Vector3.up + player.transform.position;
            r.enabled = true;
            r.SetPositions(positions);
        }
    }
}