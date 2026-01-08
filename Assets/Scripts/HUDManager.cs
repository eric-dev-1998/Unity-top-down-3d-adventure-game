using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    // Coin and keys counter text object:
    public Text coinCounter;
    public Text keyCounter;

    // Update is called once per frame
    void Update()
    {
        coinCounter.text = "Monedas: " + PlayerInventory.coins.ToString();
        keyCounter.text = "Llaves" + PlayerInventory.keys.ToString();
    }
}
