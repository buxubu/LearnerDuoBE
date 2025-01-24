namespace LearnerDuo.Extentions
{
    public static class DateTimeExtentions
    {
        public static int CaculateAge(this DateTime date)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - date.Year;
            if (date.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
