using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Reflection;


public class API
{
    static string connectionString = "Server=DESKTOP-VKUTRSM;Database=SpatialDataProjectDB;Integrated Security=True;"; // Replace with your connection string

    static public Point ReadPointFromDatabase(int id)
    {
        string query = "SELECT PointXY FROM SpatialXY WHERE IdPointXY = @IdPoint";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdPoint", id);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                // Read the custom Point object directly from the SqlDataReader
                Point point = (Point)reader["PointXY"];
                return point;
            }
            else
            {
                return new Point();
            }
        }
    }

    static public int AddPointAndGetId(double x, double y)
    {
        string query = "INSERT INTO SpatialXY (PointXY) OUTPUT INSERTED.IdPointXY VALUES (@pointData)";
        string pointData = $"{x};{y}";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@pointData", pointData);

            connection.Open();
            int newId = (int)command.ExecuteScalar();
            Console.WriteLine("Punkt został dodany do bazy danych z ID: " + newId);
            return newId;
        }
    }


    static public void DistanceFromOriginAPI()
    {
        Console.Write("Podaj wartość X: ");
        double x = Convert.ToDouble(Console.ReadLine());
        Console.Write("Podaj wartość Y: ");
        double y = Convert.ToDouble(Console.ReadLine());

        int newId = AddPointAndGetId(x, y);
        Point point = ReadPointFromDatabase(newId);

        string query = "DECLARE @point AS Point; SET @point = @pointData; SELECT @point.DistanceFromOrigin() AS Result;";
        string pointData = $"{point._x};{point._y}";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@pointData", pointData);

            connection.Open();
            var result = command.ExecuteScalar();
            Console.WriteLine("Odległość od (0,0) to " + result + "\n");
        }
    }


    static public void DistanceFromPointAPI()
    {
        Console.WriteLine("Punkt 1:");
        Console.Write("Podaj wartość X: ");
        double x1 = Convert.ToDouble(Console.ReadLine());
        Console.Write("Podaj wartość Y: ");
        double y1 = Convert.ToDouble(Console.ReadLine());

        Console.WriteLine("Punkt 2:");
        Console.Write("Podaj wartość X: ");
        double x2 = Convert.ToDouble(Console.ReadLine());
        Console.Write("Podaj wartość Y: ");
        double y2 = Convert.ToDouble(Console.ReadLine());

        int id1 = AddPointAndGetId(x1, y1);
        int id2 = AddPointAndGetId(x2, y2);

        Point point1 = ReadPointFromDatabase(id1);
        Point point2 = ReadPointFromDatabase(id2);

        string query = "DECLARE @point1 AS Point; " +
                       "DECLARE @point2 AS Point; " +
                       "SET @point1 = @pointData1; " +
                       "SET @point2 = @pointData2; " +
                       "SELECT @point1.DistanceFromPoint(@point2) AS Result;";
        string pointData1 = $"{point1._x};{point1._y}";
        string pointData2 = $"{point2._x};{point2._y}";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@pointData1", pointData1);
            command.Parameters.AddWithValue("@pointData2", pointData2);

            connection.Open();
            var result = command.ExecuteScalar();
            Console.WriteLine("Odległość od " + point1 + " do " + point2 + " to " + result + "\n");
        }
    }


    static public void IsPointInPolygonAPI()
    {
        Console.Write("Podaj wartość X punktu: ");
        double x = Convert.ToDouble(Console.ReadLine());
        Console.Write("Podaj wartość Y punktu: ");
        double y = Convert.ToDouble(Console.ReadLine());

        int pointId = AddPointAndGetId(x, y);
        Point point = ReadPointFromDatabase(pointId);

        List<Point> pointsTab = new List<Point>();
        Console.Write("Z ilu punktów wielokąt: ");
        int input = Convert.ToInt32(Console.ReadLine());
        while (input > 0)
        {
            Console.Write("Podaj wartość X: ");
            double px = Convert.ToDouble(Console.ReadLine());
            Console.Write("Podaj wartość Y: ");
            double py = Convert.ToDouble(Console.ReadLine());
            int polygonPointId = AddPointAndGetId(px, py);
            pointsTab.Add(ReadPointFromDatabase(polygonPointId));
            input--;
        }
        string polygonData = string.Join(",", pointsTab.ConvertAll(p => $"{p._x};{p._y}"));
        string query = "DECLARE @point AS Point; DECLARE @polygon AS NVARCHAR(MAX); SET @point = @pointData; SET @polygon = @polygonData; SELECT @point.IsPointInPolygon(@polygon) AS Result;";
        string pointData = $"{point._x};{point._y}";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@pointData", pointData);
            command.Parameters.AddWithValue("@polygonData", polygonData);

            connection.Open();
            var result = command.ExecuteScalar();
            Console.WriteLine("Czy punkt jest w wielokącie? " + result + "\n");
        }
    }


    static public void DistanceToPolygonAPI()
    {
        Console.Write("Podaj wartość X punktu: ");
        double x = Convert.ToDouble(Console.ReadLine());
        Console.Write("Podaj wartość Y punktu: ");
        double y = Convert.ToDouble(Console.ReadLine());

        int pointId = AddPointAndGetId(x, y);
        Point point = ReadPointFromDatabase(pointId);

        List<Point> pointsTab = new List<Point>();
        Console.Write("Z ilu punktów wielokąt: ");
        int input = Convert.ToInt32(Console.ReadLine());
        while (input > 0)
        {
            Console.Write("Podaj wartość X: ");
            double px = Convert.ToDouble(Console.ReadLine());
            Console.Write("Podaj wartość Y: ");
            double py = Convert.ToDouble(Console.ReadLine());
            int polygonPointId = AddPointAndGetId(px, py);
            pointsTab.Add(ReadPointFromDatabase(polygonPointId));
            input--;
        }
        string polygonData = string.Join("&", pointsTab.ConvertAll(p => $"{p._x};{p._y}"));
        string query = "DECLARE @point AS Point; " +
                       "DECLARE @polygon AS NVARCHAR(MAX);" +
                       "SET @point = @pointData; SET @polygon = @polygonData;" +
                       "SELECT @point.DistanceToPolygon(@polygon) AS Result;";
        string pointData = $"{point._x};{point._y}";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@pointData", pointData);
            command.Parameters.AddWithValue("@polygonData", polygonData);

            connection.Open();
            var result = command.ExecuteScalar();
            Console.WriteLine("Najkrótsza odległość to: " + result + "\n");
        }
    }

    static public void DistanceToSegmentAPI()
    {
        Console.WriteLine("Punkt referencyjny:");
        Console.Write("Podaj wartość X: ");
        double x = Convert.ToDouble(Console.ReadLine());
        Console.Write("Podaj wartość Y: ");
        double y = Convert.ToDouble(Console.ReadLine());

        int pointId = AddPointAndGetId(x, y);
        Point referencePoint = ReadPointFromDatabase(pointId);

        Console.WriteLine("Punkt początkowy odcinka:");
        Console.Write("Podaj wartość X: ");
        double x1 = Convert.ToDouble(Console.ReadLine());
        Console.Write("Podaj wartość Y: ");
        double y1 = Convert.ToDouble(Console.ReadLine());

        Console.WriteLine("Punkt końcowy odcinka:");
        Console.Write("Podaj wartość X: ");
        double x2 = Convert.ToDouble(Console.ReadLine());
        Console.Write("Podaj wartość Y: ");
        double y2 = Convert.ToDouble(Console.ReadLine());

        int segmentStartId = AddPointAndGetId(x1, y1);
        int segmentEndId = AddPointAndGetId(x2, y2);

        Point segmentStart = ReadPointFromDatabase(segmentStartId);
        Point segmentEnd = ReadPointFromDatabase(segmentEndId);

        string query = "DECLARE @referencePoint AS Point; " +
                       "DECLARE @segmentStart AS Point; " +
                       "DECLARE @segmentEnd AS Point; " +
                       "SET @referencePoint = @referencePointData; " +
                       "SET @segmentStart = @segmentStartData; " +
                       "SET @segmentEnd = @segmentEndData; " +
                       "SELECT @referencePoint.DistanceToSegment(@segmentStart, @segmentEnd) AS Result;";
        string referencePointData = $"{referencePoint._x};{referencePoint._y}";
        string segmentStartData = $"{segmentStart._x};{segmentStart._y}";
        string segmentEndData = $"{segmentEnd._x};{segmentEnd._y}";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@referencePointData", referencePointData);
            command.Parameters.AddWithValue("@segmentStartData", segmentStartData);
            command.Parameters.AddWithValue("@segmentEndData", segmentEndData);

            connection.Open();
            var result = command.ExecuteScalar();
            Console.WriteLine("Najkrótsza odległość od punktu do odcinka to: " + result + "\n");
        }
    }


    static public void AreaOfPolygonAPI()
    {
        Console.Write("Z ilu punktów wielokąt: ");
        int input = Convert.ToInt32(Console.ReadLine());

        Console.Write("Podaj wartość X: ");
        double x = Convert.ToDouble(Console.ReadLine());
        Console.Write("Podaj wartość Y: ");
        double y = Convert.ToDouble(Console.ReadLine());
        int pointId = AddPointAndGetId(x, y);
        Point point = ReadPointFromDatabase(pointId);

        List<Point> pointsTab = new List<Point>();
        while (input-1 > 0)
        {
            Console.Write("Podaj wartość X: ");
            double px = Convert.ToDouble(Console.ReadLine());
            Console.Write("Podaj wartość Y: ");
            double py = Convert.ToDouble(Console.ReadLine());
            int polygonPointId = AddPointAndGetId(px, py);
            pointsTab.Add(ReadPointFromDatabase(polygonPointId));
            input--;
        }
        string pointData = $"{point._x};{point._y}";
        string polygonData = string.Join("&", pointsTab.ConvertAll(p => $"{p._x};{p._y}"));
        string query = "DECLARE @point AS Point;" +
                       "SET @point = @pointData;" +
                       "DECLARE @polygon AS NVARCHAR(MAX); " +
                       "SET @polygon = @polygonData; " +
                       "SELECT @point.AreaOfPolygonNonStatic(@polygon) AS Result;";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@pointData", pointData);
            command.Parameters.AddWithValue("@polygonData", polygonData);

            connection.Open();
            var result = command.ExecuteScalar();
            Console.WriteLine("Pole wielokąta: " + result + "\n");
        }
    }



    static public void Main(String[] args)
    {
        bool flag = true;
        while (flag)
        {
            Console.WriteLine(" ------------------------------------- ");
            Console.WriteLine("|1-odleglosc punktu od (0,0)          |");
            Console.WriteLine("|2-odleglosc dwoch punktow            |");
            Console.WriteLine("|3-sprawdz czy punkt jest w wielokacie|");
            Console.WriteLine("|4-odleglosc punktu od odcinka        |");
            Console.WriteLine("|5-odleglosc punktu od wielokata      |");
            Console.WriteLine("|6-pole wielokata                     |");
            Console.WriteLine("|Esc-end                              |");
            Console.WriteLine(" -------------------------------------");
            Console.Write("Akcja:");
            var keyInfo = Console.ReadKey(intercept: true);  // Read key without displaying

            if (keyInfo.Key == ConsoleKey.Escape)
            {
                flag = false;
                continue;
            }

            int input;
            if (int.TryParse(keyInfo.KeyChar.ToString(), out input))
            {
                switch (input)
                {
                    case 1:
                        Console.WriteLine("1");
                        DistanceFromOriginAPI();
                        break;
                    case 2:
                        Console.WriteLine("2");
                        DistanceFromPointAPI();
                        break;
                    case 3:
                        Console.WriteLine("3");
                        IsPointInPolygonAPI();
                        break;
                    case 4:
                        Console.WriteLine("4");
                        DistanceToSegmentAPI();
                        break;
                    case 5:
                        Console.WriteLine("5");
                        DistanceToPolygonAPI();
                        break;
                    case 6:
                        Console.WriteLine("6");
                        AreaOfPolygonAPI();
                        break;
                    case 7:
                        flag = false;
                        break;
                    default:
                        Console.WriteLine("Nieprawidłowy wybór. Spróbuj ponownie.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Nieprawidłowy wybór. Spróbuj ponownie.");
            }
        }
    }
}
