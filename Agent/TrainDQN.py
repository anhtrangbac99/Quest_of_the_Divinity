from abc import abstractproperty
from json.decoder import JSONDecodeError
import numpy as np
from numpy.core.numeric import NaN
from numpy.lib.type_check import nan_to_num
from DQNAgent import DQNAgent
# from utils import plot_learning_curve, make_env
from Server.train_client import TrainSocket
from Config.config import *
from Utils.utils import *
import pandas as pd
import socket
import json

if __name__ == '__main__':
    best_score = 0
    load_checkpoint = LOAD_CHECK_POINT
    n_games = 250

    agent = DQNAgent(gamma=0.1, epsilon=0.7, alpha=0.0001,replace_target_cnt=50,
                     input_dims=INPUT_DIM,
                     n_actions=NUM_ACTIONS, mem_size=50000, eps_min=0.1,
                     batch_size=32, replace=1000, eps_dec=1e-5,
                     checkpoint_dir=CHECKPOINT_DIR, algo='DQNAgent',
                     env_name='Quest_of_Divinity')
    #agent.save_models()
    agent.load_models()

    if load_checkpoint:
        agent.load_models()

    conn = TrainSocket()
    conn.connect()
    conn.get_env_state()
    n_steps = 0
    scores, eps_history, steps_array = [], [], []
    data = conn.sock.recv(BUFFER_SIZE)
    string_env = json.loads(data.decode())
    new_game = False
    game_num = 1
    file = create_file(1)
    save_data = []
    # So that the socket is never closed
    while True:
        if new_game:
            #Create new log file
            game_num += 1
            log(save_data,file)
            file.close()
            file = create_file(game_num)
            save_data = []
            #Receive new data from new game 
            new_game = False

            # data = conn.sock.recv(BUFFER_SIZE)
            # string_env = json.loads(data.decode())
        
        #Choose action
        score = 0
        action,reward,state = agent.choose_action(string_env)
        print(string_env)

        while reward[action] <= 0:
            score += reward[action]

            scores.append(score)
            steps_array.append(n_steps)
            avg_score = np.mean(scores[-100:])

            save_data.append(create_log_json(string_env,action,score,avg_score,best_score))

            agent.store(state,action,reward[action],state, False)
            action,reward,state = agent.choose_action(string_env)
        action_info = pd.read_csv(ACTION_ID_FILE_PATH)
        action_id = action_info.at[action,'ActionID']
        associate_action_id = action_info.at[action,'AssociateActionID']

        # if not np.isnan(associate_action_id) and np.int(associate_action_id) > 10 and np.int(associate_action_id) < 80:
        #     action_str = str(np.int(associate_action_id))
        # else:
        action_str = str(np.int(action_id)) + ' ' + str(np.int(associate_action_id)) if not np.isnan(associate_action_id) else str(action_id)
       
        #Send action to game
        conn.handle_data(action_str)


        #Calculate score
        score += reward[action]

        scores.append(score)
        steps_array.append(n_steps)
        avg_score = np.mean(scores[-100:])

        #Save log file
        save_data.append(create_log_json(string_env,action,score,avg_score,best_score))

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


        #Store and learn
        next_state = decode_state(string_env)

        agent.store(state,action,reward[action],next_state, False)
        agent.learn()
        n_steps += 1


        # print('score: ', score,
        #     ' average score %.1f' % avg_score, 'best score %.2f' % best_score,
        #     'epsilon %.2f' % agent.epsilon, 'steps', n_steps)
        if avg_score > best_score:
            if not load_checkpoint:
                agent.save_models()
            best_score = avg_score
        
        eps_history.append(agent.epsilon)

    socket.close()
