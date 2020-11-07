using System.Collections.Generic;

namespace TMDC_LogParser
{
    public class StatisticWeb: StatisticBase
    {
        #region public class CoalitionStatistics
        public class CoalitionStatistics
        {
            #region public class UnitStatistics
            public class UnitStatistics
            {
                public int ShameCrashes = 0;
                public readonly Mission.DcsObject Unit;
                public readonly Dictionary<string, int> Weapons = new Dictionary<string, int>();
                public readonly List<Mission.DcsKilledObject> Kills = new List<Mission.DcsKilledObject>();

                public UnitStatistics(Mission.DcsObject unit)
                {
                    this.Unit = unit;
                }
            }
            #endregion

            public readonly Mission.Coalitions Coalition;
            public readonly Dictionary<string, int> AirStatistic = new Dictionary<string, int>();
            public readonly Dictionary<string, int> GroundStatistic = new Dictionary<string, int>();
            public readonly Dictionary<string, int> WeaponStatistic = new Dictionary<string, int>();
            public readonly Dictionary<string, UnitStatistics> UnitStatistic = new Dictionary<string, UnitStatistics>();

            public CoalitionStatistics(Mission.Coalitions coalition)
            {
                this.Coalition = coalition;
            }

            public void ProcessCrashes(Mission.DcsCrashedObject value)
            {
                if (this.AirStatistic.TryGetValue(value.TypeName.Name, out var item))
                {
                    this.AirStatistic[value.TypeName.Name] = ++item;
                }
                else
                {
                    this.AirStatistic[value.TypeName.Name] = 1;
                }
            }
            public void Process(Mission.DcsDeadObject value)
            {
                if (this.GroundStatistic.TryGetValue(value.TypeName.Name, out var item))
                {
                    this.GroundStatistic[value.TypeName.Name] = ++item;
                }
                else
                {
                    this.GroundStatistic[value.TypeName.Name] = 1;
                }
            }
            public void Process(Mission.DcsShotObject value)
            {
                // WeaponStatistic
                if (this.WeaponStatistic.TryGetValue(value.WeaponType, out int count))
                {
                    this.WeaponStatistic[value.WeaponType] = ++count;
                }
                else
                {
                    this.WeaponStatistic[value.WeaponType] = 1;
                }

                // UnitStatistic
                if (value.Category.IsAir)
                {
                    UnitStatistics unitStatistic;
                    if (!this.UnitStatistic.TryGetValue(value.UnitName, out unitStatistic))
                    {
                        unitStatistic = new UnitStatistics(value);
                        this.UnitStatistic.Add(value.UnitName, unitStatistic);
                    }

                    if (!unitStatistic.Weapons.TryGetValue(value.WeaponType, out count))
                    {
                        unitStatistic.Weapons[value.WeaponType] = ++count;
                    }
                    else
                    {
                        unitStatistic.Weapons[value.WeaponType] = 1;
                    }
                }
            }
            public void Process(Mission.DcsKilledObject value)
            {
                if (value.Category.IsAir)
                {
                    UnitStatistics unitStatistic;
                    if (!this.UnitStatistic.TryGetValue(value.UnitName, out unitStatistic))
                    {
                        unitStatistic = new UnitStatistics(value);
                        this.UnitStatistic.Add(value.UnitName, unitStatistic);
                    }

                    unitStatistic.Kills.Add(value);
                }
            }
            public void ProcessShames(Mission.DcsCrashedObject value)
            {
                if (value.Category.IsAir)
                {
                    UnitStatistics unitStatistic;
                    if (!this.UnitStatistic.TryGetValue(value.UnitName, out unitStatistic))
                    {
                        unitStatistic = new UnitStatistics(value);
                        this.UnitStatistic.Add(value.UnitName, unitStatistic);
                    }

                    unitStatistic.ShameCrashes++;
                }
            }
        }
        #endregion

        public CoalitionStatistics[] _Coalition;

