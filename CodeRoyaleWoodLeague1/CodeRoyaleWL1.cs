using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;


public class Point
{
    public int x;
    public int y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return $"({x}, {y})";
    }

    public static double Distance(Point pt1, Point pt2)
    {
        double x1 = Math.Pow(pt2.x - pt1.x, 2);
        double y1 = Math.Pow(pt2.y - pt1.y, 2);
        double dist = Math.Sqrt(x1 + y1);

        return dist;
    }

}

public class Unit
{
    public Point pt;
    public int type;
    public int owner;
    public int health;

    // -1 = QUEEN, 0 = KNIGHT, 1 = ARCHER, 2 = GIANT
    enum UnitType
    {
        Queen = -1,
        Knight = 0,
        Archer = 1,
        Giant = 2
    };

    enum UnitOwner
    {
        NoStructure = -1,
        Friendly = 0,
        Enemy = 1
    };

    public Unit(int x, int y, int type, int owner, int health)
    {
        this.pt = new Point(x, y);
        this.type = type;
        this.owner = owner;
        this.health = health;
    }

    public bool IsQueen()
    {
        return this.type == (int)UnitType.Queen;
    }

    public bool IsKnight()
    {
        return this.type == (int)UnitType.Knight;
    }

    public bool IsArcher()
    {
        return this.type == (int)UnitType.Archer;
    }

    public bool IsGiant()
    {
        return this.type == (int)UnitType.Giant;
    }

    public static bool IsQueen(int type)
    {
        return type == (int)UnitType.Queen;
    }

    public static bool IsKnight(int type)
    {
        return type == (int)UnitType.Knight;
    }

    public static bool IsArcher(int type)
    {
        return type == (int)UnitType.Archer;
    }

    public static bool IsGiant(int type)
    {
        return type == (int)UnitType.Giant;
    }

    public bool IsNoStructure()
    {
        return this.owner == (int)UnitOwner.NoStructure;
    }

    public bool IsFriendly()
    {
        return this.owner == (int)UnitOwner.Friendly;
    }

    public bool IsEnemy()
    {
        return this.owner == (int)UnitOwner.Enemy;
    }

    public static bool IsNoStructure(int owner)
    {
        return owner == (int)UnitOwner.NoStructure;
    }

    public static bool IsFriendly(int owner)
    {
        return owner == (int)UnitOwner.Friendly;
    }

    public static bool IsEnemy(int owner)
    {
        return owner == (int)UnitOwner.Enemy;
    }
}

public class Units
{
    public List<Unit> units;

    public Units(int numUnits)
    {
        units = new List<Unit>(numUnits);
    }

    public void createUnits(int x, int y, int type, int owner, int health)
    {
        Unit unit = new Unit(x, y, type, owner, health);
        units.Add(unit);
    }

    public delegate bool FindClosestUnitsDelegate(Unit unit);

    public Point FindClosestSite(Unit unit,
        FindClosestUnitsDelegate unitDelegate)
    {
        Point closestSite = null;
        double closestSiteDistance = -1;

        foreach (Unit m_unit in units)
        {
            if (!unitDelegate.Invoke(m_unit))
            {
                continue;
            }
            double siteDistance = Point.Distance(unit.pt, m_unit.pt);
            if (closestSite == null)
            {
                closestSite = new Point(m_unit.pt.x, m_unit.pt.y);
                closestSiteDistance = siteDistance;
            }
            else
            {
                if (siteDistance < closestSiteDistance)
                {
                    closestSite.x = m_unit.pt.x;
                    closestSite.y = m_unit.pt.y;
                    closestSiteDistance = siteDistance;
                }
            }
        }

        return closestSite;
    }
}

public class Site
{
    public int siteId;
    public Point pt;
    public int radius;
    public int structureType;
    public int goldRemaining;
    public int maxMineSize;
    public int owner;
    public int param1;
    public int param2;

    public int buildCount = 0;
    public int updateCount = 0;

    enum StructureType
    {
        NoStructure = -1,
        Goldmine = 0,
        Tower = 1,
        Barracks = 2
    };

    enum BarracksType
    {
        Knight = 0,
        Archer = 1,
        Giant = 2
    };

    public void CreateSite(int siteId, int x, int y, int radius)
    {
        this.siteId = siteId;
        this.pt = new Point(x, y);
        this.radius = radius;
    }

