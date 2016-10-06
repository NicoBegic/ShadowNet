using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Inventory.Categories
{
    public interface IItemCategory
    {
        void addNewItem();

        void deleteSelectedItems();

        void moveDown();

        void moveUp();

        void moveSelectedItemsDown();

        void moveSelectedItemsUp();
    }
}
