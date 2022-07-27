using System;
using System.Collections.Generic;
using _Scripts.Buffs;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts
{
    public class BuffPanelBehavior: MonoBehaviour
    {
        [SerializeField] private GameObject buffPanel;
        [SerializeField] private GameObject buffIconPrefab;
        [SerializeField] private Vector3 offset;

        public void RefreshBuffDisplay(Dictionary<ScriptableBuff, TimedBuff> buffs)
        {
            foreach (Transform child in buffPanel.transform) Destroy(child.gameObject);

            foreach (var buff in buffs)
            {
                if (buff.Value.isFinished) continue;
                
                var obj = Instantiate(buffIconPrefab, buffPanel.transform);
                var img = obj.GetComponent<Image>();
                img.sprite = buff.Key.buffIcon;
            }
        }
        
        private void Update()
        {
            buffPanel.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position + offset);
        }
    }
}