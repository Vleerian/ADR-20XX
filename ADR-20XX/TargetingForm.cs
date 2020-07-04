using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net;
using System.IO.Compression;
using System.IO;
using System.Xml.Linq;
using System.Threading;

namespace GoldenTubes
{
    public partial class TargetingForm : Form
    {
        //====================VARIABLE DECLARATIONS====================//
        private WebClient client = new WebClient();
        private Dictionary<string, Dictionary<string, string>> regionDict;
        private List<string> regionList;
        private Dictionary<string, Dictionary<string, string>> nationDict;
        private List<string> nationList;
        private Dictionary<string, Dictionary<string, string>> raidableDict;
        private List<int>UpdateLength;
        string YearMonthDay = "";
        string Target = "";
        bool running = false;
        Thread upThread;
        string User = "";
        bool UpdateTheTime = true;
		private System.Windows.Forms.Timer m_timerUpdateElements = null;
		private DateTime m_LastClockTime = DateTime.MinValue;
		//============================================================//

		public TargetingForm()
        {
            InitializeComponent();

            //This sets the "YearMonthDay" variable that is used in filenames. Format is YYYY-MM-DD
            YearMonthDay = DateTime.Today.Year.ToString() + "-" + DateTime.Today.Month.ToString() + "-" + DateTime.Today.Day.ToString();
            try
            {
                //Attempts to load JSON data parsed from the day's daily data dump
                regionDict = getJSONData<Dictionary<string, Dictionary<string, string>>>(YearMonthDay + "RDict.JSON");
                regionList = getJSONData<List<string>>(YearMonthDay + "RList.JSON");
                nationDict = getJSONData<Dictionary<string, Dictionary<string, string>>>(YearMonthDay + "NDict.JSON");
                nationList = getJSONData<List<string>>(YearMonthDay + "NList.JSON");
                raidableDict = getJSONData<Dictionary<string, Dictionary<string, string>>>(YearMonthDay + "TDict.JSON");
                UpdateLength = getJSONData<List<int>>("UpdateLength.JSON");
            }
            catch
            {
                //If it fails, it will notify the user that no JSON data was found, and will instantiate all the variables.
                MessageBox.Show("No JSON data found. Updating, please wait...", "Notice");
                regionDict = new Dictionary<string, Dictionary<string, string>>();
                regionList = new List<string>();
                nationDict = new Dictionary<string, Dictionary<string, string>>();
                nationList = new List<string>();
                raidableDict = new Dictionary<string, Dictionary<string, string>>();
                UpdateLength = new List<int>();
                //It will then update the data
                updateData();
            }
            //Starts the variance calculator thread
            running = true;

            upThread = new Thread(ThreadUClock);
            upThread.Start();
            while (!upThread.IsAlive) ; //Waits for the thread to begin

			m_timerUpdateElements = new System.Windows.Forms.Timer();
			m_timerUpdateElements.Tick += UpdateElements;
			UpdateElements(null, null);
			m_timerUpdateElements.Start();
        }

		private void TargetButton_Click(object sender, EventArgs e)
        {
            if(TargetName.Text.Trim() != "")
            {
                Target = TargetName.Text.ToLower().Replace(' ', '_');
                if(!regionDict.ContainsKey(Target))
                {
                    MessageBox.Show("Invalid target - region does not exist.", "Error");
                    return;
                }
                string First = regionDict[Target]["FirstNation"];
                int index = Convert.ToInt32(nationDict[First]["Index"]);
                string region = nationList[Convert.ToInt32(Math.Round((((index * 0.04) - Convert.ToInt32(TriggerTime.Text)) / 0.04)))];
                region = nationDict[region]["Region"];
                ManualRegion.Text = region;

            }
            else
            {
                MessageBox.Show("No target entered.", "Error");
            }
        }

        //Helper function for loading JSON data from files
        private T getJSONData<T>(string file)
        {
            StreamReader fs = new StreamReader(file);
            string JSON = fs.ReadToEnd();
            T returnData = JsonConvert.DeserializeObject<T>(JSON);
            return returnData;
        }

