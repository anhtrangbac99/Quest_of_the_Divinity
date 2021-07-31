from abc import abstractproperty
from json.decoder import JSONDecodeError
import numpy as np
from numpy.core.numeric import NaN
from numpy.lib.type_check import nan_to_num
from torch.utils import data
from DQNAgent import DQNAgent
# from utils import plot_learning_curve, make_env
from Server.train_client import TrainSocket
from Config.config import *
from Utils.utils import *
import pandas as pd
import socket
import json
import torch
from Memory.dataset import *
from tqdm import tqdm
from torch.utils.tensorboard import SummaryWriter

if __name__ == '__main__':
    best_score = -1000
    load_checkpoint = LOAD_CHECK_POINT
    n_games = 250

    agent = DQNAgent(gamma=0.1, epsilon=0.7, alpha=0.0001,replace_target_cnt=50,
                     input_dims=INPUT_DIM,
                     n_actions=NUM_ACTIONS, mem_size=50000, eps_min=0.1,
                     batch_size=32, replace=1000, eps_dec=1e-5,
                     checkpoint_dir=CHECKPOINT_DIR, algo='DQNAgent',
                     env_name='Quest_of_Divinity')
    #agent.save_models()
    agent.load_models()
    
    dataset = Feeder(IMITATION_LOG_DIR)
    writer = SummaryWriter('TensorBoard/TrainAgent_DQN')
    # dataloader = torch.utils.data.DataLoader(dataset,batch_size=BATCH_SIZE,shuffle=True,num_workers=NUM_WORKER)
    for i in range(NUM_EPOCHS):
        for idx,string_env in enumerate(dataset):

        # So that the socket is never closed
            #Choose action
            # print(string_env)
            action,reward,state = agent.choose_action(string_env)

            while reward[action] <= 0:
                agent.store(state,action,reward[action],state, False)
                action,reward,state = agent.choose_action(string_env)
        
        
            try:
                next_state = decode_state_old_test(dataset[idx+1])
            except IndexError:
                break

            agent.store(state,action,reward[action],next_state, False)
            agent.learn(writer)
