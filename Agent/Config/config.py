
import torch
HOST = '127.0.0.1'
PORT = 27972
BUFFER_SIZE = 4096

LOAD_CHECK_POINT = False
INPUT_DIM = 516
NUM_ACTIONS = 290

ACTION_ID_FILE_PATH = 'Agent_GitHub\ActionIDList.csv'
CARD_ID_FILE_PATH = 'Agent_GitHub\Card\CardIDList.csv'
CHECKPOINT_DIR = 'Agent_GitHub\Models'
CHECKPOINT_DIR_HEUR = 'Agent_GitHub\Models\Heuristic'
CHECKPOINT_DIR_IMITATION = 'Agent_GitHub\Models\\1stImitationAgent_OldData.ckpt'
IMITATION_LOG_DIR = 'Agent_GitHub\Log\ImitationLog\Train\\'
FIGHT_RESULT_DIR = 'Agent_GitHub\\fight.txt'
NFSP_DIR = 'Agent_GitHub\Models\\NFSP'
NFSP_IMITATION_DIR = 'Agent_GitHub\Models\\NFSP\1stImitationAgent_OldData.ckpt'
NFSP_IMITATION_DIR_DDQN = 'Agent_GitHub\Models\\NFSP\1stImitationAgent_OldData_DDQN.ckpt'
# ACTION_ID_FILE_PATH = 'ActionIDList.csv'
# CARD_ID_FILE_PATH = 'Card/CardIDList.csv'
# CHECKPOINT_DIR = 'Models'
# CHECKPOINT_DIR_HEUR = 'Models/Heuristic'
# CHECKPOINT_DIR_IMITATION = 'Models/1stImitationAgent_OldData.ckpt'
# IMITATION_LOG_DIR = 'Log/ImitationLog/Train/'
# FIGHT_RESULT_DIR = 'fight.txt'
# NFSP_DIR = 'Models/NFSP'
# NFSP_IMITATION_DIR = 'Models/NFSP/1stImitationAgent_OldData.ckpt'
# NFSP_IMITATION_DIR_DDQN = 'Models/NFSP/1stImitationAgent_OldData_DDQN.ckpt'
BATCH_SIZE = 128
NUM_WORKER = 10
NUM_EPOCHS = 5
NUM_GROUP_ACTION = 6
NUY = 0.7
DEVICE = torch.device('cuda:0')