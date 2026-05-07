using UnityEngine;
using UnityEngine.AI;

public class GoTo : MonoBehaviour
{
    //public Transform target;

    public GameObject target;
   

    void Update()
    {
        gameObject.GetComponent<NavMeshAgent>().SetDestination(target.transform.position);
    }
}

