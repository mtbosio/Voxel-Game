using System;
using System.Collections;
using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    private Item item;
    [SerializeField] private Collider myCollider;
    [SerializeField] private float moveSpeed = 8f;
    private bool isPickedUp = false;
    public void Initialize(Item item){
        this.item = item;
        myCollider = GetComponentInChildren<Collider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(!isPickedUp && other.CompareTag("Player")){
            isPickedUp = true;
            bool canAdd = InventoryManager.instance.AddItem(item);
            if(canAdd){
                StartCoroutine(MoveAndCollect(other.transform));
            } else {
                isPickedUp = false;
            }
        }
    }

    private IEnumerator MoveAndCollect(Transform target){
        Destroy(myCollider);
        while(Vector3.Distance(transform.position, target.position) > 0.1f){
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
    }

    
}
