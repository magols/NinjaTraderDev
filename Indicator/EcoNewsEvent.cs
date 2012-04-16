using System;

namespace EcoNewsControl
{
    public class NewsEvent
    {
		private string country;
		private string date;
		private string forecast;
		private string previous;
		private string impact;
		private string time;
		private DateTime local;
		private int id;
		private string title;
		private bool ac, af;
		
		
		
        public string Country { get{return country;} set{country = value; }}
        public string Date { get{return date;} set{date = value; }}
        public DateTime DateTimeLocal { get{return local;} set{local = value; }}
        public string Forecast { get{return forecast;} set{forecast = value; }}
        public int ID { get{return id;} set{id = value; }}
        public string Impact { get{return impact;} set{impact = value; }}
        public string Previous { get{return previous;} set{previous = value; }}
        public string Time { get{return time;} set{time = value; }}
        public string Title { get{return title;} set{title = value; }}
        public bool AlertFired { get{return af;} set{af = value; }}
        public bool AlertChecked { get{return ac;} set{ac = value; }}
    }
}
