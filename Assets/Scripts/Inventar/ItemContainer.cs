using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Inventory
{
    public class ItemContainer : MonoBehaviour
    {
        private static GameObject ContainerPrefab;

        public bool _isSelected;

        private string _name;

        private GameObject _containerObject;

        private IList<Item> _items;

        private int _weight = 0;

        public ItemContainer()
        {
            _containerObject = Instantiate(ContainerPrefab);
        }
    }
}
