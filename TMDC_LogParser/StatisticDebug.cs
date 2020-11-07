using System.Collections.Generic;

namespace TMDC_LogParser
{
    public class StatisticDebug: StatisticBase
    {
        private Mission _Mission;

        public StatisticDebug(Mission mission)
        {
            this._Mission = mission;
            if (this._Mission != null)
            {
                this._Mission.Parse();
            }

        }

        public override string[] ToLines()
        {
            List<string> lines = new List<string>();

            if (this._Mission != null)
            {
                lines.Add("Debug Summary\n");
                lines.Add("-------------------------------\n");
                lines.Add("MISSION:\n");
                lines.Add("-------------------------------\n");
                lines.Add($"START: {this._Mission.InitTime}\n");
                lines.Add($"END: {this._Mission.EndTime}\n");

                lines.Add("\n");
                lines.Add("-------------------------------\n");
                lines.Add("CRASHED:\n");
                lines.Add("-------------------------------\n");
                var crashList = this._Mission.GetCrashes();
                foreach (var item in crashList)
                {
                    lines.Add(item.ToString() + "\n");
                }

                lines.Add("\n");
                lines.Add("-------------------------------\n");
                lines.Add("DEAD:\n");
                lines.Add("-------------------------------\n");
                var deadList = this._Mission.GetDeads();
                foreach (var item in deadList)
                {
                    lines.Add(item.ToString() + "\n");
                }

                lines.Add("\n");
                lines.Add("-------------------------------\n");
                lines.Add("KILLS:\n");
                lines.Add("-------------------------------\n");
                var killedList = this._Mission.GetKillers();
                foreach (var item in killedList)
                {
                    lines.Add(item.ToString() + "\n");
                }

                lines.Add("\n");
                lines.Add("-------------------------------\n");
                lines.Add("SHOTS:\n");
                lines.Add("-------------------------------\n");
                var shotList = this._Mission.GetShots();
                foreach (var item in shotList)
                {
                    lines.Add(item.ToString() + "\n");
                }

                lines.Add("\n");
                lines.Add("-------------------------------\n");
                lines.Add("BASE-CAPTURES:\n");
                lines.Add("-------------------------------\n");
                var baseList = this._Mission.GetCaptures();
                foreach (var item in baseList)
                {
                    lines.Add(item.ToString() + "\n");
                }


                var summarySides = this._Mission.GetSummarySides();
                foreach (var side in summarySides)
                {
                    lines.Add("\n");
                    lines.Add("-------------------------------\n");
                    lines.Add($"SUMMARY: {side.SideId}\n");
                    lines.Add("-------------------------------\n");
                    foreach (var group in side.Groups)
                    {
                        lines.Add($"### Group: {group.Name}\n");
                        foreach (var unit in group.Units)
                        {
                            lines.Add(unit.ToString() + "\n");
                        }
                    }
                }
            }
            else
            {
                lines.Add("Debug Summary\n");
                lines.Add("-------------------------------\n");
                lines.Add("MISSION:\n");
                lines.Add("Mission is NULL!");
                lines.Add("-------------------------------\n");
            }

            return lines.ToArray();
        }
    }
}
