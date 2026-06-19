using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class LootBox : MonoBehaviour
{
    private const int LootImageSlotCount = 4;

    [SerializeField] List<string> LootNames = new List<string>() { "Stim", "Grenade", "PistolAmmo", "AssaultAmmo", "ShotgunAmmo", "Bandage" };
    [SerializeField] int LootCount = 0;
    public List<string> CurrentLoot = new List<string>();
    public GameObject Player;
    public bool IsLooted = false;
    public bool Randomized = false;
    public bool ForceMaximumLootCount = false;
    public GameObject LootLight;

    public GameObject[] LootIMAGES;
    public Texture PistolAmmoIMAGE, AssaultAmmoIMAGE, ShotgunAmmoIMAGE, StimIMAGE, GrenadeIMAGE, EmptyIMAGE, BandageIMAGE, CondensorIMAGE;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        Player = GameObject.Find("MainPlayer");

        if (Player == null)
        {
            Player = GameObject.Find("ThirdPersonTEST");
        }

        Transform lootLightTransform = transform.Find("LootLight");

        if (lootLightTransform != null)
        {
            LootLight = lootLightTransform.gameObject;
        }

        ResolveLootImages();
    }

    private void Update()
    {
        if (IsLooted && LootLight != null)
        {
            LootLight.SetActive(false);
        }
    }
    public void RandomizeLoot()
    {
       
            LootCount = ForceMaximumLootCount ? LootImageSlotCount : Random.Range(1, LootImageSlotCount + 1);

            for (int i = 0; i < LootCount; i++)
            {
                CurrentLoot.Add(LootNames[Random.Range(0, LootNames.Count)]);
                Debug.Log(CurrentLoot);
            }

        AssignImages();

        


        // Update is called once per frame
       
    }

    public void AssignImages()
    {
        ResolveLootImages();

        for (int i = 0; i < CurrentLoot.Count; i++)
        {
            if (i >= LootIMAGES.Length || LootIMAGES[i] == null)
            {
                continue;
            }

            switch (CurrentLoot[i])
            {
                case "Stim":
                    SetLootImage(i, StimIMAGE);
                    continue;

                case "Grenade":
                    SetLootImage(i, GrenadeIMAGE);
                    continue;

                case "PistolAmmo":
                    SetLootImage(i, PistolAmmoIMAGE);
                    continue;

                case "AssaultAmmo":
                    SetLootImage(i, AssaultAmmoIMAGE);
                    continue;

                case "ShotgunAmmo":
                    SetLootImage(i, ShotgunAmmoIMAGE);
                    continue;

                case "Bandage":
                    SetLootImage(i, BandageIMAGE);
                    continue;

                case "Condensor":
                    SetLootImage(i, CondensorIMAGE);
                    continue;
            }

            return;



        }
    }
        public void ResetImages()
        {
            ResolveLootImages();

            for (int i = 0; i < LootIMAGES.Length; i++)
            {
                SetLootImage(i, EmptyIMAGE);
            }
        }

    private void ResolveLootImages()
    {
        if (LootIMAGES == null || LootIMAGES.Length != LootImageSlotCount)
        {
            LootIMAGES = new GameObject[LootImageSlotCount];
        }

        LootIMAGES[0] = LootIMAGES[0] != null ? LootIMAGES[0] : FindSceneGameObjectEvenIfInactive("LootOne");
        LootIMAGES[1] = LootIMAGES[1] != null ? LootIMAGES[1] : FindSceneGameObjectEvenIfInactive("LootTwo");
        LootIMAGES[2] = LootIMAGES[2] != null ? LootIMAGES[2] : FindSceneGameObjectEvenIfInactive("LootThree");
        LootIMAGES[3] = LootIMAGES[3] != null ? LootIMAGES[3] : FindSceneGameObjectEvenIfInactive("LootFour");
    }

    private GameObject FindSceneGameObjectEvenIfInactive(string objectName)
    {
        GameObject activeObject = GameObject.Find(objectName);

        if (activeObject != null)
        {
            return activeObject;
        }

        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject sceneObject in allObjects)
        {
            if (sceneObject.name == objectName && sceneObject.scene.IsValid())
            {
                return sceneObject;
            }
        }

        return null;
    }

    private void SetLootImage(int index, Texture texture)
    {
        if (index < 0 || LootIMAGES == null || index >= LootIMAGES.Length || LootIMAGES[index] == null)
        {
            return;
        }

        RawImage rawImage = LootIMAGES[index].GetComponent<RawImage>();

        if (rawImage != null)
        {
            rawImage.texture = texture;
        }
    }

    public void SetLootPool(List<string> lootNames)
    {
        if (lootNames == null || lootNames.Count == 0)
        {
            return;
        }

        LootNames = new List<string>(lootNames);
    }
}
