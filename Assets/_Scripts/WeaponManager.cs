using System;
using _Scripts.Projectiles;
using UnityEngine;

namespace _Scripts
{
    public class WeaponManager : MonoBehaviour
    {
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
    }
    
    [Serializable]
    public class Weapon
    {
        // Read from Weapons.json
        public int id, steps, shopPrice;
        public string weaponName, weaponDescription, dataPath, explosionFXPath;
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
            GameObject explosionPrefab = Resources.Load<GameObject>("ExplosionFX/" + explosionFXPath);
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
