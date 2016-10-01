using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections;

namespace Inventory
{
    /// <summary>
    /// InventoryItem represents an item in the player's inventory.
    /// </summary>
    public class Item : MonoBehaviour
    {
        public event EventHandler WeightChangedEvent;

        private const int VALUE_FRACTIONAL_DIGITS = 2;
        private const int WEIGHT_FRACTIONAL_DIGITS = 3;

        public string _name { get; private set; }
        public string _description { get; private set; }
        public bool _isSelected { get; private set; }
        public bool _isCarried { get; private set; }
        public DecimalNumber _value { get; private set; } //_value is in nusen (1/100 nuyen).
        public DecimalNumber _weight { get; private set; } //_weight is in grams.

        public UnityEvent InstantiateHeader;

        private Toggle _selectionToggle, _carryToggle;
        private InputField _nameField, _descriptionField, _valueField, _weightField;

        /// <summary>
        /// Initializes the Item.
        /// </summary>
        void Start()
        {
            assignInputFields();
            registerToggleListeners();
            registerInputFieldListeners();
        }

        /// <summary>
        /// Destroys the gameObject this Item is attached to.
        /// </summary>
        public void delete()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Makes this Items gameObject invisible.
        /// </summary>
        public void hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Initializes an Item with the given values.
        /// </summary>
        /// <param name="name">The Item's name.</param>
        /// <param name="description">The Item's description.</param>
        /// <param name="value">The Item's value in nusen (1/100 nuyen).</param>
        /// <param name="isCarried">Wether this Item is carried or not.</param>
        /// <param name="weight">The Item's weight in grams.</param>
        public void initialize(string name, string description, DecimalNumber value, bool isCarried, DecimalNumber weight)
        {
            _name = name;
            _description = description;
            _value = value;
            _isCarried = isCarried;
            _weight = weight;
            displayNewValues();
        }

        /// <summary>
        /// Moves this Item down by 1, if there is space.
        /// </summary>
        /// <returns>If the Item was successfully moved.</returns>
        public bool moveDown()
        {
            if (transform.parent.childCount <= transform.GetSiblingIndex() + 1)
            {
                return false;
            }

            transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
            return true;
        }

        /// <summary>
        /// Moves this Item up by 1, if there is space.
        /// </summary>
        /// <returns>If the Item was successfully moved.</returns>
        public bool moveUp()
        {
            if (transform.parent.childCount <= 0)
            {
                return false;
            }

            transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
            return true;
        }

        /// <summary>
        /// Makes the Item visible.
        /// </summary>
        public void show()
        {
            gameObject.SetActive(true);
        }

        /// <param name="parent">The Transform component of an Object that is supposed to be this Item's parent.</param>
        /// <require>parent != null</require>
        /// <require>this.transform != null</require>
        /// <ensure>transform.parent == parent</ensure>
        public void setParent(Transform parent)
        {
            transform.parent = parent;
        }

        #region Constructor helper methods
        private void assignToggles()
        {
            _selectionToggle = transform.Find("IsSelected").GetComponent<Toggle>();
            _carryToggle = transform.Find("IsCarried").GetComponent<Toggle>();
        }

        private void assignInputFields()
        {
            _nameField = transform.Find("Name").GetComponent<InputField>();
            _descriptionField = transform.Find("Beschreibung").GetComponent<InputField>();
            _valueField = transform.Find("Wert").GetComponent<InputField>();
            _weightField = transform.Find("Gewicht").GetComponent<InputField>();
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
                return (DecimalNumber.isValidInput(VALUE_FRACTIONAL_DIGITS, inputText)) ? inputChar : '\0';
            };

            _valueField.onEndEdit.AddListener(new UnityAction<string>(delegate (string inputText)
            {
                _value = DecimalNumber.GetValue(VALUE_FRACTIONAL_DIGITS, inputText);
            }));

            _weightField.onValidateInput += delegate (string inputText, int inputIndex, char inputChar)
            {
                return (DecimalNumber.isValidInput(WEIGHT_FRACTIONAL_DIGITS, inputText)) ? inputChar : '\0';
            };

            _weightField.onEndEdit.AddListener(new UnityAction<string>(delegate (string inputText)
            {
                _weight = DecimalNumber.GetValue(WEIGHT_FRACTIONAL_DIGITS, inputText);
                weightChanged();
            }));
        }
        #endregion

        private void weightChanged()
        {
            WeightChangedEvent(this, null);
        }

        private void displayNewValues()
        {
            _nameField.text = _name;
            _descriptionField.text = _description;
            _valueField.text = _value.ToString();
            _carryToggle.isOn = _isCarried;
            _weightField.text = _weight.ToString();
        }
    }
}
