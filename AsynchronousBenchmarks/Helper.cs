namespace Helper
{
    public static class Instances
    {
        public static readonly object obj = new object();
        public static readonly Vector4 vector = new Vector4(1f, -2.1f, -4f, 5.5f);

    }

    public struct Vector4
    {
        public float x, y, z, w;

        public Vector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
    }
}