    public void UpdateSite(int structureType, int goldRemaining,
        int maxMineSize, int owner, int param1, int param2)
    {
        this.structureType = structureType;
        this.goldRemaining = goldRemaining;
        this.maxMineSize = maxMineSize;
        this.owner = owner;
        this.updateCount++;
        if (IsTower())
        {
            if((this.buildCount > 0) && ((this.updateCount + 1) % 10 == 0))
            {
                this.buildCount--;
            }
            if(param1 > this.param1)
            {
                this.buildCount++;
            }
        }
        else if(IsNoStructure())
        {
            this.buildCount = 0;
            this.updateCount = 0;
            this.param1 = 0;
            this.param2 = 0;
        }
        this.param1 = param1;
        this.param2 = param2;
    }

    public bool IsNoStructure()
    {
        return this.structureType == (int)StructureType.NoStructure;
    }

    public bool IsGoldmine()
    {
        return this.structureType == (int)StructureType.Goldmine;
    }

    public bool IsTower()
    {
        return this.structureType == (int)StructureType.Tower;
    }

    public bool IsBarracks()
    {
        return this.structureType == (int)StructureType.Barracks;
    }

    public bool IsFriendly()
    {
        return this.owner == 0;
    }

    public bool IsEnemy()
    {
        return this.owner == 1;
    }

    public int GoldIncomeRate()
    {
        if(IsGoldmine())
        {
            return this.param1;
        }

        return -1;
    }

    public int TowerHealth()
    {
        if (IsTower())
        {
            return this.param1;
        }

        return -1;
    }

    public bool CanTrainBarracks()
    {
        // if param1 is the number of turns until training can start
        return IsBarracks() && IsFriendly() && (this.param1 == 0);
    }

    public bool IsKnightBarracks()
    {
        return this.IsBarracks() && (this.param2 == (int)BarracksType.Knight);
    }

    public bool IsArcherBarracks()
    {
        return this.IsBarracks() && (this.param2 == (int)BarracksType.Archer);
    }

    public bool IsGiantBarracks()
    {
        return this.IsBarracks() && (this.param2 == (int)BarracksType.Giant);
    }
}

public class Sites
{
    public Dictionary<int, Site> m_sites;

    public Sites(int numSites)
    {
        m_sites = new Dictionary<int, Site>(numSites);
    }

    public void CreateSite(int siteId, int x, int y, int radius)
    {
        Site site = new Site();
        site.CreateSite(siteId, x, y, radius);
        m_sites.Add(siteId, site);
    }

    public void UpdateSite(int siteId, int structureType,
        int goldRemaining, int maxMineSize, int owner,
        int param1, int param2)
    {
        m_sites[siteId].UpdateSite(structureType, goldRemaining,
            maxMineSize, owner, param1, param2);
    }

    public delegate bool FindSiteDelegate(Site site);
    public delegate bool FindClosestUnitsDelegate(Site site, int gold);

    public int FindClosestSite(Unit unit, FindSiteDelegate moveDelegate)
    {
        int closestSite = -1;
        double closestSiteDistance = -1;

        foreach (KeyValuePair<int, Site> site in m_sites)
        {
            if (!moveDelegate.Invoke(site.Value))
            {
                continue;
            }
            double siteDistance = Point.Distance(unit.pt, site.Value.pt);
            if (closestSite == -1)
            {
                closestSite = site.Key;
                closestSiteDistance = siteDistance;
            }
            else
            {
                if (siteDistance < closestSiteDistance)
                {
                    closestSite = site.Key;
                    closestSiteDistance = siteDistance;
                }
            }
        }

        return closestSite;
    }

    public List<int> FindClosestTrainUnits(int gold, Unit unit,
        FindClosestUnitsDelegate trainSiteDelegate)
    {
        List<int> trainSites = new List<int>();
        int closestSite = -1;
        double closestSiteDistance = -1;

        foreach (KeyValuePair<int, Site> site in m_sites)
        {
            if (!trainSiteDelegate.Invoke(site.Value, gold))
            {
                // Skip no structure and enemy structures
                continue;
            }
            double siteDistance = Point.Distance(unit.pt, site.Value.pt);
            if (closestSite == -1)
            {
                closestSite = site.Key;
                closestSiteDistance = siteDistance;
            }
            else
            {
                if (siteDistance < closestSiteDistance)
                {
                    closestSite = site.Key;
                    closestSiteDistance = siteDistance;
                }
            }
        }
        if (closestSite != -1)
        {
            trainSites.Add(closestSite);
        }

        return trainSites;
    }

