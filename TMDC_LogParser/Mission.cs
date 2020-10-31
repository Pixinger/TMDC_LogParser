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
            protected Match Match { get => _Match; }


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
                return $"Grp({Group}), Name({Unit}), Id({Id}), Type({Type}), Pos({PositionX}/{PositionY}/{PositionZ})";
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
                return base.ToString() + $", {WeaponType}"; 
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
                return $"'{Coalition}' captured '{Place}' airbase";
            }
        }

        #endregion

        private List<string> _Lines = new List<string>(); // Contains the RAW log text

        // Used after calling Parse()
        private List<DcsObject> _Crashes = new List<DcsObject>();
        private List<DcsObject> _Kills = new List<DcsObject>();
        private List<DcsShotObject> _Shots = new List<DcsShotObject>();
        private List<CapturedObject> _Captures = new List<CapturedObject>();

        public Mission(string init_time)
        {
            this.InitTime = init_time;
            this.EndTime = "NONE";
        }

        public void Parse()
        {
            this._Crashes.Clear();
            this._Kills.Clear();
            this._Shots.Clear();
            this._Captures.Clear();

            for (int i = 0; i < this._Lines.Count; i++)
            {
                string line = this._Lines[i];
                if (line.StartsWith("[CRASH]"))
                {
                    this._Crashes.Add(new DcsObject(line));
                }
                else if (line.StartsWith("[KILL]"))
                {
                    this._Kills.Add(new DcsObject(line));
                }
                else if (line.StartsWith("[SHOT]"))
                {
                    this._Shots.Add(new DcsShotObject(line));
                }
                else if (line.StartsWith("[BASE_CAPTURED]"))
                {
                    this._Captures.Add(new CapturedObject(line));
                }
                else if (line.StartsWith("[SUM]"))
                {
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
                EndTime = groups[1].Value;
            }
            else
            {
                this._Lines.Add(group.Value);
            }
        }

        public string InitTime { get; }
        public string EndTime { get; private set; }
        public DcsObject[] GetCrashes() { return _Crashes.ToArray(); }
        public DcsObject[] GetKills() { return _Kills.ToArray(); }
        public DcsShotObject[] GetShots() { return _Shots.ToArray(); }
        public CapturedObject[] GetCaptures() { return _Captures.ToArray(); }

        public override string ToString()
        {
            return $"Mission: {InitTime} to {EndTime}";
        }

    }
}
