using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;

namespace Inventory
{
    /// <summary>
    /// InventoryItem represents an item in the player's inventory.
    /// </summary>
    public class Item : MonoBehaviour
    {
        private const int VALUE_FRACTIONAL_DIGITS = 2;
        private const int WEIGHT_FRACTIONAL_DIGITS = 3;

        private static GameObject ItemPrefab;

        public int _weight; //_weight is in grams.

        private bool _isSelected, _isCarried;
        private string _name, _description;
        private int _value; //_value is in cents.

        private GameObject _itemObject;
        private Toggle _selectionToggle, _carryToggle;
        private InputField _nameField, _descriptionField, _valueField, _weightField;

        /// <summary>
        /// Creates a new InventoryItem with blank values.
        /// </summary>
        /// <require>InventoryItem.hasPrefab()</require>
        public Item()
        {
            _itemObject = Instantiate(ItemPrefab);
            assignInputFields();
            registerToggleListeners();
            registerInputFieldListeners();
        }

        /// <returns>Wether or not InventoryItem has been assigned a valid prefab.</returns>
        public static bool hasPrefab()
        {
            return isValidInventoryItemPrefab(ItemPrefab);
        }

        /// <summary>
        /// Examines wether a GameObject can be used as a prefab for InventoryItem.
        /// The GameObject must have four children named "Name", "Description", "Value" and "Weight", each of which must have an InputField component attached.
        /// </summary>
        /// <param name="prefab">The prefab that should be examined.</param>
        /// <returns>Wether the GameObject is suitable.</returns>
        public static bool isValidInventoryItemPrefab(GameObject prefab)
        {
            if (prefab == null) return false;
            GameObject instance = Instantiate(prefab);

            return containsProperToggles(instance) && containsProperInputFields(instance);
        }

        private static bool containsProperToggles(GameObject instance)
        {
            ArrayList toggleList = new ArrayList(2);

            toggleList.Add(instance.transform.Find("IsSelected"));
            toggleList.Add(instance.transform.Find("IsCarried"));

            foreach (Transform toggle in toggleList)
            {
                if (toggle == null || toggle.GetComponent<Toggle>() == null)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool containsProperInputFields(GameObject instance)
        {
            ArrayList fieldList = new ArrayList(4);

            fieldList.Add(instance.transform.Find("Name"));
            fieldList.Add(instance.transform.Find("Description"));
            fieldList.Add(instance.transform.Find("Value"));
            fieldList.Add(instance.transform.Find("Weight"));

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
        /// <ensure>ItemPrefab == prefab</ensure>
        public void setInventoryItemPrefab(GameObject prefab)
        {
            ItemPrefab = prefab;
        }

        /// <param name="parent">The Transform component of an Object that is supposed to be this Item's parent.</param>
        /// <require>parent != null</require>
        /// <ensure>_itemObject.transform.parent == parent</ensure>
        public void assignParent(Transform parent)
        {
            _itemObject.transform.parent = parent;
        }

        private void assignToggles()
        {
            _selectionToggle = _itemObject.transform.Find("IsSelected").GetComponent<Toggle>();
            _carryToggle = _itemObject.transform.Find("IsCarried").GetComponent<Toggle>();
        }

        private void assignInputFields()
        {
            _nameField = _itemObject.transform.Find("Name").GetComponent<InputField>();
            _descriptionField = _itemObject.transform.Find("Beschreibung").GetComponent<InputField>();
            _valueField = _itemObject.transform.Find("Wert").GetComponent<InputField>();
            _weightField = _itemObject.transform.Find("Gewicht").GetComponent<InputField>();
        }

        private void registerToggleListeners()
        {
            _selectionToggle.onValueChanged.AddListener(new UnityAction<bool>(delegate (bool value)
            {
                _isSelected = value;
            }));

            _carryToggle.onValueChanged.AddListener(new UnityAction<bool>(delegate (bool value)
            {
                _isCarried = value;
            }));
        }

        private void registerInputFieldListeners()
        {
            registerTextInputFields();
            registerNumberInputFields();
        }

        private void registerTextInputFields()
        {
            _nameField.onEndEdit.AddListener(new UnityAction<string>(delegate (string inputText)
            {
                _name = inputText;
            }));

            _descriptionField.onEndEdit.AddListener(new UnityAction<string>(delegate (string inputText)
            {
                _description = inputText;
            }));
        }

        private void registerNumberInputFields()
        {
            _valueField.onValidateInput += delegate (string inputText, int inputIndex, char inputChar)
            {
                return (InputParser.isValidInput(inputText, VALUE_FRACTIONAL_DIGITS)) ? inputChar : '\0';
            };

            _valueField.onEndEdit.AddListener(new UnityAction<string>(delegate (string inputText)
            {
                _value = InputParser.parseInput(inputText, VALUE_FRACTIONAL_DIGITS);
            }));

            _weightField.onValidateInput += delegate (string inputText, int inputIndex, char inputChar)
            {
                return (InputParser.isValidInput(inputText, WEIGHT_FRACTIONAL_DIGITS)) ? inputChar : '\0';
            };

            _weightField.onEndEdit.AddListener(new UnityAction<string>(delegate (string inputText)
            {
                _weight = InputParser.parseInput(inputText, WEIGHT_FRACTIONAL_DIGITS);
            }));
        }
    }
}
