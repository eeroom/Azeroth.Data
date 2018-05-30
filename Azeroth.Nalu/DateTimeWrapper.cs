using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.Nalu
{
    /// <summary>
    /// 这个wrpper通过偏移和时间段模式 表示一段时间。
    /// 站在微秒的精度上看，一个日期时间值都是一段时间，2017-01-01 00:00:00 代表的时间范围是，2017-01-01 00:00:00:000至2017-01-01 00:00:00:999
    /// 这段时间的长度是999毫秒，
    /// 在月报，年报，周报等场景下，也需要时间段，一个月，一年，
    /// 表示2017年1月，2017-01-01  2017-01-31
    /// 这样表示没有问题，但是在数据库里面用两列时间范围来表明这是1月份的数据，在后续的查询等处理上很不便
    /// 这个wrapper引入时间段模式的概念，对于原始的datetime而言，其模式就是秒模式，长度是1s(999.9999毫秒)
    /// 数据库里只需要一列日期时间值，一列模式，一列偏移，即可表示任意的一段时间
    /// </summary>
    public class DateTimeWrapper
    {
        public DateTime StartDateTime { get;private set; }
        public DateTime EndDateTime { get; private set; }
        public int Length { get; private set; }
        public Schema SchemaEnum { get; private set; }
        public DateTimeWrapper(Schema sh,int offset,DateTime val)
        {
            this.SchemaEnum = sh;
            this.Length = System.Math.Abs(offset) + 1;
            int buffer = -1;
            switch (sh)
            {
                case Schema.yyyy:
                    this.StartDateTime = new DateTime(val.Year,1,1);
                    if (offset < 0)
                        this.StartDateTime = StartDateTime.AddYears(offset);
                    this.EndDateTime = StartDateTime.AddYears(Length).AddSeconds(buffer);
                    break;
                case Schema.yyyyMM:
                     this.StartDateTime = new DateTime(val.Year,val.Month,1);
                    if (offset < 0)
                        this.StartDateTime = StartDateTime.AddMonths(offset);
                    this.EndDateTime = StartDateTime.AddMonths(Length).AddSeconds(buffer);
                    break;
                case Schema.yyyyMMdd:
                     this.StartDateTime = new DateTime(val.Year,val.Month,val.Day);
                    if (offset < 0)
                        this.StartDateTime = StartDateTime.AddDays(offset);
                    this.EndDateTime = StartDateTime.AddDays(Length).AddSeconds(buffer);
                    break;
                case Schema.yyyyMMddHH:
                    this.StartDateTime = new DateTime(val.Year, val.Month, val.Day,val.Hour,0,0);
                    if (offset < 0)
                        this.StartDateTime = StartDateTime.AddHours(offset);
                    this.EndDateTime = StartDateTime.AddHours(Length).AddSeconds(buffer);
                    break;
                case Schema.yyyyMMddHHmm:
                    this.StartDateTime = new DateTime(val.Year, val.Month, val.Day, val.Hour, val.Minute, 0);
                    if (offset < 0)
                        this.StartDateTime = StartDateTime.AddMinutes(offset);
                    this.EndDateTime = StartDateTime.AddMinutes(Length).AddSeconds(buffer);
                    break;
                case Schema.yyyyMMWW:
                    break;
                default:
                    break;
            }
        }


        public List<DateTime> ToList()
        {
            List<DateTime> lst= new List<DateTime>();
            for (int i = 0; i < Length; i++)
            {
                switch (this.SchemaEnum)
                {
                    case Schema.yyyy:
                        lst.Add(StartDateTime.AddYears(i));
                        break;
                    case Schema.yyyyMM:
                        lst.Add(StartDateTime.AddMonths(i));
                        break;
                    case Schema.yyyyMMdd:
                        lst.Add(StartDateTime.AddDays(i));
                        break;
                    case Schema.yyyyMMddHH:
                        lst.Add(StartDateTime.AddHours(i));
                        break;
                    case Schema.yyyyMMddHHmm:
                        lst.Add(StartDateTime.AddMinutes(i));
                        break;
                    case Schema.yyyyMMWW:
                        break;
                    default:
                        break;
                }
            }
            return lst;
        }

        public override string ToString()
        {
            return string.Format("{0}至{1}",this.StartDateTime.ToString("yyyy-MM-dd HH:mm:ss"),this.EndDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        public enum Schema
        {
            yyyy,
            yyyyMM,
            yyyyMMdd,
            yyyyMMddHH,
            yyyyMMddHHmm,
            /// <summary>
            /// 周
            /// </summary>
            yyyyMMWW
        }
    }
}
