using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Projectil : MonoBehaviour
{
    private float moveSpeed = 4.4f;
    private Vector3 moveDirection = new Vector3(0, 1, 0);
    
    public event SpawnHandler.OnSpawnEnemy OnDestroyProjectil;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckPosition());
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Destroy(gameObject);
    }

    private IEnumerator CheckPosition()
    {
        while (gameObject.transform.position.y < 4)
            yield return new WaitForSeconds(2);

        OnDestroyProjectil?.Invoke(gameObject);
    }
}
