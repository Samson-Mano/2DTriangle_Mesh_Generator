using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DTriangle_Mesh_Generator.mesh_control.delaunay_triangulation
{
    public class point_d
    {
        private double eps = 0.000001; // 10^-6
        public double x;
        public double y;

        public point_d(double i_x, double i_y)
        {
            this.x = i_x;
            this.y = i_y;
        }

        // Operators
        public point_d sub(point_d other_pt)
        {
            double ab_x = this.x - other_pt.x;
            double ab_y = this.y - other_pt.y;

            return new point_d(ab_x, ab_y);
        }

        public point_d add(point_d other_pt)
        {
            double ab_x = this.x + other_pt.x;
            double ab_y = this.y + other_pt.y;

            return new point_d(ab_x, ab_y);
        }

        public double dot(point_d other_pt)
        {
            return ((this.x * other_pt.x) + (this.y * other_pt.y));
        }

        public double cross(point_d other_pt)
        {
            return ((this.y * other_pt.x) - (this.x * other_pt.y));
        }
        public point_d mult(double v)
        {
            return (new point_d(this.y * v, this.y * v));
        }

        public bool Equals(point_d other_pt)
        {
            if (Math.Abs(this.x - other_pt.x) <= eps && Math.Abs(this.y - other_pt.y) <= eps)
            {
                return true;
            }
            return false;
        }

        public static double GetAngle(point_d A_pt, point_d B_pt, point_d C_pt)
        {
            // Get the dot product.
            double dot_product = DotProduct(A_pt, B_pt, C_pt);

            // Get the cross product.
            double cross_product = CrossProductLength(A_pt, B_pt, C_pt);

            // Calculate the angle.
            return Math.Atan2(cross_product, dot_product);
        }

        private static double CrossProductLength(point_d A_pt, point_d B_pt, point_d C_pt)
        {
            // Get the vectors' coordinates.
            double BAx = A_pt.x - B_pt.x;
            double BAy = A_pt.y - B_pt.y;
            double BCx = C_pt.x - B_pt.x;
            double BCy = C_pt.y - B_pt.y;

            // Calculate the Z coordinate of the cross product.
            return (BAx * BCy - BAy * BCx);
        }

        // Return the dot product AB · BC.
        // Note that AB · BC = |AB| * |BC| * Cos(theta).
        private static double DotProduct(point_d A_pt, point_d B_pt, point_d C_pt)
        {
            // Get the vectors' coordinates.
            double BAx = A_pt.x - B_pt.x;
            double BAy = A_pt.y - B_pt.y;
            double BCx = C_pt.x - B_pt.x;
            double BCy = C_pt.y - B_pt.y;

            // Calculate the dot product.
            return (BAx * BCx + BAy * BCy);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.x, this.y);
        }
    }
}
