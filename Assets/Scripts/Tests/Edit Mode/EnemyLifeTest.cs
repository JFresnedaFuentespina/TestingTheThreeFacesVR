using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyLifeTest
{
    [UnityTest]
    public IEnumerator EnemyTakeDamage()
    {
        var enemyGO = new GameObject();
        var enemyLife = enemyGO.AddComponent<EnemyLife>();
        enemyLife.currentHp = 10f;
        enemyLife.Damage(2f);
        yield return null;
        Assert.AreEqual(8f, enemyLife.currentHp);
        Object.DestroyImmediate(enemyGO);
    }
}
