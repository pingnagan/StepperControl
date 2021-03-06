﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ControllerCNC.Primitives;

namespace ControllerCNC.ShapeEditor
{
    class FacetShape
    {
        internal readonly IEnumerable<Point2Dmm> DefinitionPoints;

        internal double TotalPerimeter { get { return _totalPerimeter; } }

        private readonly double _totalPerimeter;


        internal FacetShape(IEnumerable<Point2Dmm> points)
        {
            DefinitionPoints = points.ToArray();
            var previousPoint = DefinitionPoints.First();
            foreach (var point in DefinitionPoints)
            {
                var lineLength = previousPoint.DistanceTo(point);
                _totalPerimeter += lineLength;
                previousPoint = point;
            }
        }


        internal double GetNextPointPercentage(double startPercentage)
        {
            var currentLength = 0.0;
            var previousPoint = DefinitionPoints.First();
            foreach (var point in DefinitionPoints.Skip(1))
            {
                var currentPercentage = currentLength / _totalPerimeter;
                if (currentPercentage > startPercentage)
                    return currentPercentage;

                var lineLength = previousPoint.DistanceTo(point);
                currentLength += lineLength;
                previousPoint = point;
            }

            return 1.0;
        }

        internal Point2Dmm GetPoint(double percentage)
        {
            var currentLength = 0.0;
            var previousPoint = DefinitionPoints.First();
            if (percentage > 0)
                foreach (var point in DefinitionPoints.Skip(1))
                {
                    var lineLength = previousPoint.DistanceTo(point);
                    var previousPercentage = currentLength / _totalPerimeter;
                    currentLength += lineLength;
                    var currentPercentage = currentLength / _totalPerimeter;
                    var percentageRange = currentPercentage - previousPercentage;

                    if (currentPercentage >= percentage)
                    {
                        var percentageDelta = percentage - previousPercentage;
                        var segmentPercentage = percentageDelta / percentageRange;
                        var c1 = previousPoint.C1 + (point.C1 - previousPoint.C1) * segmentPercentage;
                        var c2 = previousPoint.C2 + (point.C2 - previousPoint.C2) * segmentPercentage;
                        return new Point2Dmm(c1, c2);
                    }
                    previousPoint = point;
                }

            return new Point2Dmm(previousPoint.C1, previousPoint.C2);
        }
    }
}