    public int GetTowerBuildCount()
    {
        int buildCount = 0;
        foreach (KeyValuePair<int, Site> site in m_sites)
        {
            if(site.Value.IsTower())
            {
                buildCount += site.Value.buildCount;
            }
        }

        return buildCount;
    }

}

class Train
{
    public int trainCount = 0;

    public Train()
    {

    }

    //public int SelectTrainUnits(Sites sites)
    //{
    //    List<int> trainSites = null;
    //    if (((trainCount + 1) % 3) == 0)
    //    {
    //        trainSites = sites.FindClosestTrainUnits(gold, enemyQueen,
    //            new Sites.FindClosestUnitsDelegate(CanTrainArchers));
    //    }
    //    else
    //    {
    //        trainSites = sites.FindClosestTrainUnits(gold, enemyQueen,
    //            new Sites.FindClosestUnitsDelegate(CanTrainBarracks));
    //    }
    //}
}

class Player
{
    public static bool CanTrainBarracks(Site site, int gold)
    {
        return site.CanTrainBarracks() && site.IsKnightBarracks() && (gold >= 80);
    }

    public static bool CanTrainArchers(Site site, int gold)
    {
        return site.CanTrainBarracks() && site.IsArcherBarracks() && (gold >= 100);
    }

    public static bool IsNoStructure(Site site)
    {
        return site.IsNoStructure();
    }

    public static bool IsFriendlyMineBuilding(Site site)
    {
        return site.IsNoStructure() || (site.IsFriendly() && site.IsGoldmine() && (site.GoldIncomeRate() < site.maxMineSize));
    }

    public static bool IsFriendlyTowerBuilding(Site site)
    {
        return site.IsNoStructure() || (site.IsFriendly() && site.IsTower() && (site.TowerHealth() < 700));
    }

    public static bool IsFriendlyStructure(Site site)
    {
        return site.IsFriendly() && (site.IsBarracks() || site.IsTower());
    }

    public static bool IsEnemyKnight(Unit unit)
    {
        return unit.IsKnight() && unit.IsEnemy();
    }

    public static bool IsFriendlyArcher(Unit unit)
    {
        return unit.IsArcher() && unit.IsFriendly();
    }

