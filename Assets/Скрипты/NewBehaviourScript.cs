using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Скрипты
{
    public class NewBehaviourScript : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerHealth hp = other.GetComponent<PlayerHealth>();
                if (hp != null)
                {
                    hp.TakeDamage(10f);
                }
            }
        }
    }
}