using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.UndressUtil.Plugin.UndressItem
{
    interface IUndressItem
    {
        Texture Icon { get;  }
        bool Active { get; }
        bool Available { get; }
        Color Color { get; }
        void Dress();
        void Undress();
        void Toggle();
        void Update();
        void Apply();
    }
}
