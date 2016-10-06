using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using Inventory.Items;

namespace Inventory.Categories
{
    /// <summary>
    /// An ItemCategory contains and manages a list of Items.
    /// </summary>
    /// <typeparam name="T">The Type of Item this ItemCategory contains.</typeparam>
    public class AbstractItemCategory<T> : MonoBehaviour, IItemCategory where T : AbstractItem
    {
        public bool _IsSelected
        {
            get
            {
                return _isSelectedToggle.isOn;
            }
        } //Wether this ItemCategory is currently selected.

        public Transform _ItemContainer
        {
            private get
            {
                return _itemContainer;
            }

            set
            {
                moveItemsToContainer(value);
                _itemContainer = value;
            }
        } //The parent of this ItemCategory's Items.

        private IList<T> _items = new List<T>();

        private GameObject _categoryHeaderPrefab;
        private GameObject _itemPrefab;
        private GameObject _categoryHeader;

        private Transform _itemContainer;

        private Toggle _isSelectedToggle;

        /// <param name="item">An Item that should be added to the bottom of this ItemCategory</param>
        /// 
        /// <require>item != null</require>
        public void addItem(T item)
        {
            if (item == null)
            {
                return;
            }

            _items.Add(item);
            item.setParent(_ItemContainer);

            if (_isSelectedToggle.isOn)
            {
                item.show();
            }
            else
            {
                item.hide();
            }
        }

        /// <param name="items">Items that should be added to the bottom of this Category.</param>
        /// 
        /// <require>items != null</require>
        public void addItems(IEnumerable<T> items)
        {
            if (items == null)
            {
                return;
            }
            foreach (T item in items)
            {
                if (item != null)
                {
                    addItem(item);
                }
            }
        }

        /// <summary>
        /// Adds an empty Item to the bottom of this Category.
        /// </summary>
        public void addNewItem()
        {
            T item = Instantiate(_itemPrefab).GetComponent<T>();
            item.initialize();
            addItem(item);
        }

        /// <summary>
        /// Deletes all selected Items.
        /// </summary>
        public void deleteSelectedItems()
        {
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                T item = _items[i];
                if (item._IsSelected)
                {
                    item.delete();
                    _items.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Initializes the ItemCategory to be ready for use.
        /// </summary>
        /// <param name="headerPrefab">The prefab for this ItemCategory's header.</param>
        /// <param name="itemPrefab">The prefab for an empty Item in this ItemCategory.</param>
        /// <param name="itemContainer">The Transform of the parent of this ItemCategory's Items.</param>
        /// 
        /// <require>headerPrefab != null</require>
        /// <require>itemPrefab != null</require>
        /// <require>itemContainer != null</require>
        public void initialize(GameObject headerPrefab, GameObject itemPrefab, ToggleGroup categoryToggleGroup, Transform itemContainer)
        {
            if (headerPrefab == null) { throw new ArgumentNullException("headerPrefab"); }
            if (itemPrefab == null) { throw new ArgumentNullException("itemPrefab"); }
            if (itemContainer == null) { throw new ArgumentNullException("itemContainer"); }

            _categoryHeaderPrefab = headerPrefab;
            _itemPrefab = itemPrefab;

            _isSelectedToggle = transform.GetComponent<Toggle>();
            if (categoryToggleGroup != null)
            {
                categoryToggleGroup.RegisterToggle(_isSelectedToggle);
                _isSelectedToggle.group = categoryToggleGroup;
            }

            _ItemContainer = itemContainer;
            _categoryHeader = (GameObject)Instantiate(_categoryHeaderPrefab, _ItemContainer);
            _categoryHeader.SetActive(_isSelectedToggle.isOn);

            registerHeaderToggles();
            registerCategoryToggle();
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
            int index = _items.Count - 1;
            while (index >= 0 && _items[index]._IsSelected)
            {
                index--;
            }

            index = (index == _items.Count) ? index-- : index;

            for (int i = index; i >= 0; i--)
            {
                T item = _items[i];
                if (item._IsSelected)
                {
                    item.moveDown();

                    _items.RemoveAt(i);
                    _items.Insert(i + 1, item);
                }
            }
        }

        /// <summary>
        /// Moves the selected Items in this ItemCategory down by 1, if there is room.
        /// </summary>
        public void moveSelectedItemsUp()
        {
            int index = 0;
            while (index < _items.Count && _items[index]._IsSelected)
            {
                index++;
            }

            index = (index == 0) ? 1 : index;

            for (int i = index; i < _items.Count; i++)
            {
                T item = _items[i];
                if (item._IsSelected)
                {
                    item.moveUp();

                    _items.RemoveAt(i);
                    _items.Insert(i - 1, item);
                }
            }

            _categoryHeader.transform.SetAsFirstSibling();
        }

        private void registerHeaderToggles()
        {
            _categoryHeader.transform.FindChild("FirstLine").FindChild("IsSelected").GetComponent<Toggle>().onValueChanged.AddListener((bool isOn) =>
            {
                foreach (T item in _items)
                {
                    item._IsSelected = isOn;
                }
            });
        }

        private void registerCategoryToggle()
        {
            _isSelectedToggle.onValueChanged.AddListener((bool isOn) =>
            {
                if (isOn)
                {
                    foreach (T item in _items)
                    {
                        item.show();
                    }
                }
                else
                {
                    foreach (T item in _items)
                    {
                        item.hide();
                    }
                }
                _categoryHeader.SetActive(isOn);
            });
        }

        private void moveItemsToContainer(Transform container)
        {
            foreach (T item in _items)
            {
                item.setParent(_ItemContainer);
                if (_isSelectedToggle.isOn)
                {
                    item.show();
                }
                else
                {
                    item.hide();
                }
            }
        }

        /// <summary>
        /// To create an ItemCategory, instantiate an ItemCategory prefab, add an ItemCategory component and initialize it.
        /// </summary>
        protected AbstractItemCategory() { }
    }
}
