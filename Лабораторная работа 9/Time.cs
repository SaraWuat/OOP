using System;

namespace Practice9
{
    public class Time
    {
        private int hours;
        private int minutes;
        private static int count = 0;

        public static int Count
        {
            get { return count; }
        }

        public int Hours
        {
            get { return hours; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Часы не могут быть отрицательными.");
                hours = value;
            }
        }

        public int Minutes
        {
            get { return minutes; }
            set
            {
                if (value < 0 || value > 59)
                    throw new ArgumentException("Минуты должны быть в диапазоне от 0 до 59.");
                minutes = value;
            }
        }

        public long TotalMinutes
        {
            get { return hours * 60L + minutes; }
        }

        public Time() : this(0, 0)
        {
        }

        public Time(int hours, int minutes)
        {
            Hours = hours;
            Minutes = minutes;
            count++;
        }

        public static Time FromMinutes(long totalMinutes)
        {
            if (totalMinutes < 0)
                throw new ArgumentException("Время не может быть отрицательным.");
            long h = totalMinutes / 60;
            if (h > int.MaxValue)
                throw new ArgumentException("Слишком большое значение времени.");
            return new Time((int)h, (int)(totalMinutes % 60));
        }

        public static Time AddMinutes(Time t, int m)
        {
            if (t == null)
                throw new ArgumentNullException("t");
            return FromMinutes(t.TotalMinutes + m);
        }

        public Time AddMinutes(int m)
        {
            return FromMinutes(TotalMinutes + m);
        }

        public static Time operator ++(Time t)
        {
            return t.AddMinutes(1);
        }

        public static Time operator --(Time t)
        {
            if (t.TotalMinutes == 0)
                throw new ArgumentException("Нельзя вычесть минуту из 00:00.");
            return t.AddMinutes(-1);
        }

        public static explicit operator int(Time t)
        {
            return t.hours;
        }

        public static implicit operator bool(Time t)
        {
            return t.hours != 0 && t.minutes != 0;
        }

        public static Time operator +(Time t, int minutes)
        {
            return t.AddMinutes(minutes);
        }

        public static Time operator +(int minutes, Time t)
        {
            return t.AddMinutes(minutes);
        }

        public static Time operator -(Time t, int minutes)
        {
            return t.AddMinutes(-minutes);
        }

        public static Time operator -(int minutes, Time t)
        {
            return FromMinutes(minutes - t.TotalMinutes);
        }

        public override string ToString()
        {
            return string.Format("{0:D2}:{1:D2}", hours, minutes);
        }
    }
}