    static void Main(string[] args)
    {
        string[] inputs;
        int numSites = int.Parse(Console.ReadLine());
        Sites sites = new Sites(numSites);

        int trainCount = 0;
        int previousBuild = -1;

        for (int i = 0; i < numSites; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            int siteId = int.Parse(inputs[0]);
            int x = int.Parse(inputs[1]);
            int y = int.Parse(inputs[2]);
            int radius = int.Parse(inputs[3]);
            sites.CreateSite(siteId, x, y, radius);
        }

        // game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            int gold = int.Parse(inputs[0]);
            int touchedSite = int.Parse(inputs[1]); // -1 if none
            int buildCount = 0;
            int goldBuildingCount = 0;
            int towerBuildingCount = 0;
            int knightBuildingCount = 0;

            for (int i = 0; i < numSites; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int siteId = int.Parse(inputs[0]);
                int goldRemaining = int.Parse(inputs[1]); // -1 if unknown
                int maxMineSize = int.Parse(inputs[2]); // -1 if unknown
                int structureType = int.Parse(inputs[3]); // -1 = No structure, 0 = Goldmine, 1 = Tower, 2 = Barracks
                int owner = int.Parse(inputs[4]); // -1 = No structure, 0 = Friendly, 1 = Enemy
                int param1 = int.Parse(inputs[5]);
                int param2 = int.Parse(inputs[6]);
                sites.UpdateSite(siteId, structureType, goldRemaining,
                    maxMineSize, owner, param1, param2);
                if (owner == 0)
                {
                    buildCount++;
                    if (structureType == 0)
                    {
                        goldBuildingCount += param1;
                    }
                    else if (structureType == 2)
                    {
                        knightBuildingCount++;
                    }
                    if (touchedSite == siteId)
                    {
                        if (previousBuild == siteId)
                        {
                            previousBuild = -1;
                        }
                    }
                }
            }

            towerBuildingCount = sites.GetTowerBuildCount();

            int numUnits = int.Parse(Console.ReadLine());
            Unit queen = null;
            Unit enemyQueen = null;
            Units units = new Units(numUnits);

            for (int i = 0; i < numUnits; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int x = int.Parse(inputs[0]);
                int y = int.Parse(inputs[1]);
                int owner = int.Parse(inputs[2]);
                int unitType = int.Parse(inputs[3]); // -1 = QUEEN, 0 = KNIGHT, 1 = ARCHER, 2 = GIANT
                int health = int.Parse(inputs[4]);
                if (Unit.IsFriendly(owner) && Unit.IsQueen(unitType))
                {
                    queen = new Unit(x, y, unitType, owner, health);
                }
                else if (Unit.IsEnemy(owner) && Unit.IsQueen(unitType))
                {
                    enemyQueen = new Unit(x, y, unitType, owner, health);
                }
                else
                {
                    units.createUnits(x, y, unitType, owner, health);
                }
            }

            Point closestEnemyUnit = units.FindClosestSite(queen,
                new Units.FindClosestUnitsDelegate(IsEnemyKnight));
            Point escapeDirection = null;
            int closestSite = -1;
            string buildType = "";

            if ((closestEnemyUnit != null) &&
                (Point.Distance(queen.pt, closestEnemyUnit) < 100))
            {
                escapeDirection = new Point(0, 0);
                if (queen.pt.x > closestEnemyUnit.x)
                {
                    escapeDirection.x = 1920;
                }
                if (queen.pt.y > closestEnemyUnit.y)
                {
                    escapeDirection.y = 1000;
                }
            }
            else
            {
                Sites.FindSiteDelegate findSiteDelegate;
                if(knightBuildingCount == 0)
                {
                    buildType = "BARRACKS-KNIGHT";
                    findSiteDelegate = new Sites.FindSiteDelegate(IsNoStructure);
                }
                else if(goldBuildingCount <= towerBuildingCount)
                {
                    buildType = "MINE";
                    findSiteDelegate = new Sites.FindSiteDelegate(IsFriendlyMineBuilding);
                }
                else
                {
                    buildType = "TOWER";
                    findSiteDelegate = new Sites.FindSiteDelegate(IsFriendlyTowerBuilding);
                }
                //else
                //{
                //    switch (buildCount % 2)
                //    {
                //        case 0:
                //            buildType = "TOWER";
                //            findSiteDelegate = new Sites.FindSiteDelegate(IsFriendlyTowerBuilding);
                //            break;
                //        case 1:
                //            buildType = "MINE";
                //            findSiteDelegate = new Sites.FindSiteDelegate(IsFriendlyMineBuilding);
                //            break;
                //        default:
                //            Console.Error.WriteLine($"Should not hit. BuildCount: {buildCount}");
                //            findSiteDelegate = new Sites.FindSiteDelegate(IsNoStructure);
                //            break;
                //    }
                //}
                closestSite = sites.FindClosestSite(queen, findSiteDelegate);
            }

            List<int> trainSites = null;
            //if (((trainCount + 1) % 3) == 0)
            //{
            //    trainSites = sites.FindClosestTrainUnits(gold, enemyQueen,
            //        new Sites.FindClosestUnitsDelegate(CanTrainArchers));
            //}
            //else
            {
                trainSites = sites.FindClosestTrainUnits(gold, enemyQueen,
                    new Sites.FindClosestUnitsDelegate(CanTrainBarracks));
            }

            if (closestSite != -1)
            {
                Console.WriteLine($"BUILD {closestSite} {buildType}");
                previousBuild = closestSite;
            }
            else if (escapeDirection != null)
            {
                Console.WriteLine($"MOVE {escapeDirection.x} {escapeDirection.y}");
            }
            else
            {
                Console.WriteLine("WAIT");
            }

            string trainLine = "TRAIN";
            if (trainSites != null)
            {
                foreach (int trainSite in trainSites)
                {
                    trainLine += $" {trainSite}";
                    trainCount++;
                }
            }
            Console.WriteLine(trainLine);
            Console.Error.WriteLine($"BuildCount: {buildCount}");
            Console.Error.WriteLine($"TrainCount: {trainCount}");
            Console.Error.WriteLine($"Gold: {gold}");
            Console.Error.WriteLine($"QueenPosition: {queen.pt}");
            if (closestEnemyUnit != null)
            {
                Console.Error.WriteLine($"EnemyUnit: {closestEnemyUnit}");
            }
            if (escapeDirection != null)
            {
                Console.Error.WriteLine($"EscapeDirection: {escapeDirection}");
            }
        }
    }
}