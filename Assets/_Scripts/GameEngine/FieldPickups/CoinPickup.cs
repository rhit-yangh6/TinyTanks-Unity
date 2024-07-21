using UnityEngine;

namespace _Scripts.GameEngine.FieldPickups
{
    public class CoinPickup : FieldPickup
    {
        [SerializeField] private int coinAmount = 8;
        protected override void Trigger()
        {
            PlayerData.Instance.GainMoney(coinAmount);
        }
    }
}