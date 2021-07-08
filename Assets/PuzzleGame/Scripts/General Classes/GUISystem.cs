using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUISystem : MonoBehaviour
{

    public Text itemName;                   // Name of the currently active item

    public Sprite glider;
    public Sprite hand;
    public Sprite icewand;
    public Sprite magnet;

    private Image[] slots = new Image[3];
    public Image slotFirst;                 // The main slot   (Middle)
    public Image slotLast;                  // The last slot   (Left)
    public Image slotSecond;                // The second slot (Right)

    private List<Sprite> items = new List<Sprite>();
    private CharacterMovement player;       // Reference to the player to activate the selected skill

    private void Awake()
    {
        slots[0] = slotLast;
        slots[1] = slotFirst;
        slots[2] = slotSecond;

        //foreach (Image slot in slots)
        //{
        //    if (slot.sprite != null)
        //    {
        //        unlockedItems.Add(slot.sprite);
        //    }
        //}

        //UpdateGUI();
    }

    public void Init(CharacterMovement player)
    {
        this.player = player;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            SwitchItem(false);
        }
        else if(Input.GetKeyDown(KeyCode.R))
        {
            SwitchItem(true);
        }
    }

    public bool AddItem(PowerupType type)
    {

        if (GameManager.Instance.collectedItems.Add(type))
        {
            switch (type)
            {
                case PowerupType.Magnet:
                    items.Insert(0, magnet);
                    break;
                case PowerupType.Icewand:
                    items.Insert(0, icewand);
                    break;
                case PowerupType.Glider:
                    items.Insert(0, glider);
                    break;
                case PowerupType.Hand:
                    items.Insert(0, hand);
                    break;
            }
            UpdateGUI();
            return true;
        }

        return false;
    }

    public void SwitchItem(bool switchRight)
    {
        List<Sprite> tempList = new List<Sprite>();
        int direction = (switchRight ? 1 : items.Count - 1);

        for (int i = 0; i < items.Count; i++)
        {
            tempList.Add(items[(i + direction) % items.Count]);
        }

        items = tempList;
        UpdateGUI();
    }

    public void UpdateGUI()
    {
        int totalItems = items.Count;

        slotFirst.sprite = (totalItems > 0 ? items[0] : null);
        slotSecond.sprite = (totalItems > 1 ? items[1] : null);
        slotLast.sprite = (totalItems > 2 ? items[items.Count - 1] : null);
        
        slotFirst.GetComponent<Animation>().Stop();
        slotFirst.GetComponent<Animation>().Play();

        itemName.text = (slotFirst.sprite != null ? slotFirst.sprite.name : "No Items");

        foreach (Image slot in slots)
        {
            slot.enabled = slot.sprite;
        }

        if(totalItems > 0)
        {
            player.SetActiveSkill(SkillCheck(items[0].name.ToLower()));
        }
    }

    private PowerupType SkillCheck(string type)
    {
        switch (type)
        {
            case "magnet":
                return PowerupType.Magnet;
            case "icewand":
                return PowerupType.Icewand;
            case "glider":
                return PowerupType.Glider;
            default:
                return PowerupType.Hand;
        }
    }
    
}
