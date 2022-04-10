using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Item
{
   public abstract class BaseItem:MonoBehaviour
    {
        public enum ItemType
        {
            Firearms,
            Attachment,
            Throwing
        }

        public ItemType CurrentItemType;
        public int ItemId;
        public String ItemName;
    }
}
