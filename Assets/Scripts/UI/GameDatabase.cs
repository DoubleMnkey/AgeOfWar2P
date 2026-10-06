using UnityEngine;

public static class GameDatabase
{
    public static readonly int[] MaxHealth =
    {
        0,
        500,
        1100,
        2000,
        3200,
        4700
    };

    public static readonly int[] EvolutionExp =
    {
        0,
        4000,
        14000,
        45000,
        200000
    };

    public static readonly int[] SlotCost =
    {
        2000,
        6000,
        15000
    };

    public static readonly int[,] UnitCost =
    {
        {15,25,100},
        {50,75,500},
        {200,400,1000},
        {1500,2000,7000},
        {5000,6000,20000}
    };

    public static readonly int[,] TurretCost =
    {
        {100,200,500},
        {500,750,1000},
        {1500,3000,6000},
        {7000,9000,14000},
        {24000,40000,100000}
    };

    public static readonly int[,] KillGoldReward =
    {
        {20,33,130},
        {65,98,650},
        {300,600,1500},
        {2250,3000,10500},
        {7500,9000,30000}
    };

    public static readonly int[,] KillExpReward =
    {
        {60,99,390},
        {195,294,1950},
        {780,1560,3900},
        {5850,7800,27300},
        {19500,23400,78000}
    };
}
