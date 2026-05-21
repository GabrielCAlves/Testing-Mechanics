using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minimalist.SampleScenes
{
    public interface IHealable
    {
        public void ReceiveHeal(float heal);
    }
}