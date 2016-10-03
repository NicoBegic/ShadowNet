using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections;

namespace Inventory
{
    /// <summary>
    /// Item represents an item in the player's inventory.
    /// </summary>
    public class Item : MonoBehaviour
    {
        public event EventHandler WeightChangedEvent;

        private const int VALUE_FRACTIONAL_DIGITS = 2;
        private const int WEIGHT_FRACTIONAL_DIGITS = 3;

        public string _Name { get { return _nameField.text; } }
        public string _Description { get { return _descriptionField.text; } }
        public bool _IsSelected { get { return _selectionToggle.isOn; } }
        public bool _IsCarried { get { return _carryToggle.isOn; } }
        public bool _currentWeightChangeAllowed = true;
        public DecimalNumber _Value { get { return DecimalNumber.GetValue(2, _valueField.text); } } //_value is in Nuyen.
        public DecimalNumber _Weight { get; private set; } //_weight is in Kilograms.

        private Toggle _selectionToggle, _carryToggle;
        private InputField _nameField, _descriptionField, _valueField, _weightField;

        /// <summary>
        /// Initializes the Item.
        /// </summary>
        void Start()
        {
            assignInputFields();
            registerNumberInputFields();
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
            _nameField.text = name;
            _descriptionField.text = description;
            _valueField.text = value.ToString();
            _carryToggle.isOn = isCarried;
            _weightField.text = weight.ToString();
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
            if (transform.GetSiblingIndex() <= 0)
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
            _descriptionField = transform.Find("Description").GetComponent<InputField>();
            _valueField = transform.Find("Value").GetComponent<InputField>();
            _weightField = transform.Find("Weight").GetComponent<InputField>();
        }

        private void registerNumberInputFields()
        {
            _valueField.onValidateInput += delegate (string inputText, int inputIndex, char inputChar)
            {
                string newText = inputText.Insert(inputIndex, inputChar.ToString());
                return (DecimalNumber.isValidInput(VALUE_FRACTIONAL_DIGITS, newText)) ? inputChar : '\0';
            };

            _weightField.onValidateInput += delegate (string inputText, int inputIndex, char inputChar)
            {
                string newText = inputText.Insert(inputIndex, inputChar.ToString());

                if (!DecimalNumber.isValidInput(WEIGHT_FRACTIONAL_DIGITS, newText))
                {
                    return '\0';
                }

                _Weight = DecimalNumber.GetValue(3, newText);
                weightChanged();

                if (_currentWeightChangeAllowed)
                {
                    return inputChar;
                }

                _Weight = DecimalNumber.GetValue(3, inputText);
                _currentWeightChangeAllowed = true;
                return '\0'; //TODO: Put error message here.
            };
        }
        #endregion

        private void weightChanged()
        {
            WeightChangedEvent(this, null);
        }

        /// <summary>
        /// To create a new Item, instantiate an Item prefab and set its parent.
        /// </summary>
        private Item() { }
    }
}
