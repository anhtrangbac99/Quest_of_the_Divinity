import random
import json
from datetime import datetime
import numpy as np
import pandas as pd
from Config.config import *
from Card.card import *
import torch



def reservoir_random( set_size, K ):
    result = []
    N = 0
    for item in range(set_size):
        N += 1
        if len( result ) < K:
            result.append( item )
        else:
            s = int(random.random() * N)
            if s < K:
                result[ s ] = item
    return result

def decode_state_old(str_env, isImitation = False):
    player_life = [str_env['player_life']]
    player_gold = [str_env['player_life']] 
    opponent_life = [str_env['opponent_life']]
    time = [str_env['time']]
    player_board_card_info = list()
    opponent_board_card_info = list()

    for x in str_env['player_board_card_info']:
        player_board_card_info.append(x['id'])
        player_board_card_info.append(x['status'])
        player_board_card_info.append(x['damage'])
        player_board_card_info.append(x['life'])

    for x in str_env['opponent_board_card_info']:
        opponent_board_card_info.append(x['id'])
        opponent_board_card_info.append(x['status'])
        opponent_board_card_info.append(x['damage'])
        opponent_board_card_info.append(x['life'])
        
    player_hand_card_id = str_env['player_hand_card_id']
    
    state = player_life + player_gold + opponent_life + time + player_board_card_info + opponent_board_card_info + player_hand_card_id

    if isImitation:
        state += [str_env['action']]
        
    return state 

def decode_state_old_test(str_env, isImitation = False):
    player_life = [str_env['player_life']]
    player_gold = [str_env['player_life']] 
    opponent_life = [str_env['opponent_life']]
    player_board_card_info = [0 for i in range(56)]
    opponent_board_card_info = [0 for i in range(56)]
    player_hand_card_id = [0 for i in range(56)]
    
    player_board_card_info_position = list()
    opponent_board_card_info_position = list()
    card_info = pd.read_csv(CARD_ID_FILE_PATH)

    for x in str_env['player_board_card_info']:
        player_board_card_info_position.append(x['id'])
        player_board_card_info_position.append(x['status'])
        player_board_card_info_position.append(x['damage'])
        player_board_card_info_position.append(x['life'])

        card = Card(x['id'],card_info)
        player_board_card_info_position.extend(card.statusList())


    for x in str_env['opponent_board_card_info']:
        opponent_board_card_info_position.append(x['id'])
        opponent_board_card_info_position.append(x['status'])
        opponent_board_card_info_position.append(x['damage'])
        opponent_board_card_info_position.append(x['life'])
        
        card = Card(x['id'],card_info)
        opponent_board_card_info_position.extend(card.statusList())
        
    player_hand_card_id_position = str_env['player_hand_card_id']
    
    
    for x in str_env['player_board_card_info']:
        if x['id'] in [0,-1]:
            continue
        player_board_card_info[(x['id']-1)] = 1
#         player_board_card_info[(x['id']-1)*4 + 1] = x['status']
#         player_board_card_info[(x['id']-1)*4 + 2] = x['damage']
#         player_board_card_info[(x['id']-1)*4 + 3] = x['life']
        
    for x in str_env['opponent_board_card_info']:
        if x['id'] in [0,-1]:
            continue
        opponent_board_card_info[(x['id']-1)] = 1
#         opponent_board_card_info[(x['id']-1)*4 + 1] = x['status']
#         opponent_board_card_info[(x['id']-1)*4 + 2] = x['damage']
#         opponent_board_card_info[(x['id']-1)*4 + 3] = x['life']
        
    for x in str_env['player_hand_card_id']:
        if x in [0,-1]:
            continue
    
    state = player_life + player_gold + opponent_life + player_board_card_info + opponent_board_card_info + player_hand_card_id + player_board_card_info_position + opponent_board_card_info_position + player_hand_card_id_position

    if isImitation:
        state += [str_env['action']]
        
    return state 

