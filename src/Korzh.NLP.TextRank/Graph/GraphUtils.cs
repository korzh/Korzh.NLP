using System;
using System.Linq;

using MathNet.Numerics.LinearAlgebra;

namespace Korzh.NLP.TextRank.Graph
{
    internal static class GraphUtils
    {

        public static DirectedGraph<T> ToStochastic<T>(Graph<T> graph)
        {
            var sgrapah = new DirectedGraph<T>();

            var nodeOutDegree = graph.OutDegree;

            //calculate the outdegree weight sum
            var weightedDegree = nodeOutDegree.ToDictionary(item => item.Key, item => item.Key.Neighbors.Sum(nab => nab.Weight));

            foreach (var nodewithweight in weightedDegree) {

                var source = nodewithweight.Key;
                double outdegree = nodewithweight.Value;

                foreach (var nab in source.Neighbors) {

                    var target = nab.GraphNode;
                    double weight = nab.Weight <= 0.0 ? 1 : nab.Weight;

                    //Create new node and add this to graph
                    sgrapah.AddEdge(source.Value, target.Value, weight / outdegree);
                }
            }
            return sgrapah;
        }

        public  static DirectedGraph<T> BuildDirectedGraph<T>(Graph<T> graph)
        {
            var dgraph = new DirectedGraph<T>();

            foreach (var node in graph.Nodes) {
                foreach (var nab in node.Neighbors) {

                    var target = nab.GraphNode;
                    double weight = nab.Weight <= 0.0 ? 1 : nab.Weight;

                    //Create new node and add this to graph
                    dgraph.AddEdge(node.Value, target.Value, weight);
                }
            }

            return dgraph;
        }

    }
}
