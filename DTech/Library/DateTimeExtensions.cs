namespace DTech.Library
{
    public static class DateTimeExtensions
    {
        public static string ToTimeAgo(this DateTime dt)
        {
            var ts = DateTime.UtcNow - dt;
            if (ts.TotalMinutes < 1)
                return "Recently";
            if (ts.TotalMinutes < 60)
                return $"{(int)ts.TotalMinutes} minutes ago";
            if (ts.TotalHours < 24)
                return $"{(int)ts.TotalHours} hours ago";
            if (ts.TotalDays < 30)
                return $"{(int)ts.TotalDays} days ago";
            return $"{dt}";
        }
    }
}
