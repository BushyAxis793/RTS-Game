﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tent : MonoBehaviour, ISelectable {

	[SerializeField]
	Transform spawnPoint, flag;

	[SerializeField]
	GameObject hpBarPrefab;

	protected Healthbar healthbar;

	private void Start()
	{
		Unit.SelectableUnits.Add(this);
		SetSelected(false);
	}

	private void OnDestroy()
	{
		Unit.SelectableUnits.Remove(this);
	}

	public void SetSelected(bool selected)
	{
		flag.gameObject.SetActive(selected);
		healthbar.gameObject.SetActive(selected);
	}

	void Spawn (GameObject prefab)
	{
		var buyable = prefab.GetComponent<Buyable>();
		if (!buyable || !Money.TrySpendMoney(buyable.cost)) return;

		var unit = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
		unit.SendMessage("Command", flag.position,SendMessageOptions.DontRequireReceiver);
		MoneyEarner.ShowMoneyText(unit.transform.position, -(int)buyable.cost);
	}

	void Command(Vector3 flagPosition)
	{
		flag.position = flagPosition;
	}

	void Command(Unit unit)
	{
		Command(unit.transform.position);
	}

}
