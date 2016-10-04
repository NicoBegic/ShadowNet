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

        public bool _IsSelected
        {
            get
            {
                return _isSelectedToggle.isOn;
            }
        }
        public bool _currentWeightChangeAllowed { private get; set; }

        public DecimalNumber _TotalCarriedWeight
        {
            get
            {
                return _totalCarriedWeight;
            }
        }

        public Transform _itemContainer
        {
            private get
            {
                return _itemContainer;
            }

            set
            {
                setItemContainer(value);
            }
        }

        private IList<T> _items;

        private DecimalNumber _totalWeight = DecimalNumber.GetValue(3, 0);
        private DecimalNumber _totalCarriedWeight = DecimalNumber.GetValue(3, 0);

        private GameObject _categoryHeaderPrefab;
        private GameObject _ItemPrefab;
        private GameObject _categoryHeader;

        private Toggle _isSelectedToggle;

        void Start()
        {
            _items = new List<T>();
            _isSelectedToggle = GetComponent<Toggle>();

            registerCategoryToggle();
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
        public void initialize(GameObject headerPrefab, GameObject itemPrefab, Transform itemContainer)
        {
            if (headerPrefab == null) { throw new ArgumentNullException("headerPrefab"); }
            if (itemPrefab == null) { throw new ArgumentNullException("itemPrefab"); }
            if (itemContainer == null) { throw new ArgumentNullException("itemContainer"); }

            _categoryHeaderPrefab = headerPrefab;
            _ItemPrefab = itemPrefab;

            registerHeaderToggles();
            setItemContainer(itemContainer);
        }

        /// <param name="item">An Item that should be added to the bottom of this ItemCategory</param>
        /// 
        /// <require>item != null</require>
        public void addItem(T item)
        {
            if (item == null)
            {
                return;
            }

            if (!_totalWeight.canAdd(item._Weight))
            {
                return;
            }

            _items.Add(item);
            item.setParent(_itemContainer);
            if (!_isSelectedToggle.isOn)
            {
                item.hide();
            }
            updateWeight();
        }

        /// <param name="items">Items that should be added to the bottom of this Category.</param>
        /// 
        /// <require>items != null</require>
        public void addItems(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                if (item == null)
                {
                    return;
                }
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
                if (firstNonSelectedEncountered && item._IsSelected)
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

        private void registerWeightEvent(T item)
        {
            item.WeightChangedEvent += new EventHandler(onWeightChanged);
        }

        private void onWeightChanged(object sender, EventArgs e)
        {
            if (isTotalWeightValid())
            {
                DecimalNumber oldWeight = _totalWeight;
                updateWeight();
                WeightChangedEvent(this, null);

                if (_currentWeightChangeAllowed)
                {
                    return;
                }

                _totalWeight = oldWeight;
                _currentWeightChangeAllowed = true;
            }

            T item = (T)sender;
            item._currentWeightChangeAllowed = false;
        }

        private bool isTotalWeightValid()
        {
            DecimalNumber totalWeight = DecimalNumber.GetValue(3, 0);
            foreach (T item in _items)
            {
                if (!totalWeight.canAdd(item._Weight))
                {
                    return false;
                }
                totalWeight = totalWeight.plus(item._Weight);
            }
            return true;
        }

        private void updateWeight()
        {
            DecimalNumber totalCarriedWeight = DecimalNumber.GetValue(3, 0);
            DecimalNumber totalWeight = DecimalNumber.GetValue(3, 0);
            foreach (T item in _items)
            {
                if (item._IsSelected)
                {
                    totalCarriedWeight = totalCarriedWeight.plus(item._Weight);
                }
                totalWeight = totalWeight.plus(item._Weight);
            }
            _totalCarriedWeight = totalCarriedWeight;
            _totalWeight = totalWeight;
        }

        private void registerCategoryToggle()
        {
            _isSelectedToggle.onValueChanged.AddListener(delegate (bool isOn)
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

        private void registerHeaderToggles()
        {
            _categoryHeader.transform.Find("IsSelected").GetComponent<Toggle>().onValueChanged.AddListener(delegate (bool isOn)
            {
                foreach (T item in _items)
                {
                    item._IsSelected = isOn;
                }
            });

            _categoryHeader.transform.Find("IsCarried").GetComponent<Toggle>().onValueChanged.AddListener(delegate (bool isOn)
            {
                foreach (T item in _items)
                {
                    item._IsCarried = isOn;
                }
                updateWeight();
            });
        }

        private void setItemContainer(Transform container)
        {
            _itemContainer = container;

            _categoryHeader = (GameObject)Instantiate(_categoryHeaderPrefab, _itemContainer);
            _categoryHeader.SetActive(_isSelectedToggle.isOn);
            foreach (T item in _items)
            {
                item.setParent(_itemContainer);
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
        private ItemCategory() { }
    }
}