        public StatisticWeb(Mission mission)
        {
            this._Coalition = new CoalitionStatistics[] { new CoalitionStatistics(Mission.Coalitions.Neutral), new CoalitionStatistics(Mission.Coalitions.Red), new CoalitionStatistics(Mission.Coalitions.Blue) };

            if (mission != null)
            {
                mission.Parse();

                var crashes = mission.GetCrashes(); // AIR
                foreach (var value in crashes)
                {
                    this._Coalition[value.Coalition.ToInt32()].ProcessCrashes(value);
                }

                var shameCrashes = mission.GetShameCrashes(); // ShameCrashes (AIR)
                foreach (var value in shameCrashes)
                {
                    this._Coalition[value.Coalition.ToInt32()].ProcessShames(value);
                }

                var deads = mission.GetDeads(); // GROUND
                foreach (var value in deads)
                {
                    this._Coalition[value.Coalition.ToInt32()].Process(value);
                }

                var killers = mission.GetKillers(); // Ground and Air objects
                foreach (var value in killers)
                {
                    this._Coalition[value.Coalition.ToInt32()].Process(value);
                }

                var shots = mission.GetShots(); // Ground and Air objects
                foreach (var value in shots)
                {
                    this._Coalition[value.Coalition.ToInt32()].Process(value);
                }
            }
        }

        public override string[] ToLines()
        {
            List<string> lines = new List<string>();

            foreach (var side in this._Coalition)
            {
                if ((side.GroundStatistic.Count > 0) || (side.AirStatistic.Count > 0) || (side.WeaponStatistic.Count > 0) || (side.UnitStatistic.Count > 0))
                {
                    lines.Add("---------------------");
                    lines.Add($"{side.Coalition}:");
                    lines.Add("---------------------");

                    if (side.GroundStatistic.Count > 0)
                    {
                        lines.Add("GROUND SUMMARY:");
                        foreach (var key in side.GroundStatistic.Keys)
                        {
                            var count = side.GroundStatistic[key];
                            lines.Add($"- {key}: {count}");
                        }
                        lines.Add("");
                    }

                    if (side.AirStatistic.Count > 0)
                    {
                        lines.Add("AIR SUMMARY:");
                        foreach (var key in side.AirStatistic.Keys)
                        {
                            var count = side.AirStatistic[key];
                            lines.Add($"- {key}: {count}");
                        }
                        lines.Add("");
                    }

                    if (side.WeaponStatistic.Count > 0)
                    {
                        lines.Add("WEAPON SUMMARY:");
                        foreach (var key in side.WeaponStatistic.Keys)
                        {
                            var count = side.WeaponStatistic[key];
                            lines.Add($"- {key}: {count}");
                        }
                        lines.Add("");
                    }

                    if (side.UnitStatistic.Count > 0)
                    {
                        foreach (var unit in side.UnitStatistic.Values)
                        {
                            if (unit.Weapons.Count > 0 || unit.Kills.Count > 0 || unit.ShameCrashes > 0)
                            {
                                lines.Add($"UNIT: [{unit.Unit.UnitName}]");
                                if (unit.ShameCrashes > 0)
                                {
                                    lines.Add($"+ Shame crashes: {unit.ShameCrashes}");
                                }

                                if (unit.Weapons.Count > 0)
                                {
                                    lines.Add($"+ Fired:");
                                    foreach (var weapon in unit.Weapons)
                                    {
                                        lines.Add($"  - {weapon.Key}: {weapon.Value}");
                                    }
                                }

                                if (unit.Kills.Count > 0)
                                {
                                    lines.Add($"+ Kills:");
                                    foreach (var kill in unit.Kills)
                                    {
                                        lines.Add($"  - {kill.Target.TypeName} ({kill.Target.UnitName}) {kill.Weapon}");
                                    }
                                }
                                lines.Add("");
                            }
                        }
                    }
                }
            }

            return lines.ToArray();
        }
    }
}
