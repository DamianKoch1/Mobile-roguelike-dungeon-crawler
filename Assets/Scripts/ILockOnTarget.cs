using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILockOnTarget
{
    void OnLockedOff(Player player);
    void OnLockedOn(Player player);
}
