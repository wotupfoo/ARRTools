using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RailroadsOnlineSaveViewer.Types;

namespace RailroadsOnlineSaveViewer
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SplineType
    {
        Rails,
        BridgeRails,
        BankVariable,
        BankConstant,
        WallVariable,
        WallConstant,
        BridgeWooden,
        BridgeSteel
    }

    public class SplineSegment
    {
        public Location LocationStart;
        public Location LocationEnd;
        public bool Visible;

        public SplineSegment(Location locationStart, Location locationEnd, bool visible)
        {
            LocationStart = locationStart;
            LocationEnd = locationEnd;
            Visible = visible;
        }
    }

    public class Spline
    {
        public Location Location;
        public SplineType Type;
        public SplineSegment[] Segments;

        private int? controlPointsIndexStart;
        private int? controlPointsIndexEnd;

        private int? visibilityIndexStart;
        private int? visibilityIndexEnd;

        public void SetControlPointIndexStart(int i)
        {
            controlPointsIndexStart = i;
        }

        public void SetControlPointIndexEnd(int i)
        {
            controlPointsIndexEnd = i;
        }

        public void SetVisibilityIndexStart(int i)
        {
            visibilityIndexStart = i;
        }

        public void SetVisibilityIndexEnd(int i)
        {
            visibilityIndexEnd = i;
        }

        public void BuildSegments(Location[] controlPoints, bool[] visibility, int index)
        {
            if (controlPointsIndexStart == null || controlPointsIndexEnd == null)
            {
                throw new System.Exception("Unable to parse control points, start and end indexes not set!");
            }

            if (visibilityIndexStart == null || visibilityIndexEnd == null)
            {
                throw new System.Exception("Unable to parse visibility points, start and end indexes not set!");
            }

            if (controlPointsIndexStart != visibilityIndexStart + index || controlPointsIndexEnd != visibilityIndexEnd + index + 1)
            {
                throw new System.Exception("Visibility segments count and control points count not same!");
            }

            Location firstControlPoint = controlPoints[(int)controlPointsIndexStart];
            if (firstControlPoint.X != Location.X || firstControlPoint.Y != Location.Y || firstControlPoint.Z != Location.Z)
            {
                throw new System.Exception("First control point and spline location are different!");
            }

            int j = 0;
            Segments = new SplineSegment[(int)controlPointsIndexEnd - (int)controlPointsIndexStart];
            for (int i = (int)controlPointsIndexStart; i < (int)controlPointsIndexEnd; i++)
            {
                Segments[j] = new SplineSegment(controlPoints[i], controlPoints[i + 1], visibility[i - index]);
                j++;
            }
        }

        public Spline() { }

        public Spline(Location location, SplineType type, SplineSegment[] segments)
        {
            Location = location;
            Type = type;
            Segments = segments;
        }
    }
}
