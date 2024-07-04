using System;
using UnityEngine;

namespace _Scripts.GameEngine.WeaponExtraData
{
    public class GearExtraData : WeaponExtraData
    {
        public int cap = 0;
        public int gearNum = 0;

        public void AddOneGear()
        {
            gearNum = Math.Min(cap, gearNum + 1);
        }

        public void ClearAllGears()
        {
            gearNum = 0;
        }
    }
}