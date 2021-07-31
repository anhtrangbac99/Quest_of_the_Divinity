#using for DQN_Heuristic
import os
import torch as T
import torch.nn as nn
import torch.nn.functional as F
import torch.optim as optim
import numpy as np

class DQNetwork(nn.Module):
    def __init__(self,alpha, n_actions,name,input_dim,chekpoint_dir):
        super(DQNetwork,self).__init__()

        self.checkpoint_dir = chekpoint_dir
        self.checkpoint_file = os.path.join(self.checkpoint_dir,name)

        self.dense1 = nn.Linear(input_dim,512)
        self.dense2 = nn.Linear(512,256)
        self.dense3 = nn.Linear(256,128)
        self.dense4 = nn.Linear(128,n_actions)

        self.optimized = optim.RMSprop(self.parameters(),lr=alpha)
        self.loss = nn.MSELoss()
        self.device = T.device('cuda:0' if T.cuda.is_available else 'cpu')

        self.to(self.device)

    def forward(self,state):
        dense1 = F.relu(self.dense1(state))
        dense2 = F.relu(self.dense2(dense1))
        dense3 = F.relu(self.dense3(dense2))
        actions = F.softmax(self.dense4(dense3))

        return actions

    def save_checkpoint(self):
        print('########## Saving checkpoint ##########')
        try:
            T.save(self.state_dict(), self.checkpoint_file)
        except FileNotFoundError:
            os.mkdir(self.checkpoint_file)
            T.save(self.state_dict(), self.checkpoint_file)


    def load_checkpoint(self):
        print('########## Loading checkpoint ##########')
        self.load_state_dict(T.load(self.checkpoint_file))