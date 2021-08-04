using UnityEngine;
using System;
using System.Text;
using System.Threading;
using System.Net.Sockets;


public class Client
{
    public TCP Tcp { get => tcp; private set => tcp = value; }

    private TCP tcp;

    public Client(string host, int port)
    {
        try
        {
            TcpClient client = new TcpClient(host, port);
            if (client.Client.Connected)
            {
                tcp = new TCP();
                tcp.Connect(client);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception encountered: {ex}");
        }
    }

    public void Close()
    {
        tcp?.Close();
    }
    public class TCP
    {
        public TcpClient socket;
        private NetworkStream stream;
        private byte[] receive_buffer;

        public void Connect(TcpClient socket)
        {
            this.socket = socket;
            socket.ReceiveBufferSize = Constants.API__CLIENT_BUFFER_SIZE;
            socket.SendBufferSize = Constants.API__CLIENT_BUFFER_SIZE;

            stream = socket.GetStream();
            // Register as game client
            Byte[] _write = Encoding.ASCII.GetBytes("/REGISTER-GAME-CLIENT");
            stream.Write(_write, 0, _write.Length);

            receive_buffer = new byte[Constants.API__CLIENT_BUFFER_SIZE];

            stream.BeginRead(receive_buffer, 0, Constants.API__CLIENT_BUFFER_SIZE, ReceiveCallback, null);
        }
        public void Close()
        {
            socket.Close();
            stream.Close();
        }

        public void ActionCallback()
        {
            HandleData("/GET-ENV-STATE");
        }
        public void AbortCallback()
        {
            string respond = "/ABORT";

            Byte[] _write = Encoding.ASCII.GetBytes(respond);
            stream.Write(_write, 0, _write.Length);
        }

        private void ReceiveCallback(IAsyncResult res)
        { 
            try
            {
                if (!socket.Connected) return;

                int byte_length = stream.EndRead(res);
                if (byte_length <= 0)
                {
                    return;
                }

                byte[] data = new byte[byte_length];
                Array.Copy(receive_buffer, data, byte_length);

                HandleData(Encoding.ASCII.GetString(data));

                stream.BeginRead(receive_buffer, 0, Constants.API__CLIENT_BUFFER_SIZE, ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error on receiving TCP data: {ex}");
            }
        }
        
        private void HandleData(string command)
        {
            if (command.Equals("/GET-ENV-STATE"))
            {
                APIDirector.GetInstance().GetEnvState();                
            }
            else if (command.Contains("/ACTION"))
            {
                // WE'LL PROCESS THE MESSAGE HERE!
                string[] api_commands = command.Split(' ');

                // PARSING STRINGS TO NUMBERS
                int action, associated_id = 0;
                action = int.Parse(api_commands[1]);
                if (api_commands.Length > 2)
                {
                    associated_id = int.Parse(api_commands[2]);
                }

                // SORT ACTIONS OUT
                if (action == Constants.API__END_TURN_CODE)
                {
                    APIDirector.GetInstance().EndTurn();
                }
                else if (action <= Constants.PLAYER__MAXIMUM_CARDS_ON_BOARD)
                {
                    APIDirector.GetInstance().AttackCard(action - 1, associated_id - 1);
                }
                else
                {
                    action -= Constants.PLAYER__MAXIMUM_CARDS_ON_BOARD;
                    if (associated_id == 0)
                    {
                        APIDirector.GetInstance().PlayCardFromHand(action - 1);
                    }
                    else
                    {
                        // NOT THE FINAL DUEL
                        if (associated_id <= 10 || associated_id >= 80)
                        {
                            APIDirector.GetInstance().PlayCardFromHand(action - 1, (associated_id - 1) % 10, associated_id / 10 != 0);
                        }
                        // THE FINAL DUEL
                        else
                        {
                            APIDirector.GetInstance().PlayCardFromHand(action - 1, associated_id, false);
                        }
                    }
                }
            }
        }

        public void SendEnvironmentState(string json)
        {
            string respond = "/ENV-STATE " + json;

            Byte[] _write = Encoding.ASCII.GetBytes(respond);
            stream.Write(_write, 0, _write.Length);
        }

        public void GameOver()
        {
            string respond = "/GAME-OVER";

            Byte[] _write = Encoding.ASCII.GetBytes(respond);
            stream.Write(_write, 0, _write.Length);
        }
    }
}
