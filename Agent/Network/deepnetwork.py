#using for DQN
import os
import torch as T
import torch.nn as nn
import torch.nn.functional as F
import torch.optim as optim
import numpy as np

class DQNetwork(nn.Module):
    def __init__(self,alpha, n_actions,name,input_dim,chekpoint_dir,device = 'cuda:0'):
        super(DQNetwork,self).__init__()

        self.checkpoint_dir = chekpoint_dir
        self.checkpoint_file = os.path.join(self.checkpoint_dir,name)

        self.dense1 = nn.Linear(input_dim,2048)
        self.dense2 = nn.Linear(2048,1024)
        self.dense3 = nn.Linear(1024,1024)
        self.dense4 = nn.Linear(1024,512)
        self.dense5 = nn.Linear(512,512)
        self.dense6 = nn.Linear(512,256)
        self.dense7 = nn.Linear(256,n_actions)

        self.optimized = optim.RMSprop(self.parameters(),lr=alpha)
        self.loss = nn.MSELoss()
        self.device = T.device(T.device('cuda:0') if T.cuda.is_available else 'cpu')

        self.to(self.device)

    def forward(self,state):
        dense1 = F.relu(self.dense1(state))
        dense2 = F.relu(self.dense2(dense1))
        dense3 = F.relu(self.dense3(dense2))
        dense4 = F.relu(self.dense4(dense3))
        dense5 = F.relu(self.dense5(dense4))
        dense6 = F.relu(self.dense6(dense5))
        actions = F.relu(self.dense7(dense6))

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
        self.load_state_dict(T.load(self.checkpoint_file,map_location='cuda:0'))