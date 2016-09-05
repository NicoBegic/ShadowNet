using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Inventory
{
    public class Container : MonoBehaviour
    {
        private string _name;

        private GameObject _containerObject;

        private IList<Item> _items;
    }
}