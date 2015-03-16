using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;


namespace ConsoleApplication2
{
    class Program
    {
        #region declare
        private static string BaseUrl = "http://osialm12.orasi.com";
        private static string LoadUrl = "LoadTest/rest";
        private static string Credentials = "jon.fowler:Katie123";
        private static string CredentialsExample = "joe.doe:abc123";

        private static System.Net.Http.HttpClient HttpClient1;

        private static string TestId = "1";
        private static string TestInstanceId = "1";
        private static string TimeSlotHours = "0";
        private static string TimeSlotMinutes = "30";
        private static string PostRunAction = "Do Not Collate";
        private static string TestSetId = "1";
        private static string TestRunId = "1";
        private static string TestGroupName = "TestGroup1";
        private static string ScriptId = "1";

        private static bool Action_GetTests;
        private static bool Action_GetTestGroups;
        private static bool Action_GetTestInstanceRun;
        private static bool Action_GetTestInstanceRunStatus;

        private static bool Action_CreateTest;
        private static bool Action_CreateTestGroup;
        private static bool Action_CreateTestInstance;
        private static bool Action_CreateTestInstanceRun;
        
        private static bool Action_Help;
#endregion
        static void Main(string[] args)
        {
            //For testing
            //AuthenticateB();
            //return;

            ParseArgs(args);

            if (Action_Help)
            {
                Help();
                return;
            }

            HttpClient1 = new System.Net.Http.HttpClient();
            HttpClient1.BaseAddress = new System.Uri(BaseUrl);
            HttpClient1.DefaultRequestHeaders.Accept.Clear();
            HttpClient1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

            Authenticate();

            //GET
            if (Action_GetTests)                 GetTests();
            if (Action_GetTestGroups)            GetTestGroups(TestId);
            if (Action_GetTestInstanceRun)       GetRuns();
            if (Action_GetTestInstanceRunStatus) GetRunStatus(TestRunId);

            //POST
            if (Action_CreateTest)               CreateTest();
            if (Action_CreateTestGroup)          CreateTestGroup(TestId, TestGroupName, ScriptId);
            if (Action_CreateTestInstance)       CreateTestInstance(TestId, TestSetId, TestInstanceId);
            if (Action_CreateTestInstanceRun)    CreateTestInstanceRun(TestId, TestInstanceId, TimeSlotHours, TimeSlotMinutes, PostRunAction);

            //Get_Results_Metadata();
            //Get_Result_Metadata();
            //Get_Result_Report_Data();
            
            LogOut();
        }

