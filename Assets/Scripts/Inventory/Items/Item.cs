using Inventory.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;

namespace Inventory.Items
{
    class Item : AbstractItem
    {
        public int _Quantity
        {
            get
            {
                if ("".Equals(_quantityField.text))
                {
                    return 1;
                }
                return int.Parse(_quantityField.text);
            }

            private set
            {
                _quantityField.text = value.ToString();
            }
        }

        private InputField _quantityField;

        /// <summary>
        /// Initializes this Item with the given values.
        /// </summary>
        /// <param name="quantity">How many of this Item is in the inventory.</param>
        /// <param name="name">This Item's name.</param>
        /// <param name="level">This Item's item level.</param>
        /// <param name="description">This Item's description.</param>
        /// <param name="wiFiEnabled">Wether this Item's WiFi capabilities are enabled.</param>
        /// <param name="value">This Item's value in Nuyen.</param>
        /// <param name="legality">This Item's legality.</param>
        /// <param name="wiFiBonus">This Item's bonus from enabling WiFi.</param>
        /// <param name="matrixDamage">This Item's received damage.</param>
        /// <param name="matrixHealth">This Item's maximum health.</param>
        public void initialize(int quantity, string name, int? level, string description, bool wiFiEnabled, int? value, Legality legality, string wiFiBonus, int? matrixDamage, int? matrixHealth)
        {
            initialize(name, level, description, wiFiEnabled, value, legality, wiFiBonus, matrixDamage, matrixHealth);
            _Quantity = quantity;
        }

        new protected void assignInputFields()
        {
            base.assignInputFields();
            _quantityField = transform.FindChild("FirstLine").FindChild("Quantity").GetComponent<InputField>();
        }
    }
}
