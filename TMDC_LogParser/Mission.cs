using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TMDC_LogParser
{
    public class Mission
    {
        #region public enum Coalitions 
        public enum Coalitions : int
        {
            Neutral = 0,
            Red = 1,
            Blue = 2,
        }
        #endregion
        #region public enum Categories
        public enum Categories
        {
            Airplane = 1,
            Helcopter = 2,
            Ground_Unit = 3,
            Ship = 4,
            Structure = 5,
        }
        #endregion

        #region public class Coalition
        public class Coalition
        {
            private readonly Coalitions _Coalition;

            public Coalition(string text)
            {
                if (text.ToLower() == "red")
                    this._Coalition = Coalitions.Red;
                else if (text.ToLower() == "blue")
                    this._Coalition = Coalitions.Blue;
                else
                    this._Coalition = Coalitions.Neutral;
            }
            public Coalition(Coalitions coalition)
            {
                this._Coalition = coalition;
            }
            public Coalition(int coalition)
            {
                this._Coalition = (Coalitions)coalition;
            }

            public int ToInt32() { return (int)_Coalition; }
            public Coalitions ToEnum() { return _Coalition; }

            public override string ToString()
            {
                return this._Coalition.ToString();
            }
        }
        #endregion
        #region public class Category
        public class Category
        {
            private readonly Categories _Category;

            public Category(Categories category)
            {
                this._Category = category;
            }
            public Category(int category)
            {
                this._Category = (Categories)category;
            }

            public bool IsAir
            {
                get
                {
                    return (_Category == Categories.Airplane) || (_Category == Categories.Helcopter);
                }
            }

            public override string ToString()
            {
                return this._Category.ToString();
            }
        }
        #endregion
        #region public class Weapon
        public class Weapon
        {
            private readonly string _Weapon = "";

            public Weapon(string text)
            {
                _Weapon = text;
            }

            public override string ToString()
            {
                return _Weapon != null? this._Weapon.ToString() : "";
            }
        }
        #endregion
        #region public class Position
        public class Position
        {
            public readonly double X;
            public readonly double Y;
            public readonly double Z;

            public Position(string position)
            {
                var regexPos = new Regex(@"(-?[0-9]+.?[0-9]*)\|(-?[0-9]+.?[0-9]*)\|(-?[0-9]+.?[0-9]*)");
                var match = regexPos.Match(position);
                if ((match.Success) && (match.Groups.Count == 4))
                {
                    this.X = Convert.ToDouble(match.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
                    this.Y = Convert.ToDouble(match.Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture);
                    this.Z = Convert.ToDouble(match.Groups[3].Value, System.Globalization.CultureInfo.InvariantCulture);
                }
            }

            public override string ToString()
            {
                return $"{this.X}/{this.Y}/{this.Z}";
            }
        }
        #endregion
        #region public class TypeName
        public class TypeName
        {
            public readonly string Name;
            public TypeName(string text)
            {
                this.Name = text;
            }

            public override string ToString()
            {
                return this.Name;
            }
        }
        #endregion

        #region public class DcsObject
        public class DcsObject
        {
            public readonly double Time;
            public readonly string GroupName;
            public readonly string UnitName;
            public readonly Coalition Coalition;
            public readonly Category Category;
            public readonly int Id;
            public readonly TypeName TypeName;
            public readonly Position Position;

            public readonly string Remainder;

            public DcsObject(string line)
            {
                // ---------------------- [FLAG]   Time | Grp  |Unit  | Coal | Cat  |UniId | Type |    position        |optional
                var regex = new Regex(@"(\[.{4}\])(.+?)\|(.+?)\|(.+?)\|(.+?)\|(.+?)\|(.+?)\|(.+?)\|\((.+?\|.+?\|.+?)\)\|?(.*)?");
                var match = regex.Match(line);
                if (match.Success)
                {
                    if (match.Groups.Count > 2)
                        this.Time = Convert.ToDouble(match.Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture);

                    if (match.Groups.Count > 3)
                        this.GroupName = match.Groups[3].Value;

                    if (match.Groups.Count > 4)
                        this.UnitName = match.Groups[4].Value;

                    if (match.Groups.Count > 5)
                        this.Coalition = new Coalition(Convert.ToInt32(match.Groups[5].Value));

                    if (match.Groups.Count > 6)
                        this.Category = new Category(Convert.ToInt32(match.Groups[6].Value));

                    if (match.Groups.Count > 7)
                        this.Id = Convert.ToInt32(match.Groups[7].Value);

                    if (match.Groups.Count > 8)
                        this.TypeName = new TypeName(match.Groups[8].Value);

                    if (match.Groups.Count > 9)
                        this.Position = new Position(match.Groups[9].Value);

                    if (match.Groups.Count > 10)
                        Remainder = match.Groups[10].Value;
                }
            }

            public override string ToString()
            {
                return $"Grp({this.GroupName}), Unit({this.UnitName}), Coal({this.Coalition}), Cat({this.Category}), Id({this.Id}), Type({this.TypeName}), Pos({this.Position})";
            }
        }
        #endregion
        #region public class DcsDeadObject : DcsObject
        public class DcsDeadObject : DcsObject
        {
            public DcsDeadObject(string line)
                : base(line)
            {
            }
        }
        #endregion
        #region public class DcsCrashedObject : DcsObject
        public class DcsCrashedObject : DcsObject
        {
            public DcsCrashedObject(string line)
                : base(line)
            {
            }
        }
        #endregion
        #region public class DcsShotObject : DcsObject
        public class DcsShotObject : DcsObject
        {
            public readonly string WeaponType;

            public DcsShotObject(string line)
                : base(line)
            {
                if (Remainder != null)
                    this.WeaponType = Remainder;
                else
                    this.WeaponType = "";

            }

            public override string ToString()
            {
                return base.ToString() + $", Weapon({this.WeaponType})";
            }
        }
        #endregion
        #region public class DcsKilledObject : DcsObject
        public class DcsKilledObject : DcsObject
        {
            public readonly DcsObject Target;
            public readonly Weapon Weapon;

            public DcsKilledObject(string line)
                : base(line)
            {
                if (Remainder != null)
                {
                    this.Target = new DcsObject(Remainder);
                    this.Weapon = new Weapon(this.Target.Remainder);
                }
            }

            public override string ToString()
            {
                return $"Killer({base.ToString()}) Target({this.Target}) Weapon({this.Weapon})";
            }
        }
        #endregion
        #region public class DcsSummaryObject : DcsObject
        public class DcsSummaryObject : DcsObject
        {
            public readonly double Life;

            public DcsSummaryObject(string line)
                : base(line)
            {
                if (Remainder != null)
                    this.Life = Convert.ToDouble(Remainder, System.Globalization.CultureInfo.InvariantCulture);
            }

            public override string ToString()
            {
                return base.ToString() + $", Life({this.Life})";
            }
        }
        #endregion

        #region public class CapturedObject
        public class CapturedObject
        {
            public readonly string Coalition;
            public readonly string Place;

            public CapturedObject(string line)
            {
                var regex = new Regex(@"(\[.+\])(.+)\|(.+)");
                var _Match = regex.Match(line);
                var match = _Match;
                if (match.Success)
                {
                    if (match.Groups.Count > 2)
                    {
                        this.Coalition = match.Groups[2].Value;
                    }
                    if (match.Groups.Count > 3)
                    {
                        this.Place = match.Groups[3].Value;
                    }
                }
            }

            public override string ToString()
            {
                return $"'{this.Coalition}' captured '{this.Place}' airbase";
            }
        }

        #endregion

        #region public class Summary
        public class Summary
        {
            private List<string> _Lines = new List<string>(); // Contains the RAW log text

            #region public class Side
            public class Side
            {
                public readonly string SideId;
                public readonly List<Group> Groups = new List<Group>();

                public Side(string sideId)
                {
                    this.SideId = sideId;
                }
            }
            #endregion
            #region public class Group
            public class Group
            {
                public readonly string Name;
                public readonly List<DcsSummaryObject> Units = new List<DcsSummaryObject>();

                public Group(string name)
                {
                    this.Name = name;
                }
            }
            #endregion

            public readonly List<Side> Sides = new List<Side>();

            public void AddLine(string line)
            {
                this._Lines.Add(line);
            }

            private bool CheckLine(int index, string text, out string data)
            {
                if (index >= this._Lines.Count)
                {
                    data = "";
                    return false;
                }

                string line = this._Lines[index];
                if (line.StartsWith(text))
                {
                    data = line.Remove(0, text.Length);
                    return true;
                }
                else
                {
                    data = "";
                    return false;
                }
            }

            public void Parse()
            {
                this.Sides.Clear();

                int i = 0;
                while (this.CheckLine(i, "[SUM][SID]", out string data))
                {
                    Side side = new Side(data);
                    this.Sides.Add(side);

                    i++;
                    while (this.CheckLine(i, "[SUM][GRP]", out data))
                    {
                        var group = new Group(data);
                        side.Groups.Add(group);

                        i++;
                        while (this.CheckLine(i, "[SUM][UNI]", out data))
                        {
                            data = "[DUMMY]0|" + data; // We need this to match the REGEX
                            var unit = new DcsSummaryObject(data);
                            group.Units.Add(unit);

                            i++;
                        }
                    }
                }
            }
        }
        #endregion

        private List<string> _Lines = new List<string>(); // Contains the RAW log text

        // Used after calling Parse()
        private List<DcsCrashedObject> _Crashes = new List<DcsCrashedObject>();
        private List<DcsCrashedObject> _ShameCrashes = new List<DcsCrashedObject>(); // Wird aus Crashes und Killed errechnet.
        private List<DcsDeadObject> _Deads = new List<DcsDeadObject>();
        private List<DcsKilledObject> _Kills = new List<DcsKilledObject>();
        private List<DcsShotObject> _Shots = new List<DcsShotObject>();

        private List<CapturedObject> _Captures = new List<CapturedObject>();

        private Summary _Summary = new Summary();

        public Mission(string init_time)
        {
            this.InitTime = init_time;
            this.EndTime = "NONE";
        }

        public void Parse()
        {
            this._Summary.Parse();
            this._Crashes.Clear();
            this._Deads.Clear();
            this._Shots.Clear();
            this._Captures.Clear();

            for (int i = 0; i < this._Lines.Count; i++)
            {
                string line = this._Lines[i];
                if (line.StartsWith("[CRSH]"))
                {
                    this._Crashes.Add(new DcsCrashedObject(line));
                }
                else if (line.StartsWith("[DEAD]"))
                {
                    this._Deads.Add(new DcsDeadObject(line));
                }               
                else if (line.StartsWith("[KILR]"))
                {
                    this._Kills.Add(new DcsKilledObject(line));
                }
                else if (line.StartsWith("[SHOT]"))
                {
                    this._Shots.Add(new DcsShotObject(line));
                }
                else if (line.StartsWith("[BASE]"))
                {
                    this._Captures.Add(new CapturedObject(line));
                }
                else if (line.StartsWith("[INIT]"))
                {
                }
                else if (line.StartsWith("[START]"))
                {
                }
                else if (line.StartsWith("[END]"))
                {
                }
            }

            // Jetzt müssen wir noch die Crashes mit den Kills vergleichen, um die selbstverschuldeten Abstürze zu finden.
            List<DcsKilledObject> killers = new List<DcsKilledObject>(this.GetKillers());
            List<DcsCrashedObject> crashes = new List<DcsCrashedObject>(this.GetCrashes());
            foreach (var killer in killers)
            {
                var targetUnitName = killer.Target.UnitName;
                for (int i = 0; i < crashes.Count; i++)
                {
                    if (crashes[i].UnitName == targetUnitName)
                    {
                        crashes.RemoveAt(i);
                        break;
                    }
                }
            }
            // Die noch verbleibenden crashes sind shame's
            _ShameCrashes.AddRange(crashes);
        }
        public void AddLine(GroupCollection groups)
        {
            var group = groups[4];
            if (group.Value.StartsWith("[END]"))
            {
                this.EndTime = groups[1].Value;
            }
            else if (group.Value.StartsWith("[SUM]"))
            {
                this._Summary.AddLine(group.Value);
            }
            else
            {
                this._Lines.Add(group.Value);
            }
        }

        public string InitTime { get; }
        public string EndTime { get; private set; }

        public DcsCrashedObject[] GetShameCrashes() { return this._ShameCrashes.ToArray(); }
        public DcsCrashedObject[] GetCrashes() { return this._Crashes.ToArray(); }
        public DcsDeadObject[] GetDeads() { return this._Deads.ToArray(); }
        public DcsKilledObject[] GetKillers() { return this._Kills.ToArray(); }
        public DcsShotObject[] GetShots() { return this._Shots.ToArray(); }
        public CapturedObject[] GetCaptures() { return this._Captures.ToArray(); }
        public Summary.Side[] GetSummarySides() { return this._Summary.Sides.ToArray(); }

        public override string ToString()
        {
            return $"Mission: {this.InitTime} to {this.EndTime}";
        }

    }
}