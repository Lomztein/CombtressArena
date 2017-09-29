using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBalanceItem {

    float GetDamageValue();
    float GetHealthValue();
    string GetDamageType();
    string GetHealthType();

}