        private static void Help()
        {
            string fn = System.AppDomain.CurrentDomain.FriendlyName;
            Console.WriteLine("{0} Help", fn);
            Console.WriteLine();

            Console.WriteLine("Parameters (Connect): ");
            Console.WriteLine("/BaseUrl <Base Url> (Default = {0})", BaseUrl);
            Console.WriteLine("/LoadUrl <Load Url> (Default = {0})", LoadUrl);
            Console.WriteLine("/Credentials <username:password> (Default = {0})", CredentialsExample);
            Console.WriteLine();

            Console.WriteLine("Parameters (Properties):");
            Console.WriteLine("/TestId <Test Id> (Default = {0})", TestId);
            Console.WriteLine("/TestInstanceId <Test Instance Id> (Default = {0})", TestInstanceId);
            Console.WriteLine("/TestSetId <Test Set Id> (Default = {0})", TestSetId);
            Console.WriteLine("/TestRunId <Test Run Id> (Default = {0})", TestRunId);
            Console.WriteLine("/TimeSlotHours <Time Slot Hours> (Default = {0})", TimeSlotHours);
            Console.WriteLine("/TimeSlotMinutes <Time Slot Minutes> (Default = {0})", TimeSlotMinutes);
            Console.WriteLine("/PostRunAction <Post Run Action> (Default = {0})", PostRunAction);
            Console.WriteLine("/TestGroupName <Test Group Name> (Default = {0})", TestGroupName);
            Console.WriteLine("/ScriptId <Script Id> (Default = {0})", ScriptId);
            Console.WriteLine();

            Console.WriteLine("Parameters (Actions GET/Read):");
            Console.WriteLine("/GetTests");
            Console.WriteLine("/GetTestGroups");
            Console.WriteLine("/GetRuns");
            Console.WriteLine("/GetRunStatus");
            Console.WriteLine();
            Console.WriteLine("Parameters (Actions POST/Write):");
            Console.WriteLine("/CreateTest");
            Console.WriteLine("/CreateTestGroup");
            Console.WriteLine("/CreateTestInstance");
            Console.WriteLine("/CreateTestInstanceRun");
            Console.WriteLine();

            Console.WriteLine("Examples:");
            Console.WriteLine("{0} /GetTests /BaseUrl {1} /LoadUrl {2} /Credentials {3}", fn, BaseUrl, LoadUrl, CredentialsExample);
            Console.WriteLine("{0} /GetTests", fn);
            Console.WriteLine("{0} /GetTestGroups /TestId {1}", fn, TestId);
            Console.WriteLine("{0} /GetRuns", fn);
            Console.WriteLine("{0} /GetRunStatus /TestRunId {1}", fn, TestRunId);
            Console.WriteLine("{0} /GetTests /GetRuns /GetRunStatus /TestRunId {1}", fn, TestRunId);
            Console.WriteLine("{0} /CreateTest (Currently uses a hard coded xml file named Test2.xml)", fn);
            Console.WriteLine("{0} /CreateTestInstance /TestId {1} /TestInstanceId {2} /TetSetId {3}",
                fn, TestId, TestInstanceId, TestSetId);
            Console.WriteLine("{0} /CreateTestGroup /TestId {1} /TestGroupName {2} /ScriptId {3}",
                fn, TestId, TestGroupName, ScriptId);
            Console.WriteLine("{0} /CreateTestInstanceRun /TestId {1} /TestInstanceId {2} /TetSetId {3} /TimeSlotHours {4} /TimeSlotMinutes {5} /PostRunAction {6}",
                fn, TestId, TestInstanceId, TestSetId, TimeSlotHours, TimeSlotMinutes, PostRunAction);

            //if (CREATETESTINSTANCE) CreateTestInstance(TESTID, TESTSETID, TESTINSTANCEID);
            //if (CREATETESTINSTANCERUN) CreateTestInstanceRun(TESTID, TESTINSTANCEID, TIMESLOTHOURS, TIMESLOTMINUTES, POSTRUNACTION);
        }

