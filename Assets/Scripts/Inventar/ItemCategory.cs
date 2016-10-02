using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Inventory
{
    public abstract class ItemCategory : MonoBehaviour
    {
        public bool _IsSelected { get { return _isSelectedToggle.isOn; } }

        public GameObject _categoryHeader;

        private IList<Item> _items;

        private int _weight = 0;

        private Toggle _isSelectedToggle;

        void Start()
        {
            _items = new List<Item>();

            _isSelectedToggle = gameObject.GetComponentInChildren<Toggle>();
        }

        /// <summary>
        /// To create an ItemCategory, instantiate an ItemCategory prefab and set its parent.
        /// </summary>
        private ItemCategory() { }
    }
}
