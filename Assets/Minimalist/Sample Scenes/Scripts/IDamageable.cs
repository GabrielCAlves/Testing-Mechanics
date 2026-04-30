using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minimalist.SampleScenes
{
    public interface IDamageable
    {
        public void TakeDamage(float damage);
    }
}