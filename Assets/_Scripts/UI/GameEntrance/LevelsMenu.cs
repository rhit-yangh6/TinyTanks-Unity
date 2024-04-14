using _Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Scripts.UI.GameEntrance
{
    public class LevelsMenu : MonoBehaviour
    {
        [SerializeField] private GameObject levelScrollListContent;
        [SerializeField] private GameObject levelCellPrefab;
        
        private void OnEnable()
        {
            PopulateLevels();
        }

        private void PopulateLevels()
        {
            foreach (Transform child in levelScrollListContent.transform) {
                Destroy(child.gameObject);
            }
            
            Level[] levels = LevelManager.Instance.GetAllLevels();

            foreach (Level l in levels)
            {
                GameObject cellObj = Instantiate(levelCellPrefab, levelScrollListContent.transform);
                Image s = cellObj.GetComponentsInChildren<Image>()[1];
                TextMeshProUGUI tmpGUI = cellObj.GetComponentInChildren<TextMeshProUGUI>();
                Button button = cellObj.GetComponent<Button>();

                
                tmpGUI.text = l.name;
                
                button.onClick.AddListener(() => SceneManager.LoadScene(l.path));

                s.sprite = l.levelPreviewSprite;

                /*
                int weaponId = w.id;

                if (PlayerData.Instance.GetWeaponLevelFromId(weaponId) > 0)
                {
                    s.sprite = w.weaponIconSprite;

                    button.onClick.AddListener(() => wdp.SetDetails(weaponId));

                    s.GetComponent<DragDropGrid>().weaponId = weaponId;
                }
                else
                {
                    s.sprite = weaponLockedSprite;
                    button.interactable = false;
                }
                */
            }
        }
    }
}
