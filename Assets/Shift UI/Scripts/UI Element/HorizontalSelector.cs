﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace Michsky.UI.Shift
{
    public class HorizontalSelector : MonoBehaviour
    {
        [Header("Settings")]
        public int defaultIndex = 0;
        public bool invokeAtStart;
        public bool invertAnimation;
        public bool loopSelection;
        [HideInInspector] public int index = 0;
		
	    [Header("Saving")]
        public bool saveValue;
        public string selectorTag = "Tag Text";

        [Header("Indicators")]
        public bool enableIndicators = true;
        public Transform indicatorParent;
        public GameObject indicatorObject;

        [Header("Items")]
        public List<Item> itemList = new List<Item>();

        private TextMeshProUGUI label;
        private TextMeshProUGUI labeHelper;
        private Animator selectorAnimator;
        string newItemTitle;

        [System.Serializable]
        public class Item
        {
            public string itemTitle = "Item Title";
            public UnityEvent onValueChanged = new UnityEvent();
        }

        void Awake()
        {
            selectorAnimator = gameObject.GetComponent<Animator>();
            label = transform.Find("Text").GetComponent<TextMeshProUGUI>();
            labeHelper = transform.Find("Text Helper").GetComponent<TextMeshProUGUI>();
        }

        void Start()
        {
            if (saveValue == true)
            {
                if (PlayerPrefs.HasKey(selectorTag + "HSelectorValue") == true) { defaultIndex = PlayerPrefs.GetInt(selectorTag + "HSelectorValue"); }
                else { PlayerPrefs.SetInt(selectorTag + "HSelectorValue", defaultIndex); }
            }

            label.text = itemList[defaultIndex].itemTitle;
            labeHelper.text = label.text;
            index = defaultIndex;

            if(enableIndicators == true)
            {
                foreach (Transform child in indicatorParent)
                    Destroy(child.gameObject);

                for (int i = 0; i < itemList.Count; ++i)
                {
                    GameObject go = Instantiate(indicatorObject, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                    go.transform.SetParent(indicatorParent, false);
                    go.name = itemList[i].itemTitle;

                    Transform onObj;
                    onObj = go.transform.Find("On");
                    Transform offObj;
                    offObj = go.transform.Find("Off");

                    if (i == index)
                    {
                        onObj.gameObject.SetActive(true);
                        offObj.gameObject.SetActive(false);
                    }

                    else
                    {
                        onObj.gameObject.SetActive(false);
                        offObj.gameObject.SetActive(true);
                    }
                }
            }

            else
            {
                Destroy(indicatorParent);
            }

            if (invokeAtStart == true)
                itemList[index].onValueChanged.Invoke();
        }

        public void PreviousClick()
        {
            if (loopSelection == false)
            {
                if (index != 0)
                {
                    labeHelper.text = label.text;

                    if (index == 0)
                        index = itemList.Count - 1;

                    else
                        index--;

                    label.text = itemList[index].itemTitle;

                    try { itemList[index].onValueChanged.Invoke(); }

                    catch { }

                    selectorAnimator.Play(null);
                    selectorAnimator.StopPlayback();

                    if (invertAnimation == true)
                        selectorAnimator.Play("Forward");
                    else
                        selectorAnimator.Play("Previous");

                    if (saveValue == true)
                        PlayerPrefs.SetInt(selectorTag + "HSelectorValue", index);
                }
            }

            else
            {
                labeHelper.text = label.text;

                if (index == 0)
                    index = itemList.Count - 1;

                else
                    index--;

                label.text = itemList[index].itemTitle;

                try { itemList[index].onValueChanged.Invoke(); }
                catch { }

                selectorAnimator.Play(null);
                selectorAnimator.StopPlayback();

                if (invertAnimation == true)
                    selectorAnimator.Play("Forward");
                else
                    selectorAnimator.Play("Previous");

                if (saveValue == true)
                    PlayerPrefs.SetInt(selectorTag + "HSelectorValue", index);
            }

            if (saveValue == true)
                PlayerPrefs.SetInt(selectorTag + "HSelectorValue", index);

            if (enableIndicators == true)
            {
                for (int i = 0; i < itemList.Count; ++i)
                {
                    GameObject go = indicatorParent.GetChild(i).gameObject;

                    Transform onObj;
                    onObj = go.transform.Find("On");
                    Transform offObj;
                    offObj = go.transform.Find("Off");

                    if (i == index)
                    {
                        onObj.gameObject.SetActive(true);
                        offObj.gameObject.SetActive(false);
                    }

                    else
                    {
                        onObj.gameObject.SetActive(false);
                        offObj.gameObject.SetActive(true);
                    }
                }
            }
        }

        public void ForwardClick()
        {
            if (loopSelection == false)
            {
                if (index != itemList.Count - 1)
                {
                    labeHelper.text = label.text;

                    if ((index + 1) >= itemList.Count)
                        index = 0;
                    else
                        index++;

                    label.text = itemList[index].itemTitle;

                    try { itemList[index].onValueChanged.Invoke(); }
                    catch { }

                    selectorAnimator.Play(null);
                    selectorAnimator.StopPlayback();

                    if (invertAnimation == true)
                        selectorAnimator.Play("Previous");
                    else
                        selectorAnimator.Play("Forward");

                    if (saveValue == true)
                        PlayerPrefs.SetInt(selectorTag + "HSelectorValue", index);
                }
            }

            else
            {
                labeHelper.text = label.text;

                if ((index + 1) >= itemList.Count)
                    index = 0;
                else
                    index++;

                label.text = itemList[index].itemTitle;

                try { itemList[index].onValueChanged.Invoke(); }
                catch { }

                selectorAnimator.Play(null);
                selectorAnimator.StopPlayback();

                if (invertAnimation == true)
                    selectorAnimator.Play("Previous");
                else
                    selectorAnimator.Play("Forward");

                if (saveValue == true)
                    PlayerPrefs.SetInt(selectorTag + "HSelectorValue", index);
            }

            if (saveValue == true)
                PlayerPrefs.SetInt(selectorTag + "HSelectorValue", index);

            if (enableIndicators == true)
            {
                for (int i = 0; i < itemList.Count; ++i)
                {
                    GameObject go = indicatorParent.GetChild(i).gameObject;

                    Transform onObj;
                    onObj = go.transform.Find("On");
                    Transform offObj;
                    offObj = go.transform.Find("Off");

                    if (i == index)
                    {
                        onObj.gameObject.SetActive(true);
                        offObj.gameObject.SetActive(false);
                    }

                    else
                    {
                        onObj.gameObject.SetActive(false);
                        offObj.gameObject.SetActive(true);
                    }
                }
            }
        }

        public void CreateNewItem(string title)
        {
            Item item = new Item();
            newItemTitle = title;
            item.itemTitle = newItemTitle;
            itemList.Add(item);
        }

        public void UpdateUI()
        {
            if (label == null || itemList.Count < index)
                return;

            label.text = itemList[index].itemTitle;

            if (enableIndicators == true)
            {
                foreach (Transform child in indicatorParent)
                    Destroy(child.gameObject);

                for (int i = 0; i < itemList.Count; ++i)
                {
                    GameObject go = Instantiate(indicatorObject, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                    go.transform.SetParent(indicatorParent, false);
                    go.name = itemList[i].itemTitle;

                    Transform onObj;
                    onObj = go.transform.Find("On");
                    Transform offObj;
                    offObj = go.transform.Find("Off");

                    if (i == index)
                    {
                        onObj.gameObject.SetActive(true);
                        offObj.gameObject.SetActive(false);
                    }

                    else
                    {
                        onObj.gameObject.SetActive(false);
                        offObj.gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}