namespace ReviewsDashboard.Repos
{
    public class TimeRep:ITimeRep
    {
        public DateTime GetCurrentTime()
        {
            DateTime serverTime = DateTime.Now;
            DateTime _localTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(serverTime, TimeZoneInfo.Local.Id, "Central Standard Time");
            var res = _localTime - new DateTime(_localTime.Year, _localTime.Month, _localTime.Day);
            return _localTime;
        }
    }
}