        private static void ParseArgs(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "/?":
                        Action_Help = true;
                        break;
                    case "/baseurl":
                        BaseUrl = args[++i];
                        break;
                    case "/loadurl":
                        LoadUrl = args[++i];
                        break;
                    case "/credientials":
                        Credentials = args[++i];
                        break;
                    case "/testid":
                        TestId = args[++i];
                        break;
                    case "/testinstanceid":
                        TestInstanceId = args[++i];
                        break;
                    case "/testrunid":
                        TestRunId = args[++i];
                        break;
                    case "/testsetid":
                        TestSetId = args[++i];
                        break;
                    case "/timslothours":
                        TimeSlotHours = args[++i];
                        break;
                    case "/timslotminutes":
                        TimeSlotMinutes = args[++i];
                        break;
                    case "/testgroupname":
                        TestGroupName = args[++i];
                        break;
                    case "/scriptid":
                        ScriptId = args[++i];
                        break;
                    case "/gettests":
                        Action_GetTests = true;
                        break;
                    case "/gettestgroups":
                        Action_GetTestGroups = true;
                        break;
                    case "/getruns":
                        Action_GetTestInstanceRun = true;
                        break;
                    case "/getrunstatus":
                        Action_GetTestInstanceRunStatus = true;
                        break;
                    case "/createtest":
                        Action_CreateTest = true;
                        break;
                    case "/createtestgroup":
                        Action_CreateTestGroup = true;
                        break;
                    case "/createtestinstance":
                        Action_CreateTestInstance = true;
                        break;
                    case "/createtestinstancerun":
                        Action_CreateTestInstanceRun = true;
                        break;
                    default:
                        break;

                }
            }
        }
        private static void Authenticate()
        {
            if (string.IsNullOrEmpty(Credentials))
            {
                throw new Exception("Credentials were not supplied.");
            }
            var url = string.Format("{0}/{1}", LoadUrl, "authentication-point/authenticate");
            byte[] cred = UTF8Encoding.UTF8.GetBytes(Credentials);
            HttpClient1.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(cred));
            //HTTPCLIENT.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            
            HttpResponseMessage message = HttpClient1.GetAsync(url).Result;

            string description = string.Empty;
            if (message.IsSuccessStatusCode)
            {
                string result = message.Content.ReadAsStringAsync().Result;
                description = result;
            }
            else
            {
                description = message.ReasonPhrase;
                throw new Exception(string.Format("Error authenticating. URL: {0}. Reason phrase: {1}. Request message:{2}.", url, message.ReasonPhrase, message.RequestMessage));
            }
        }
        private static void AuthenticateB()
        {
            //The version for the blog
            var baseAddress = "http://osialm12.orasi.com";
            //var baseAddress = "https://osialm12.orasi.com:8443/qcbin";
            //var baseAddress = "http://pso.orasi.com/qcbin";
        
            var url = "LoadTest/rest/authentication-point/authenticate";
            var credentials = "joe.doe:password";

            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            
            client.BaseAddress = new System.Uri(baseAddress);
            byte[] cred = UTF8Encoding.UTF8.GetBytes(credentials);
            client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(cred));

            HttpResponseMessage message = null;
            try
            {
                message = client.GetAsync(url).Result;
            }
            catch(System.Net.WebException we)
            {
                Console.Write(we.Message);
            }
            //catch(System.AggregateException ae)
            //{
            //    Console.Write(ae.InnerExceptions.Count);
            //    Console.Write(ae.InnerExceptions[0].Message);
            //    Console.Write(ae.Message);
            //}
            

            if (!message.IsSuccessStatusCode)
            {
                Console.WriteLine(
                    string.Format(
                        "Error authenticating. Reason phrase: {0}. Request message:{1}.",
                        message.ReasonPhrase, message.RequestMessage
                        )
                    );
            }
        }

        private static void LogOut()
        {
            var url = string.Format("{0}/{1}", LoadUrl, "authentication-point/logout");

            HttpClient1.CancelPendingRequests();
            HttpResponseMessage message = HttpClient1.GetAsync(url).Result;

            string description = string.Empty;
            if (message.IsSuccessStatusCode)
            {
                string result = message.Content.ReadAsStringAsync().Result;
                description = result;
            }
            else
            {
                description = message.ReasonPhrase;
                throw new Exception(string.Format("Error Logging out. URL: {0}. Reason phrase: {1}. Request message:{2}.", url, message.ReasonPhrase, message.RequestMessage));
            }

        }

        /// <summary>
        /// Returns the metadata on the results of a run.
        /// </summary>
        private static void Get_Results_Metadata()
        {
            var url = string.Format("{0}/{1}", LoadUrl, "domains/ORASI_INTERNAL/projects/Orasi_Internal_Sandbox/Runs/8/Results");

            HttpResponseMessage message = HttpClient1.GetAsync(url).Result;

            string description = string.Empty;
            if (message.IsSuccessStatusCode)
            {
                string result = message.Content.ReadAsStringAsync().Result;
                description = result;
            }
            else
            {
                description = message.ReasonPhrase;
                throw new Exception(string.Format("Error getting results metadata. URL: {0}. Reason phrase: {1}. Request message:{2}.", url, message.ReasonPhrase, message.RequestMessage));
            }
        }

        /// <summary>
        /// Returns the metadata for a single run result..
        /// </summary>
        private static void Get_Result_Metadata()
        {
            var url = string.Format("{0}/{1}", LoadUrl, "domains/ORASI_INTERNAL/projects/Orasi_Internal_Sandbox/Runs/8/Results/1020");

            HttpResponseMessage message = HttpClient1.GetAsync(url).Result;

            string description = string.Empty;
            if (message.IsSuccessStatusCode)
            {
                string result = message.Content.ReadAsStringAsync().Result;
                description = result;
            }
            else
            {
                description = message.ReasonPhrase;
                throw new Exception(string.Format("Error getting result metadata. URL: {0}. Reason phrase: {1}. Request message:{2}.", url, message.ReasonPhrase, message.RequestMessage));
            }
        }
        private static void Get_Result_Report_Data()
        {
            var url = string.Format("{0}/{1}", LoadUrl, "domains/ORASI_INTERNAL/projects/Orasi_Internal_Sandbox/Runs/8/Results/1020/data");

            HttpResponseMessage message = HttpClient1.GetAsync(url).Result;

            string description = string.Empty;
            if (message.IsSuccessStatusCode)
            {
                string result = message.Content.ReadAsStringAsync().Result;
                description = result;
            }
            else
            {
                description = message.ReasonPhrase;
                throw new Exception(string.Format("Error getting result report data. URL: {0}. Reason phrase: {1}. Request message:{2}.", url, message.ReasonPhrase, message.RequestMessage));
            }
        }

        private static void GetRuns()
        {
            var url = string.Format("{0}/{1}", LoadUrl, "domains/ORASI_INTERNAL/projects/Orasi_Internal_Sandbox/Runs");

            HttpResponseMessage message = HttpClient1.GetAsync(url).Result;

            string description = string.Empty;
            if (message.IsSuccessStatusCode)
            {
                string result = message.Content.ReadAsStringAsync().Result;
                Console.WriteLine("Get_Runs()");
                ListRuns(result);
                Console.WriteLine("");
            }
            else
            {
                description = message.ReasonPhrase;
                throw new Exception(string.Format("Error getting run status. URL: {0}. Reason phrase: {1}. Request message:{2}.", url, message.ReasonPhrase, message.RequestMessage));
            }
        }
        private static void ListRun(string runXml)
        {
            XmlDocument xmld = new XmlDocument();
            xmld.LoadXml(runXml);

            XmlElement root = xmld.DocumentElement;
            Console.WriteLine("Get_Run_Status()");
            OutputRun(root);
            Console.WriteLine();
        }

        private static void ListRuns(string runsXml)
        {
            XmlDocument xmld = new XmlDocument();
            xmld.LoadXml(runsXml);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmld.NameTable);
            nsmgr.AddNamespace("runs", "http://www.hp.com/PC/REST/API");

            XmlElement root = xmld.DocumentElement;

            XmlNodeList runs = root.SelectNodes("//runs:Run", nsmgr);
            foreach (XmlNode run in runs)
            {
                OutputRun(run);
            }
        }

        private static void OutputRun(XmlNode run)
        {
            Console.WriteLine("TestID: {0}, TestInstanceID: {1}, TimeslotID: {2}, ID: {3}, Duration: {4}, RunState: {5}, RunSLAStatus: {6}",
                run["TestID"].InnerText, run["TestInstanceID"].InnerText, run["TimeslotID"].InnerText, run["ID"].InnerText, run["Duration"].InnerText,
                run["RunState"].InnerText, run["RunSLAStatus"].InnerText);
            //, run["PostRunAction"].InnerText);
            // , PostRunAction: {7}
        }

        private static void OutputRun(XmlElement run)
        {
            Console.WriteLine("TestID: {0}, TestInstanceID: {1}, TimeslotID: {2}, ID: {3}, Duration: {4}, RunState: {5}, RunSLAStatus: {6}",
                run["TestID"].InnerText, run["TestInstanceID"].InnerText, run["TimeslotID"].InnerText, run["ID"].InnerText, run["Duration"].InnerText,
                run["RunState"].InnerText, run["RunSLAStatus"].InnerText);
            //, run["PostRunAction"].InnerText);
            // , PostRunAction: {7}
        }

        private static void GetRunStatus(string runId)
        {
            var url = string.Format("{0}/{1}/{2}", LoadUrl, "domains/ORASI_INTERNAL/projects/Orasi_Internal_Sandbox/Runs", runId);

            HttpResponseMessage message = HttpClient1.GetAsync(url).Result;

            string description = string.Empty;
            if (message.IsSuccessStatusCode)
            {
                string result = message.Content.ReadAsStringAsync().Result;
                XmlDocument xmld = new XmlDocument();
                xmld.LoadXml(result);

                Console.WriteLine("Get_Run_Status()");
                OutputRun(xmld.DocumentElement);
                Console.WriteLine();
            }
            else
            {
                description = message.ReasonPhrase;
                throw new Exception(string.Format("Error getting run status. URL: {0}. Reason phrase: {1}. Request message:{2}.", url, message.ReasonPhrase, message.RequestMessage));
            }
        }

        private static void GetTests()
        {
            var url = string.Format("{0}/{1}", LoadUrl, "domains/ORASI_INTERNAL/projects/Orasi_Internal_Sandbox/tests");
            //appending /1 results in "bad request"

            HttpResponseMessage message = HttpClient1.GetAsync(url).Result;
            //string jsonText = Newtonsoft.Json.JsonConvert.DeserializeObject(message, );

            string description = string.Empty;
            if (message.IsSuccessStatusCode)
            {
                string result = message.Content.ReadAsStringAsync().Result;
                Console.WriteLine("Get_Tests()");
                ListTests(result);
                Console.WriteLine();
            }
            else
            {
                description = message.ReasonPhrase;
            }
        }
        private static void GetTestGroups(string testId)
        {
            var url = string.Format("{0}/{1}/{2}/{3}", LoadUrl, "domains/ORASI_INTERNAL/projects/Orasi_Internal_Sandbox/tests", testId, "Groups");

            HttpResponseMessage message = HttpClient1.GetAsync(url).Result;

            string description = string.Empty;
            if (message.IsSuccessStatusCode)
            {
                string result = message.Content.ReadAsStringAsync().Result;
                Console.WriteLine("GetTestGroups()");
                //ListTests(result);
                Console.WriteLine(result);
                Console.WriteLine();
            }
            else
            {
                description = message.ReasonPhrase;
            }
        }


        private static void ListTests(string testsXml)
        {
            XmlDocument xmld = new XmlDocument();
            xmld.LoadXml(testsXml);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmld.NameTable);
            nsmgr.AddNamespace("tests", "http://www.hp.com/PC/REST/API");

            //XmlNode test;
            XmlElement root = xmld.DocumentElement;
            //test = root.SelectSingleNode("//tests:Test", nsmgr);

            XmlNodeList xmlNodeList = root.SelectNodes("//tests:Test", nsmgr);
            foreach (XmlNode n in xmlNodeList)
            {
                Console.WriteLine("ID: {0}, Name: {1}, Created By: {2}, LastModified: {3}, TestFolderPath: {4}", n["ID"].InnerText, n["Name"].InnerText, n["CreatedBy"].InnerText, n["LastModified"].InnerText, n["TestFolderPath"].InnerText);
            }
        }

        private static void Test_CreationB()
        {
            var baseUrl = "http://osialm12.orasi.com";
            var endpointUrl = "/LoadTest/rest/authentication-point/authenticate";
            var credentials = "joe.doe:abc123";
            HttpResponseMessage message;

            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            client.BaseAddress = new System.Uri(baseUrl);

            byte[] cred = UTF8Encoding.UTF8.GetBytes(credentials);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(cred));
            message = client.GetAsync(endpointUrl).Result;

            if (!message.IsSuccessStatusCode)
            {
                Console.WriteLine(message.ReasonPhrase);
            }

            endpointUrl = "/LoadTest/rest/domains/ORASI_INTERNAL/projects/Orasi_Internal_Sandbox/tests";

            XmlDocument xmld = new XmlDocument();
            var xmltext = System.IO.File.ReadAllText(@"C:\Orasi\VS\Projects\HPPCRestTest1\Test.xml");
            xmld.LoadXml(xmltext); //test valid xml formatting

            client.DefaultRequestHeaders.Accept.Clear();
            //HTTPCLIENT.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

            System.Net.Http.HttpContent content = new StringContent(xmltext, UTF8Encoding.UTF8, "application/xml");
            HttpContentHeaders headers = content.Headers;
            message = client.PostAsync(endpointUrl, content).Result;
            headers = content.Headers;

            string description = string.Empty;
            if (message.IsSuccessStatusCode)
            {
                string result = message.Content.ReadAsStringAsync().Result;
                description = result;
            }
            else
            {
                description = message.ReasonPhrase;
            }
        }
        private static void CreateTest()
        {
            var url = string.Format("{0}/{1}", LoadUrl, "domains/ORASI_INTERNAL/projects/Orasi_Internal_Sandbox/tests");

            XmlDocument xmld = new XmlDocument();
            var xmltext = System.IO.File.ReadAllText(@"C:\Orasi\VS\Projects\HPPCRestTest1\Test2.xml");
            xmld.LoadXml(xmltext); //test valid xml formatting 

            System.Net.Http.HttpContent content = new StringContent(xmltext, UTF8Encoding.UTF8, "application/xml");
            HttpContentHeaders headers = content.Headers;
            HttpResponseMessage message = HttpClient1.PostAsync(url, content).Result;
            headers = content.Headers;

            string result = message.Content.ReadAsStringAsync().Result;

            Console.WriteLine("Test_Creation()");
            if (message.IsSuccessStatusCode)
            {
                Console.WriteLine("Test creation succeeded with the following information:");
                XmlDocument resultXmld = new XmlDocument();
                resultXmld.LoadXml(result);
                //Test_Instance_Creation(n) //should call this with test id
            }
            else
            {
                Console.WriteLine("Test creation failed with the following information:");
                Console.WriteLine("ReasonPhrase: {0}", message.ReasonPhrase);
                Console.WriteLine("Result: {0}", result);
            }
            Console.WriteLine("");
        }

        private static void CreateTestInstance(string testId, string testSetId, string testInstanceId)
        {
            var url = string.Format("{0}/{1}", LoadUrl, "domains/ORASI_INTERNAL/projects/Orasi_Internal_Sandbox/testInstances");

            var testInstanceCreationXml = string.Format(
                "<TestInstance xmlns=\"http://www.hp.com/PC/REST/API\">" + 
                "    <TestID>{0}</TestID>" +
                "    <TestSetID>{1}</TestSetID>" +
                "    <TestInstanceID>{2}</TestInstanceID>" +
                "</TestInstance>", testId, testSetId, testInstanceId);

            XmlDocument xmld = new XmlDocument();
            xmld.LoadXml(testInstanceCreationXml); //test valid xml formatting

            System.Net.Http.HttpContent content = new StringContent(testInstanceCreationXml, UTF8Encoding.UTF8, "application/xml");
            HttpContentHeaders headers = content.Headers;
            HttpResponseMessage message = HttpClient1.PostAsync(url, content).Result;
            headers = content.Headers;

            string result = message.Content.ReadAsStringAsync().Result;

            Console.WriteLine("Test_Instance_Creation()");
            if (message.IsSuccessStatusCode)
            {
                Console.WriteLine("Test_Instance_Creation succeeded with the following information:");
                XmlDocument resultXmld = new XmlDocument();
                resultXmld.LoadXml(result);
                Console.WriteLine("Result: {0}", result);
                //OutputRun(resultXmld.DocumentElement);
            }
            else
            {
                Console.WriteLine("Test_Instance_Creation failed with the following information:");
                Console.WriteLine("ReasonPhrase: {0}", message.ReasonPhrase);
                Console.WriteLine("Result: {0}", result);
            }
            Console.WriteLine("");
        }

        private static void CreateTestInstanceRun(string testId, string testInstance, string timeslotHours, string timeslotMinutes, string postRunAction)
        {
            var url = string.Format("{0}/{1}", LoadUrl, "domains/ORASI_INTERNAL/projects/Orasi_Internal_Sandbox/Runs");

            var testRunCreationXml = string.Format(
                "<Run xmlns=\"http://www.hp.com/PC/REST/API\">" +
                "<PostRunAction>{4}</PostRunAction>" +
                "<TestID>{0}</TestID>" +
                "<TestInstanceID>{1}</TestInstanceID>" +
                "<TimeslotDuration>" +
                "    <Hours>{2}</Hours>" +
                "    <Minutes>{3}</Minutes>" +
                "</TimeslotDuration>" +
                "<VudsMode>false</VudsMode>" +
                "</Run>", testId, testInstance, timeslotHours, timeslotMinutes, postRunAction);

            XmlDocument xmld = new XmlDocument();
            xmld.LoadXml(testRunCreationXml); //test valid xml formatting

            //string jsonText = Newtonsoft.Json.JsonConvert.SerializeXmlNode(xmld);

            System.Net.Http.HttpContent content = new StringContent(testRunCreationXml, UTF8Encoding.UTF8, "application/xml");
            HttpResponseMessage message = HttpClient1.PostAsync(url, content).Result;

            string result = message.Content.ReadAsStringAsync().Result;

            Console.WriteLine("Test_Run_Creation()");
            if (message.IsSuccessStatusCode)
            {
                Console.WriteLine("Test run creation succeeded with the following information:");
                XmlDocument resultXmld = new XmlDocument();
                resultXmld.LoadXml(result);
                OutputRun(resultXmld.DocumentElement);
            }
            else
            {
                Console.WriteLine("Test run creation failed with the following information:");
                Console.WriteLine("ReasonPhrase: {0}", message.ReasonPhrase);
                Console.WriteLine("Result: {0}", result);
                //Result: <Exception xmlns="http://www.hp.com/PC/REST/API">
                //  <ExceptionMessage>Failed to create reservation for new run.</ExceptionMessage>
                //  <ErrorCode>1200</ErrorCode>
                //</Exception>
            }
            Console.WriteLine("");
        }

        private static void CreateTestGroup(string testId, string groupName, string scriptId)
            //string testInstance, string timeslotHours, string timeslotMinutes, string postRunAction)
        {
            var url = string.Format("{0}/{1}/{2}/{3}/{4}", LoadUrl, "domains/ORASI_INTERNAL/projects/Orasi_Internal_Sandbox/tests", testId, "Groups", groupName);

            string vUsers = "5";

            var testRunCreationXml = string.Format(
                "<Group xmlns=\"http://www.hp.com/PC/REST/API\">" +
                "    <Name>{0}</Name>" +
                "    <Vusers>{1}</Vusers>" +
                "    <Script>" +
                "        <ID>{2}</ID>" +
                "    </Script>" +
                "    <RTS>" +
                "        <Pacing>" +
                "            <NumberOfIterations>1</NumberOfIterations>" +
                "            <StartNewIteration Type=\"immediately\"/>" +
                "        </Pacing>" +
                "        <ThinkTime Type=\"replay\"/>" +
                "        <Log Type=\"standard\">" +
                "            <LogOptions Type=\"on error\">" +
                "                <CacheSize>1</CacheSize>" +
                "            </LogOptions>" +
                "        </Log>" +
                "    </RTS>" +
                "</Group>", groupName, vUsers, scriptId);
            #region textxml2

            var testRunCreationXml2 = string.Format(
                "<Groups xmlns=\"http://www.hp.com/PC/REST/API\">" +
                "  <Group>" +
                "    <Name>{0}</Name>" +
                "    <Vusers>{1}</Vusers>" +
                "    <Script>" +
                "      <ID>{2}</ID>" +
                "    </Script>" +
                "    <RTS>" +
                "      <Pacing>" +
                "        <NumberOfIterations>1</NumberOfIterations>" +
                "        <StartNewIteration Type=\"random interval\">" +
                "          <DelayAtRangeOfSeconds>60</DelayAtRangeOfSeconds>" +
                "          <DelayAtRangeToSeconds>90</DelayAtRangeToSeconds>" +
                "        </StartNewIteration>" +
                "      </Pacing>" +
                "      <ThinkTime Type=\"random\">" +
                "        <MinPercentage>50</MinPercentage>" +
                "        <MaxPercentage>150</MaxPercentage>" +
                "        <LimitThinkTimeSeconds>0</LimitThinkTimeSeconds>" +
                "      </ThinkTime>" +
                "      <Log Type=\"extended\">" +
                "        <ParametersSubstituion>true</ParametersSubstituion>" +
                "        <DataReturnedByServer>true</DataReturnedByServer>" +
                "        <AdvanceTrace>false</AdvanceTrace>" +
                "        <LogOptions Type=\"on error\">" +
                "          <CacheSize>100</CacheSize>" +
                "        </LogOptions>" +
                "      </Log>" +
                "    </RTS>" +
                "  </Group>" +
                "</Groups>", groupName, vUsers, scriptId);
            #endregion
            XmlDocument xmld = new XmlDocument();
            xmld.LoadXml(testRunCreationXml); //test valid xml formatting
            var ssss = xmld.InnerXml.ToString();

            System.Net.Http.HttpContent content = new StringContent(testRunCreationXml, UTF8Encoding.UTF8, "application/xml");
            HttpResponseMessage message = HttpClient1.PostAsync(url, content).Result;

            string result = message.Content.ReadAsStringAsync().Result;

            Console.WriteLine("Test_Group_Creation()");
            if (message.IsSuccessStatusCode)
            {
                Console.WriteLine("Test group creation succeeded with the following information:");
                XmlDocument resultXmld = new XmlDocument();
                resultXmld.LoadXml(result);
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine("Test group creation failed with the following information:");
                Console.WriteLine("ReasonPhrase: {0}", message.ReasonPhrase);
                Console.WriteLine("Result: {0}", result);
            }
            Console.WriteLine("");
        }

    }

}
