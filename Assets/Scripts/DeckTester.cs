using System.Collections.Generic;
using UnityEngine;
using System;

public class DeckTester : MonoBehaviour
{
    [SerializeField] PlayerController _player = null;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            ChangeTurn();
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            AddEnergy();
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _player.TakeDamage(500);
        }
    }

    // adds energy for testing purposes
    private void AddEnergy()
    {
        _player.PlayerEnergy++;
        if(_player.PlayerEnergy > 99)
        {
            Debug.Log("Maximum energy!");
            _player.PlayerEnergy = 99;
        }
    }

    private void ChangeTurn()
    {
        Debug.Log("Resetting Boosts!");
        _player.OnTurn();
    }
}
