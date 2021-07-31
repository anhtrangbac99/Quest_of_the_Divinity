from Server.train_client import TrainSocket
import json
from Config.config import *

conn = TrainSocket()
conn.connect()
conn.get_env_state()

n_steps = 0
data = conn.sock.recv(BUFFER_SIZE)
string_env = json.loads(data.decode())