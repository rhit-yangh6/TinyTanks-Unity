using System;
using _Scripts.Projectiles;
using UnityEditor;
using UnityEngine;

namespace _Scripts
{
    public class WeaponManager : MonoBehaviour
    {
        //public static WeaponManager Instance { get; private set; }
        
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

        private void Start()
        {
            // LoadWeapons();
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
        public int id, steps;
        public string weaponName;
        public string weaponDescription, dataPath;
        public float damage, radius, maxMagnitude;
        
        public string[] upgradeDescriptions;
        public int[] upgradeCosts;
        public ExtraWeaponTerm[] extraWeaponTerms;
        
        // Later Generated
        public Sprite weaponIconSprite;
        public GameObject projectilePrefab;

        public void SetParams()
        {
            IProjectile p = projectilePrefab.GetComponent<IProjectile>();
            p.SetParameters(damage, radius, maxMagnitude, steps, extraWeaponTerms);
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
}
