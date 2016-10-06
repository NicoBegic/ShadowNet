using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace Inventory.Items
{
    public enum Legality
    {
        None, Restricted, Illegal
    }

    /// <summary>
    /// AbstractItem represents an item in the player's inventory.
    /// </summary>
    public class AbstractItem : MonoBehaviour
    {
        public bool _IsSelected
        {
            get
            {
                return _selectionToggle.isOn;
            }

            set
            {
                _selectionToggle.isOn = value;
            }
        }
        public string _Name
        {
            get
            {
                return _nameField.text;
            }

            private set
            {
                _nameField.text = value == null ? "" : value;
            }
        }
        public int? _Level
        {
            get
            {
                if ("".Equals(_levelField.text))
                {
                    return null;
                }
                return int.Parse(_levelField.text);
            }

            private set
            {
                _levelField.text = value == null ? "" : value.ToString();
            }
        }
        public string _Description
        {
            get
            {
                return _descriptionField.text;
            }

            private set
            {
                _descriptionField.text = value == null ? "" : value;
            }
        }
        public bool _WiFiEnabled
        {
            get
            {
                return _wiFiToggle.isOn;
            }

            private set
            {
                _wiFiToggle.isOn = value;
            }
        }
        public int? _Value
        {
            get
            {
                if ("".Equals(_valueField.text))
                {
                    return null;
                }
                return int.Parse(_valueField.text);
            }

            private set
            {
                _valueField.text = value == null ? "" : value.ToString();
            }
        } //_value is in Nuyen.
        public Legality _Legality
        {
            get
            {
                return (Legality)_legality.value;
            }

            private set
            {
                _legality.value = (int)value;
            }
        }
        public string _WiFiBonus
        {
            get
            {
                return _wiFiBonusField.text;
            }

            private set
            {
                _wiFiBonusField.text = value == null ? "" : value;
            }
        }
        public int? _MatrixDamage
        {
            get
            {
                if ("".Equals(_matrixDamageField.text))
                {
                    return null;
                }
                return int.Parse(_matrixDamageField.text);
            }

            private set
            {
                _matrixDamageField.text = value == null ? "" : value.ToString();
            }
        }
        public int? _MatrixHealth
        {
            get
            {
                if ("".Equals(_matrixHealthField.text))
                {
                    return null;
                }
                return int.Parse(_matrixHealthField.text);
            }

            private set
            {
                _matrixHealthField.text = value == null ? "" : value.ToString();
            }
        }

        private bool _isInitialized = false;

        private Dropdown _legality;
        private InputField _nameField, _levelField, _descriptionField, _valueField, _wiFiBonusField, _matrixDamageField, _matrixHealthField;
        private Toggle _selectionToggle, _wiFiToggle;

        /// <summary>
        /// Destroys the gameObject this AbstractItem is attached to.
        /// </summary>
        public void delete()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Makes this AbstractItems gameObject invisible.
        /// </summary>
        public void hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Initializes this AbstractItem.
        /// </summary>
        public void initialize()
        {
            if (_isInitialized)
            {
                return;
            }
            _isInitialized = true;

            assignDropdowns();
            assignInputFields();
            assignToggles();
        }

        /// <summary>
        /// Moves this AbstractItem down by 1, if there is space.
        /// </summary>
        /// <returns>If the AbstractItem was successfully moved.</returns>
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
        /// Moves this AbstractItem up by 1, if there is space.
        /// </summary>
        /// <returns>If the AbstractItem was successfully moved.</returns>
        public bool moveUp()
        {
            if (transform.GetSiblingIndex() <= 0)
            {
                return false;
            }

            transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
            return true;
        }

        /// <param name="parent">The Transform component of an Object that is supposed to be this AbstractItem's parent.</param>
        /// <require>parent != null</require>
        /// <require>this.transform != null</require>
        /// <ensure>transform.parent == parent</ensure>
        public void setParent(Transform parent)
        {
            transform.SetParent(parent);
        }

        /// <summary>
        /// Makes the AbstractItem visible.
        /// </summary>
        public void show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Initializes this AbstractItem with the given values.
        /// </summary>
        /// <param name="name">This AbstractItems name.</param>
        /// <param name="level">This AbstractItems item level.</param>
        /// <param name="description">This AbstractItems description.</param>
        /// <param name="wiFiEnabled">Wether this AbstractItems WiFi capabilities are enabled.</param>
        /// <param name="value">This AbstractItems value in Nuyen.</param>
        /// <param name="legality">This AbstractItems legality.</param>
        /// <param name="wiFiBonus">This AbstractItems bonus from enabling WiFi.</param>
        /// <param name="matrixDamage">This AbstractItems received damage.</param>
        /// <param name="matrixHealth">This AbstractItems maximum health.</param>
        protected void initialize(string name, int? level, string description, bool wiFiEnabled, int? value, Legality legality, string wiFiBonus, int? matrixDamage, int? matrixHealth)
        {
            initialize();
            _Name = name;
            _Level = level;
            _Description = description;
            _WiFiEnabled = wiFiEnabled;
            _Value = value;
            _Legality = legality;
            _WiFiBonus = wiFiBonus;
            _MatrixDamage = matrixDamage;
            _MatrixHealth = matrixHealth;
        }

        #region Initialization helper methods
        protected void assignDropdowns()
        {
            _legality = transform.FindChild("FirstLine").FindChild("Legality").GetComponent<Dropdown>();
        }

        protected void assignInputFields()
        {
            Transform firstLine = transform.FindChild("FirstLine");
            _nameField = firstLine.FindChild("Name").GetComponent<InputField>();
            _levelField = firstLine.FindChild("Level").GetComponent<InputField>();
            _descriptionField = firstLine.FindChild("Description").GetComponent<InputField>();
            _valueField = firstLine.FindChild("Value").GetComponent<InputField>();

            Transform secondLine = transform.FindChild("SecondLine");
            _wiFiBonusField = secondLine.FindChild("WiFiBonus").GetComponent<InputField>();
            _matrixDamageField = secondLine.FindChild("Damagemonitor").FindChild("Damage").GetComponent<InputField>();
            _matrixHealthField = secondLine.FindChild("Damagemonitor").FindChild("Health").GetComponent<InputField>();
        }

        protected void assignToggles()
        {
            _selectionToggle = transform.FindChild("FirstLine").FindChild("IsSelected").GetComponent<Toggle>();
            _wiFiToggle = transform.FindChild("FirstLine").FindChild("WiFiEnabled").GetComponent<Toggle>();
        }
        #endregion

        private void Start()
        {
            initialize();
        }

        /// <summary>
        /// To create a new AbstractItem, instantiate an AbstractItem prefab and set its parent.
        /// </summary>
        protected AbstractItem() { }
    }
}
