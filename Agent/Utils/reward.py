from pandas.core.accessor import CachedAccessor
from Card.card import Card
import pandas as pd
import numpy as np
from Config.config import *
import math

def Reward(player_card_list,opponent_card_list,player_hand_card_id_list,opponent_life,player_life,player_gold):
    player_cards = list()
    opponent_cards = list()
    player_hand_cards = list()

    card_info = pd.read_csv(CARD_ID_FILE_PATH)
    have_final_duel = False
    have_slot = False
    is_empty = False
    for card in player_card_list:
        if card['id'] in [0,-1]:
            have_slot = True
        player_cards.append(Card(card_id = card['id'],info = card_info,status = card['status'],cur_damage = card['damage'],cur_life = card['life']))
    
    for card in opponent_card_list:
        opponent_cards.append(Card(card_id = card['id'],info = card_info,status = card['status'],cur_damage = card['damage'],cur_life = card['life']))
    
    for card_id in player_hand_card_id_list:
        if card_id == 56:
            have_final_duel = True

        player_hand_cards.append(Card(card_id = card_id,info = card_info))

    if have_slot:
        is_empty = True   
    reward = list()
    reward_sacrified_final_duel = list()
    reward_buff = list()
    for player_card in player_cards:
        for opponent_card in opponent_cards:
            reward.append(Reward_Attack_Card(player_card,opponent_card))
            if have_final_duel:
                reward_sacrified_final_duel.append(Reward_Sacrified_Final_Duel(player_card,opponent_card,player_gold))
            else:
                reward_sacrified_final_duel.append(0)
        reward.append(Reward_Attack_Avatar(player_card,opponent_life))
        for player_other_card in player_cards:
            reward_buff.append(Reward_Buff(player_card,player_other_card))
    
    reward_play_hand_cards = list()
    reward_play_hand_cards_target_opponent = list()
    reward_play_hand_cards_target_player = list()
    for player_hand_card in player_hand_cards:
        reward_play_hand_cards.append(Reward_Play(player_hand_card,player_gold,is_empty=is_empty))
        for opponent_card in opponent_cards:
            reward_play_hand_cards_target_opponent.append(Reward_Play(player_hand_card,player_gold,opponent_card,player_life))
        for player_card in player_cards:
            reward_play_hand_cards_target_player.append(Reward_Play(player_hand_card,player_gold,player_card,player_life,is_player=1))

    reward = reward +reward_buff+ reward_play_hand_cards + reward_play_hand_cards_target_opponent + reward_play_hand_cards_target_player + reward_sacrified_final_duel + [0.5]

    return reward

def Reward_Attack_Card(chosen_card,target_card):
    if chosen_card.card_id in [0,-1] or target_card.card_id in [0,-1] or chosen_card.status == 0:
        return 0
    reward_attack = chosen_card.cur_damage + target_card.warchief*2 + target_card.status*target_card.isNotPioneer()*target_card.totalValue() - target_card.cur_damage/2
 
    reward_summon = chosen_card.isHellhound()*chosen_card.summoner if chosen_card.cur_damage > target_card.cur_life else 0
    reward_guard = target_card.isGuard()*10
    reward_pioneer = (1-target_card.isNotPioneer())*target_card.pioneerValue()

    reward_reduce_damage = 0
    reward_disable_enemies = 0

    if chosen_card.reduce_enemies_damage > 0 :
        reward_reduce_damage  += target_card.cur_life + target_card.totalValue()
    
    if chosen_card.disable_enemies > 0 :
        reward_disable_enemies  += target_card.cur_damage + target_card.totalValue()

    return reward_attack + reward_summon + reward_guard + reward_pioneer + reward_disable_enemies + reward_reduce_damage

def Reward_Attack_Avatar(chosen_card,opponent_life):
    if chosen_card.card_id in [0,-1] or chosen_card.status == 0:
        return 0
    try:
        return chosen_card.cur_damage*2 - math.log(opponent_life)
    except ValueError:
        return chosen_card.cur_damage*2

