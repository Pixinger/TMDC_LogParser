using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TMDC_LogParser
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            this.InitializeComponent();
        }

        private Mission[] ParseLogfile(string filename)
        {
            List<Mission> missions = new List<Mission>();
            Mission mission = null;

            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs, Encoding.Default))
            {
                string expression = @"(.+)INFO +SCRIPTING: \[\[TMDC\]\]\|?(.+)?\|(\d+): +(.+)";
                var regex = new Regex(expression);

                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    var match = regex.Match(line);
                    if ((match.Success) && (match.Groups.Count > 4))
                    {
                        if (match.Groups[4].Value.StartsWith("[INIT]"))
                        {
                            if (mission != null)
                                missions.Add(mission);

                            mission = new Mission(match.Groups[1].Value);
                        }
                        else
                        {
                            if (mission == null)
                                mission = new Mission("NONE");

                            mission.AddLine(match.Groups);
                        }
                    }
                }
            }

            if (mission != null)
                missions.Add(mission);

            return missions.ToArray();
        }

        private void lstMissions_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.rtxtMission.Clear();

            var mission = this.lstMissions.SelectedItem as Mission;
            if (mission != null)
            {
                mission.Parse();

                this.rtxtMission.AppendText("-------------------------------\n");
                this.rtxtMission.AppendText("MISSION:\n");
                this.rtxtMission.AppendText("-------------------------------\n");
                this.rtxtMission.AppendText($"START: {mission.InitTime}\n");
                this.rtxtMission.AppendText($"END: {mission.EndTime}\n");

                this.rtxtMission.AppendText("\n");
                this.rtxtMission.AppendText("-------------------------------\n");
                this.rtxtMission.AppendText("CRASHED:\n");
                this.rtxtMission.AppendText("-------------------------------\n");
                var crashList = mission.GetCrashes();
                foreach (var item in crashList)
                {
                    this.rtxtMission.AppendText(item.ToString() + "\n");
                }

                this.rtxtMission.AppendText("\n");
                this.rtxtMission.AppendText("-------------------------------\n");
                this.rtxtMission.AppendText("DEAD:\n");
                this.rtxtMission.AppendText("-------------------------------\n");
                var deadList = mission.GetDeads();
                foreach (var item in deadList)
                {
                    this.rtxtMission.AppendText(item.ToString() + "\n");
                }

                this.rtxtMission.AppendText("\n");
                this.rtxtMission.AppendText("-------------------------------\n");
                this.rtxtMission.AppendText("KILLS:\n");
                this.rtxtMission.AppendText("-------------------------------\n");
                var killedList = mission.GetKills();
                foreach (var item in killedList)
                {
                    this.rtxtMission.AppendText(item.ToString() + "\n");
                }

                this.rtxtMission.AppendText("\n");
                this.rtxtMission.AppendText("-------------------------------\n");
                this.rtxtMission.AppendText("SHOTS:\n");
                this.rtxtMission.AppendText("-------------------------------\n");
                var shotList = mission.GetShots();
                foreach (var item in shotList)
                {
                    this.rtxtMission.AppendText(item.ToString() + "\n");
                }

                this.rtxtMission.AppendText("\n");
                this.rtxtMission.AppendText("-------------------------------\n");
                this.rtxtMission.AppendText("BASE-CAPTURES:\n");
                this.rtxtMission.AppendText("-------------------------------\n");
                var baseList = mission.GetCaptures();
                foreach (var item in baseList)
                {
                    this.rtxtMission.AppendText(item.ToString() + "\n");
                }


                var summarySides = mission.GetSummarySides();
                foreach (var side in summarySides)
                {
                    this.rtxtMission.AppendText("\n");
                    this.rtxtMission.AppendText("-------------------------------\n");
                    this.rtxtMission.AppendText($"SUMMARY: {side.SideId}\n");
                    this.rtxtMission.AppendText("-------------------------------\n");
                    foreach (var group in side.Groups)
                    {
                        this.rtxtMission.AppendText($"### Group: {group.Name}\n");
                        foreach (var unit in group.Units)
                        {
                            this.rtxtMission.AppendText(unit.ToString() + "\n");
                        }
                    }
                }

            }
        }
        private void menOpen_Click(object sender, System.EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.RestoreDirectory = true;
                dlg.Multiselect = false;
                dlg.AddExtension = true;
                dlg.CheckFileExists = true;
                dlg.CheckPathExists = true;
                dlg.Filter = "Dcs-Log|*.log";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    this.lstMissions.Items.Clear();
                    var missions = this.ParseLogfile(dlg.FileName);
                    if (missions != null)
                    {
                        foreach (var mission in missions)
                        {
                            this.lstMissions.Items.Add(mission);
                        }
                    }
                }
            }
        }
    }
}
