using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//main menu
public class MainMenuCanvas : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private TMP_Dropdown biomeSetup;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Core.GameManager gameManager;
    [SerializeField] private GameObject loadImage;
    private void Start() {
        loadImage.SetActive(false);
        var allBiomes = FakeDatabase.Instance.GetBiomes();
        List<string> biomeMenu = new();
        foreach (var biome in allBiomes)
        {
            biomeMenu.Add(biome.biomeName);
        }
        biomeSetup.AddOptions(biomeMenu);

        FakeDatabase.Instance.selectedBiome = allBiomes[0];

        biomeSetup.onValueChanged.AddListener(SelectNewBiome);

        background.sprite = FakeDatabase.Instance.selectedBiome.background;
    }
    private void SelectNewBiome(int index)
    {
        Debug.Log("new biome selected");
        var biome = FakeDatabase.Instance.GetBiomes()[index];
        FakeDatabase.Instance.selectedBiome = biome;     //test
        background.sprite = biome.background;


    }
    public void StartGame()
    {
        loadImage.SetActive(true);
        Invoke("CreateWorld", 0.1f);
        Invoke("DisableLoadScene", 3f);
    }
    private void DisableLoadScene()
    {
        loadImage.SetActive(false);
    }
    private void CreateWorld()
    {
        int seed = -1;
        if(inputField.text == "")
        {
            Debug.Log("string is empty");
        }
        else
        seed = Convert.ToInt32(inputField.text);
        Debug.Log("seed is: " + seed);
        gameManager.StartWorld(seed);
    }
}
