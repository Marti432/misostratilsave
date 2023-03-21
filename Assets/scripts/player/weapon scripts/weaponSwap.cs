using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponSwap : MonoBehaviour
{
    [SerializeField, Tooltip("list of all player usable weapons")]
    GameObject[] allWeapons;

    [SerializeField]
    int[] currentWeapons = new int[2];
    [SerializeField] bool isHoldingPrimary;
    void Start()
    {
        isHoldingPrimary = allWeapons[currentWeapons[0]].activeSelf;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && !isHoldingPrimary) WeaponSwitch();
        if (Input.GetKeyDown(KeyCode.Alpha2) && isHoldingPrimary) WeaponSwitch();
    }

    void WeaponSwitch()
    {
        if (!isHoldingPrimary) 
        {
            allWeapons[currentWeapons[0]].SetActive(true);
            Debug.Log(allWeapons[currentWeapons[0]].transform.name);
            allWeapons[currentWeapons[1]].SetActive(false);
            allWeapons[currentWeapons[1]].GetComponent<weaponPlayer>().isShooting = false;
        }
        else
        {
            allWeapons[currentWeapons[0]].SetActive(false);
            allWeapons[currentWeapons[0]].GetComponent<weaponPlayer>().isShooting = false;
            allWeapons[currentWeapons[1]].SetActive(true);
            Debug.Log(allWeapons[currentWeapons[1]].transform.name);
        }
        isHoldingPrimary = !isHoldingPrimary;
    }
    void OnApplicationQuit()
    {
        for (int i = 0; i < currentWeapons.Length; i++)
        {
            allWeapons[currentWeapons[i]].SetActive(true);
        }
    }
}