		//Helper function for saving JSON data files
		private void saveJSONData(string data, string fileName)
        {
            FileStream fstream = new FileStream(fileName, FileMode.Create);
            byte[] outstream = Encoding.ASCII.GetBytes(data.ToCharArray(), 0, data.Length);
            fstream.Write(outstream, 0, outstream.Length);
        }

        //Helper function for getting the index of a nation in the nation list. Returns -1 if it is not in the dictionary.
        private int getNationIndex(string nation)
        {
            if (nationDict.ContainsKey(nation))
                return Convert.ToInt32(nationDict[nation]["Index"]);
            else
                return -1;
            
        }

        private void updateData()
        {
            byte[] file; //This will contain the region data dump file.
            try
            {
                //If we already have the region data dump, don't download it again.
                file = File.ReadAllBytes(YearMonthDay + "regions.gz");
            }
            catch
            {
                //If we don't have it, download it
                client.DownloadFile("http://www.nationstates.net/pages/regions.xml.gz", YearMonthDay + "regions.gz");
                file = File.ReadAllBytes(YearMonthDay + "regions.gz");
            }
            string text; //The JSON string
            using (GZipStream stream = new GZipStream(new MemoryStream(file), CompressionMode.Decompress))
            {
                //Decrompress the data dump. I don't know remember what half this shit does
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    byte[] decompressed = memory.ToArray();
                    text = System.Text.ASCIIEncoding.ASCII.GetString(decompressed);
                }

            }

            List<string> passwordList = new List<string>();
            List<string> founderlessList = new List<string>();

            client.Headers.Add("user-agent", "ALL HAIL 20XX. Startup request to get passworded regions. Main Dev - doomjaw@hotmail.com");
            string xmlSrc = client.DownloadString("https://www.nationstates.net/cgi-bin/api.cgi?q=regionsbytag;tags=password");
            foreach (string i in xmlSrc.Replace("<WORLD><REGIONS>", "").Replace("</WORLD></REGIONS>", "").Split(',').ToList<string>())
                passwordList.Add(i.ToLower().Replace(' ', '_'));
            client.Headers.Add("user-agent", "ALL HAIL 20XX. Startup request to get founderless regions. Main Dev - doomjaw@hotmail.com");
            xmlSrc = client.DownloadString("https://www.nationstates.net/cgi-bin/api.cgi?q=regionsbytag;tags=founderless");
            foreach (string i in xmlSrc.Replace("<WORLD><REGIONS>", "").Replace("</WORLD></REGIONS>", "").Split(',').ToList<string>())
                founderlessList.Add(i.ToLower().Replace(' ', '_'));

