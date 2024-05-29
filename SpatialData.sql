DROP TABLE SpatialXY;
DROP TYPE [dbo].[Point];
DROP ASSEMBLY SpatialFunctionsNew;

CREATE ASSEMBLY SpatialFunctionsNew
FROM 'D:\Programy\Visual Studio - projekty\SpatialDataFinal\bin\Debug\SpatialDataFinal.dll';
GO

CREATE TYPE Point
EXTERNAL NAME [SpatialFunctionsNew].[Point]
GO

CREATE TABLE SpatialXY
(
    IdPointXY INT IDENTITY(1,1) PRIMARY KEY,
    PointXY Point
);
GO




--1
--DECLARE @point AS Point
--SET @point = '3;4';
--PRINT @point.DistanceFromOrigin();
--GO

----2
--DECLARE @point AS Point
--DECLARE @point2 AS Point
--SET @point = '3;4';
--SET @point2 = '4;5';
--PRINT @point.DistanceFromPoint(@point2);
--GO

----3
--DECLARE @point AS Point;
--DECLARE @polygon AS NVARCHAR(MAX);
--SET @point = '21;1';
--SET @polygon = '0;0&2;0&2;2&0;2'
--PRINT @point.IsPointInPolygon(@polygon);
--GO

----4
--DECLARE @point3 AS Point;
--DECLARE @polygon3 AS NVARCHAR(MAX);
--SET @point3 = '-3;0';
--SET @polygon3 = '0;0&2;0&2;2&0;2'
--PRINT @point3.DistanceToPolygon(@polygon3);

--DECLARE @point4 AS Point;
--DECLARE @polygon4 AS NVARCHAR(MAX);
--SET @point4 = '0;0';
--SET @polygon4 = '10;10&10;-10&10;20';
--PRINT @point4.DistanceToPolygon(@polygon4);

------5
--DECLARE @point AS Point;
--DECLARE @polygon AS NVARCHAR(MAX);
--SET @point = '0;0';
--SET @polygon = '2;0&2;2&0;2'
--PRINT @point.AreaOfPolygonNonStatic(@polygon);
--GO
