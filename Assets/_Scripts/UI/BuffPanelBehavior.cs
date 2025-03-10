﻿using System.Collections.Generic;
using _Scripts.Buffs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class BuffPanelBehavior: MonoBehaviour
    {
        [SerializeField] private GameObject buffPanel;
        [SerializeField] private GameObject buffIconPrefab;
        [SerializeField] private Transform target;
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

                var bd = obj.GetComponent<BuffDisplay>();
                bd.buffKey = buff.Key.buffKey;
                bd.buffDuration = buff.Value.duration;
            }
        }
        
        private void Update()
        {
            transform.rotation = Quaternion.identity;
            transform.position = target.position + offset;
        }
    }
}