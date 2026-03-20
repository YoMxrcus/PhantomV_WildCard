using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using JetBrains.Annotations;
public class LootBox : MonoBehaviour
{

    [SerializeField] List<string> LootNames = new List<string>() { "Stim", "Grenade", "PistolAmmo", "AssaultAmmo", "ShotgunAmmo" };
    [SerializeField] int LootCount = 0;
    public List<string> CurrentLoot = new List<string>();
    public GameObject Player;
    public bool IsLooted = false;
    public bool Randomized = false;

    public GameObject[] LootIMAGES;
    public Texture PistolAmmoIMAGE, AssaultAmmoIMAGE, ShotgunAmmoIMAGE, StimIMAGE, GrenadeIMAGE, EmptyIMAGE;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        Player = GameObject.Find("MainPlayer");
    }
    public void RandomizeLoot()
    {
       
            LootCount = Random.Range(1, 5);

            for (int i = 0; i < LootCount; i++)
            {
                CurrentLoot.Add(LootNames[Random.Range(0, 5)]);
                Debug.Log(CurrentLoot);
            }

        AssignImages();

        


        // Update is called once per frame
       
    }

    public void AssignImages()
    {
        for (int i = 0; i < CurrentLoot.Count; i++)
        {

            switch (CurrentLoot[i])
            {
                case "Stim":
                    LootIMAGES[i].GetComponent<RawImage>().texture = StimIMAGE;
                    continue;

                case "Grenade":
                    LootIMAGES[i].GetComponent<RawImage>().texture = GrenadeIMAGE;
                    continue;

                case "PistolAmmo":
                    LootIMAGES[i].GetComponent<RawImage>().texture = PistolAmmoIMAGE;
                    continue;

                case "AssaultAmmo":
                    LootIMAGES[i].GetComponent<RawImage>().texture = AssaultAmmoIMAGE;
                    continue;

                case "ShotgunAmmo":
                    LootIMAGES[i].GetComponent<RawImage>().texture = ShotgunAmmoIMAGE;
                    continue;
            }

            return;



        }
    }
        public void ResetImages()
        {
            LootIMAGES[0].GetComponent<RawImage>().texture = EmptyIMAGE;
            LootIMAGES[1].GetComponent<RawImage>().texture = EmptyIMAGE;
            LootIMAGES[2].GetComponent<RawImage>().texture = EmptyIMAGE;
            LootIMAGES[3].GetComponent<RawImage>().texture = EmptyIMAGE;
        }
}