def Reward_Buff(chosen_card,target_card):
    if chosen_card.card_id in [0,-1] or target_card.card_id in [0,-1] or chosen_card.status == 0:
        return 0
    if chosen_card.card_id == target_card.card_id:
        return 0
    if chosen_card.status == 2:
        return 0
    reward_heal_minion = chosen_card.heal_minion * (1 - target_card.isNotPioneer()) * (target_card.totalValue() + target_card.warchief + target_card.cur_damage)
    if target_card.cur_life == target_card.initial_life:
        reward_heal_minion = 0
    reward_buff_damage = chosen_card.buff_damage * (target_card.cur_life + target_card.totalValue())
    if chosen_card.card_id in [1,9,14,29,30,52,40]:
        reward_buff_damage =  -10
    if chosen_card.card_id in [12,13,14,23,52,36,40]:
        reward_heal_minion = -10
    return reward_buff_damage + reward_heal_minion

def Reward_Play(chosen_card,player_gold,target_card = None,player_life = 0,is_player = 0,is_empty=True):
    if is_player == 1 and target_card != None and chosen_card.card_id in [41,42,43]:
        return 0
    if is_player == 0 and target_card != None and chosen_card.card_id in [46,47]:
        return 0
    if chosen_card.gold > player_gold:
        return 0
    if chosen_card.card_id in [0,-1]:
        return 0
    reward_play = 0
    if target_card:
        if target_card.card_id in [0,-1] or chosen_card.main_class not in [7,9]:
            return 0
        if chosen_card.main_class == 9 and is_player==0:
            return 0
        if chosen_card.card_id in [53,54,55]:
            return Reward_Sacrified(chosen_card=chosen_card,target_card=target_card,player_life=player_life)
        if chosen_card.card_id in [41,42,43]:
            try:
                reward_attack = chosen_card.deal_spell_damage + target_card.status*target_card.isNotPioneer()*target_card.totalValue() - math.log(target_card.cur_life)
            except ValueError:
                reward_attack = chosen_card.deal_spell_damage + target_card.status*target_card.isNotPioneer()*target_card.totalValue()

            reward_bonus = 5 if (chosen_card.card_id == 42 and target_card.cur_life <= 3) else 0
            reward_disable_enemies = 0

            if chosen_card.disable_enemies > 0 :
                reward_disable_enemies  += target_card.cur_damage + target_card.totalValue()*2

            return reward_attack + reward_bonus + reward_disable_enemies + 5

        if chosen_card.card_id == 47:
            return target_card.cur_damage + 5 + target_card.cur_life + target_card.totalValue() + target_card.warchief + target_card.guard
        if chosen_card.card_id in [44,45,46,48]:
            return 0
    else:
        if chosen_card.card_id in [53,54,55,41,42,43,47]:
            return 0
        if chosen_card.gold > player_gold:
            return 0
        if not is_empty:
            return 0
        reward_bonus = 0
        if chosen_card.main_class in [7,9]:
            reward_bonus = 10
        reward_play = chosen_card.initial_life + reward_bonus + chosen_card.initial_life + chosen_card.totalValue() + chosen_card.raider + chosen_card.champion + chosen_card.pioneer + chosen_card.warchief + chosen_card.guard*5
    return reward_play

def Reward_Sacrified_Final_Duel(chosen_card,target_card,player_gold):
    if chosen_card.card_id in [0,-1] or target_card.card_id in [0,-1]:
        return 0
    if player_gold < 8 :
        return 0
    if abs(chosen_card.cur_damage-target_card.cur_life) < (target_card.cur_damage-chosen_card.cur_life):
        return 0
    
    return chosen_card.sacrifiedFinalDuel() + 20 - chosen_card.cur_life - chosen_card.cur_damage - chosen_card.totalValue() - chosen_card.warchief - chosen_card.guard

def Reward_Sacrified(chosen_card,target_card,player_life):
    if chosen_card.card_id in [0,-1] or target_card.card_id in [0,-1]:
        return 0
    if target_card.main_class == 1:
        return 0
    if chosen_card.card_id == 55:
        if target_card.gold/2 < player_life:
            return 0
        return target_card.sacrifiedRighteousImmolation() + 15 - target_card.gold/2 - target_card.cur_life - target_card.cur_damage - target_card.totalValue() - target_card.warchief - target_card.guard
   
    if target_card.gold < 2 and chosen_card.card_id == 53:
        return 0
    
    punish = target_card.gold/2 if chosen_card.card_id == 53 else target_card.gold
    return chosen_card.totalValue() + 20 - punish
