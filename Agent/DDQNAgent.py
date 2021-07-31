
import numpy as np
import torch as T
from Network.deepnetwork import DQNetwork
from Memory.replaymemmory import ReplayBuffer
from Utils.reward import *
from Utils.utils import *
from Config.config import *
class DDQNAgent(object):
    def __init__(self, gamma, epsilon, alpha, input_dim,n_actions,
                 mem_size, batch_size, eps_min=0.01, eps_dec=5e-7,
                 replace=1000, algo=None, env_name=None, chkpt_dir='CheckPoint'):
        self.gamma = gamma
        self.epsilon = epsilon
        self.alpha = alpha
        self.n_actions = n_actions
        self.input_dim = input_dim
        self.batch_size = batch_size
        self.eps_min = eps_min
        self.eps_dec = eps_dec
        self.replace_target_cnt = replace
        self.algo = algo
        self.env_name = env_name
        self.chkpt_dir = chkpt_dir
        self.action_space = [i for i in range(n_actions)]
        self.learn_step_counter = 0
        self.losses = 0.0
        self.memory = ReplayBuffer(mem_size, input_dim, n_actions)

        self.q_eval = DQNetwork(self.alpha, self.n_actions,
                                    input_dim=self.input_dim,
                                    name=self.env_name+'_'+self.algo+'_q_eval',
                                    chekpoint_dir=self.chkpt_dir)
        self.q_next = DQNetwork(self.alpha, self.n_actions,
                                    input_dim=self.input_dim,
                                    name=self.env_name+'_'+self.algo+'_q_next',
                                    chekpoint_dir=self.chkpt_dir)

    def store(self, state, action, reward, state_, done):
        self.memory.store(state, action, reward,False, state_, done)

    def sample_batch(self):
        state, action, reward, new_state, done =   self.memory.sample_batch(self.batch_size)

        states = T.tensor(state).to(self.q_eval.device)
        rewards = T.tensor(reward).to(self.q_eval.device)
        dones = T.tensor(done).to(self.q_eval.device)
        actions = T.tensor(action).to(self.q_eval.device)
        states_ = T.tensor(new_state).to(self.q_eval.device)

        return states, actions, rewards, states_, dones

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
            action = T.argmax(actions).item()
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
    def replace_target_network(self):
        if self.replace_target_cnt is not None and self.learn_step_counter % self.replace_target_cnt == 0:
            self.q_next.load_state_dict(self.q_eval.state_dict())

    def decrement_epsilon(self):
        self.epsilon = self.epsilon - self.eps_dec if self.epsilon > self.eps_min else self.eps_min

    def learn(self,writer=None):
        if self.memory.memory_current_size < self.batch_size:
            return

        self.q_eval.optimized.zero_grad()

        self.replace_target_network()

        states, actions, rewards, states_, dones = self.sample_batch()

        indices = np.arange(self.batch_size)

        q_pred = self.q_eval.forward(states)[indices, actions]
        q_next = self.q_next.forward(states_)
        q_eval = self.q_eval.forward(states_)

        max_actions = T.argmax(q_eval, dim=1)
        # q_next[dones] = 0.0


        q_target = rewards + self.gamma*q_next[indices, max_actions]
        loss = self.q_eval.loss(q_target, q_pred).to(self.q_eval.device)
        loss.backward()

        self.q_eval.optimized.step()
        self.learn_step_counter += 1

        self.losses += loss
        if self.learn_step_counter % BATCH_SIZE == 0:
            try:
                writer.add_scalar('training loss',self.losses/BATCH_SIZE,self.learn_step_counter)
                self.losses = 0.0
            except:
                pass
            print(loss)
        self.decrement_epsilon()
        if self.learn_step_counter % 20 == 0:
            self.save_models()

    def save_models(self):
        self.q_eval.save_checkpoint()
        self.q_next.save_checkpoint()

    def load_models(self):
        self.q_eval.load_checkpoint()
        self.q_next.load_checkpoint()