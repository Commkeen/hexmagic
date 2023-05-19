using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HexCoordinates
{
    public int X {get; private set;}
    public int Y {get {return -X - Z;}}
    public int Z {get; private set;}

    public HexCoordinates(int x, int z)
    {
        X = x;
        Z = z;
    }

    public bool Equals(HexCoordinates o) => X == o.X && Z == o.Z;
    public override bool Equals(object o) => o is HexCoordinates other && this.Equals(other);
    public override int GetHashCode() => (X,Z).GetHashCode();
    public static bool operator ==(HexCoordinates lhs, HexCoordinates rhs) => lhs.Equals(rhs);
    public static bool operator !=(HexCoordinates lhs, HexCoordinates rhs) => !(lhs == rhs);

    public Vector3 ToWorldCoordinates()
    {
        var x = (X + Z*0.5f + (Z % 1)*0.5f) * HexMetrics.innerRadius * 2;
        var y = 0f;
        var z = Z * HexMetrics.outerRadius * 1.5f;
        return new Vector3(x,y,z);
    }

    public HexCoordinates GetNeighbor(HexDirection direction)
    {
        switch (direction)
        {
            case HexDirection.NE:
                return new HexCoordinates(X, Z+1);
            case HexDirection.E:
                return new HexCoordinates(X+1, Z);
            case HexDirection.SE:
                return new HexCoordinates(X+1, Z-1);
            case HexDirection.SW:
                return new HexCoordinates(X, Z-1);
            case HexDirection.W:
                return new HexCoordinates(X-1, Z);
            case HexDirection.NW:
                return new HexCoordinates(X-1, Z+1);
            default:
                return this;
        }
    }

    public List<HexCoordinates> GetAllNeighbors()
    {
        var results = new List<HexCoordinates>();
        for (int i = 0; i < 6; i++)
        {
            results.Add(GetNeighbor((HexDirection)i));
        }
        return results;
    }

    public HashSet<HexCoordinates> GetAllInRadius(int radius)
    {
        var results = new HashSet<HexCoordinates>();
        var curTiles = new HashSet<HexCoordinates>();
        var nextTiles = new HashSet<HexCoordinates>();
        results.Add(this);
        curTiles.Add(this);
        for (int i = 0; i < radius; i++)
        {
            foreach (var tile in curTiles)
            {
                for (int d = 0; d < 6; d++)
                {
                    var n = tile.GetNeighbor((HexDirection)d);
                    if (!results.Contains(n)) {nextTiles.Add(n);}
                    results.Add(n);
                }
            }
            var tmp = curTiles;
            curTiles = nextTiles;
            nextTiles = tmp;
            nextTiles.Clear();
        }
        //Debug.Log($"Found {results.Count} tiles for radius {radius}");
        return results;
    }

    public int DistanceTo (HexCoordinates other)
	{
		int xy =
			(X < other.X ? other.X - X : X - other.X) +
			(Y < other.Y ? other.Y - Y : Y - other.Y);

		return (xy + (Z < other.Z ? other.Z - Z : Z - other.Z)) / 2;
	}

    public static HexCoordinates FromPosition (Vector3 position)
    {
        float x = position.x / (HexMetrics.innerRadius * 2f);
        float y = -x;
        
        float offset = position.z / (HexMetrics.outerRadius * 3f);
		x -= offset;
		y -= offset;

		int iX = Mathf.RoundToInt(x);
		int iY = Mathf.RoundToInt(y);
		int iZ = Mathf.RoundToInt(-x -y);

		if (iX + iY + iZ != 0) {
			float dX = Mathf.Abs(x - iX);
			float dY = Mathf.Abs(y - iY);
			float dZ = Mathf.Abs(-x -y - iZ);

			if (dX > dY && dX > dZ) {
				iX = -iY - iZ;
			}
			else if (dZ > dY) {
				iZ = -iX - iY;
			}
		}

        return new HexCoordinates(iX, iZ);
    }

    public static HexCoordinates FromOffsetCoordinates(int x, int z)
    {
        return new HexCoordinates(x - z/2,z);
    }

    public override string ToString()
    {
        return $"{X}, {Y}, {Z}";
    }

    public string ToStringOnSeparateLines()
    {
        return $"{X}\n{Y}\n{Z}";
    }
}