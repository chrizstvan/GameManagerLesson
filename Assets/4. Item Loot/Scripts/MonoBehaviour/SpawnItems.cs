using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItems : MonoBehaviour, ISpwans
{
    public ItemPickUps_SO[] itemDefinition;

    int _whichToSpawn = 0;
    int _totalSpawnWeight = 0;
    int _chosen = 0;

    public Rigidbody itemSpawned { get; set; }
    public Renderer itemMaterial { get; set; }
    public ItemPickUp itemType { get; set; }

	private void Start()
	{
        foreach (ItemPickUps_SO ip in itemDefinition )
        {
            _totalSpawnWeight += ip.spawnChanceWeight;
        }
	}

    public void CreateSpawn()
    {
        //Spawn with weighted possibility
        _chosen = Random.Range(0, _totalSpawnWeight);

        foreach (ItemPickUps_SO ip in itemDefinition)
        {
            _whichToSpawn += ip.spawnChanceWeight;
            if (_whichToSpawn >= _chosen)
            {
                itemSpawned = Instantiate(ip.itemSpawnObject, transform.position, Quaternion.identity);

                itemMaterial = itemSpawned.GetComponent<Renderer>();
                itemMaterial.material = ip.itemMaterial;

                itemType = itemSpawned.GetComponent<ItemPickUp>();
                itemType.itemDefinition = ip;
                break;
            }
        }
    }
}
