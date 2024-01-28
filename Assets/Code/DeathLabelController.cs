using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DeathLabelController : MonoBehaviour
{
    public GameObject player1Win;
    public GameObject player2Win;

    private void Start()
    {
        player1Win.SetActive(false);
        player2Win.SetActive(false);
    }
}