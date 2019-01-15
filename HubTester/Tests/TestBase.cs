using HubTest;
using Renci.SshNet;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HubTests.Tests
{
    public abstract class TestBase : ITest, IDisposable
    {
        private const int LOGIN_RETRIES = 10;

        protected string ipAddress = HubTester.Properties.Settings.Default.HubIpAddress;
        //protected string sshKeyFile;

        protected SshClient sshClient;
        protected ShellStream shellStream;

        StreamWriter streamWriter;
        StreamReader streamReader;

        static protected NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        const string _prompt_pattern = "root@zeushub:.*#";

        public TestBase()
        {
        }


        protected string ReadLine()
        {
            string line = streamReader.ReadLine();

            logger.Trace($"ReadLine: {line}");

            return line;
        }

        protected string ReadToEnd()
        {
            if (streamReader.EndOfStream)
                return "";

            string line = streamReader.ReadToEnd();

            if (line.Length > 0)
                logger.Trace($"ReadToEnd: {line}");

            return line;
        }

        protected string ReadUntil(Regex regx, int timeout_sec = 10)
        {
            string buffer = "";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();
            while (true)
            {
                buffer += ReadToEnd();
                if (regx.Match(buffer).Success)
                    return buffer;
                if (stopwatch.Elapsed.TotalSeconds > timeout_sec)
                    throw new ReadUntilTimeoutException($"Timeout waiting for {regx} after {timeout_sec}s.\r\nOutput =\r\n{buffer}");
            }
        }

        protected string WriteCommand(string command, int timeout_sec = 1, string prompt=_prompt_pattern, int cmd_delay_ms=150)
        {
            ReadToEnd();

            WriteLine(command);
            Thread.Sleep(cmd_delay_ms);

            string ecmd = Regex.Escape(command + "\r\n");
            Regex regx = new Regex($"({ecmd})(.*)({prompt})", RegexOptions.Singleline);

            string rs = "";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();
            while (true)
            {
                rs += ReadToEnd();
                if (regx.Match(rs).Success)
                    break;
                if (stopwatch.Elapsed.TotalSeconds > timeout_sec)
                    throw new ReadUntilTimeoutException($"Timeout({timeout_sec}s) waiting for: {command}.\r\nOutput =\r\n{rs}");
            }

            // flush
            ReadToEnd();

            // We should get 4 groups, g0=all,g1=command,g2=result,g3=prompt
            // Note that streamWriter.NewLine should be set to "\n"
            var m = regx.Match(rs);

            if (m.Groups.Count < 4)
                throw new WriteCommandException(
                    $"Error executing command: {command}.\r\nReturn was:\r\n{rs}");

            // We return the command result g2
            return m.Groups[2].Value.Trim(new char[] { '\r', '\n' });

        }

        protected void WriteLine(string format, params object[] args)
        {
            string value = string.Format(format, args);

            logger.Trace($"Write: {value}");

            streamWriter.WriteLine(value);
        }

        PrivateKeyFile KeyFile
        {
            get
            {
                string keystr =
@"-----BEGIN RSA PRIVATE KEY-----
MIIEpAIBAAKCAQEA3tC8Xfr1IhxtPVgFyyDP35gcJD3KzM4D8IiDmB4FrfgQCjPS
6EYwHTgWWavwVZzFI0U/FQFGZHh7cFYL1Ah4lt/NO7BIqdaRTplI38a2Xvua3ycf
4fb3BJJ2z+wgg29l0BuVHqSznbBdKFdk5wDaqterH93cJtPD125+fRbizwmM5+Uu
S/Eq6msAMTfN+sM5l+YCLMLsilaJbvYQxnAtSvyeF9yWmSuBYxgAVpDKYx1ZQBDV
xnJOb+cmD1nAZ2krR+n6kDKrUA71BpouljuiK65pmvV9wxqmQu8q8Wd5qtwKtbMG
becjkhpAaxDSbt6zWpuvvKMH6lSxbJcQgpDpVQIDAQABAoIBABEsL0hi/h+Z50Vy
Ekg8iCjodUrJxGaSfjU8oD/KGI/27W2L3vZt6mlmGDrjCIvgoET+okUBKya1LnZS
+2hlGr4uE4hFJuSIF/zhzX8JtqaNZ7tJLajhutoMW8HcpgjbPhKS/aQ1923w2M0y
JyGyrCe8pYC4Pa50ZcSlzHPhnqtsISaC/v1VI2RhYlWlAyc1dW2HLdnieYIrH7Nm
BE5k9ULHoCtNawYtlyOiann3fhUswqFf2fyYUpWh02IQceRvy10iiKeIcOlK4Q6h
8gCRcgB/8koh5WkUvQNLGPh6qtoKlpNJuA7O0niC5sF/58Gjn13Iah1+zth6dfFH
q0WOwAUCgYEA+yv/w7qESzwcdwTfqdGKgcQaextcAQ5hiBs362k9yvsC59MHcwjb
MYSDCtlOhh2hWdzoGuneH3Ec/xhXroQGwQssBmnQrqbS9THgQgR4rIjbBnW53uk+
Yi5C3iOzY1w/kWyHokNE69fKwt+gv2G52yzqDMnUJXZzt/tTteeOxfMCgYEA4xky
OfJGKnZ3JbtNtfcDq93mn7Ja1l4SnFcNLP8QpYgJYCl8EoEG6CkbxStBWhT9GN/D
pk/36y8E5KBZTC4Bd+ze6IpGMKzk6H2nFqk0guR2fvdn8M5/96Ytri/lFLMx2yk5
abcIIIx5ZEjBj0xthg0qn0DQL1/+2yWp6I6j/ZcCgYALXQk8PLMBiF2tUM4hq7Y9
erbL/QfjkFf/RWP5RZCjH0oZXOXIYY0xJ1KaagxwauTC4QirNwtylrZ+IAbPgW+g
yjWasKzdSfTfXPtNYVBoVeS63RakrB7DaF5kGG9kPmct2CAmyi5TETc+K8nk/rHe
5aBNJdBwRXSkzT0TTL+b2wKBgQC1v9qEqbMiFMFyfx7IfLKBgAwszu8IFS3L1ZeD
5XaLI/5s6YaUwJKohw3klKOu1pFgsOTCW8nMfUrrNrGA2Gjwc7BKZy+ZkSV6M+Xf
qveNsg5rutGC1aFwr5xrhYVPNcK25Z0/T+6/Le2RJvFSIBafbDYqUhLLd4ZeGRyM
NGxmbQKBgQDjYVFKreD1arVdrIZKsvIfi/xUBIntV8+DwHafIDg90jGM0Ccb7/Vu
62eakRhfnXAOl8zYcWh4Eyd9BduZomvXyo1qH9ETbxxro65MH6gZ+IuXgMKlPs1f
oa+scorRkCJkGyyHJK+PZL8kEnc7tKMoeBnpJ9cHEUVCklf2etylGw==
-----END RSA PRIVATE KEY-----";

                MemoryStream mem = new MemoryStream(Encoding.UTF8.GetBytes(keystr));

                return new PrivateKeyFile(mem);
            }
        }

        private TestStatus testStatus = new TestStatus();
        public TestStatus TestStatus
        {
            get
            {
                return testStatus;
            }
            set
            {
                testStatus = value;
            }

        }

        public string TestErrorTxt
        {
            get
            {
                return TestStatus.ErrorMsg;
            }
            set
            {
                logger.Error($"TestError: {value}");

                TestStatus = new TestStatus
                {
                    Test = this,
                    ErrorMsg = value,
                    PropertyName = TestStatusPropertyNames.ErrorMsg
                };
                OnPropertyChanged(TestStatus.PropertyName.ToString());
            }
        }

        public string TestStatusTxt
        {
            get
            {
                return TestStatus.Status;
            }
            set
            {
                logger.Debug($"TestStatus: {value}");

                TestStatus = new TestStatus
                {
                    Test = this,
                    Status = value,
                    PropertyName = TestStatusPropertyNames.Status
                };
                OnPropertyChanged(TestStatus.PropertyName.ToString());
            }
        }

        public Exception TestStatusException
        {
            get
            {
                return TestStatus.Exception;
            }
            set
            {
                logger.Error(value, "TestStatusExecption");

                TestStatus = new TestStatus
                {
                    Test = this,
                    Exception = value,
                    PropertyName = TestStatusPropertyNames.Exception
                };
                OnPropertyChanged(TestStatus.PropertyName.ToString());
            }
        }

        string hub_eui;
        public string HUB_EUI
        {
            get
            {
                return hub_eui;
            }
            set
            {
                hub_eui = value;

                TestStatus = new TestStatus
                {
                    Test = this,
                    Status = value,
                    PropertyName = TestStatusPropertyNames.HUB_EUI
                };
                OnPropertyChanged(TestStatus.PropertyName.ToString());
            }
        }

        public ShowQuestionDlg TestStatusQuestion
        {
            get
            {
                return TestStatus.ShowQuestionDlg;
            }
            set
            {
                logger.Trace($"TestStatusQuestion: {value.Caption}");

                TestStatus.ShowQuestionDlg = value;

                TestStatus.PropertyName = TestStatusPropertyNames.ShowQuestionDlg;
                OnPropertyChanged(TestStatus.PropertyName.ToString());
            }
        }

        public CancellationToken CancelToken { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual bool Setup()
        {
            if (CancelToken.IsCancellationRequested) { TestStatusTxt = "Connection Canceled"; return false; }

            TestStatusTxt = "Setting up test";
            Connect();

            return true;
        }

        public void Connect()
        {
            var connectionInformation = new ConnectionInfo(ipAddress, "support",
                    new PrivateKeyAuthenticationMethod("support", KeyFile));

            logger.Trace("Connect");
            sshClient = new SshClient(connectionInformation);
            sshClient.Connect();

            shellStream = sshClient.CreateShellStream("SSH Shell", 80, 24, 800, 600, 1024);

            streamWriter = new StreamWriter(shellStream)
            {
                AutoFlush = true,
                NewLine = "\n"
            };
            streamReader = new StreamReader(shellStream);


            WriteLine("su - root");
            Thread.Sleep(100);
            //streamReader.ReadToEnd();
            streamWriter.WriteLine("A1l3r0nR0!!");
            Thread.Sleep(100);
            streamReader.ReadToEnd();

            // See if root login is successful
            WriteCommand("");

        }
        public abstract bool Run();

        public virtual bool TearDown()
        {
            bool tearDownResult = true;

            try
            {
                Dispose();
            }
            catch
            {
                tearDownResult = false;
            }

            return tearDownResult;
        }

        public virtual void Dispose()
        {
            streamWriter.Dispose();
            streamReader.Dispose();
            shellStream.Dispose();
            sshClient.Dispose();

            logger.Trace("Connection Disposed");

        }

        public void ScpUpload(string local, string remote)
        {
            var connectionInformation = new ConnectionInfo(ipAddress, "support",
                    new PrivateKeyAuthenticationMethod("support", KeyFile));

            ScpClient scp = new Renci.SshNet.ScpClient(connectionInformation);
            scp.Connect();
            scp.Upload(new System.IO.FileInfo(local), remote);
            scp.Dispose();

        }

        public void ScpDownload(string remote, string local)
        {
            var connectionInformation = new ConnectionInfo(ipAddress, "support",
                    new PrivateKeyAuthenticationMethod("support", KeyFile));

            ScpClient scp = new Renci.SshNet.ScpClient(connectionInformation);
            scp.Connect();
            scp.Download(remote, new System.IO.FileInfo(local));
            scp.Dispose();
        }

        public string EUIToLittleEndian(string BigEndianEUI)
        {
            byte[] beui = Encoding.ASCII.GetBytes(BigEndianEUI);
            Array.Reverse(beui);
            string eui = "";
            for (int i = 0; i < beui.Length; i += 2)
            {
                char n1 = (char)beui[i];
                char n2 = (char)beui[i + 1];

                eui += string.Format("{0}{1}", n2, n1);
            }

            return eui;
        }

    }
}
