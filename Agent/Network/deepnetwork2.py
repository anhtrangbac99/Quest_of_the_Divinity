# Using for Imitation Networkimport os
import torch as T
import torch.nn as nn
import torch.nn.functional as F
import torch.optim as optim
import numpy as np
import os

class ImitationNetwork(nn.Module):
    def __init__(self,alpha, n_actions,name,input_dim,chekpoint_dir):
        super(ImitationNetwork,self).__init__()

        self.checkpoint_dir = chekpoint_dir
        self.checkpoint_file = os.path.join(self.checkpoint_dir,name)

        self.dense1 = nn.Linear(input_dim,2048)
        self.dense2 = nn.Linear(2048,1024)
        self.dense3 = nn.Linear(1024,512)
        self.dense4 = nn.Linear(512,256)
        self.dense5 = nn.Linear(256,128)
        self.dense6 = nn.Linear(128,64)
        self.dense7 = nn.Linear(64,32)
        self.dense8 = nn.Linear(32,n_actions)

        self.optimized = optim.RMSprop(self.parameters(),lr=alpha)
        self.loss = nn.CrossEntropyLoss()
        self.device = T.device('cuda:0' if T.cuda.is_available else 'cpu')

        self.to(self.device)
        
        
    def forward(self,state):
        dense1 = F.relu(self.dense1(state))
        dense2 = F.relu(self.dense2(dense1))
        dense3 = F.relu(self.dense3(dense2))
        dense4 = F.relu(self.dense4(dense3))
        dense5 = F.relu(self.dense5(dense4))
        dense6 = F.relu(self.dense6(dense5))
        dense7 = F.relu(self.dense7(dense6))

        
#         actions = F.softmax(self.dens8(dense7))
        actions = self.dense8(dense7)
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
        

class ImitationNetwork_Fuzzy(nn.Module):
    def __init__(self,alpha, n_actions,name,input_dim,chekpoint_dir):
        super(ImitationNetwork_Fuzzy,self).__init__()

        self.checkpoint_dir = chekpoint_dir
        self.checkpoint_file = os.path.join(self.checkpoint_dir,name)

        self.dense1 = nn.Linear(input_dim,1024)
#         self.dense2 = nn.Linear(2048,1024)
        self.dense3 = nn.Linear(1024,512)
#         self.dense4 = nn.Linear(512,256)
        self.dense5 = nn.Linear(512,128)
        self.dense6 = nn.Linear(128,64)
#         self.dense7 = nn.Linear(64,32)
        self.dense8 = nn.Linear(64,n_actions)

        self.optimized = optim.RMSprop(self.parameters(),lr=alpha)
        self.loss = nn.CrossEntropyLoss()
        self.device = T.device('cuda:0' if T.cuda.is_available else 'cpu')

        self.to(self.device)
        
        
    def forward(self,state):
        dense1 = F.relu(self.dense1(state))
#         dense2 = F.relu(self.dense2(dense1))
        dense3 = F.relu(self.dense3(dense1))
#         dense4 = F.relu(self.dense4(dense3))
        dense5 = F.relu(self.dense5(dense3))
        dense6 = F.relu(self.dense6(dense5))
#         dense7 = F.relu(self.dense7(dense6))

        
#         actions = F.softmax(self.dens8(dense7))
        actions = self.dense8(dense6)
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

class RNN(nn.Module):
    def __init__(self,input_size,output_size,hidden_size):
        super(RNN, self).__init__()

        self.dense1 = nn.Linear(input_size,1024)
        self.dense2 = nn.Linear(1024,512)

        self.rnn = nn.LSTM(
            input_size=512,
            hidden_size=hidden_size,
            num_layers=5,
            batch_first=True)
        self.dense3 = nn.Linear(hidden_size,output_size)

    def forward(self, x):
        dense1 = self.dense1(x.float())
        dense2 = self.dense2(dense1)
        out, (h_n, h_c) = self.rnn(dense2, None)
        out = self.dense3(out)
        return out[:, -1, :]