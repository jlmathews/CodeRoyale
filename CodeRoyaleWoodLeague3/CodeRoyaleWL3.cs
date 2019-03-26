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

    public Unit(int x, int y, int type, int owner, int health)
    {
        this.pt = new Point(x, y);
        this.type = type;
        this.owner = owner;
        this.health = health;
    }

    public bool IsQueen()
    {
        return this.type == -1;
    }

    public bool IsKnight()
    {
        return this.type == 0;
    }

    public bool IsArcher()
    {
        return this.type == 1;
    }

    public bool IsFriendly()
    {
        return this.owner == 0;
    }

    public bool IsEnemy()
    {
        return this.owner == 1;
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
    public int owner;
    public int param1;
    public int param2;

    public void CreateSite(int siteId, int x, int y, int radius)
    {
        this.siteId = siteId;
        this.pt = new Point(x, y);
        this.radius = radius;
    }

    public void UpdateSite(int structureType, int owner, int param1, int param2)
    {
        this.structureType = structureType;
        this.owner = owner;
        this.param1 = param1;
        this.param2 = param2;
    }

    public bool IsNoStructure()
    {
        return this.structureType == -1;
    }

    public bool IsBarracks()
    {
        return this.structureType == 2;
    }

    public bool IsFriendly()
    {
        return this.owner == 0;
    }

    public bool IsEnemy()
    {
        return this.owner == 1;
    }

    public bool CanTrainBarracks()
    {
        return IsBarracks() && IsFriendly() && (this.param1 == 0);
    }

    public bool IsKnightBarracks()
    {
        return this.param2 == 0;
    }

    public bool IsArcherBarracks()
    {
        return this.param2 == 1;
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

    public void UpdateSite(int siteId, int structureType, int owner, int param1, int param2)
    {
        m_sites[siteId].UpdateSite(structureType, owner, param1, param2);
    }

    public delegate bool FindSiteDelegate(Site site);
    public delegate bool FindClosestUnitsDelegate(Site site, int gold);

    public int FindClosestSite(Unit unit, FindSiteDelegate moveDelegate)
    {
        int closestSite = -1;
        double closestSiteDistance = -1;

        foreach (KeyValuePair<int, Site> site in m_sites)
        {
            if(!moveDelegate.Invoke(site.Value))
            {
                continue;
            }
            double siteDistance = Point.Distance(unit.pt, site.Value.pt);
            if(closestSite == -1)
            {
                closestSite = site.Key;
                closestSiteDistance = siteDistance;
            }
            else
            {
                if(siteDistance < closestSiteDistance)
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
        if(closestSite != -1)
        {
            trainSites.Add(closestSite);
        }

        return trainSites;
    }

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

        int buildCount = 0;
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

            for (int i = 0; i < numSites; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int siteId = int.Parse(inputs[0]);
                int ignore1 = int.Parse(inputs[1]); // used in future leagues
                int ignore2 = int.Parse(inputs[2]); // used in future leagues
                int structureType = int.Parse(inputs[3]); // -1 = No structure, 2 = Barracks
                int owner = int.Parse(inputs[4]); // -1 = No structure, 0 = Friendly, 1 = Enemy
                int param1 = int.Parse(inputs[5]);
                int param2 = int.Parse(inputs[6]);
                sites.UpdateSite(siteId, structureType, owner, param1, param2);
                if(owner == 0)
                {
                    if(touchedSite == siteId)
                    {
                        if(previousBuild == siteId)
                        {
                            buildCount++;
                            previousBuild = -1;
                        }
                    }
                }
            }

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
                int unitType = int.Parse(inputs[3]); // -1 = QUEEN, 0 = KNIGHT, 1 = ARCHER
                int health = int.Parse(inputs[4]);
                if((owner == 0) && (unitType == -1))
                {
                    queen = new Unit(x, y, unitType, owner, health);
                }
                else if ((owner == 1) && (unitType == -1))
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
                (Point.Distance(queen.pt, closestEnemyUnit) < 400))
            {
                escapeDirection = new Point(0, 0);
                if(queen.pt.x > closestEnemyUnit.x)
                {
                    escapeDirection.x = 1920;
                }
                if(queen.pt.y > closestEnemyUnit.y)
                {
                    escapeDirection.y = 1000;
                }
            }
            //else if ((closestEnemyUnit != null) &&
            //    (Point.Distance(queen.pt, closestEnemyUnit) < 700))
            //{
            //    Point closestFriendlyArcher = units.FindClosestSite(queen,
            //        new Units.FindClosestUnitsDelegate(IsFriendlyArcher));
            //    if (closestFriendlyArcher != null)
            //    {
            //        escapeDirection = new Point(closestFriendlyArcher.x,
            //            closestFriendlyArcher.y);
            //    }
            //}
            else
            {
                closestSite = sites.FindClosestSite(queen,
                        new Sites.FindSiteDelegate(IsNoStructure));
                if (((buildCount + 1) % 3) == 0)
                {
                    buildType = "BARRACKS-ARCHER";
                }
                else
                {
                    buildType = "BARRACKS-KNIGHT";
                }
            }

            List<int> trainSites = null;
            if (((trainCount + 1) % 3) == 0)
            {
                trainSites = sites.FindClosestTrainUnits(gold, enemyQueen,
                    new Sites.FindClosestUnitsDelegate(CanTrainArchers));
            }
            else
            {
                trainSites = sites.FindClosestTrainUnits(gold, enemyQueen,
                    new Sites.FindClosestUnitsDelegate(CanTrainBarracks));
            }

            if(closestSite != -1)
            {
                Console.WriteLine($"BUILD {closestSite} {buildType}");
                previousBuild = closestSite;
            }
            else if(escapeDirection != null)
            {
                Console.WriteLine($"MOVE {escapeDirection.x} {escapeDirection.y}");
            }
            else
            {
                Console.WriteLine("WAIT");
            }

            string trainLine = "TRAIN";
            if(trainSites != null)
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
            if(escapeDirection != null)
            {
                Console.Error.WriteLine($"EscapeDirection: {escapeDirection}");
            }
        }
    }
}