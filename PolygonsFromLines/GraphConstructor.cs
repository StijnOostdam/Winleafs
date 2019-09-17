﻿using PolygonsFromLines.Models;
using ShortestCycleBasis.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PolygonsFromLines
{
    internal static class GraphConstructor
    {
        /// <summary>
        /// Constructs a connected graph from the given set of lines
        /// </summary>
        public static Graph<PointF> ConstructGraphFromLines(IList<Line> lines)
        {
            var distinctPointsInLines = lines.Select(line => line.Start).ToList();
            distinctPointsInLines.AddRange(lines.Select(line => line.End));
            distinctPointsInLines = distinctPointsInLines.Distinct().ToList();

            var pointsWithNeighbouringPoints = new List<ConstructableVertex>();

            foreach (var point in distinctPointsInLines)
            {
                var neighbouringPoints = lines.Where(line => line.Start == point).Select(line => line.End).ToList();
                neighbouringPoints.AddRange(lines.Where(line => line.End == point).Select(line => line.Start));

                pointsWithNeighbouringPoints.Add(new ConstructableVertex
                {
                    Point = point,
                    NeighbouringPoints = neighbouringPoints,
                    Visited = false
                });
            }

            var graph = new Graph<PointF>();

            foreach (var vertex in pointsWithNeighbouringPoints)
            {
                ConstructVertex(vertex, graph);
            }

            return graph;
        }

        /// <summary>
        /// Processes a ConstructableVertex and adds it to the graph if it does not exist already.
        /// For each of the neighbouring points, a connection is made and the point is constructed
        /// if it does not exist in the graph yet.
        /// </summary>
        private static void ConstructVertex(ConstructableVertex constructableVertex, Graph<PointF> graph)
        {
            var graphVertex = GraphVertexFromPoint(graph, constructableVertex.Point);

            if (graphVertex == null)
            {
                graphVertex = new GraphVertex<PointF>(constructableVertex.Point);

                graph.Vertices.Add(graphVertex);
            }

            foreach (var neighbouringPoint in constructableVertex.NeighbouringPoints)
            {
                AddOrConnectNeighbour(graphVertex, neighbouringPoint, graph);
            }
        }

        /// <summary>
        /// Adds or connect a neighbouring point for an existing point in the graph.
        /// If the neighbour exists, a connection is made.
        /// If the neighbour does not exists, a new vertex is constructed and a connection is made.
        /// </summary>
        private static void AddOrConnectNeighbour(GraphVertex<PointF> graphVertex, PointF neighbouringPoint, Graph<PointF> graph)
        {
            var neighbour = GraphVertexFromPoint(graph, neighbouringPoint);
            var distance = EuclideanDistance(graphVertex.Point, neighbouringPoint);

            if (neighbour == null)
            {
                neighbour = new GraphVertex<PointF>(neighbouringPoint);

                graph.Vertices.Add(neighbour);
            }

            graphVertex.Neighbours.Add(neighbour);
            graphVertex.Weights.Add(distance);
        }

        /// <summary>
        /// If exists in the graph, returns the graph vertex object that lies on the given point
        /// </summary>
        private static GraphVertex<PointF> GraphVertexFromPoint(Graph<PointF> graph, PointF point)
        {
            return graph.Vertices.FirstOrDefault(vertex => vertex.Point.X == point.X && vertex.Point.Y == point.Y);
        }

        /// <summary>
        /// Calculates the euclidean distance between two points
        /// </summary>
        private static double EuclideanDistance(PointF point1, PointF point2)
        {
            return Math.Sqrt(Math.Pow(point2.X - point1.X, 2) + Math.Pow(point2.Y - point1.Y, 2));
        }
    }
}