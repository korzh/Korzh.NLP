using System;

using MathNet.Numerics.LinearAlgebra;

namespace Korzh.NLP.TextRank
{
    internal static class Utils
    {
        public static double CosineSimilarity(Vector<double> v1, Vector<double> v2)
        {
            double l1 = Math.Sqrt(v1 * v1);
            double l2 = Math.Sqrt(v2 * v2);
            double similarity = (v1 * v2) / (l1 * l2);

            return similarity;
        }
    }
}
