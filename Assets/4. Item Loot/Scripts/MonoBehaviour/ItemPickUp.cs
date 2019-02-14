using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemPickUps_SO itemDefinition;

    public CharacterStats charStats;
    CharacterInventory _charInventory;

    GameObject _foundStats; 

    #region Constructor
    public ItemPickUp()
    {
        _charInventory = CharacterInventory.instance;
    }
	#endregion

	private void Start()
	{
        if (charStats == null)
        {
            _foundStats = GameObject.FindGameObjectWithTag("Player");
            charStats = _foundStats.GetComponent<CharacterStats>();
        }
	}

    void StoreItemInInvntory()
    {
        _charInventory.StoreItem(this);
    }

    public void UseItem()
    {
        switch(itemDefinition.itemType)
        {
            case ItemTypeDefinitions.HEALTH:
                charStats.ApplyHealth(itemDefinition.itemAmount);
                Debug.Log(charStats.GetHealth());
                break;

            case ItemTypeDefinitions.MANA:
                charStats.ApplyMana(itemDefinition.itemAmount);
                break;

            case ItemTypeDefinitions.WEALTH:
                charStats.GiveWealth(itemDefinition.itemAmount);
                break;

            case ItemTypeDefinitions.WEAPON:
                charStats.ChangeWeapon(this);
                break;

            case ItemTypeDefinitions.ARMOR:
                charStats.ChangeArmor(this);
                break;
        }
    }

	private void OnTriggerEnter(Collider other)
	{
        if (other.tag == "Player")
        {
            if (itemDefinition.isStoreable)
            {
                StoreItemInInvntory();
            }
            else
            {
                UseItem();
            }
        }
	}
}
