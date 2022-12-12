namespace Billiard.Utilities
{
    class float3x3
    {
        public float[,] Matrix { get; } = new float[3, 3];

        public float3x3(params float[] list)
        {
            for (int i = 0; i < 9; i++)
            {
                Matrix[i % 3, i / 3] = list[i];
            }
        }

        public float3x3() : this(1, 0, 0, 0, 1, 0, 0, 0, 1) { }

        public static float3x3 operator *(float3x3 a, float3x3 b)
        {
            float[] temp = new float[9];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    float sum = 0;

                    for (int k = 0; k < 3; k++)
                    {
                        sum += a.Matrix[i, k] * b.Matrix[k, j];
                    }

                    temp[i + j * 3] = sum;
                }
            }

            return new float3x3(temp);
        }
    }
}
