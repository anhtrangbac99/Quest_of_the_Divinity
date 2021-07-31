from abc import abstractproperty
from json.decoder import JSONDecodeError
import numpy as np
from numpy.core.numeric import NaN
from numpy.lib.type_check import nan_to_num
from DQNAgent import DQNAgent
from DDQNAgent import DDQNAgent
from ImitationAgent import ImitationAgent
from DQNAgent_Heuristic import DQNAgent as Heuristic
# from utils import plot_learning_curve, make_env
from Server.train_client import TrainSocket
from Config.config import *
from Utils.utils import *
import pandas as pd
import socket
import json

if __name__ == '__main__':
    best_score = 0

    agent_heuristic = Heuristic(gamma=0.1, epsilon=0.7, alpha=0.0001,replace_target_cnt=50,
                     input_dims=INPUT_DIM,
                     n_actions=NUM_ACTIONS, mem_size=50000, eps_min=0.1,
                     batch_size=32, replace=1000, eps_dec=1e-5,
                     checkpoint_dir=CHECKPOINT_DIR_HEUR, algo='DQNAgent',
                     env_name='Quest_of_Divinity')
    # agent_heuristic.load_models()


    agentImitation_NFSP = ImitationAgent(alpha=0.001,
                 input_dims=516,
                 n_actions=NUM_ACTIONS, mem_size=50000, 
                 batch_size=32, replace=1000,
                 checkpoint_dir='Agent_Github\Models\Evaluate\\NFSP\\1stImitationAgent_OldData_DDQN.ckpt',algo='Imitation',
                 env_name='Quest_of_Divinity')
    agentImitation_NFSP.load_models()   

    agentDQN = DDQNAgent(gamma=0.1, epsilon=0.7, alpha=0.0001,input_dim=INPUT_DIM,
                 n_actions=NUM_ACTIONS, mem_size=50000, eps_min=0.1,
                 batch_size=32, replace=1000, eps_dec=1e-5,
                 chkpt_dir='Agent_Github\Models\Evaluate\\NFSP\\', algo='DDQNAgent',
                 env_name='Quest_of_Divinity')
    agentDQN.load_models()

    conn = TrainSocket()
    conn.connect()
    conn.get_env_state()

    n_steps = 0
    data = conn.sock.recv(BUFFER_SIZE)
    string_env = json.loads(data.decode())
    new_game = False
    game_num = 1
    # file = create_file(1)
    # save_data = []
    # win = np.load('Agent_GitHub\Fight\\NFSP_DQN.npy')
    # win = list(win)
    # win = [[0,0,0]]

    total_received_dmg = []
    total_due_dmg = []
    total_card_used = []
    total_action_count = []

    total_received_dmg = list(np.load('Agent_GitHub\Fight\\recv_dmg_human.npy'))
    total_due_dmg = list(np.load('Agent_GitHub\Fight\\due_dmg_human.npy'))
    total_card_used = list(np.load('Agent_GitHub\Fight\\card_used_human.npy'))
    total_action_count = list(np.load('Agent_GitHub\Fight\\action_count_human.npy'))

    received_dmg = 0
    due_dmg = 0
    card_used = 0
    action_count = 0

    score_DDQN_NFSP = 0.0
    score_Imi_DDQN_NFSP = 0.0
    num_DDQN_NFSP = 0
    num_Imi_DDQN_NFSP = 0
    nuy_DDQN_NFSP = 0.5
    count_DDQN_NFSP = 0
    # So that the socket is never closed
    while True:
        if new_game:
            #Create new log file
            # game_num += 1
            # log(save_data,file)
            # file.close()
            # file = create_file(game_num)
            # save_data = []
            #Receive new data from new game 


            # lst = [0,0,0]
            # new_game = False
            # if user_id == 0:
            #     lst[0] = win[-1][0]+1
            #     lst[1] = win[-1][1]
            #     lst[2] = player_life
            # else:
            #     lst[0] = win[-1][0]
            #     lst[1] = win[-1][1] + 1
            #     lst[2] = player_life

            # win.append(lst)
            # np.save('Agent_GitHub\Fight\\NFSP_DQN',win)

            total_received_dmg.append(received_dmg)
            total_due_dmg.append(due_dmg)
            total_card_used.append(card_used)
            total_action_count.append(action_count)
            received_dmg = 0
            due_dmg = 0
            card_used = 0
            action_count = 0
            np.save('Agent_GitHub\Fight\\recv_dmg_human',total_received_dmg)
            np.save('Agent_GitHub\Fight\\due_dmg_human',total_due_dmg)
            np.save('Agent_GitHub\Fight\\card_used_human',total_card_used)
            np.save('Agent_GitHub\Fight\\action_count_human',total_action_count)

            # with open(FIGHT_RESULT_DIR, "w") as f:
            #     string = str(win[0]) + '-' + str(win[1])
            #     f.write(string)
            # print(win)

            # data = conn.sock.recv(BUFFER_SIZE)
            # string_env = json.loads(data.decode())
        
        #Choose action
        
        score = 0
        user_id = string_env['player_id'] 
        player_life = string_env['player_life']
        print(string_env)

        # while user_id == 0:
            # agent = agent_heuristic
            # action,reward,_ = agent.choose_action(string_env)

            # while reward[action] <= 0:
            #     action,reward,state = agent.choose_action(string_env)
            # data = conn.sock.recv(BUFFER_SIZE)
            # string_env = json.loads(data.decode())
            # print(string_env)
            # print('here')

            # continue


        if user_id == 1:
            if count_DDQN_NFSP > 5:
                count_DDQN_NFSP = 0
                nuy_DDQN_NFSP = 0.5 

            dynamic = np.random.random()

            if  dynamic < num_DDQN_NFSP:

            # if True:
                action,reward,state = agentDQN.choose_action(string_env)
                score = reward[action]
                score_DDQN_NFSP += score
                num_DDQN_NFSP += 1 

                count_iter = 0
                while reward[action] <= 0:
                    action,reward,state = agentDQN.choose_action(string_env)
                    count_iter += 1

                    if count_iter > 20:
                        action = 289
                        break
                
            else:
                action,sco,reward,state = agentImitation_NFSP.choose_action(string_env)
                score_Imi_DDQN_NFSP += sco
                num_Imi_DDQN_NFSP += 1

            action_info = pd.read_csv(ACTION_ID_FILE_PATH)
            action_id = action_info.at[action,'ActionID']
            associate_action_id = action_info.at[action,'AssociateActionID']

            action_str = str(np.int(action_id)) + ' ' + str(np.int(associate_action_id)) if not np.isnan(associate_action_id) else str(action_id)
       


        # action,reward,_ = agent.choose_action(string_env)

        # while reward[action] <= 0:
        #     action,reward,state = agent.choose_action(string_env)


        # action_info = pd.read_csv(ACTION_ID_FILE_PATH)
        # action_id = action_info.at[action,'ActionID']
        # associate_action_id = action_info.at[action,'AssociateActionID']

        # action_str = str(np.int(action_id)) + ' ' + str(np.int(associate_action_id)) if not np.isnan(associate_action_id) else str(action_id)
       
        if user_id ==1: 
            action_count += 1
            if action_id in [1,2,3,4,5,6,7] and associate_action_id in [1,2,3,4,5,6,7,8]:
                if associate_action_id != 8:
                    received_dmg += string_env['opponent_board_card_info'][int(associate_action_id-1)]['damage']

                due_dmg += string_env['player_board_card_info'][int(action_id-1)]['damage']


            if action_id in [8,9,10,11,12,13,14,15,16]:
                card_used += 1

            conn.handle_data(action_str)

        # if user_id == 0:
        #     if action_id in [1,2,3,4,5,6,7] and associate_action_id in [1,2,3,4,5,6,7,8]:
        #         received_dmg += string_env['player_board_card_info'][int(action_id-1)]['damage']

        #Send action to game
        # conn.handle_data(action_str)

        #Receive new game state
        try:
            data = conn.sock.recv(BUFFER_SIZE)
        except:
            action_str = str(np.int(999))
            conn.handle_data(action_str)
            data = conn.sock.recv(BUFFER_SIZE)

        if not data:
            break
        
        try:
            string_env = json.loads(data.decode())
        except JSONDecodeError:
            new_game = True
            continue

        next_state = decode_state_old_test(string_env)

        if user_id == 1:
            if  dynamic < num_DDQN_NFSP:
            # if True:
                agentDQN.store(state,action,reward[action],next_state, False)

                return_reward = [max(reward[:56])] +[max(reward[56:105])] + [max(reward[105:114])]  + [max(reward[114:177])] + [max(reward[177:289])] + [reward[289]]     
                agentImitation_NFSP.store(state,action,return_reward)    

                agentDQN.learn()

            

            else:
                agentImitation_NFSP.store(state,action,reward)
                agentImitation_NFSP.learn()
            
            if n_steps % 50 == 0:
                try:
                    smax = torch.nn.functional.softmax(torch.tensor([score_DDQN_NFSP/num_DDQN_NFSP,score_Imi_DDQN_NFSP/num_Imi_DDQN_NFSP]))
                except ZeroDivisionError:
                    count_DDQN_NFSP += 1  
                    continue
                nuy_DDQN_NFSP = smax[0]
                score_DDQN_NFSP = 0.0
                score_Imi_DDQN_NFSP = 0.0
                num_DDQN_NFSP = 0
                num_Imi_DDQN_NFSP = 0
            

    socket.close()
