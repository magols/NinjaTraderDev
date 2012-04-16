using System;

namespace EcoNewsControl
{
    using System.IO;
    using System.Windows.Forms;
    using System.Xml.Serialization;

    [Serializable]
    public class EcoNewsSettings
    {
        private string _cf;

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

        public EcoNewsSettings()
        {
            _impactLow = true;
            _impactHigh = true;
            _impactMedium = true;
            _usd = _eur = _jpy = _gbp = _chf = _cad = _cny = _aud = _nzd = true;
            _alertFile = "";
            _alertMins = 1;
            _refreshMins = 30;
            _todaysNewsOnly = false;
        }

        [XmlIgnore]
        public string ConfigFile
        {
            get { return _cf; }
            set { _cf = value; }
        }

        public bool TodaysNews
        {
            get { return _todaysNewsOnly; }
            set { _todaysNewsOnly = value; }
        }

        public string AlertFile
        {
            get { return _alertFile; }
            set { _alertFile = value; }
        }

        public int RefreshInterval
        {
            get { return _refreshMins; }
            set { _refreshMins = Math.Max(5, value); }
        }

        public int AlertMinutes
        {
            get { return _alertMins; }
            set { _alertMins = Math.Max(1, value); }
        }

        public bool ImpactHigh
        {
            get { return _impactHigh; }
            set { _impactHigh = value; }
        }

        public bool ImpactMedium
        {
            get { return _impactMedium; }
            set { _impactMedium = value; }
        }

        public bool ImpactLow
        {
            get { return _impactLow; }
            set { _impactLow = value; }
        }

        public bool Usd
        {
            get { return _usd; }
            set { _usd = value; }
        }

        public bool Eur
        {
            get { return _eur; }
            set { _eur = value; }
        }

        public bool Jpy
        {
            get { return _jpy; }
            set { _jpy = value; }
        }

        public bool Gbp
        {
            get { return _gbp; }
            set { _gbp = value; }
        }

        public bool Chf
        {
            get { return _chf; }
            set { _chf = value; }
        }

        public bool Aud
        {
            get { return _aud; }
            set { _aud = value; }
        }

        public bool Cad
        {
            get { return _cad; }
            set { _cad = value; }
        }

        public bool Cny
        {
            get { return _cny; }
            set { _cny = value; }
        }

        public bool Nzd
        {
            get { return _nzd; }
            set { _nzd = value; }
        }

        public bool LoadXmlSettings()
        {
            XmlSerializer mySerializer;
            FileStream myFileStream = null;
            bool fileExists = false;

            try
            {
                // Create an XmlSerializer for the type.
                mySerializer = new XmlSerializer(typeof(EcoNewsSettings));
                FileInfo fi = new FileInfo(_cf + @"TSEcoNews.config");
                // If the config file exists, open it.
                if (fi.Exists)
                {
                    myFileStream = fi.OpenRead();
                    EcoNewsSettings ecoSettings = (EcoNewsSettings)mySerializer.Deserialize(myFileStream);

                    _impactHigh = ecoSettings.ImpactHigh;
                    _impactMedium = ecoSettings.ImpactMedium;
                    _impactLow = ecoSettings.ImpactLow;

                    _alertFile = ecoSettings.AlertFile;
                    _alertMins = ecoSettings.AlertMinutes;
                    _refreshMins = ecoSettings.RefreshInterval;
                    _todaysNewsOnly = ecoSettings.TodaysNews;

                    _usd = ecoSettings.Usd;
                    _eur = ecoSettings.Eur;
                    _jpy = ecoSettings.Jpy;
                    _gbp = ecoSettings.Gbp;
                    _chf = ecoSettings.Chf;
                    _cad = ecoSettings.Cad;
                    _cny = ecoSettings.Cny;
                    _aud = ecoSettings.Aud;
                    _nzd = ecoSettings.Nzd;

                    fileExists = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // If the FileStream is open, close it.
                if (myFileStream != null)
                {
                    myFileStream.Close();
                }
            }

            return fileExists;
        }

        public void SaveXmlSettings()
        {
            StreamWriter myWriter = null;
            XmlSerializer mySerializer;
            try
            {
                // Create an XmlSerializer for the 
                // ApplicationSettings type.
                mySerializer = new XmlSerializer(typeof(EcoNewsSettings));
                myWriter =
                  new StreamWriter(_cf + @"TSEcoNews.config", false);
                mySerializer.Serialize(myWriter, this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // If the FileStream is open, close it.
                if (myWriter != null)
                {
                    myWriter.Close();
                }
            }
        }
    }
}