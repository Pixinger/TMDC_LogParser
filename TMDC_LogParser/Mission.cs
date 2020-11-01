using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TMDC_LogParser
{
    internal class Mission
    {
        #region public class DcsObject
        public class DcsObject
        {
            public readonly string Group;
            public readonly string Unit;
            public readonly string Number;
            public readonly string Id;
            public readonly string Type;
            public readonly string PositionX;
            public readonly string PositionY;
            public readonly string PositionZ;

            private Match _Match = null;
            protected Match Match { get => this._Match; }


            public DcsObject(string line)
            {
                var regex = new Regex(@"(\[.+\])(.+)\|(.+)\|(\d+)\|(\d+)\|(.+)\|\((.+\|.+\|.+)\)\|?(.*)?");
                this._Match = regex.Match(line);
                if (this._Match.Success)
                {
                    if (this._Match.Groups.Count > 2)
                        this.Group = this._Match.Groups[2].Value;
                    else
                        this.Group = "";

                    if (this._Match.Groups.Count > 3)
                        this.Unit = this._Match.Groups[3].Value;
                    else
                        this.Unit = "";

                    if (this._Match.Groups.Count > 4)
                        this.Number = this._Match.Groups[4].Value;
                    else
                        this.Number = "";

                    if (this._Match.Groups.Count > 5)
                        this.Id = this._Match.Groups[5].Value;
                    else
                        this.Id = "";

                    if (this._Match.Groups.Count > 6)
                        this.Type = this._Match.Groups[6].Value;
                    else
                        this.Type = "";

                    if (this._Match.Groups.Count > 7)
                    {
                        this.PositionX = "";
                        this.PositionY = "";
                        this.PositionZ = "";

                        var regexPos = new Regex(@"(-?[0-9]+.?[0-9]*)\|(-?[0-9]+.?[0-9]*)\|(-?[0-9]+.?[0-9]*)");
                        var matchPos = regexPos.Match(this._Match.Groups[7].Value);

                        if ((matchPos.Success) && (matchPos.Groups.Count == 4))
                        {
                            this.PositionX = matchPos.Groups[1].Value;
                            this.PositionY = matchPos.Groups[2].Value;
                            this.PositionZ = matchPos.Groups[3].Value;
                        }
                    }
                }
            }

            public override string ToString()
            {
                return $"Grp({this.Group}), Name({this.Unit}), Id({this.Id}), Type({this.Type}), Pos({this.PositionX}/{this.PositionY}/{this.PositionZ})";
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
                if (this.Match.Groups.Count > 8)
                    this.WeaponType = this.Match.Groups[8].Value;
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
            public readonly string Initiator;

            public DcsKilledObject(string line)
                : base(line)
            {
                if (this.Match.Groups.Count > 8)
                    this.Initiator = this.Match.Groups[8].Value;
                else
                    this.Initiator = "";

            }

            public override string ToString()
            {
                return base.ToString() + $", Killer({this.Initiator})";
            }
        }
        #endregion
        #region public class DcsSummaryObject : DcsObject
        public class DcsSummaryObject : DcsObject
        {
            public readonly string Life;

            public DcsSummaryObject(string line)
                : base(line)
            {
                if (this.Match.Groups.Count > 8)
                    this.Life = this.Match.Groups[8].Value;
                else
                    this.Life = "";

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
                    while (CheckLine(i, "[SUM][GRP]", out data))
                    {
                        var group = new Group(data);
                        side.Groups.Add(group);

                        i++;
                        while (CheckLine(i, "[SUM][UNI]", out data))
                        {
                            data = "[DUMMY]" + data; // We need this to match the REGEX
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
                if (line.StartsWith("[CRASH]"))
                {
                    this._Crashes.Add(new DcsCrashedObject(line));
                }
                else if (line.StartsWith("[DEAD]"))
                {
                    this._Deads.Add(new DcsDeadObject(line));
                }
                else if (line.StartsWith("[KILL]"))
                {
                    this._Kills.Add(new DcsKilledObject(line));
                }
                else if (line.StartsWith("[SHOT]"))
                {
                    this._Shots.Add(new DcsShotObject(line));
                }
                else if (line.StartsWith("[BASE_CAPTURED]"))
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
        public DcsObject[] GetCrashes() { return this._Crashes.ToArray(); }
        public DcsObject[] GetDeads() { return this._Deads.ToArray(); }
        public DcsObject[] GetKills() { return this._Kills.ToArray(); }
        public DcsShotObject[] GetShots() { return this._Shots.ToArray(); }
        public CapturedObject[] GetCaptures() { return this._Captures.ToArray(); }
        public Summary.Side[] GetSummarySides() { return _Summary.Sides.ToArray(); }

        public override string ToString()
        {
            return $"Mission: {this.InitTime} to {this.EndTime}";
        }

    }
}
