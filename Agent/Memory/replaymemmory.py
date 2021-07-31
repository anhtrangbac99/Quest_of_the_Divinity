import numpy as np
from Utils.utils import reservoir_random
import torch
from Config.config import *
class ReplayBuffer(object):
    def __init__(self, max_size, input_shape, n_actions,isImitation = False):
        self.memory_size = max_size
        self.memory_current_size = 0

        self.state_memory = np.zeros((self.memory_size,input_shape),dtype=np.float32)
        self.next_state_memory = np.zeros((self.memory_size,input_shape),dtype=np.float32)

        self.action_memory = np.zeros(self.memory_size,dtype=np.int64)
        
        if isImitation:
            self.reward_memory = np.zeros((self.memory_size,NUM_GROUP_ACTION),dtype=np.float32)
        else:
            self.reward_memory = np.zeros(self.memory_size,dtype=np.float32)
        self.terminal_memory = np.zeros(self.memory_size,dtype=np.bool)

    def store(self, state, action ,reward ,imitation = False, next_state = None, done = None):            
        index = self.memory_current_size % self.memory_size

        try:
            self.state_memory[index] = state
        except:
            self.state_memory[index] = state.cpu()

        try:
            self.action_memory[index] = action
        except:
            self.action_memory[index] = action.cpu()
        try:
            self.reward_memory[index] = reward
        except:
            self.reward_memory[index] = reward.cpu()
        
        if not imitation:
            try:
                self.terminal_memory[index] = done
            except:
                self.terminal_memory[index] = done.cpu()
            try:
                self.next_state_memory[index] = next_state
            except:
                self.next_state_memory[index] = next_state.cpu()
        self.memory_current_size += 1

    def sample_batch(self, batch_size,imitation=False):
        max_memory_size = min(self.memory_current_size,self.memory_size)
        batch = reservoir_random(max_memory_size,batch_size)
        
        states = self.state_memory[batch]
        actions = self.action_memory[batch]
        rewards = self.reward_memory[batch]
        
        if not imitation:
            terminals = self.reward_memory[batch]
            next_states = self.next_state_memory[batch]

            return states,actions,rewards,next_states,terminals
        
        states,actions,rewards = map(lambda x: torch.tensor(x),(states,actions,rewards))
        return states,actions,rewards