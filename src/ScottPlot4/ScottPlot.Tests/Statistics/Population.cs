using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using ScottPlot;
using ScottPlot.Statistics;
using System.Linq;

namespace ScottPlotTests.Statistics
{
    internal class Population
    {
        // TODO: add tests to check mean, stdev, stderr, etc. of a known population

        [Test]
        public void Test_Population_Curve()
        {
            Random rand = new Random(0);
            var ages = new ScottPlot.Statistics.Population(rand, 44, 78, 2);

            double[] curveXs = DataGen.Range(ages.minus3stDev, ages.plus3stDev, .1);
            double[] curveYs = ages.GetDistribution(curveXs, false);

            var plt = new ScottPlot.Plot(400, 300);
            plt.AddScatterPoints(ages.values, DataGen.Random(rand, ages.values.Length),
                markerSize: 10, markerShape: MarkerShape.openCircle);
            plt.AddScatterLines(curveXs, curveYs);
            plt.Grid(lineStyle: ScottPlot.LineStyle.Dot);

            TestTools.SaveFig(plt);
        }

        [Test]
        public void Test_Population_CurveSideways()
        {
            Random rand = new Random(0);
            var ages = new ScottPlot.Statistics.Population(rand, 44, 78, 2);

            double[] curveXs = DataGen.Range(ages.minus3stDev, ages.plus3stDev, .1);
            double[] curveYs = ages.GetDistribution(curveXs, false);

            var plt = new ScottPlot.Plot(400, 300);
            plt.AddScatterPoints(DataGen.Random(rand, ages.values.Length), ages.values,
                markerSize: 10, markerShape: MarkerShape.openCircle);
            plt.AddScatterLines(curveYs, curveXs);
            plt.Grid(lineStyle: ScottPlot.LineStyle.Dot);

            TestTools.SaveFig(plt);
        }

        /// <summary>
        /// 测试通过四分位法来识别离群值<br/>
        /// Sample From <a href="https://online.stat.psu.edu/stat200/lesson/3/3.2">Identifying Outliers: IQR Method</a>
        /// </summary>
        [Test]
        public void Test_Identifying_Outliers()
        {
            float multiplier = 1.5f;
            var colPerDim = new List<Student>()
            {
                new Student() { Name = "01",ScoreNumber=5 },
                new Student() { Name = "02",ScoreNumber=8 },
                new Student() { Name = "03",ScoreNumber=11 },
                new Student() { Name = "04",ScoreNumber=12 },
                new Student() { Name = "05",ScoreNumber=12 },
                new Student() { Name = "06",ScoreNumber=12 },
                new Student() { Name = "07",ScoreNumber=13 },
                new Student() { Name = "08",ScoreNumber=13 },
                new Student() { Name = "09",ScoreNumber=13 },
                new Student() { Name = "10",ScoreNumber=13 },
                new Student() { Name = "11",ScoreNumber=14 },
                new Student() { Name = "12",ScoreNumber=14 },
                new Student() { Name = "13",ScoreNumber=14 },
                new Student() { Name = "14",ScoreNumber=15 },
                new Student() { Name = "15",ScoreNumber=15 },
                new Student() { Name = "16",ScoreNumber=15 },
                new Student() { Name = "17",ScoreNumber=15 },
                new Student() { Name = "18",ScoreNumber=15 },
            };
            colPerDim = colPerDim.OrderBy(x => x.ScoreNumber).ToList();
            var outliers = colPerDim.Outliers(multiplier);
        }
    }

    public class Student : IComparable<Student>
    {
        public string Name { get; set; }
        public int ScoreNumber { get; set; }

        public int CompareTo(Student other)
        {
            return this.ScoreNumber.CompareTo(other.ScoreNumber);
        }
    }

    /// <summary>
    /// 学生统计扩展类
    /// </summary>
    public static class StudentCollectionStats
    {
        private const int minQty = 5;
        private const string outOfRangeMessage = "The collection must have at least objects to be properly analyzed.";
        private const string noDataMessage = "Collection must contain data to be analyzed.";

        /// <summary>
        /// Calculates and returns a list of outliers from a give dataset
        /// </summary>
        /// <param name="data">A collection of objects, sorted by some measurement. To be analyzed according to that measurement. </param>
        /// <param name="multiplier">A multiplier for the average range, beyond which a datum counts as an outlier. </param>
        /// <returns></returns>
        public static IEnumerable<Student> Outliers(this List<Student> data, float multiplier = 1.5f)
        {
            if (data.Count() >= minQty)
            {
                //中位数
                var med = data.GetMedian();
                var q1 = data[(int)Math.Floor(data.Count * 1 / 4d)];
                var q3 = data[(int)Math.Floor(data.Count * 3 / 4d)];
                //四分位间距
                var iqr = q3.ScoreNumber - q1.ScoreNumber;

                var outliers = new List<Student>();
                foreach (var datum in data)
                {
                    //经验认为小于Q1-1.5*(Q3-Q1)的值，或者大于Q3+1.5*(Q3-Q1)的值，被认为是异常值
                    if (datum.ScoreNumber > q3.ScoreNumber + iqr * multiplier || datum.ScoreNumber < q1.ScoreNumber - iqr * multiplier)
                    {
                        outliers.Add(datum);
                    }
                }

                return outliers;
            }
            else
                throw new IndexOutOfRangeException(outOfRangeMessage);
        }
    }
}