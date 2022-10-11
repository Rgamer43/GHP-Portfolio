using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{

    public GameObject weapon;
    
    public float maxRecoil;
    public float recoil;
    public float addedRecoil;
    //public Transform baseRot;
    public Transform startRot;

    public void Start()
    {
        //startRot = weapon.transform;
        //startRot = baseRot;
        //startRot.rotation = new Quaternion(baseRot.rotation.x, baseRot.rotation.y, baseRot.rotation.z, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //print("Weapon Recoil startRot.rotation.z: " + startRot.rotation.z);

        //startRot.localEulerAngles = new Vector3(startRot.localEulerAngles.x, startRot.localEulerAngles.y, 0);

        startRot.localEulerAngles = new Vector3(0, 0, 0);

        weapon.transform.Rotate(0, 0, recoil);
        if(recoil > 0) recoil -= Time.deltaTime * 5;
        else
        {
            recoil = 0;
            var minRecoil = Quaternion.Euler(0, 0, 0);
            // Dampen towards the target rotation
            startRot.rotation = Quaternion.Slerp(startRot.rotation, minRecoil, Time.deltaTime * addedRecoil / 2);
            weapon.transform.localEulerAngles = new Vector3(0, -90, startRot.localEulerAngles.z);
        }
    }

    public void addRecoil()
    {
        recoil += addedRecoil;
        if (recoil > maxRecoil) recoil = maxRecoil;
    }
}
