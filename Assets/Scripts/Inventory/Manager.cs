using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using Inventory.Categories;

namespace Inventory
{
    public class Manager : MonoBehaviour
    {
        public GameObject _iItemCategoryPrefab; //The Prefab for IItemCategories.

        public Transform _itemContainer; //The parent of all Item GameObjects.
        public Transform _categoryContainer; //The parent of all GenericItemCategory GameObjects.

        #region ItemPrefabs
        public GameObject _itemPrefab; //The Prefab for Items.
        public GameObject _itemCategoryHeaderPrefab; //The Prefab for the Item header.
        #endregion

        #region SoftwarePrefabs
        public GameObject _softwarePrefab; //The Prefab for Items.
        public GameObject _softwareCategoryHeaderPrefab; //The Prefab for the Item header.
        #endregion

        private ICollection<IItemCategory> _categories;

        private ToggleGroup _categoryToggleGroup;

        void Start()
        {
            _categories = new HashSet<IItemCategory>();
            _categoryToggleGroup = gameObject.AddComponent<ToggleGroup>();
            _categoryToggleGroup.allowSwitchOff = true;

            setupItemCategory();
            setupSoftwareCategory();

            _categoryToggleGroup.SetAllTogglesOff();
        }

        /// <summary>
        /// Adds a new empty Item to the currently selected ItemCategory.
        /// If none is selected, nothing happens.
        /// If multiple ItemCategories are selected, all ItemCategories are unselected.
        /// </summary>
        public void addNewItemToSelectedCategory()
        {
            performActionOnSelectedCategory((IItemCategory selectedCategory) =>
            {
                selectedCategory.addNewItem();
            });
        }

        /// <summary>
        /// Deletes all selected Items in the currently selected ItemCategory.
        /// </summary>
        public void deleteSelectedItems()
        {
            performActionOnSelectedCategory((IItemCategory selectedCategory) =>
            {
                selectedCategory.deleteSelectedItems();
            });
        }

        /// <summary>
        /// Moves the selected ItemCategory down by 1, if there is room.
        /// </summary>
        public void moveSelectedCategoryDown()
        {
            performActionOnSelectedCategory((IItemCategory selectedCategory) =>
            {
                selectedCategory.moveDown();
            });
        }

        /// <summary>
        /// Moves the selected ItemCategory up by 1, if there is room.
        /// </summary>
        public void moveSelectedCategoryUp()
        {
            performActionOnSelectedCategory((IItemCategory selectedCategory) =>
            {
                selectedCategory.moveUp();
            });
        }

        /// <summary>
        /// Moves the selected Items in the currently selected ItemCategory down by 1, if there is room.
        /// </summary>
        public void moveSelectedItemsDown()
        {
            performActionOnSelectedCategory((IItemCategory selectedCategory) =>
            {
                selectedCategory.moveSelectedItemsDown();
            });
        }

        /// <summary>
        /// Moves the selected Items in the currently selected ItemCategory up by 1, if there is room.
        /// </summary>
        public void moveSelectedItemsUp()
        {
            performActionOnSelectedCategory((IItemCategory selectedCategory) =>
            {
                selectedCategory.moveSelectedItemsUp();
            });
        }

        private void performActionOnSelectedCategory(Action<IItemCategory> action)
        {
            IItemCategory selectedCategory = getSelectedCategory();
            if (selectedCategory != null)
            {
                action(selectedCategory);
            }
        }

        private IItemCategory getSelectedCategory()
        {
            List<IItemCategory> selectedCategories = new List<IItemCategory>();

            foreach (Toggle toggle in _categoryToggleGroup.ActiveToggles())
            {
                selectedCategories.Add(toggle.GetComponent<IItemCategory>());
            }

            switch (selectedCategories.Count)
            {
                case 1:
                    return selectedCategories[0];
                default:
                    _categoryToggleGroup.SetAllTogglesOff();
                    return null;
            }
        }

        private void setupItemCategory()
        {
            GameObject itemCategoryGameObject = (GameObject)Instantiate(_iItemCategoryPrefab, _categoryContainer);
            ItemCategory itemCategory = itemCategoryGameObject.AddComponent<ItemCategory>();
            itemCategory.initialize(_itemCategoryHeaderPrefab, _itemPrefab, _categoryToggleGroup, _itemContainer);
            _categories.Add(itemCategory);

            itemCategoryGameObject.transform.FindChild("Name").GetComponent<Text>().text = "Gegenstände";
        }

        private void setupSoftwareCategory()
        {
            GameObject softwareCategoryGameObject = (GameObject)Instantiate(_iItemCategoryPrefab, _categoryContainer);
            SoftwareCategory softwareCategory = softwareCategoryGameObject.AddComponent<SoftwareCategory>();
            softwareCategory.initialize(_softwareCategoryHeaderPrefab, _softwarePrefab, _categoryToggleGroup, _itemContainer);
            _categories.Add(softwareCategory);

            softwareCategoryGameObject.transform.FindChild("Name").GetComponent<Text>().text = "Software";
        }
    }
}