            //Parse the XML string from the data dump into the LINQ XDocument object
            XDocument xmlDoc = XDocument.Parse(text);
            //How many regions we've iterated through
            int counter = 0;
            //how many nations we've iterated through
            int nationcounter = 0;
            foreach (XElement Region in xmlDoc.Descendants("REGION"))
            {
                counter++;
                Dictionary<string, string> tmp = new Dictionary<string, string>(); //Create a dictionary that will store region data
                string name = Region.Element("NAME").Value;
                tmp.Add("Name", name); //Add a name attribute
                tmp.Add("Index", counter.ToString()); //Add an index attribute (useful in conjunction with the region list)
                tmp.Add("NumNations", Region.Element("NUMNATIONS").Value); //Add an attribute for the number of nations in the region
                tmp.Add("FirstNation", Region.Element("NATIONS").ToString().Replace("<NATIONS>", "").Replace("</NATIONS>", "").Split(':')[0]); //Firstnation is the first updating nation in the region
                string founder = "True";
                if (founderlessList.Contains(name.ToLower().Replace(' ', '_')))
                    founder = "False";
                tmp.Add("Foundered", founder); //Add an attribute for the founder of the region
                string password = "False";
                if (passwordList.Contains(name.ToLower().Replace(' ', '_')))
                    password = "True";
                tmp.Add("Passworded", password); //Passworded shit
                tmp.Add("Delegate", Region.Element("DELEGATE").Value); //Add an attribute for the delegate of the region
                tmp.Add("DelegateVotes", Region.Element("DELEGATEVOTES").Value); //Add an attribute for the delegate of the region
                tmp.Add("DelegateAuth", Region.Element("DELEGATEAUTH").Value); //CHECKMATE, [VIOLET]
                foreach (string Nation in Region.Element("NATIONS").ToString().Replace("<NATIONS>", "").Replace("</NATIONS>", "").Split(':'))
                {
                    nationcounter++;
                    nationList.Add(Nation); //Add to the nation list
                    Dictionary<string, string> tmp2 = new Dictionary<string, string>(); //Create a dictionary that will store nation data
                    tmp2.Add("Name", Nation); //Name Attribute
                    tmp2.Add("Index", nationcounter.ToString()); //Index attribute (Equivalent to #of nations before it)
                    tmp2.Add("Region", name);
                    if (Nation == tmp["FirstNation"])
                        tmp.Add("FirstNationIndex", nationcounter.ToString()); //If it's the first nation in the region, store it's index in the region.
                    if (Nation.Trim() != "" && !nationDict.ContainsKey(Nation.ToLower().Replace(' ', '_')))
                        nationDict.Add(Nation.ToLower().Replace(' ', '_'), tmp2); //There was a few issues with natons lacking a name, so we take these.
                }
                if (!regionDict.ContainsKey(Region.Element("NAME").Value.ToLower().Replace(' ', '_')))
                    regionDict.Add(Region.Element("NAME").Value.ToLower().Replace(' ', '_'), tmp); //If it doesn't exist in the regiondict, add that shit
                if (!regionList.Contains(Region.Element("NAME").Value.ToLower().Replace(' ', '_')))
                    regionList.Add(Region.Element("NAME").Value.ToLower().Replace(' ', '_')); //If it doens't exist in the list add that shit.
                if ((regionDict[name.ToLower().Replace(' ', '_')]["Foundered"] == "False" ||
                    regionDict[name.ToLower().Replace(' ', '_')]["DelegateAuth"].Contains("X")) &&
                    regionDict[name.ToLower().Replace(' ', '_')]["Passworded"] == "False")
                {
                    raidableDict.Add(Region.Element("NAME").Value.ToLower().Replace(' ', '_'), tmp);
                }
            }



            //Finally, save al lof them into JSON format files
            saveJSONData(JsonConvert.SerializeObject(regionDict), YearMonthDay + "RDict.JSON");
            saveJSONData(JsonConvert.SerializeObject(regionList), YearMonthDay + "RList.JSON");
            saveJSONData(JsonConvert.SerializeObject(nationDict), YearMonthDay + "NDict.JSON");
            saveJSONData(JsonConvert.SerializeObject(nationList), YearMonthDay + "NList.JSON");
            saveJSONData(JsonConvert.SerializeObject(raidableDict), YearMonthDay + "TDict.JSON");
        }

