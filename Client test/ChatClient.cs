﻿using System.Net.Sockets;
using System.Text;

namespace Client_test
{
    public partial class ChatClient : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        private Thread receiveThread;
        int mynum;
        bool isConnected;
        string nickname;
        static string str;
        static bool isGameStarted;
        static Job job;
        internal ShipType ship;

        Dictionary<Job, string> jobDisplay = new Dictionary<Job, string> // enum Job에 따른 한글 표시
        {
            { Job.Robber, "도적" },
            { Job.Cop, "경찰" }
        };
        Dictionary<ShipType, string> shipDisplay = new Dictionary<ShipType, string> // enum ShipType에 따른 한글 표시
        {
            { ShipType.newbie_ship, "초급자 전용 함선" },
            { ShipType.resource_ship, "자원 함선" },
            { ShipType.sailor_ship, "선원 함선" },
            { ShipType.galaxy_moving_ship, "초은하 이동 함선" },
            { ShipType.thief_ship, "도적 함선" }
        };

        public ChatClient()
        {
            InitializeComponent();
            button2.Enabled = false;
            button3.Enabled = false;
            isConnected = false;
            isGameStarted = false;
            comboBox1.SelectedIndex = 0; // default = 전체
        }

        private void button1_Click(object sender, EventArgs e) // 연결
        {
            try
            {
                if (int.TryParse(textBox3.Text, out int port))
                {
                    client = new TcpClient(textBox2.Text, port);
                    stream = client.GetStream();
                    receiveThread = new Thread(ReceiveMessages);
                    receiveThread.IsBackground = true;
                    receiveThread.Start();
                    listBox1.Items.Add("Connected to Server...");
                    isConnected = true;
                    button2.Enabled = true;
                    button3.Enabled = true;
                    button1.Enabled = false;
                    textBox4.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ReceiveMessages()
        {
            byte[] buffer = new byte[102400];
            string msg = "";

            while (true)
            {
                try
                {
                    buffer = new byte[102400];
                    if (msg != "")
                    {
                        buffer = Encoding.UTF8.GetBytes(msg);
                    }
                    while (true)
                    {
                        byte[] data = new byte[256];
                        int bytesRead = stream.Read(data, 0, data.Length);
                        if (bytesRead == 0)
                            break;
                        data = data.Where(x => x != 0).ToArray();
                        if (buffer.Length == 102400)
                        {
                            buffer = data;
                        }
                        else
                        {
                            buffer = buffer.Concat(data).ToArray();
                        }

                        msg = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                        if (msg.Contains('◊'))
                        {
                            break;
                        }
                    }
                    if (Encoding.UTF8.GetString(buffer, 0, buffer.Length).Split("◊").Length == 1)
                    {
                        msg = "";
                    }
                    else
                    {
                        msg = Encoding.UTF8.GetString(buffer, 0, buffer.Length).Split("◊")[1];
                    }
                    string[] message = Encoding.UTF8.GetString(buffer, 0, buffer.Length).Split("◊")[0].Split('⧫');

                    if (message[0] == "0") // 채팅
                    {
                        Invoke(new Action(() => listBox1.Items.Add(message[1])));
                    }
                    else if (message[0] == "1") // 연결 종료
                    {
                        client.Close();
                        if (message[1] != "")
                        {
                            MessageBox.Show(message[1]);
                        }

                        Invoke(new Action(() =>
                        {
                            listBox1.Items.Add("DisConnected from server...");
                            button2.Enabled = false;
                            button3.Enabled = false;
                            button1.Enabled = true;
                            isConnected = false;
                            textBox4.Enabled = true;
                            listBox2.Items.Clear();
                        }));

                        break;
                    }

                    else if (message[0] == "2") // 번호 지정
                    {
                        mynum = int.Parse(message[1]);
                        Invoke(new Action(() => str = textBox4.Text));
                        if (str == "")
                        {
                            nickname = "Client" + mynum.ToString();
                            Invoke(new Action(() => textBox4.Text = nickname));
                        }
                        else if (!str.Contains('⧫'))
                        {
                            nickname = str;
                        }
                        else
                        {
                            MessageBox.Show("이름에 다음 문자가 포함되어서는 안됩니다: ⧫\n기본 이름으로 진행합니다.");
                            nickname = "Client" + mynum.ToString();
                            Invoke(new Action(() => textBox4.Text = nickname));
                        }
                        stream.Write(Encoding.UTF8.GetBytes("3⧫" + nickname + '◊'));
                        stream.Flush();
                    }
                    else if (message[0] == "4") // 접속한 클라이언트 이름
                    {
                        Invoke(new Action(() => listBox2.Items.Add(message[1])));
                    }
                    else if (message[0] == "5") // 접속 종료한 클라이언트 이름
                    {
                        Invoke(new Action(() => listBox2.Items.Remove(message[1])));
                    }
                    else if (message[0] == "6") // 게임 시작
                    {
                        isGameStarted = true;
                        Invoke(new Action(() =>
                        {
                            ShipSelection shipSelection = new ShipSelection(this);
                            shipSelection.Show();
                        }));
                    }
                    else if (message[0] == "7") // 게임 종료
                    {
                        isGameStarted = false;
                    }
                    else if (message[0] == "8") // 역할 전송
                    {
                        job = (Job)int.Parse(message[1]);
                        Invoke(new Action(() => label4.Text = "직업: " + jobDisplay[job] + "\n"
                                    + "함선: " + shipDisplay[ship]));
                    }
                    Invoke(new Action(() => listBox1.TopIndex = listBox1.Items.Count - 1));
                }
                catch (Exception ex)
                {
                    break;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e) // 연결 해제
        {
            stream.Write(Encoding.UTF8.GetBytes("1⧫◊"));
            stream.Flush();
            stream.Close();
            client.Close();
            listBox1.Items.Add("DisConnected from Server...");
            button2.Enabled = false;
            button3.Enabled = false;
            button1.Enabled = true;
            isConnected = false;
            textBox4.Enabled = true;
            listBox2.Items.Clear();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) // 프로그램 종료
        {
            if (isConnected)
            {
                stream.Write(Encoding.UTF8.GetBytes("1⧫◊"));
                stream.Flush();
                stream.Close();
                client.Close();
                listBox1.Items.Add("DisConnected from server...");
                button2.Enabled = false;
                button3.Enabled = false;
                button1.Enabled = true;
                isConnected = false;
                textBox4.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e) // 전송
        {
            if (!textBox1.Text.Contains('⧫') && !textBox1.Text.Contains('◊'))
            {
                if (textBox1.Text != "")
                {
                    stream.Write(Encoding.UTF8.GetBytes("0⧫" + $"{nickname}: " + textBox1.Text + '◊'));
                    stream.Flush();
                    listBox1.Items.Add($"{nickname}: " + textBox1.Text);
                    textBox1.Text = "";
                    listBox1.TopIndex = listBox1.Items.Count - 1;
                }
                else
                {
                    MessageBox.Show("채팅은 공백이면 안됩니다.");
                }
            }
            else
            {
                MessageBox.Show("채팅에 다음 문자는 포함되면 안됩니다: ⧫, ◊");
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && isConnected) // 엔터 누르면 전송
            {
                button3.PerformClick(); // 전송 버튼
            }
        }

        // TODO: 게임 시작, 종료 여부 받아오기
        // TODO: 게임이 시작되지 않았으면 버튼 비활성화

        private void button5_Click(object sender, EventArgs e) // 저장고 확인
        {
            Storage storage = new Storage(this);
            storage.Show();
        }

        private void button4_Click(object sender, EventArgs e) // 할 일 선택
        {
            // TODO: TaskSelection 본인 턴 확인 후 실행
            // TODO: TaskSelection 혹은 다른 Task창이 열러있는지 확인 후 아무것도 열려있지 않을 때만 열기

            //if (본인 턴)
            //{
            TaskSelection taskSelection = new TaskSelection(this, false);
            taskSelection.Show();
            //}
            //else
            //{
            //    TaskSelection taskSelection = new TaskSelection(this, true);
            //    taskSelection.Show();
            //}
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // 엔터 누르면 연결
            {
                button1.PerformClick(); // 연결 버튼
            }
        }
    }

    // 직업 종류
    enum Job
    {
        Robber, // 도둑
        Cop     // 경찰
    }

    // 함선 타입
    enum ShipType
    {
        newbie_ship,             // 초급자 전용 함선
        resource_ship,           // 자원 함선
        sailor_ship,             // 선원 함선
        galaxy_moving_ship,      // 초은하 이동 함선
        thief_ship               // 도적 함선
    }
}