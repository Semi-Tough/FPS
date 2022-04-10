using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Item
{
    class  FirearmsItem:BaseItem 
    {
        public enum FirearmsType
        {
            AssaultRifle,
            HandGun,
        }

        public FirearmsType CurrentFirearmsType;
        public string ArmsName;
    }
}
