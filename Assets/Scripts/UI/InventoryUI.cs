using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject IsOpen;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
            IsOpen.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            IsOpen.SetActive(false);
        }

    }
}
