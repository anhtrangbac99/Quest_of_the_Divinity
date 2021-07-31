import numpy as np
import torch as T
from Network.deepnetwork2 import ImitationNetwork
from Memory.replaymemmory import ReplayBuffer
from Utils.reward import *
from Utils.utils import *
from Config.config import *

class ImitationAgent(object):
    def __init__(self, alpha, n_actions,
                input_dims, mem_size, batch_size,
                replace = 1000, algo = None, env_name = None, checkpoint_dir='CheckPoint'):
        
        self.alpha = alpha
        self.input_dims = input_dims
        self.n_actions = n_actions
        self.mem_size = mem_size
        self.batch_size = batch_size
        self.replace = replace
        self.env_name = env_name
        self.checkpoint_dir = checkpoint_dir
        self.algo = algo
        self.action_space = [i for i in range(n_actions)]
        self.learn_step_count = 0

        self.replay_memory = ReplayBuffer(max_size=self.mem_size,input_shape = self.input_dims,n_actions=self.n_actions,isImitation = True)

        self.network = ImitationNetwork(alpha=self.alpha,n_actions=6,name=self.env_name+'_'+self.algo+'_imitation_network',input_dim=self.input_dims,chekpoint_dir=self.checkpoint_dir)

        self.network.to(DEVICE)

    def choose_action(self, str_env,train = True):
        state = torch.from_numpy(np.array(decode_state_old_test(str_env))).float()
        state_device = state.to(DEVICE)
        idx = 0
        actions = torch.argsort(self.network.forward(state_device).detach().cpu(),descending = True)
        
        reward = Reward(str_env['player_board_card_info'],str_env['opponent_board_card_info'],str_env['player_hand_card_id'],str_env['opponent_life'],str_env['player_life'],str_env['player_gold'])
        
        while True:
            try:
                action = actions[idx]
            except IndentationError:
                action = 289
                break
            idx += 1
            if action == 0:
                map_action = np.argmax(reward[:56])
            elif action == 1:
                map_action = 56 + np.argmax(reward[56:105])
            elif action == 2:
                map_action = 105 + np.argmax(reward[105:114]) 
            elif action == 3:
                map_action = 114 + np.argmax(reward[114:177])
            elif action == 4:
                map_action = 177 + np.argmax(reward[177:289])
            else:
                map_action = 289
            if reward[map_action] > 0:
                break
        return_reward = [max(reward[:56])] +[max(reward[56:105])] + [max(reward[105:114])]  + [max(reward[114:177])] + [max(reward[177:289])] + [reward[289]]     

        return map_action,reward[action],return_reward,state
        # if not train:
        #     reward = Reward(str_env['player_board_card_info'],str_env['opponent_board_card_info'],str_env['player_hand_card_id'],str_env['opponent_life'],str_env['player_life'],str_env['player_gold'])

        # if np.random.random() > self.epsilon:
        #     state = T.tensor(state,dtype=T.float).to(self.q_eval.device)
        #     actions = self.q_eval.forward(state)
        #     rewards = T.tensor(reward,dtype=T.float).to(self.q_eval.device) 
        #     action = T.argmax(actions).item()

        # return action,reward,state

    def store(self,state,action,reward):
        self.replay_memory.store(state=state,action=action,reward=reward,imitation=True)

    def save_models(self):
        torch.save(self.network.state_dict(), self.checkpoint_dir)

    def load_models(self):
        checkpoint = torch.load(self.checkpoint_dir,map_location='cuda:0')
        self.network.load_state_dict(checkpoint)

    def learn(self):
        if self.replay_memory.memory_current_size < self.batch_size:
            return
        
        opt = torch.optim.SGD(self.network.parameters(), 0.01, 
                          momentum=0.9, 
                          weight_decay=1e-4) 
        crit = torch.nn.BCEWithLogitsLoss().to(DEVICE)

        states,actions,rewards = self.replay_memory.sample_batch(self.batch_size,True)

        # print(rewards)
        data_,reward_,_ = map(lambda x:x.to(DEVICE),( states,rewards,actions))

        pred = self.network.forward(data_)

        loss = crit(pred,torch.nn.functional.softmax(reward_))
        opt.zero_grad()
        loss.backward()
        opt.step()
        if self.learn_step_count % 20 == 0:

            #  self.save_models()
            pass
        self.learn_step_count += 1
