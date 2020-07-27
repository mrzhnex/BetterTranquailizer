namespace BetterTranquilizer
{
    public class Global
    {
        public static int maxTranqillOnSCP = 2;
        public static float fullRPDistance = 8f;
        public static float damageOnFRP = 1f;

        public static float distance_to_pickup = 2.5f;

        public static string _istoolongtopickup = "Поблизости нет тел";
        public static string _successpickup = "Вы подняли тело ";
        public static string _successdrop = "Вы бросили тело ";

        public static string _isalreadyholder = "Вы уже несете тело";
        public static string _isnotholder = "Вы не несете тело";

        public static string _iswrongowner = "Вы не можете поднять тело ";

        public static string _iscuffed = "Вы связаны";
        internal static bool can_use_commands;

        public static bool IsFullRp = false;
    }
}