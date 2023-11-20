using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ramase : MonoBehaviour
{

    bool change = false;
    [SerializeField] Rig TheRig;
    bool updatee = false;
    float compteur  =0;
    private void Update()
    {
        if (updatee)
        {

            
            if (!change)
            {
                compteur =+Time.deltaTime;
                if (compteur > 1) {
                    compteur = 1;
                }
                TheRig.weight = compteur;
            }
            else
            {
                compteur =-Time.deltaTime;
                if (compteur < 0)
                {
                    compteur = 0;
                }
                TheRig.weight = compteur;
            }
            if (compteur >= 1 || compteur <=0)
            {
                change = !change;
                updatee = !updatee;
            }
            
        }
        
    }


    public void changer()
    {
        updatee = !updatee;
    
    }
   
}
