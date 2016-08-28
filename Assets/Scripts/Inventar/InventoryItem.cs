using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;
using System;

namespace Inventory
{
    public class InventoryItem : MonoBehaviour
    {
        private static GameObject InventoryItemPrefab;

        private string _name, _description;
        private int _value, _weight; //_value is in cents and _weight is in grams.

        private GameObject _itemObject;

        public InventoryItem()
        {
            _itemObject = Instantiate(InventoryItemPrefab);
            registerListeners();
        }

        public void setInventoryItemPrefab(GameObject prefab)
        {
            InventoryItemPrefab = prefab;
        }

        private void registerListeners()
        {
            InputField nameField = _itemObject.transform.Find("Name").GetComponent<InputField>();
            nameField.onEndEdit.AddListener(new UnityAction<string>(delegate (string inputText)
            {
                _name = inputText;
            }));
            InputField descriptionField = _itemObject.transform.Find("Beschreibung").GetComponent<InputField>();
            nameField.onEndEdit.AddListener(new UnityAction<string>(delegate (string inputText)
            {
                _description = inputText;
            }));
            InputField valueField = _itemObject.transform.Find("Name").GetComponent<InputField>();
            nameField.onEndEdit.AddListener(new UnityAction<string>(delegate (string inputText)
            {
                _value = (int)Math.Round(Convert.ToDecimal(inputText) * 100);
            }));
            InputField weightField = _itemObject.transform.Find("Name").GetComponent<InputField>();
            nameField.onEndEdit.AddListener(new UnityAction<string>(delegate (string inputText)
            {
                _weight = (int)Math.Round(Convert.ToDecimal(inputText) * 1000);
            }));
        }
    }
}