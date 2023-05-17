using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainMenuCanvas : MonoBehaviour
{
    [SerializeField] private Image background;
    public Dropdown biomeSetup;
    public InputField inputField;
    [SerializeField] private Core.GameManager gameManager;
    private void Start() {
        var allBiomes = FakeDatabase.Instance.GetBiomes();
        List<string> biomeMenu = new();
        foreach (var biome in allBiomes)
        {
            biomeMenu.Add(biome.biomeName);
        }
        biomeSetup.AddOptions(biomeMenu);

        FakeDatabase.Instance.selectedBiome = allBiomes[0];

        biomeSetup.onValueChanged.AddListener(SelectNewBiome);
    }
    private void SelectNewBiome(int index)
    {
        FakeDatabase.Instance.selectedBiome = FakeDatabase.Instance.GetBiomes()[index];     //test
    }
    public void StartGame()
    {
        int seed = Convert.ToInt32(inputField.text);
        Debug.Log("seed is: " + seed);
        gameManager.StartWorld(seed);
    }
}