def decode_state(str_env, isImitation = False):
    player_life = [str_env['player_life']]
    player_gold = [str_env['player_life']] 
    opponent_life = [str_env['opponent_life']]
    player_board_card_info = [0 for i in range(56)]
    opponent_board_card_info = [0 for i in range(56)]
    player_hand_card_id = [0 for i in range(56)]
    
    player_board_card_info_position = list()
    opponent_board_card_info_position = list()
    card_info = pd.read_csv(CARD_ID_FILE_PATH)

    for x in str_env['player_board_card_info']:
        player_board_card_info_position.append(x['id'])
        player_board_card_info_position.append(x['status'])
        player_board_card_info_position.append(x['damage'])
        player_board_card_info_position.append(x['life'])

        card = Card(x['id'],card_info)
        player_board_card_info_position.extend(card.statusList())


    for x in str_env['opponent_board_card_info']:
        opponent_board_card_info_position.append(x['id'])
        opponent_board_card_info_position.append(x['status'])
        opponent_board_card_info_position.append(x['damage'])
        opponent_board_card_info_position.append(x['life'])
        
        card = Card(x['id'],card_info)
        opponent_board_card_info_position.extend(card.statusList())
        
    player_hand_card_id_position = str_env['player_hand_card_id']
    
    
    for x in str_env['player_board_card_info']:
        if x['id'] in [0,-1]:
            continue
        player_board_card_info[(x['id']-1)] = 1
#         player_board_card_info[(x['id']-1)*4 + 1] = x['status']
#         player_board_card_info[(x['id']-1)*4 + 2] = x['damage']
#         player_board_card_info[(x['id']-1)*4 + 3] = x['life']
        
    for x in str_env['opponent_board_card_info']:
        if x['id'] in [0,-1]:
            continue
        opponent_board_card_info[(x['id']-1)] = 1
#         opponent_board_card_info[(x['id']-1)*4 + 1] = x['status']
#         opponent_board_card_info[(x['id']-1)*4 + 2] = x['damage']
#         opponent_board_card_info[(x['id']-1)*4 + 3] = x['life']
        
    for x in str_env['player_hand_card_id']:
        if x in [0,-1]:
            continue
        
        player_board_card_info[(x-1)]=1
        
    player_remaining_cards_on_deck = str_env['player_remaining_cards_on_deck']
    opponent_remaining_cards = str_env['opponent_remaining_cards']
    player_used_cards = str_env['player_used_cards']
    opponent_used_cards = str_env['opponent_used_cards']

    state = player_life + player_gold + opponent_life + player_board_card_info + opponent_board_card_info + player_hand_card_id + player_board_card_info_position + opponent_board_card_info_position + player_hand_card_id_position + player_remaining_cards_on_deck + opponent_remaining_cards + player_used_cards + opponent_used_cards

    if isImitation:
        state += [str_env['action']]
        
    return state  


def create_log_json(env_state,action,reward,average_reward,best_reward):
    json_data = {
        'player_id': env_state['player_id'],
        'player_life': env_state['player_life'],
        'player_gold': env_state['player_gold'], 
        'opponent_life': env_state['opponent_life'], 
        'time': env_state['time'], 
        'player_board_card_info': env_state['player_board_card_info'],
        'opponent_board_card_info':env_state['opponent_board_card_info'] ,
        'player_hand_card_id': env_state['player_hand_card_id'],
        'player_remaining_cards_on_deck' : env_state['player_remaining_cards_on_deck'],
        'opponent_remaining_cards' : env_state['opponent_remaining_cards'],
        'player_used_cards' : env_state['player_used_cards'],
        'opponent_used_cards' : env_state['opponent_used_cards'],
        'action': int(action),
        'reward': int(reward),
        'average_reward': int(average_reward),
        'best_reward': int(best_reward), 
    }  

    return json_data

def log(save_data,file):
    
    print('### Saving log ##')
    json_data = {
        'logs':save_data
    }  
    
    json.dump(json_data,file,indent=4)

def create_file(game_number):
    current_time = datetime.now().strftime("%Y-%m-%d_%H-%M-%S_")
    string = 'Agent_Github\Log\\' + current_time + str(game_number) + '.json'
    f = open(string,'w+')
    #json.dump({'logs':[]},f)
    return f

def accuracy(pred, label):
    pred = torch.argmax(pred, dim=1).long()
#     print(pred)
    label = torch.argmax(label,dim=1).long()
    acc = torch.mean((pred == label).float())
    pred = pred.detach().cpu().numpy()
    label = label.detach().cpu().numpy()
    p = precision_score(label, pred,average='micro')
    r = recall_score(label, pred,average='micro')
    return p,r,acc 

class AverageMeter(object):

    def __init__(self):
        self.val = 0
        self.avg = 0
        self.sum = 0
        self.count = 0

    def reset(self):
        self.val = 0
        self.avg = 0
        self.sum = 0
        self.count = 0

    def update(self, val, n=1):
        self.val = val
        self.sum += val * n
        self.count += n
        self.avg = self.sum / self.count