using System;
using System.Collections.Generic;
using System.Linq;

using Korzh.NLP.TextRank.Graph;

namespace Korzh.NLP.TextRank { 

    public class PageRank<T>
    {
        /// <summary>
        /// PageRank computes a ranking of the nodes in the graph G based on
        /// the structure of the incoming links.It was originally designed as
        /// an algorithm to rank web pages.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <description></description>
        /// <param name="graph"></param>
        /// <description></description>
        /// <param name="alpha"></param>
        /// <description></description>
        /// <param name="maxItteration"></param>
        /// <description></description>
        /// <param name="tol"></param>
        /// <description></description>
        /// <param name="nstart"></param>
        /// <description></description>
        /// <param name="personalization"></param>
        /// <description></description>
        /// <param name="dangling"></param>
        /// <description></description>
        /// <returns>rankVector</returns>
        public Dictionary<T, double> Rank<T>(Graph<T> graph, double alpha = 0.85,
            double maxItteration = 100, double tol = 1.0e-6,
            Dictionary<T, double> nstart = null, Dictionary<T, double> personalization = null,
            Dictionary<T, double> dangling = null)
        {
            Dictionary<T, double> rankVector = null;
            var dgraph = new DirectedGraph<T>();

            // Boundary condition
            // Check if graph is empty then return with empty page rank dictionary
            if (graph.IsEmpty) {
                return null;
            }

            if (!graph.IsDirected) {
                // Convert to directed graph
                dgraph = GraphUtils.BuildDirectedGraph(graph);
            }
            else {
                dgraph = graph as DirectedGraph<T>;
            }

            //Create a copy in (right) stochastic form
            //stochasti matrix has a square matrix that have summation of each row is 1.
            DirectedGraph<T> sGraph = GraphUtils.ToStochastic(graph);

            int N = sGraph.Count();

            var x = new Dictionary<GraphNode<T>, double>();
            var p = new Dictionary<GraphNode<T>, double>();
            var  danglingWeights = new Dictionary<GraphNode<T>, double>();

            // Choose fixed starting vector if not given
            if (nstart == null) {
                x = sGraph.Nodes.ToDictionary(item => item as GraphNode<T>, val => 1.0 / N);
            }
            else {
                // Normalized nstart vector
                double _nSum = (double)nstart.Sum(node => node.Value);
                p = nstart.ToDictionary(item => item.Key as GraphNode<T>, item => item.Value / _nSum);
            }

            if (personalization == null) {
                p = sGraph.Nodes.ToDictionary(item => item as GraphNode<T>, val => 1.0 / N);
            }
            else {
                // Normalized personalization vector
                double _pSum = (double)personalization.Sum(node => node.Value);
                p = personalization.ToDictionary(item => item.Key as GraphNode<T>, item => item.Value / _pSum);
            }

            if (dangling == null) {
                danglingWeights = sGraph.Nodes.ToDictionary(item => item as GraphNode<T>, val => 1.0 / N);
            }
            else {
                // Normalized personalization vector
                double _dSum = (double)dangling.Sum(node => node.Value);
                danglingWeights = dangling.ToDictionary(item => item.Key as GraphNode<T>, item => item.Value / _dSum);
            }

            // Get the dangling nodes of graph
            var danglingNodes = sGraph.DanglingNodes;

            var xLast = x;

            do {
                //copy the the older rank value
                xLast = x.ToDictionary(item => item.Key, item => item.Value);
                x = xLast.ToDictionary(item => item.Key, value => 0.0);

                //calculate the dangling sum
                var danglesum = alpha * xLast.Where(v => danglingNodes.Any(item => v.Key == item)).Sum(v => v.Value);

                foreach (var page in x.Keys.ToList()) {

                    var nbrs = sGraph[page];
                    foreach (var nbr in nbrs) {
                        var xn = xLast[page];
                        var we = sGraph[page, nbr.GraphNode as GraphNode<T>];
                        x[nbr.GraphNode as GraphNode<T>] += alpha * xn * we;
                    }

                    var dnglWeight = danglingWeights.ContainsKey(page) ? danglingWeights[page] : 0;
                    var perWeight = danglingWeights.ContainsKey(page) ? danglingWeights[page] : 0;
                    x[page] += danglesum * dnglWeight + (1.0 - alpha) * perWeight;
                }

                //check convergence, l1 norm
                double err = 0.0;
                foreach (var node in x.Keys.ToList()) {
                    err += Math.Abs(x[node] - xLast[node]);
                }

                if (err < N * tol) {
                    rankVector = x.ToDictionary(item => item.Key.Value, item => item.Value);
                    break;
                }

            } while (maxItteration-- > 0);

            return rankVector;
        }
    }
}
