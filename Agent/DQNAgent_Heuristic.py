import numpy as np
import torch as T
from Network.deepnetwork1 import DQNetwork
from Memory.replaymemmory import ReplayBuffer
from Utils.reward import *
from Utils.utils import *

class DQNAgent(object):
    def __init__(self, gamma, epsilon, alpha, n_actions, replace_target_cnt,
                input_dims, mem_size, batch_size,eps_min = 0.01, eps_dec = 5e-7,
                replace = 1000, algo = None, env_name = None, checkpoint_dir='CheckPoint'):
        self.gamma = gamma
        self.epsilon = epsilon
        self.alpha = alpha
        self.n_actions = n_actions
        self.replace_target_cnt = replace_target_cnt
        self.input_dims = input_dims
        self.mem_size = mem_size
        self.batch_size = batch_size
        self.eps_min = eps_min
        self.eps_dec = eps_dec
        self.replace = replace
        self.algo = algo
        self.env_name = env_name
        self.checkpoint_dir = checkpoint_dir

        self.action_space = [i for i in range(n_actions)]
        self.learn_step_count = 0

        self.replay_memory = ReplayBuffer(max_size=self.mem_size,input_shape = self.input_dims,n_actions=self.n_actions)

        self.q_eval = DQNetwork(alpha=self.alpha,n_actions=self.n_actions,name=self.env_name+'_'+self.algo+'_q_eval',
                                input_dim=self.input_dims,chekpoint_dir=self.checkpoint_dir)

        self.q_target = DQNetwork(alpha=self.alpha,n_actions=self.n_actions,name=self.env_name+'_'+self.algo+'_q_target',
                                input_dim=self.input_dims,chekpoint_dir=self.checkpoint_dir)

    def increase_gamma(self):
        if self.learn_step_count % self.replace == 0:
            self.gamma += 0.01
    def choose_action(self, str_env):
        state = decode_state_old_test(str_env)
        reward = Reward(str_env['player_board_card_info'],str_env['opponent_board_card_info'],str_env['player_hand_card_id'],str_env['opponent_life'],str_env['player_life'],str_env['player_gold'])
        if str_env['time']  < 1:
            action = 289
        if np.random.random() > self.epsilon:
            state = T.tensor(state,dtype=T.float).to(self.q_eval.device)
            actions = self.q_eval.forward(state)
            rewards = T.tensor(reward,dtype=T.float).to(self.q_eval.device) 
            action = T.argmax(rewards).item()
        else:
            action = np.random.choice(self.action_space)
            # count = 0

            # while reward[action] <0:
            #     count += 1
            #     if count >= 20:
            #        # action = 289
            #         break
            #     action = np.random.choice(self.action_space)

        return action,reward,state
    
    def store(self,state,action,reward,next_state,done):
        self.replay_memory.store(state=state,action=action,reward=reward,next_state=next_state,done=done)

    def sample_batch(self):
        state,action,reward,next_state,teminal = self.replay_memory.sample_batch(self.batch_size)

        states = T.tensor(state).to(self.q_eval.device)
        actions = T.tensor(action).to(self.q_eval.device)
        rewards = T.tensor(reward).to(self.q_eval.device)
        next_states = T.tensor(next_state).to(self.q_eval.device)
        teminals = T.tensor(teminal).to(self.q_eval.device)

        return states,actions,rewards,next_states,teminals

    def replace_target_network(self):
        if self.learn_step_count % self.replace_target_cnt == 0:
            self.q_target.load_state_dict(self.q_eval.state_dict()) 

    def decrease_epsilon(self):
        self.epsilon = self.epsilon - self.eps_dec if self.epsilon > self.eps_min else self.eps_min

    def save_models(self):
        self.q_eval.save_checkpoint()
        self.q_target.save_checkpoint()

    def load_models(self):
        self.q_eval.load_checkpoint()
        self.q_target.load_checkpoint()

    def learn(self):
        if self.replay_memory.memory_current_size < self.batch_size:
            return
        
        self.q_eval.optimized.zero_grad()

        self.replace_target_network()

        states,actions,rewards,next_states,teminals = self.sample_batch()
        indices = np.arange(self.batch_size)

        q_pred = self.q_eval.forward(states)[indices, actions]
        q_next = self.q_target.forward(next_states).max(dim=1)[0]

        # q_next[teminals] = 0.0
        q_target = rewards + self.gamma*q_next

        loss = self.q_eval.loss(q_target, q_pred).to(self.q_eval.device)
        # if self.learn_step_count % 10 == 0:
        #     print(loss)
        loss.backward()

        self.q_eval.optimized.step()
        self.learn_step_count += 1

        self.decrease_epsilon()
        self.save_models()

        # if self.replay_memory.memory_current_size < self.batch_size:
        #     return
        
        # self.q_eval.optimized.zero_grad()

        # self.replace_target_network()

        # states,actions,rewards,next_states,teminals = self.sample_batch()
        # indices = np.arange(self.batch_size)

        # q_pred = self.q_eval.forward(states)[indices, actions]
        # q_next = self.q_target.forward(next_states).max(dim=1)[0]

        # # q_next[teminals] = 0.0
        # q_target = rewards + self.gamma*q_next

        # loss = self.q_eval.loss(q_target, q_pred).to(self.q_eval.device)
        # if self.learn_step_count % 10 == 0:
        #     print(loss)
        # loss.backward()

        # self.q_eval.optimized.step()
        # self.learn_step_count += 1

        # self.decrease_epsilon()
        # self.save_models()