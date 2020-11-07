using System.Collections.Generic;
using System.Diagnostics;
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
        private void lstMissions_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.menSummaryDebug.Checked)
                this.Summary(new StatisticDebug(this.lstMissions.SelectedItem as Mission));
            else if (this.menSummaryWeb.Checked)
                this.Summary(new StatisticWeb(this.lstMissions.SelectedItem as Mission));
        }
        private void menSummaryDebug_Click(object sender, System.EventArgs e)
        {
            menSummaryDebug.Checked = true;
            menSummaryWeb.Checked = false;
            this.Summary(new StatisticDebug(this.lstMissions.SelectedItem as Mission));
        }

        private void menSummaryWeb_Click(object sender, System.EventArgs e)
        {
            menSummaryDebug.Checked = false;
            menSummaryWeb.Checked = true;
            this.Summary(new StatisticWeb(this.lstMissions.SelectedItem as Mission));
        }


        private void Summary(StatisticBase statistic)
        {
            rtxtMission.Clear();
            if (statistic != null)
            {
                rtxtMission.Lines = statistic.ToLines();
            }
        }
    }
}
