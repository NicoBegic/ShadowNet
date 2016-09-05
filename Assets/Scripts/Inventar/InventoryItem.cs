using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;
using System;

namespace Inventory
{
    /// <summary>
    /// InventoryItem represents an item in the player's inventory.
    /// </summary>
    public class InventoryItem : MonoBehaviour
    {
        private static GameObject InventoryItemPrefab;

        public int _weight; //_weight is in grams.

        private string _name, _description;
        private int _value; //_value is in cents.

        private GameObject _itemObject;
        private InputField _nameField, _descriptionField, _valueField, _weightField;

        /// <summary>
        /// Creates a new InventoryItem with blank values.
        /// </summary>
        /// <require>InventoryItem.hasPrefab()</require>
        public InventoryItem()
        {
            _itemObject = Instantiate(InventoryItemPrefab);
            assignInputFields();
            registerListeners();
        }

        public static bool hasPrefab()
        {
            return InventoryItemPrefab != null;
        }

        /// <summary>
        /// Examines wether a GameObject can be used as a prefab for InventoryItem.
        /// The GameObject must have four children named "Name", "Beschreibung", "Wert" and "Gewicht", each of which must have an InputField component attached.
        /// </summary>
        /// <param name="prefab">The prefab that should be examined.</param>
        /// <returns>Wether the GameObject is suitable.</returns>
        public static bool isValidInventoryItemPrefab(GameObject prefab)
        {
            if (prefab == null) return false;

            GameObject instance = Instantiate(prefab);
            ArrayList fieldList = new ArrayList(4);

            fieldList.Add(instance.transform.Find("Name"));
            fieldList.Add(instance.transform.Find("Beschreibung"));
            fieldList.Add(instance.transform.Find("Wert"));
            fieldList.Add(instance.transform.Find("Gewicht"));

            foreach (Transform field in fieldList)
            {
                if (field == null || field.GetComponent<InputField>() == null)
                {
                    return false;
                }
            }

            return true;
        }

        /// <param name="prefab">The prefab that should be used as the GameObject for newly created InventoryItems</param>
        /// <require>InventoryItem.isValidInventoryItemPrefab(prefab)</require>
        public void setInventoryItemPrefab(GameObject prefab)
        {
            InventoryItemPrefab = prefab;
        }

        private void assignInputFields()
        {
            _nameField = _itemObject.transform.Find("Name").GetComponent<InputField>();
            _descriptionField = _itemObject.transform.Find("Beschreibung").GetComponent<InputField>();
            _valueField = _itemObject.transform.Find("Wert").GetComponent<InputField>();
            _weightField = _itemObject.transform.Find("Gewicht").GetComponent<InputField>();
        }

        private void registerListeners()
        {
            _nameField.onEndEdit.AddListener(new UnityAction<string>(delegate (string inputText)
            {
                _name = inputText;
            }));

            _descriptionField.onEndEdit.AddListener(new UnityAction<string>(delegate (string inputText)
            {
                _description = inputText;
            }));

            _valueField.onEndEdit.AddListener(new UnityAction<string>(delegate (string inputText)
            {
                _value = (int)Math.Round(Convert.ToDecimal(inputText) * 100);
            }));

            _weightField.onEndEdit.AddListener(new UnityAction<string>(delegate (string inputText)
            {
                _weight = (int)Math.Round(Convert.ToDecimal(inputText) * 1000);
            }));
        }
    }
}