namespace Platform
{
    public class PlatformTransformationSettings
    {
        public enum TransformDimension
        {
            // Static
            NONE = 0b_0000_0000,

            // 1-D Movement
            X = 0b_0000_0001,
            Y = 0b_0000_0010,
            Z = 0b_0000_0100,

            // 2-D Movement
            XY = X | Y,
            XZ = X | Z,
            YZ = Y | Z,

            // 3-D Movement
            ALL_DIMENSIONS = X | Y | Z,
        }

        public enum TransformFunction
        {
            SIN,
            COS
        }
    }
}