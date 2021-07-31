import pandas as pd
import numpy as np
import torch
import json
import os
from Network.deepnetwork2 import ImitationNetwork
from Memory.dataset import *
from torch.utils.data import DataLoader
from Utils.utils import *
from sklearn.metrics import precision_score, recall_score
import torch.nn as nn
from ImitationAgent import *
from Config.config import *

for i in range(10):
    state = {'player_id': 1, 'player_life': 40, 'player_gold': 0, 'opponent_life': 40, 'time': 56.30833435058594, 'player_board_card_info': [{'id': 32, 'status': 2, 'damage': 1, 'life': 2}, {'id': 8, 'status': 0, 'damage': 2, 'life': 3}, {'id': 0, 'status': 0, 'damage': 0, 'life': 0}, {'id': 0, 'status': 0, 'damage': 0, 'life': 0}, {'id': 0, 'status': 
    0, 'damage': 0, 'life': 0}, {'id': 0, 'status': 0, 'damage': 0, 'life': 0}, {'id': 0, 'status': 0, 'damage': 0, 'life': 0}], 'opponent_board_card_info': [{'id': 0, 'status': 0, 'damage': 0, 'life': 0}, {'id': 0, 'status': 0, 'damage': 0, 'life': 0}, {'id': 0, 'status': 0, 'damage': 0, 'life': 0}, {'id': 0, 'status': 0, 'damage': 0, 'life': 
    0}, {'id': 0, 'status': 0, 'damage': 0, 'life': 0}, {'id': 0, 'status': 0, 'damage': 0, 'life': 0}, {'id': 0, 'status': 0, 'damage': 0, 'life': 0}], 'player_hand_card_id': [25, 41, 0, 0, 0, 0, 0, 0, 0], 'player_remaining_cards_on_deck': [1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1], 'opponent_remaining_cards': [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1], 'player_used_cards': [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0], 'opponent_used_cards': [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]}
    agentImitation = ImitationAgent(alpha=0.001,
                    input_dims=516,
                    n_actions=NUM_ACTIONS, mem_size=50000, 
                    batch_size=32, replace=1000,
                    checkpoint_dir=CHECKPOINT_DIR_IMITATION,algo='Imitation',
                    env_name='Quest_of_Divinity')
    agentImitation.load_models()
    a,b,c = agentImitation.choose_action(state)