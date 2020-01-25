using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitButton : MonoBehaviour {

	public void SpawnUnits(GameObject prefab)
	{
		CameraControl.SpawnUnits(prefab);
	}
}
