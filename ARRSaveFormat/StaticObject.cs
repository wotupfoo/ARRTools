namespace ARRSaveFormat.Types
{
    public class Location
    {
        public float X;
        public float Y;
        public float Z;

        public Location(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    public class Rotation
    {
        public float X;
        public float Y;
        public float Z;

        public Rotation(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    public abstract class StaticObject
    {
        public Location Location;
        public Rotation Rotation;

        public StaticObject() { }

        public StaticObject(Location location, Rotation rotation)
        {
            Location = location;
            Rotation = rotation;
        }
    }
}
