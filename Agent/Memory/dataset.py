from torch.utils.data import Dataset 
import json
import os
from Config.config import *
from Utils.utils import *
from Utils.reward import *
import numpy as np
from tqdm import tqdm

#Use for DQN only
class Feeder(Dataset):
    def __init__(self,data_dir):
        self.data_dir = data_dir
        self.paths = os.listdir(data_dir)
        data = list()
        for file_dir in self.paths:
            f = open(data_dir + file_dir)
            try:
                data.extend(json.load(f)['logs'])
            except json.JSONDecodeError:
                f.close()
                continue
            f.close()
        self.data = data

    def __len__(self):
        return len(self.data)

    def __getitem__(self, index):
        return self.data[index]

# New decode - All rewards
class Feeder1(Dataset):
    def __init__(self,data_dir):
        self.data_dir = data_dir
        self.paths = os.listdir(data_dir)
        data = list()
        count = 0
        for file_dir in self.paths:
            f = open(data_dir + file_dir)
            try:
                js = json.load(f)['logs']
                data.extend(js)
            except json.JSONDecodeError:
                f.close()
                continue
            f.close()
#         for i in data:
#             if decode_state_old_test(i,isImitation=True)[-1] == 289:
#                 data.remove(i)
#                 count += 1
#                 continue
#             if count == 1000:
#                 break
        self.data = data

    def __len__(self):
        return len(self.data)

    def __getitem__(self, index):
        action = decode_state(self.data[index],isImitation=True)[-1]
        if action in range(56):
            if action in [7,15,23,31,39,47,55]:
                action = 1
            else:
                action = 0
        elif action in range(56,105):
            action = 1
        elif action in range(105,114):
            action = 2
        elif action in range(114,177):
            action = 3
        elif action in range(177,289):
            action = 4
        elif action == 289:
            action = 5
        reward = Reward(self.data[index]['player_board_card_info'],self.data[index]['opponent_board_card_info'],self.data[index]['player_hand_card_id'],self.data[index]['opponent_life'],self.data[index]['player_life'],self.data[index]['player_gold'])
        # return_reward = [max(reward[:56])] +[max(reward[56:105])] + [max(reward[105:114])]  + [max(reward[114:177])] + [max(reward[177:289])] + [reward[289]]     
        return np.array(decode_state(self.data[index],isImitation=True)[:-1]),np.array(reward),action


# Old decode - All rewards
class Feeder2(Dataset):
    def __init__(self,data_dir):
        self.data_dir = data_dir
        self.paths = os.listdir(data_dir)
        data = list()
        count = 0
        for file_dir in self.paths:
            f = open(data_dir + file_dir)
            try:
                js = json.load(f)['logs']
                data.extend(js)
            except json.JSONDecodeError:
                f.close()
                continue
            f.close()
#         for i in data:
#             if decode_state_old_test(i,isImitation=True)[-1] == 289:
#                 data.remove(i)
#                 count += 1
#                 continue
#             if count == 1000:
#                 break
        self.data = data

    def __len__(self):
        return len(self.data)

    def __getitem__(self, index):
        action = decode_state_old_test(self.data[index],isImitation=True)[-1]
        if action in range(56):
            if action in [7,15,23,31,39,47,55]:
                action = 1
            else:
                action = 0
        elif action in range(56,105):
            action = 1
        elif action in range(105,114):
            action = 2
        elif action in range(114,177):
            action = 3
        elif action in range(177,289):
            action = 4
        elif action == 289:
            action = 5
        reward = Reward(self.data[index]['player_board_card_info'],self.data[index]['opponent_board_card_info'],self.data[index]['player_hand_card_id'],self.data[index]['opponent_life'],self.data[index]['player_life'],self.data[index]['player_gold'])
        # return_reward = [max(reward[:56])] +[max(reward[56:105])] + [max(reward[105:114])]  + [max(reward[114:177])] + [max(reward[177:289])] + [reward[289]]     
        return np.array(decode_state_old_test(self.data[index],isImitation=True)[:-1]),np.array(reward),action

    
# Old decode - Scenario
class Feeder3(Dataset):
    def __init__(self,data_dir):
        self.data_dir = data_dir
        self.paths = os.listdir(data_dir)
        data = list()
        count = 0
        for file_dir in self.paths:
            f = open(data_dir + file_dir)
            try:
                js = json.load(f)['logs']
                data.extend(js)
            except json.JSONDecodeError:
                f.close()
                continue
            f.close()
