
from Utils.reward import *
from Card.card import *
import pandas as pd
import numpy as np
card_info = pd.read_csv(CARD_ID_FILE_PATH)
card_board = Card(34,card_info,0,2,1)
card_board2 = Card(1,card_info,0,2,2)
card_hand = Card(card_id = 1,info = card_info)
reward = Reward_Buff(card_board,card_board2)
# print(card_info.shape)
print(card_board.name)
print(card_board2.name)
print(reward)
