from abc import abstractproperty
from json.decoder import JSONDecodeError
import numpy as np
from numpy.core.numeric import NaN
from numpy.lib.type_check import nan_to_num
from DQNAgent import DQNAgent
from DQNAgent_Heuristic import DQNAgent as Heuristic
# from utils import plot_learning_curve, make_env
from Config.config import *
from Utils.utils import *
from ImitationAgent import *
from Memory.dataset import *

if __name__ == '__main__':
    best_score = 0

    agentImitation = ImitationAgent(alpha=0.001,
                     input_dims=516,
                     n_actions=NUM_ACTIONS, mem_size=50000, 
                     batch_size=32, replace=1000,
                     checkpoint_dir=NFSP_IMITATION_DIR,algo='Imitation',
                     env_name='Quest_of_Divinity')
    agentImitation.load_models()
    # torch.save(agentImitation.network.state_dict(), 'Models/NFSP/1stImitationAgent_OldData')

    agentDQN = DQNAgent(gamma=0.1, epsilon=0.7, alpha=0.0001,replace_target_cnt=50,
                     input_dims=INPUT_DIM,
                     n_actions=NUM_ACTIONS, mem_size=50000, eps_min=0.1,
                     batch_size=32, replace=1000, eps_dec=1e-5,
                     checkpoint_dir=NFSP_DIR, algo='DQNAgent',
                     env_name='Quest_of_Divinity')
    agentDQN.load_models()
    # torch.save(agentDQN.q_eval.state_dict(), 'Models/NFSP/Quest_of_Divinity_DQNAgent_q_eval')
    # torch.save(agentDQN.q_target.state_dict(), 'Models/NFSP/Quest_of_Divinity_DQNAgent_q_target')

    dataset = Feeder(IMITATION_LOG_DIR)
    # dataloader = torch.utils.data.DataLoader(dataset,batch_size=BATCH_SIZE,shuffle=True,num_workers=NUM_WORKER)
    for i in range(NUM_EPOCHS):
        score_DQN = 0.0
        score_Imi = 0.0
        num_DQN = 0
        num_Imi = 0
        nuy = 0.5
        count = 0
        for idx,string_env in enumerate(dataset):
            if count > 5:
                count = 0
                nuy = 0.5 
            dynamic = np.random.random()
            if  dynamic < nuy:
                action,reward,state = agentDQN.choose_action(string_env)
                # while reward[action] <= 0:
                #     agentDQN.store(state,action,reward[action],state, False)
                #     action,reward,state = agentDQN.choose_action(string_env)
                score = reward[action]
                score_DQN += score
                num_DQN += 1 
                next_state = decode_state_old_test(string_env)
                agentDQN.store(state,action,reward[action],next_state, False)
                return_reward = [max(reward[:56])] +[max(reward[56:105])] + [max(reward[105:114])]  + [max(reward[114:177])] + [max(reward[177:289])] + [reward[289]]     
                agentImitation.store(state,action,return_reward)
                agentDQN.learn()

            else:
                action,sco,reward,state = agentImitation.choose_action(string_env)
                score_Imi += sco
                num_Imi += 1
                agentImitation.store(state,action,reward)
                agentImitation.learn()

            if idx % 100 == 0:
                try:
                    smax = torch.nn.functional.softmax(torch.tensor([score_DQN/num_DQN,score_Imi/num_Imi]))
                except ZeroDivisionError:
                    count += 1
                    continue
                nuy = smax[0]
                score_DQN = 0.0
                score_Imi = 0.0
                num_DQN = 0
                num_Imi = 0
                print('#########')
                print('## NUY ##')
                print(nuy)
                print('#########')