#         for i in data:
#             if decode_state_old_test(i,isImitation=True)[-1] == 289:
#                 data.remove(i)
#                 count += 1
#                 continue
#             if count == 1000:
#                 break
        self.data = data

    def __len__(self):
        return len(self.data)

    def __getitem__(self, index):
        action = decode_state_old_test(self.data[index],isImitation=True)[-1]
        if action in range(56):
            if action in [7,15,23,31,39,47,55]:
                action = 1
            else:
                action = 0
        elif action in range(56,105):
            action = 1
        elif action in range(105,114):
            action = 2
        elif action in range(114,177):
            action = 3
        elif action in range(177,289):
            action = 4
        elif action == 289:
            action = 5
        reward = Reward(self.data[index]['player_board_card_info'],self.data[index]['opponent_board_card_info'],self.data[index]['player_hand_card_id'],self.data[index]['opponent_life'],self.data[index]['player_life'],self.data[index]['player_gold'])
        return_reward = [max(reward[:56])] +[max(reward[56:105])] + [max(reward[105:114])]  + [max(reward[114:177])] + [max(reward[177:289])] + [reward[289]]     
        return np.array(decode_state_old_test(self.data[index],isImitation=True)[:-1]),np.array(return_reward),action

# Old decode - Choosen Action 
class Feeder4(Dataset):
    def __init__(self,data_dir,ran):
        self.data_dir = data_dir
        self.paths = os.listdir(data_dir)
        data = list()
        for file_dir in self.paths:
            f = open(data_dir + file_dir)
            try:
                js = json.load(f)['logs']
                data.extend(js)
            except json.JSONDecodeError:
                f.close()
                continue
            f.close()
        
        self.data = list()
        self.range = ran
        for i in tqdm(data,total=len(data)):
            if decode_state(i,isImitation=True)[-1]  in ran:
                self.data.append(i)
             

    def __len__(self):
        return len(self.data)

    def __getitem__(self, index):
        action = decode_state_old_test(self.data[index],isImitation=True)[-1]
        reward = Reward(self.data[index]['player_board_card_info'],self.data[index]['opponent_board_card_info'],self.data[index]['player_hand_card_id'],self.data[index]['opponent_life'],self.data[index]['player_life'],self.data[index]['player_gold'])
        return_reward = reward[self.range[0]:self.range[-1]]   
        
        return np.array(decode_state_old_test(self.data[index],isImitation=True)[:-1]),np.array(return_reward,dtype=np.float),action

# New decode - Scenario
class Feeder5(Dataset):
    def __init__(self,data_dir):
        self.data_dir = data_dir
        self.paths = os.listdir(data_dir)
        data = list()
        count = 0
        for file_dir in self.paths:
            f = open(data_dir + file_dir)
            try:
                js = json.load(f)['logs']
                data.extend(js)
            except json.JSONDecodeError:
                f.close()
                continue
            f.close()
#         for i in data:
#             if decode_state_old_test(i,isImitation=True)[-1] == 289:
#                 data.remove(i)
#                 count += 1
#                 continue
#             if count == 1000:
#                 break
        self.data = data

    def __len__(self):
        return len(self.data)

    def __getitem__(self, index):
        action = decode_state(self.data[index],isImitation=True)[-1]
        if action in range(56):
            if action in [7,15,23,31,39,47,55]:
                action = 1
            else:
                action = 0
        elif action in range(56,105):
            action = 1
        elif action in range(105,114):
            action = 2
        elif action in range(114,177):
            action = 3
        elif action in range(177,289):
            action = 4
        elif action == 289:
            action = 5
        reward = Reward(self.data[index]['player_board_card_info'],self.data[index]['opponent_board_card_info'],self.data[index]['player_hand_card_id'],self.data[index]['opponent_life'],self.data[index]['player_life'],self.data[index]['player_gold'])
        return_reward = [max(reward[:56])] +[max(reward[56:105])] + [max(reward[105:114])]  + [max(reward[114:177])] + [max(reward[177:289])] + [reward[289]]     
        return np.array(decode_state(self.data[index],isImitation=True)[:-1]),np.array(return_reward),action


# New decode - Scenario
class Feeder6(Dataset):
    def __init__(self,data_dir,ran):
        self.data_dir = data_dir
        self.paths = os.listdir(data_dir)
        data = list()
        for file_dir in self.paths:
            f = open(data_dir + file_dir)
            try:
                js = json.load(f)['logs']
                data.extend(js)
            except json.JSONDecodeError:
                f.close()
                continue
            f.close()
        
        self.data = list()
        self.range = ran
        for i in tqdm(data,total=len(data)):
            if decode_state(i,isImitation=True)[-1]  in ran:
                self.data.append(i)
             

    def __len__(self):
        return len(self.data)

    def __getitem__(self, index):
        action = decode_state(self.data[index],isImitation=True)[-1]
        reward = Reward(self.data[index]['player_board_card_info'],self.data[index]['opponent_board_card_info'],self.data[index]['player_hand_card_id'],self.data[index]['opponent_life'],self.data[index]['player_life'],self.data[index]['player_gold'])
        return_reward = reward[self.range[0]:self.range[-1]]   
        
        return np.array(decode_state(self.data[index],isImitation=True)[:-1]),np.array(return_reward,dtype=np.float),action
