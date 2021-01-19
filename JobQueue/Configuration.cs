namespace JobQueue
{
    public class Configuration
    {
        public static int NormalCategoryMaxThreadCount = 2;
        public static int ExpressCategoryMaxThreadCount = 5;
        public static int RetryCount = 3;
        public static bool IsLogDebuge = false;
        public static bool TimerEnable = true;
    }
}
