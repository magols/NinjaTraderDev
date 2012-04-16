using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Media;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace EcoNewsControl
{
    public partial class EcoNewsControl : UserControl
    {
        private readonly string _fp;
        private readonly EcoNewsSettings _ens;
        private Timer _myTimer;
        private readonly CultureInfo _ffDateTimeCulture = CultureInfo.CreateSpecificCulture("en-US");
        private const string FfNewsUrl = @"http://www.forexfactory.com/ffcal_week_this.xml";
        private List<NewsEvent> _list = new List<NewsEvent>();
        private List<NewsEvent> _templist = new List<NewsEvent>();
        //private string _lastLoadError;
        private DateTime _lastLoad;
        private DateTime _nextLoad;
        private DateTime _alertPlayed;
        private string _alertFile;
        private int _refreshMins;
        private int _alertMins;
        private bool _impactHigh;
        private bool _impactMedium;
        private bool _impactLow;
        private bool _todaysNewsOnly;
        private bool _usd;
        private bool _eur;
        private bool _jpy;
        private bool _gbp;
        private bool _chf;
        private bool _cad;
        private bool _cny;
        private bool _aud;
        private bool _nzd;

        public EcoNewsControl(string sounds, string configfile)
        {
            _fp = sounds;
            _ens = new EcoNewsSettings();
            _ens.ConfigFile = configfile;
            InitializeComponent();
            LoadDefaultSettings();
            LoadNews();
        }

        //public EcoNewsControl()
        //{
        //    _ens = new EcoNewsSettings();
        //    InitializeComponent();
        //    LoadDefaultSettings();
        //    LoadNews();
        //}


        private void LoadDefaultSettings()
        {

            if (!_ens.LoadXmlSettings())
            {
                _impactLow = true;
                _impactHigh = true;
                _impactMedium = true;
                _usd = _eur = _jpy = _gbp = _chf = _cad = _cny = _aud = _nzd = true;
                _alertFile = _fp + "Alert4.wav";
                _alertMins = 1;
                _refreshMins = 30;
                _todaysNewsOnly = false;
            }
            else
            {
                _impactLow = _ens.ImpactLow;
                _impactHigh = _ens.ImpactHigh;
                _impactMedium = _ens.ImpactMedium;
                _usd = _ens.Usd;
                _eur = _ens.Eur;
                _jpy = _ens.Jpy;
                _gbp = _ens.Gbp;
                _chf = _ens.Chf;
                _cad = _ens.Cad;
                _cny = _ens.Cny;
                _aud = _ens.Aud;
                _nzd = _ens.Nzd;
                _alertFile = _ens.AlertFile;
                _alertMins = _ens.AlertMinutes;
                _refreshMins = _ens.RefreshInterval;
                _todaysNewsOnly = _ens.TodaysNews;
            }

            impactCheckLow.Checked = _impactLow;
            impactCheckMedium.Checked = _impactMedium;
            impactCheckHigh.Checked = _impactHigh;

            checkedListBoxCountries.SetItemChecked(0, _usd);
            checkedListBoxCountries.SetItemChecked(1, _eur);
            checkedListBoxCountries.SetItemChecked(2, _jpy);
            checkedListBoxCountries.SetItemChecked(3, _gbp);
            checkedListBoxCountries.SetItemChecked(4, _chf);
            checkedListBoxCountries.SetItemChecked(5, _aud);
            checkedListBoxCountries.SetItemChecked(6, _cad);
            checkedListBoxCountries.SetItemChecked(7, _cny);
            checkedListBoxCountries.SetItemChecked(8, _nzd);

            alertUpDown.Value = _alertMins;
            refreshUpDown.Value = _refreshMins;
            string[] fullpath = _alertFile.Split('\\');
            textBoxAlertFile.Text = fullpath[fullpath.Length - 1];
            checkTodaysNews.Checked = _todaysNewsOnly;
        }

        private void SaveDefaultSettings(bool save)
        {
            _impactLow = _ens.ImpactLow = impactCheckLow.Checked;
            _impactMedium = _ens.ImpactMedium = impactCheckMedium.Checked;
            _impactHigh = _ens.ImpactHigh = impactCheckHigh.Checked;

            _usd = _ens.Usd = checkedListBoxCountries.GetItemChecked(0);
            _eur = _ens.Eur = checkedListBoxCountries.GetItemChecked(1);
            _jpy = _ens.Jpy = checkedListBoxCountries.GetItemChecked(2);
            _gbp = _ens.Gbp = checkedListBoxCountries.GetItemChecked(3);
            _chf = _ens.Chf = checkedListBoxCountries.GetItemChecked(4);
            _aud = _ens.Aud = checkedListBoxCountries.GetItemChecked(5);
            _cad = _ens.Cad = checkedListBoxCountries.GetItemChecked(6);
            _cny = _ens.Cny = checkedListBoxCountries.GetItemChecked(7);
            _nzd = _ens.Nzd = checkedListBoxCountries.GetItemChecked(8);

            _alertMins = _ens.AlertMinutes = (int)alertUpDown.Value;
            _refreshMins = _ens.RefreshInterval = (int)refreshUpDown.Value;
            _ens.AlertFile = _alertFile;
            _todaysNewsOnly = _ens.TodaysNews = checkTodaysNews.Checked;

            if (save) _ens.SaveXmlSettings();
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            SaveDefaultSettings(false);
            LoadNews();
            mainTabControl.SelectedIndex = 0;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectedIndex = 0;
        }

        public void LoadNews()
        {
            if (_myTimer == null)
            {
                _myTimer = new Timer();
                _myTimer.Interval = 1000;
                _myTimer.Tick += MyTimerProcessor;
                _myTimer.Start();
            }

            _lastLoad = DateTime.Now;
            _nextLoad = _lastLoad.AddMinutes((double)refreshUpDown.Value);
            label2.Text = _lastLoad.ToString("MMM-dd hh:mm tt");
            label2.ForeColor = Color.Green;
            //_lastLoadError = null;
            _templist = new List<NewsEvent>();
            try
            {
                // add a random query string to defeat server side caching.
                string urltweak = FfNewsUrl + "?x=" + Convert.ToString(DateTime.Now.Ticks);

                // ReSharper disable AccessToStaticMemberViaDerivedType
                HttpWebRequest newsReq = (HttpWebRequest)HttpWebRequest.Create(urltweak);
                // ReSharper restore AccessToStaticMemberViaDerivedType
                newsReq.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Reload);

                // fetch the xml doc from the web server

                using (HttpWebResponse newsResp = (HttpWebResponse)newsReq.GetResponse()) // check that we got a valid reponse
                    if (newsResp != null && newsResp.StatusCode == HttpStatusCode.OK)
                    {
                        // read the response stream into and xml document
                        Stream receiveStream = newsResp.GetResponseStream();
                        Encoding encode = Encoding.GetEncoding("utf-8");
                        StreamReader readStream = new StreamReader(receiveStream, encode);
                        string xmlString = readStream.ReadToEnd();

                        XmlDocument newsDoc = new XmlDocument();
                        newsDoc.LoadXml(xmlString);

                        // build collection of events

                        int itemId = 0;

                        if (newsDoc.DocumentElement != null)
                            foreach (XmlNode xmlNode in newsDoc.DocumentElement.ChildNodes)
                            {
                                NewsEvent newsEvent = new NewsEvent();
                                newsEvent.Time = xmlNode.SelectSingleNode("time").InnerText;
                                if (string.IsNullOrEmpty(newsEvent.Time))
                                    continue; // ignore tentative events!
                                newsEvent.Date = xmlNode.SelectSingleNode("date").InnerText;
                                // assembly and convert event date/time to local time.
                                newsEvent.DateTimeLocal = DateTime.SpecifyKind(DateTime.Parse(newsEvent.Date + " " + newsEvent.Time, _ffDateTimeCulture), DateTimeKind.Utc).ToLocalTime();
                                // filter events based on settings...
                                DateTime startTime = DateTime.Now;
                                DateTime endTime = startTime.AddDays(1);

                                // filter news events based on various property settings...
                                if (newsEvent.DateTimeLocal < startTime || (_todaysNewsOnly && newsEvent.DateTimeLocal.Date >= endTime.Date))
                                    continue;

                                newsEvent.ID = ++itemId;
                                newsEvent.Country = xmlNode.SelectSingleNode("country").InnerText;

                                if (!checkedListBoxCountries.CheckedItems.Contains(newsEvent.Country.ToUpper()) && newsEvent.Country.ToUpper() != "ALL")
                                    continue;

                                newsEvent.Forecast = xmlNode.SelectSingleNode("forecast").InnerText;
                                newsEvent.Impact = xmlNode.SelectSingleNode("impact").InnerText;

                                if ((!_impactLow && newsEvent.Impact.ToUpper() == "LOW")
                                    || (!_impactMedium && newsEvent.Impact.ToUpper() == "MEDIUM")
                                    || (!_impactHigh && newsEvent.Impact.ToUpper() == "HIGH"))
                                    continue;

                                newsEvent.Previous = xmlNode.SelectSingleNode("previous").InnerText;
                                newsEvent.Title = xmlNode.SelectSingleNode("title").InnerText;
                                newsEvent.AlertFired = false;
                                newsEvent.AlertChecked = false;
                                _templist.Add(newsEvent);
                            }
                        ReconcileLists();
                        PopulateGlacialList(_list);
                    }
                    else // handle unexpected scenarios...
                        if (newsResp == null)
                            throw new Exception("Web response was null.");
                        else
                            throw new Exception(string.Format("Web response status code = {0}", newsResp.StatusCode));
            }
            catch (Exception)
            {
                //_lastLoadError = ex.Message;
                label2.Text = "Unsuccesful";
                label2.ForeColor = Color.Red;
            }
        }

        private void ReconcileLists()
        {
            for (int j = 0; j < _templist.Count; j++)
            {
                for (int i = 0; i < _list.Count; i++)
                {
                    if (_templist[j].Time == _list[i].Time
                        && _templist[j].Impact == _list[i].Impact
                        && _templist[j].Country == _list[i].Country
                        && _templist[j].Previous == _list[i].Previous
                        && _templist[j].Forecast == _list[i].Forecast
                        && _templist[j].Title == _list[i].Title)
                    {
                        _templist[j].AlertChecked = _list[i].AlertChecked;
                        _templist[j].AlertFired = _list[i].AlertFired;
                        break;
                    }
                }
            }
            _list = _templist;
        }

        private void MyTimerProcessor(object sender, EventArgs e)
        {
            if (DateTime.Now >= _nextLoad)
                LoadNews();
            TimeSpan tc = _nextLoad - DateTime.Now;
            label4.Text = string.Format("{0:0}:{1:00}:{2:00}", (int)tc.TotalHours, tc.Minutes, tc.Seconds);
            CheckForAlerts();
        }

        private void CheckForAlerts()
        {
            if (_list == null || _list.Count < 1 || newsList == null || newsList.Rows == null || newsList.Rows.Count < 1)
                return;
            TimeSpan alertTimeSpan = new TimeSpan(0, _alertMins, 0);
            TimeSpan fiveSeconds = new TimeSpan(0, 0, 5);
            for (int i = 0; i < newsList.Rows.Count; i++)
            {
                if (newsList.Rows[i].Cells == null)
                    continue;
                if (newsList.Rows[i].Cells.Count < 8)
                    continue;
                if (newsList.Rows[i].Cells[6] == null)
                    continue;

                newsList.Rows[i].Cells[7].Value = GetTimeLeft(_list[i].DateTimeLocal);             

                if (_list[i].AlertFired)
                    continue;

                if (!_list[i].AlertChecked)
                    continue;



                if ((DateTime)newsList.Rows[i].Cells[0].Value - DateTime.Now >= alertTimeSpan)
                    continue;

                if (DateTime.Now - _alertPlayed < fiveSeconds)
                    continue;

                try
                {
                    SoundPlayer player = new SoundPlayer();
                    player.SoundLocation = _alertFile;
                    player.Play();
                }
                catch (Exception)
                {

                }
                _alertPlayed = DateTime.Now;
                _list[i].AlertFired = true;
            }
        }

        private void PopulateGlacialList(IList<NewsEvent> pist)
        {
            newsList.AutoGenerateColumns = false;
            newsList.DataSource = null;
            newsList.DataSource = pist;
            newsList.AutoGenerateColumns = false;
            newsList.Columns[0].DataPropertyName = "DateTimeLocal";
            newsList.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            newsList.Columns[1].DataPropertyName = "Impact";
            newsList.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            newsList.Columns[2].DataPropertyName = "Country";
            newsList.Columns[2].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            newsList.Columns[3].DataPropertyName = "Previous";
            newsList.Columns[3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            newsList.Columns[4].DataPropertyName = "Forecast";
            newsList.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            newsList.Columns[5].DataPropertyName = "Title";
            newsList.Columns[5].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            newsList.Columns[6].DataPropertyName = "AlertChecked";
            newsList.Columns[6].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //for (int i = 0; i < newsList.Rows.Count; i++)
            //{
            //    switch(pist[i].Impact.ToUpper())
            //    {
            //        case "HIGH":
            //            newsList.Rows[i].Cells[1].Style.BackColor = Color.Red;
            //            break;
            //        case "MEDIUM":
            //            newsList.Rows[i].Cells[1].Style.BackColor = Color.Orange;
            //            break;
            //        case "LOW":
            //            newsList.Rows[i].Cells[1].Style.BackColor = Color.Yellow;
            //            break;
            //    }
            //}
        }

        private static string GetTimeLeft(DateTime t)
        {
            if (DateTime.Now > t) return "History";

            TimeSpan ts = t - DateTime.Now.AddMinutes(-1);

            string days = (int)ts.TotalDays == 1 ? "Day" : "Days";
            string hours = ts.Hours == 1 ? "Hr" : "Hrs";


            if ((int)ts.TotalDays > 0)
                return string.Format("{0} {1} {2:00} {3} {4:00} Min", (int)ts.TotalDays, days, ts.Hours, hours, ts.Minutes);
            if ((int)ts.TotalHours > 0)
                return string.Format("{0} {1} {2:00} Min", ts.Hours, hours, ts.Minutes);
            return (int)ts.TotalMinutes > 1 ? string.Format("{0} Min", (int)ts.TotalMinutes) : string.Format("{0} Seconds", ts.Seconds);
        }

        private void buttonUpdateNow_Click(object sender, EventArgs e)
        {
            LoadNews();
        }

        private void buttonLookupAlertFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = _fp;

            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                string[] fullpath = openFileDialog1.FileNames[0].Split('\\');
                textBoxAlertFile.Text = fullpath[fullpath.Length - 1];
                SoundPlayer player = new SoundPlayer();
                player.SoundLocation = _alertFile = openFileDialog1.FileName;
                player.Play();
            }
            catch (Exception)
            {
            }
        }

        private void buttonSaveDefaults_Click(object sender, EventArgs e)
        {
            SaveDefaultSettings(true);
            LoadNews();
            mainTabControl.SelectedIndex = 0;
        }

        private void buttonLoadDefaults_Click(object sender, EventArgs e)
        {
            LoadDefaultSettings();
        }

        private void newsList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 6 || e.RowIndex < 0 || e.RowIndex >= _list.Count)
                return;
            if (_list[e.RowIndex].AlertChecked)
            {
                _list[e.RowIndex].AlertChecked = false;
                _list[e.RowIndex].AlertFired = false;
            }
            else
            {
                _list[e.RowIndex].AlertChecked = true;
            }
            newsList.Invalidate();
        }
    }
}
