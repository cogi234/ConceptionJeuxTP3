using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class detectionbox : MonoBehaviour
{
    GameObject oiseau;
    PatrouilleOiseau patrouilleOiseau;
    // Start is called before the first frame update
    void Start()
    {
        oiseau = GameObject.FindGameObjectWithTag("lookDirectionbird");
        patrouilleOiseau=GameObject.FindGameObjectWithTag("lookDirectionbird").GetComponent<PatrouilleOiseau>();
        transform.position = oiseau.transform.position;
    }
    private void Update()
    {
        transform.position = oiseau.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
          
            patrouilleOiseau.l1.detection = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            patrouilleOiseau.l1.detection = false;
        }
        
           
        

    }


}
