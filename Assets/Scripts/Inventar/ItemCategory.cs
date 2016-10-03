using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace Inventory
{
    /// <summary>
    /// An ItemCategory contains and manages a list of Items.
    /// </summary>
    /// <typeparam name="T">The Type of Item this ItemCategory contains.</typeparam>
    public class ItemCategory<T> : MonoBehaviour where T : Item
    {
        public event EventHandler WeightChangedEvent;

        public bool _IsSelected { get { return _isSelectedToggle.isOn; } }
        public bool _currentWeightChangeAllowed;

        public DecimalNumber _Weight { get { return _weight; } }

        public GameObject _categoryHeaderPrefab;
        public GameObject _ItemPrefab;

        private IList<T> _items;

        private DecimalNumber _weight = DecimalNumber.GetValue(3, 0);

        private GameObject _categoryHeader;

        private Toggle _isSelectedToggle;

        private Transform _itemContainer;

        void Start()
        {
            _items = new List<T>();

            _isSelectedToggle = gameObject.GetComponentInChildren<Toggle>();
        }

        /// <param name="item">An Item that should be added to the bottom of this ItemCategory</param>
        public void addItem(T item)
        {
            _items.Add(item);
            item.setParent(_itemContainer);
        }

        /// <param name="items">Items that should be added to the bottom of this Category.</param>
        public void addItems(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                addItem(item);
            }
        }

        /// <summary>
        /// Adds an empty Item to the bottom of this Category.
        /// </summary>
        public void addNewItem()
        {
            addItem(Instantiate(_ItemPrefab).GetComponent<T>());
        }

        /// <param name="item">The Item that should be looked for.</param>
        /// <returns>Wether this ItemCategory contains the given Item.</returns>
        public bool contains(T item)
        {
            return _items.Contains(item);
        }

        /// <summary>
        /// Moves this ItemCategory down by 1, if there is room.
        /// </summary>
        public void moveDown()
        {
            if (transform.parent.childCount <= (transform.GetSiblingIndex() + 1))
            {
                return;
            }
            transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
        }

        /// <summary>
        /// Moves this ItemCategory up by 1, if there is room.
        /// </summary>
        public void moveUp()
        {
            if (transform.GetSiblingIndex() <= 0)
            {
                return;
            }
            transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
        }

        /// <summary>
        /// Moves the selected Items in this ItemCategory down by 1, if there is room.
        /// </summary>
        public void moveSelectedItemsDown()
        {
            bool firstNonSelectedEncountered = false;
            for (int i = (_items.Count - 1); i >= 0; i--)
            {
                T item = _items[i];
                if (firstNonSelectedEncountered)
                {
                    item.moveDown();

                    _items.RemoveAt(i);
                    _items.Insert(i + 1, item);
                }
                firstNonSelectedEncountered = !item._IsSelected;
            }
        }

        /// <summary>
        /// Moves the selected Items in this ItemCategory down by 1, if there is room.
        /// </summary>
        public void moveSelectedItemsUp()
        {
            foreach (T item in _items)
            {
                if (item._IsSelected && item.moveUp())
                {
                    int newindex = _items.IndexOf(item) - 1;
                    _items.Remove(item);
                    _items.Insert(newindex, item);
                }
            }
            _categoryHeader.transform.SetAsFirstSibling();
        }

        /// <summary>
        /// Deletes all selected Items.
        /// </summary>
        public void deleteSelectedItems()
        {
            foreach (T item in _items)
            {
                if (item._IsSelected)
                {
                    item.delete();
                }
            }
        }

        /// <param name="container">The GameObject that should be the parent of the Items in this ItemCategory.</param>
        public void setItemContainer(Transform container)
        {
            if (_categoryHeader == null)
            {
                _categoryHeader = Instantiate(_categoryHeaderPrefab);
            }

            _itemContainer = container;

            _categoryHeader.transform.SetParent(_itemContainer);
            foreach (T item in _items)
            {
                item.setParent(_itemContainer);
            }
        }

        private void registerWeightEvent(T item)
        {
            item.WeightChangedEvent += new EventHandler(onWeightChanged);
        }

        private void onWeightChanged(object sender, EventArgs e)
        {
            DecimalNumber oldWeight = _Weight;
            updateTotalWeight();
            WeightChangedEvent(this, null);

            if (_currentWeightChangeAllowed)
            {
                return;
            }

            _weight = oldWeight;
            T item = (T)sender;
            item._currentWeightChangeAllowed = false;
            _currentWeightChangeAllowed = true;
        }

        private void updateTotalWeight()
        {
            DecimalNumber totalWeight = DecimalNumber.GetValue(3, 0);
            foreach (T item in _items)
            {
                totalWeight = totalWeight.plus(item._Weight);
            }
            _weight = totalWeight;
        }

        /// <summary>
        /// To create an ItemCategory, instantiate an ItemCategory prefab and set its parent.
        /// </summary>
        private ItemCategory() { }
    }
}
