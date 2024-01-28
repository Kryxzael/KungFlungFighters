using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnRandom : MonoBehaviour
{

    private void Start()
    {
        int keptChildIndex = UnityEngine.Random.Range(0, transform.childCount);

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == keptChildIndex);
        }
    }
}
