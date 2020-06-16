namespace Utilities
{
    class Double3x3
    {
        public double[,] Matrix { get; } = new double[3, 3];

        public Double3x3(params double[] list)
        {
            for (int i = 0; i < 9; i++)
            {
                Matrix[i % 3, i / 3] = list[i];
            }
        }

        public Double3x3() : this(1, 0, 0, 0, 1, 0, 0, 0, 1) { }

        public static Double3x3 operator *(Double3x3 a, Double3x3 b)
        {
            double[] temp = new double[9];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    double sum = 0;

                    for (int k = 0; k < 3; k++)
                    {
                        sum += a.Matrix[i, k] * b.Matrix[k, j];
                    }

                    temp[i + j * 3] = sum;
                }
            }

            return new Double3x3(temp);
        }
    }
}