        //A function for use with threads :DDDDDDDDDDDDDDDDDD
        private void ThreadUClock()
        {
            TimeSpan TodayStamp; //Timestamp of the current date at the Major/Minor update
            TimeZoneInfo TZInfo;

            while (running) //DO THIS SHIT UNTIL I TELL YOU TO STOP
            {
                TZInfo = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

				DateTime ESTNow = TimeZoneInfo.ConvertTime(DateTime.Now, TZInfo);

				DateTime UpdateStart = ESTNow;
				UpdateStart = UpdateStart.AddSeconds(-UpdateStart.Second);
				UpdateStart = UpdateStart.AddMinutes(-UpdateStart.Minute);
				if (UpdateStart.Hour >= 12)
					UpdateStart = UpdateStart.AddHours(12 - UpdateStart.Hour);
				else
					UpdateStart = UpdateStart.AddHours(-UpdateStart.Hour);

				TodayStamp = TimeZoneInfo.ConvertTimeToUtc(UpdateStart, TZInfo) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

				if (!UpdateTheTime || User.Trim() == "") //This is so the user can tell it to stop updating the time.
                {
                    Thread.Sleep(500); //This is here so that we don't keep running through a continue command every 1000th of a second.
                    continue;
                }
                //This is at the beginning of the loop so that regardless of what happens, it will not run over the request limit.
                Thread.Sleep(1000);
                try
                {
                    //I SWEAR VIOLET, I'LL BE A GOOD BOY
                    client.Headers.Add("user-agent", "ALL HAIL 20XX. Currently in use by " + User + ". Main Dev - doomjaw@hotmail.com");
					//Grab the latest changes like they're juicy dicks
					string xmlSrc = client.DownloadString("https://www.nationstates.net/cgi-bin/api.cgi?q=happenings;filter=change;limit=5");
					XDocument xmlDoc = XDocument.Parse(xmlSrc); //Parsing.
                    double NationStamp = 0;
                    XElement EventWeCareAbout = null; //Self documenting!
					var elmHappenings = xmlDoc.Root.Element("HAPPENINGS");
					if (elmHappenings != null)
					{
						foreach (XElement Event in elmHappenings.Elements())
						{
							if (Event.Element("TEXT").Value.Contains("influence in") || Event.Element("TEXT").Value.Contains("was ranked in the"))
							{
								EventWeCareAbout = Event;
							}
							NationStamp = Convert.ToInt64(Event.Element("TIMESTAMP").Value); //Parsing				
						}
					}

                    if (EventWeCareAbout != null)
                    {
                        NationStamp = Convert.ToInt64(EventWeCareAbout.Element("TIMESTAMP").Value); //Parsing
                    }
                    else if (NationStamp == 0)
                    {
                        continue;
                    }

                    int NationIndex = (EventWeCareAbout != null) ? getNationIndex(EventWeCareAbout.Element("TEXT").ToString().Split('@')[2]) : -1; //Parsing
                    double NationTime; //This is the time that we expect the CTE'd nation to have updated at, given our estimates
                    double TimePerNation; //This is how long nations take to update.
                    if (NationIndex != -1) //Only do what's in here if we have data on the CTE'd nation
                    {
                        //Calculate the time per nation (SecondsIntoUpdate / NationsUpdatedBefore)
                        string NationName = nationList[NationIndex];
                        if(nationDict.ContainsKey(NationName))
                            TimePerNation = (NationStamp - TodayStamp.TotalSeconds) / Convert.ToInt64(nationDict[NationName]["Index"]);
                        else
                        {
                            TimePerNation = 0.03f;
                        }

                        //Use the time per nation to extrapolate how long the update length is (We use this outside of the update)
                        if (UpdateLength.Count != 0)
                            UpdateLength[(ESTNow.Hour >= 12) ? 1 : 0] = (int)(nationList.Count * TimePerNation);
                        else
                        {
                            UpdateLength = new List<int>();
                            UpdateLength.Add(5400);
                            UpdateLength.Add(3600);
                        }

                        //We now calculate what time the CTE'd nation should be updating at given the data we've collected
                        NationTime = NationIndex * TimePerNation;
                    }
                    else
                    {
                        if (UpdateLength.Count == 0) //You can never error check enough.
                        {
                            UpdateLength = new List<int>();
                            UpdateLength.Add(5400);
                            UpdateLength.Add(3600);
                        }
                        //We need this so that our program doesn't get dicked on if someone creates a nation like 'fuckshitpiss' and it get's CTE'd
                        //TimePerNation = UpdateLength/NumberOfNations
                        TimePerNation = (double)UpdateLength[(ESTNow.Hour >= 12) ? 1 : 0] / (double)nationList.Count;
                        //We don't actually have any data, so we'll just calculate how many seconds into the update the CTE happened.
                        NationTime = NationStamp - TodayStamp.TotalSeconds;
                    }

                    //Our estimate - UpdateStartTime + Seconds into the update we believe the CTE'd nation should be updating
                    double Estimate = TodayStamp.TotalSeconds + NationTime;
                    //Cumulative Variance = Actual - Estimate. We compare the time the nation actually updates compared to our estimate
                    double VarianceCumulative = NationStamp - Estimate;
                    //Because we need to account for variance that has not taken place yet, we need to extrapolate
                    double VariancePerNation = VarianceCumulative / NationIndex;

                    if (Target.Trim() != "" && regionList.Contains(Target.ToLower().Replace(' ', '_'))) //If we have a target
                    {
                        //Our approximate is the first updating nation in that region * the time it takes each nation to update
                        //We can safely use that nation's index because it's index represents the number of nations before it (as lists start with 0, not 1)
                        int TargetIndex = Convert.ToInt32(regionDict[Target.ToLower().Replace(' ', '_')]["FirstNationIndex"]);

                        double approxtime = TargetIndex * TimePerNation;
                        double Variance = TargetIndex * VariancePerNation;
                        //We add the variance in, and convert it to a time...
                        TimeSpan t = TimeSpan.FromSeconds(approxtime + Variance);
                        //And spit it out for use by the user in the format of HH:MM:SS
                        UpdateTime(t.ToString(@"hh\:mm\:ss").ToString());
                    }
                    else
                    {
                        //If we have no target, just spit out 00:00:00
                        UpdateTime("00:00:00");
                    }

                    //No matter what happens, sleep 5 seconds because I'M A GOOD BOY VIOLET
                }
                catch(WebException)
                {
                    Thread.Sleep(10000);
                }
            }
        }

