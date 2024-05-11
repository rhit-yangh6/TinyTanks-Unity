using System;
using _Scripts.GameEngine;
using _Scripts.Projectiles;
using _Scripts.UI;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace _Scripts.Managers
{
    public class WeaponManager : MonoBehaviour
    {
        [SerializeField] private GameObject newWeaponMsgPanel;
        
        private static WeaponManager _i;
        private static Weapons _weaponsFromJson;
        
        public static WeaponManager Instance
        {
            get
            {
                if (_i == null)
                {
                    _i = Instantiate(Resources.Load<WeaponManager>("WeaponManager"));
                    LoadWeapons();
                }
                return _i;
            }
        }
        
        private static void LoadWeapons()
        {
            TextAsset jsonFile = Resources.Load<TextAsset>("Data/Weapons");

            _weaponsFromJson = JsonUtility.FromJson<Weapons>(jsonFile.text);
            foreach (Weapon weapon in _weaponsFromJson.weapons)
            {
                weapon.weaponIconSprite = Resources.Load<Sprite>("WeaponIcons/" + weapon.dataPath);
                weapon.projectilePrefab = Resources.Load<GameObject>("ProjectilePrefabs/" + weapon.dataPath);
                weapon.SetParams();
            }
        }

        public Weapon GetWeaponById(int idToFind)
        {
            return Array.Find(_weaponsFromJson.weapons, w => w.id == idToFind); 
        }

        public Weapon[] GetAllWeapons()
        {
            return _weaponsFromJson.weapons;
        }

        public bool UnlockWeapon(int weaponId)
        {
            var result = PlayerData.Instance.BuyWeapon(weaponId);
            if (!result) return false;
            
            var panel = Instantiate(Resources.Load<GameObject>("NewWeaponMsgPanel"),
                GameObject.FindGameObjectWithTag("Canvas").transform);
            panel.GetComponent<NewWeaponMsgBehavior>().Display(weaponId);
            return true;
        }
    }
    
    [Serializable]
    public class Weapon
    {
        // Read from Weapons.json
        public int id, steps, shopPrice;
        public string weaponName, weaponDescription, dataPath, explosionFXPath, saying;
        public float damage, radius, maxMagnitude, explosionDuration;
        
        // Also Read from Weapons.json, but Extra Data Types
        public UpgradeInfo[] upgradeInfos;
        public ExtraWeaponTerm[] extraWeaponTerms;
        
        // Later Generated
        public Sprite weaponIconSprite;
        public GameObject projectilePrefab;

        public void SetParams()
        {
            LaunchedProjectile p = projectilePrefab.GetComponent<LaunchedProjectile>();
            p.SetParameters(damage, radius, maxMagnitude, steps, explosionDuration, extraWeaponTerms);
        }
        
    }
        
    [Serializable]
    public class Weapons
    {
        public Weapon[] weapons;
    }

    [Serializable]
    public class ExtraWeaponTerm
    {
        public string term;
        public float value;
    }

    [Serializable]
    public class UpgradeInfo
    {
        public string name;
        public string description;
        public int cost;
    }
}
