using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Item
{
    class AttachmentItem:BaseItem
    {
        public enum AttachmentType
        {
            Scope,
            Grenade,
            Knife,
            Other
        }

        public AttachmentType CurrentAttachmentType;
    }
}
