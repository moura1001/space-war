using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float moveSpeed = 1.2f;
    private Vector3 moveDirection = new Vector3(0, -1, 0);
    private string id;

    public event SpawnHandler.OnSpawnEnemy OnArrivalPosition;

    public delegate void OnHit(GameObject enemy, GameObject projectil);
    public event OnHit OnHitDamage;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckPosition());
    }

    void FixedUpdate()
    {
        gameObject.transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("OnTriggerEnter2D: " + col.name);

        OnHitDamage?.Invoke(gameObject, col.gameObject);
    }

    private IEnumerator CheckPosition()
    {
        while (gameObject.transform.position.y > -4)
            yield return new WaitForSeconds(2);

        OnArrivalPosition?.Invoke(gameObject);
    }

    public string GetId()
    {
        return id;
    }
    public void SetId(string id)
    {
        this.id = id;
    }
}
