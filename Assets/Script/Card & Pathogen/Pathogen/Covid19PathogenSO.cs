using UnityEngine;

[CreateAssetMenu(fileName = "Covid19", menuName = "Pathogen/Covid19")]
public class Covid19PathogenSO : PathogenSO
{
    void Awake()
    {
        pathogenName = "Covid-19";
        maxHitPoints = 50;
        attackPower = 8;
        attackInterval = 1;
    }
}