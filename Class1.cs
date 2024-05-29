using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;
using Microsoft.SqlServer.Server;

[Serializable]
[Microsoft.SqlServer.Server.SqlUserDefinedType(Format.Native)]
public struct Point : INullable
{
    public bool IsNull { get; private set; }
    public double _x { get; set; }
    public double _y { get; set; }

    public Point(double x, double y)
    {
        _x = x;
        _y = y;
        IsNull = false;
    }
    public override string ToString()
    {
        if (this.IsNull)
            return "NULL";
        else
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("(");
            builder.Append(_x);
            builder.Append("; ");
            builder.Append(_y);
            builder.Append(")");
            return builder.ToString();
        }
    }

    public static Point Null
    {
        get
        {
            Point p = new Point();
            p.IsNull = true;
            return p;
        }
    }

    //odczytanie punktu (x;y) do zmiennej Point
    [SqlMethod(OnNullCall = false)]
    public static Point Parse(SqlString s)
    {
        if (s.IsNull)
            return Null;
        Point p = new Point();
        string[] xy = s.Value.Split(";".ToCharArray());
        p._x = double.Parse(xy[0]);
        p._y = double.Parse(xy[1]);

        return p;
    }

    //zamiana tablicy punktow ze stringa do tablicy Pointow
    [SqlMethod(OnNullCall = false)]
    public static Point [] ParseStringToArray(SqlString s)
    {
        if (s.IsNull)
        {
            //return Null;
        }

        string[] tabOfstrings = s.Value.Split("&".ToCharArray());
        Point [] points = new Point [tabOfstrings.Length];
        for(int i=0;  i<tabOfstrings.Length; i++)
        {
            string[] xy = tabOfstrings[i].Split(";".ToCharArray());
            points[i]._x = double.Parse(xy[0]);
            points[i]._y = double.Parse(xy[1]);
        }
        

        return points;
    }

    //sprawdza czy punkt znajduje sie w wielokacie
    [SqlMethod(OnNullCall = false)]
    public String IsPointInPolygon(String pointsStr)
    {
        Point [] points = ParseStringToArray(pointsStr);

        int numIntersections = 0;
        if (points.Length < 3)
        {
            return "True";
        }

        for (int i = 0; i < points.Length; i++)
        {
            var p1 = points[i];
            var p2 = points[(i + 1) % points.Length];

            if ((p1._y <= _y && p2._y > _y) || (p2._y <= _y && p1._y > _y))
            {
                if (_x < (p2._x - p1._x) * (_y - p1._y) / (p2._y - p1._y) + p1._x)
                {
                    numIntersections++;
                }
            }
        }
        return (numIntersections % 2 != 0) ? "True" : "False";
    }

    //sprawdza najkrotsza odleglosc punktu do wielokata
    [SqlMethod(OnNullCall = false)]
    public String DistanceToPolygon(String pointsStr)
    {
        Point[] points = ParseStringToArray(pointsStr);
        double minDistance = double.MaxValue;

        for (int i = 0; i < points.Length; i++)
        {
            Point p1 = points[i];
            Point p2 = points[(i + 1) % points.Length];
            String distance = this.DistanceToSegment(p1, p2);
            minDistance = Math.Min(minDistance, double.Parse(distance));
        }

        return minDistance.ToString();
    }


    [SqlMethod(OnNullCall = false)]
    public String DistanceToSegment(Point p1, Point p2)
    {
        // Równanie prostej w postaci ogólnej Ax + By + C = 0
        double A = p2._y - p1._y;
        double B = p1._x - p2._x;
        double C = p2._x * p1._y - p1._x * p2._y;

        // Odległość punktu od prostej
        double distanceToLine = Math.Abs(A * this._x + B * this._y + C) / Math.Sqrt(A * A + B * B);

        // Obliczenie współczynnika parametrycznego t dla projekcji punktu na prostą
        double t = ((this._x - p1._x) * (p2._x - p1._x) + (this._y - p1._y) * (p2._y - p1._y)) /
                   ((p2._x - p1._x) * (p2._x - p1._x) + (p2._y - p1._y) * (p2._y - p1._y));

        if (t < 0)
        {
            return Math.Sqrt(Math.Pow(this._x - p1._x, 2) + Math.Pow(this._y - p1._y, 2)).ToString();
        }
        else if (t > 1)
        {
            return Math.Sqrt(Math.Pow(this._x - p2._x, 2) + Math.Pow(this._y - p2._y, 2)).ToString();
        }
        else
        {
            return distanceToLine.ToString();
        }
    }

    //liczy pole wielokata
    [SqlMethod(OnNullCall = false)]
    public static String AreaOfPolygon(String pointsStr)
    {
        Point[] points = ParseStringToArray(pointsStr);

        double area = 0;
        int numPoints = points.Length;

        for (int i = 0; i < numPoints; i++)
        {
            Point currentPoint = points[i];
            Point nextPoint = points[(i + 1) % numPoints]; // Wrapping around to the first point

            area += (nextPoint._x - currentPoint._x) * (nextPoint._y + currentPoint._y) / 2;
        }

        return Math.Abs(area).ToString(); // Return the absolute value of the area
    }

    //liczy pole wielokata (trzeba wywolac na jednym z wierzcholkow i jako argumenty podac pozstale)
    [SqlMethod(OnNullCall = false)]
    public String AreaOfPolygonNonStatic(String pointsStr)
    {
        Point p1 = new Point(_x, _y);
        Point[] points_temp = ParseStringToArray(pointsStr);
        Point[] points = new Point[points_temp.Length+1];
        points[0] = p1;
        for(int i=0; i < points_temp.Length; i++)
        {
            points[i+1] = points_temp[i];
        }

        double area = 0;
        int numPoints = points.Length;

        for (int i = 0; i < numPoints; i++)
        {
            Point currentPoint = points[i];
            Point nextPoint = points[(i + 1) % numPoints]; // Wrapping around to the first point

            area += (nextPoint._x - currentPoint._x) * (nextPoint._y + currentPoint._y) / 2;
        }

        return Math.Abs(area).ToString(); // Return the absolute value of the area
    }

    //liczy odleglosc punktu od poczatku ukladu wspolrzednych
    [SqlMethod(OnNullCall = false)]
    public Double DistanceFromOrigin()
    {
        return DistanceFromXY(0.0, 0.0);
    }

    //liczy odleglosc punktu od punktu
    [SqlMethod(OnNullCall = false)]
    public Double DistanceFromPoint(Point pFrom)
    {
        return DistanceFromXY(pFrom._x, pFrom._y);
    }

    //pomocncza funckja do obliczenie odleglosci dwoch punktow od siebie
    [SqlMethod(OnNullCall = false)]
    public Double DistanceFromXY(double iX, double iY)
    {
        return Math.Sqrt(Math.Pow(iX - _x, 2.0) + Math.Pow(iY - _y, 2.0));
    }
}

