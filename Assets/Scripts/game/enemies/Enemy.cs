using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float moveSpeed = 1.2f;
    private Vector3 moveDirection = new Vector3(0, -1, 0);
    private string id;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        gameObject.transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("OnTriggerEnter2D");
        Destroy(gameObject);
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
