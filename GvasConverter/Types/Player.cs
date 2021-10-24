using RailroadsOnlineSaveViewer.Types;

namespace RailroadsOnlineSaveViewer
{
    public class Player
    {
        public string Name;
        public Location Location;
        public float Rotation;
        public int XP;
        public float Money;

        public Player()
        {

        }

        public Player(string name, Location location, float rotation, int xp, float money)
        {
            Name = name;
            Location = location;
            Rotation = rotation;
            XP = xp;
            Money = money;
        }
    }
}