        //The only thing this little shit does is provide a thread-safe way to change the "UpdateTime" label
        private delegate void delUpdateTime(string Time);
        private void UpdateTime(string Time)
        {
            if (InvokeRequired)
            {
                //Honestly, this is just a really complicated way to say "Call this shit with these arguments."
                Invoke(new delUpdateTime(UpdateTime), new object[] { Time });
            }
            else
            {
                //We called shit, and now we just set shit.
                EstimateTime.Text = Time;
            }
        }

        private void TargetingForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Set running to false to tell that loop up in the thread function to STOP SLAPPING YOUR DICK AROUND
            running = false;
            //If it didn't get the message, jam a coathanger in it's vagina.
            upThread.Abort();
			m_timerUpdateElements.Stop();
            //Wait for these fuckers to stop whining.
            while (upThread.IsAlive) ;
            //Save the update length
            saveJSONData(JsonConvert.SerializeObject(UpdateLength), "UpdateLength.JSON");
        }

		private void UpdateElements(object sender, EventArgs e)
		{
			if (!running)
			{
				m_timerUpdateElements.Stop();
				return;
			}

			TimeZoneInfo TZInfo = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
			DateTime ESTNow = TimeZoneInfo.ConvertTime(DateTime.Now, TZInfo);
			DateTime ESTNowNormalized = ESTNow.AddMilliseconds(-ESTNow.Millisecond);

			if (m_LastClockTime != ESTNowNormalized)
			{
				m_LastClockTime = ESTNowNormalized;
				CurrentTime.Text = ESTNow.ToString(@"hh\:mm\:ss");
			}

			// set up the timer to fire again at the next second transition
			m_timerUpdateElements.Interval = 1000 - ESTNow.Millisecond;
		}

        private void UserName_TextChanged(object sender, EventArgs e)
        {
            User = UserName.Text;
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            if(UpdateTheTime)
            {
                StopButton.Text = "Engage Engine";
                UpdateTheTime = false;
            }
            else
            {
                StopButton.Text = "Disengage Engines";
                UpdateTheTime = true;
            }
        }
    }
}
