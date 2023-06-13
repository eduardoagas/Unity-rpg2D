using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    [SerializeField] GameObject health;
    public void setHp(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f);
    }

    public IEnumerator SetHpSmooth(float newHp){
        float curHp = health.transform.localScale.x;
        float changAmt = curHp - newHp;

        while (curHp - newHp > Mathf.Epsilon){
            curHp -= changAmt * Time.deltaTime;
            health.transform.localScale = new Vector3(curHp, 1f);
            yield return null;
        }
        
        health.transform.localScale = new Vector3(newHp, 1f);

    }
}
