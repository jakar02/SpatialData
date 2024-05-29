using NUnit.Framework;
using System;
using System.Drawing;

namespace SpatialDataProjectTests
{
    [TestFixture]
    public class PointTests
    {
        [Test]
        public void TestDistanceFromOrigin()
        {
            Point p = new Point(3.0, 4.0);
            double result = p.DistanceFromOrigin();

            Assert.AreEqual(5.0, result);
        }

        [Test]
        public void TestDistanceFromPoint()
        {
            Point p1 = new Point(1.0, 1.0);
            Point p2 = new Point(4.0, 5.0);
            double result = p1.DistanceFromPoint(p2);

            Assert.AreEqual(5.0, result);
        }

        [Test]
        public void TestIsPointInPolygon()
        {
            Point p = new Point(3.0, 3.0);
            string polygon = "1,0;1,0&1,0;5,0&5,0;5,0&5,0;1,0";
            string result = p.IsPointInPolygon(polygon);

            Assert.AreEqual("True", result);
        }

        [Test]
        public void TestDistanceToPolygon()
        {
            Point p = new Point(5.0, 7.0);
            string polygon = "1,0;1,0&1,0;5,0&5,0;5,0&5,0;1,0";
            string result = p.DistanceToPolygon(polygon);

            Assert.AreEqual("2", result);
        }

        [Test]
        public void TestAreaOfPolygon()
        {
            string polygon = "1,0;1,0&1,0;5,0&5,0;5,0&5,0;1,0";
            string result = Point.AreaOfPolygon(polygon);

            Assert.AreEqual("16", result);
        }

        [Test]
        public void TestDistanceToSegment() {
            Point p = new Point(1.0, 1.0);
            Point p1 = new Point(4.0, 4.0);
            Point p2 = new Point(4.0, -2.0);
            string result = p.DistanceToSegment(p1, p2);

            Assert.AreEqual("3", result);
        }
    }
}
