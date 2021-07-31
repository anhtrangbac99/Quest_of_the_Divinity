class Card():
    def __init__(self,card_id,info,status=0, cur_damage=0, cur_life=0):
        self.card_id = card_id
        self.cur_damage = cur_damage
        self.cur_life = cur_life
        self.status = status
        if card_id in [0,-1]:
            card_id = 57
        self.name =  info.at[card_id-1,"Name"]
        self.gold =  info.at[card_id-1,"Gold"]
        self.main_class =  info.at[card_id-1,"Main class"]
        self.initial_damage =  info.at[card_id-1,"Initial damage"]
        self.initial_life =  info.at[card_id-1,"Initial life"]
        self.raider =  info.at[card_id-1,"Raider"]
        self.champion =  info.at[card_id-1,"Champion"]
        self.pioneer =  info.at[card_id-1,"Pioneer"]
        self.warchief =  info.at[card_id-1,"Warchief"]
        self.guard =  info.at[card_id-1,"Guard"]
        self.deal_damage =  info.at[card_id-1,"Deal Damage"]
        self.deal_spell_damage =  info.at[card_id-1,"Deal Spell Damage"]
        self.heal_minion =  info.at[card_id-1,"Heal minion"]
        self.heal_player =  info.at[card_id-1,"Heal player"]
        self.buff_damage =  info.at[card_id-1,"Buff Damage"]
        self.summoner =  info.at[card_id-1,"Summoner"]
        self.reduce_money =  info.at[card_id-1,"Reduce money"]
        self.card_draw =  info.at[card_id-1,"Card draw"]
        self.reduce_enemies_damage =  info.at[card_id-1,"Reduce enemies damage"]
        self.block_enemies_damage =  info.at[card_id-1,"Block enemies damage"]
        self.disable_enemies =  info.at[card_id-1,"Disable enemies"]

    def isGuard(self):
        return 1 if self.guard==1 else 0

    def totalValue(self):
        return self.deal_damage + self.deal_spell_damage + self.heal_minion + self.heal_player + self.buff_damage + self.summoner + self.reduce_money + self.card_draw + self.reduce_enemies_damage + self.block_enemies_damage + self.disable_enemies
    
    def isNotPioneer(self):
        return 1 if self.pioneer == 0 else 0

    def isHellhound(self):
        return 1 if self.card_id == 25 else 0

    def isFinalDuel(self):
        return 1 if self.card_id == 56 else 0

    def pioneerValue(self):
        if self.card_id == 3:
            return self.card_draw
        elif self.card_id == 4:
            return self.deal_damage
        elif self.card_id == 10:
            return self.reduce_money
        elif self.card_id == 14:
            return self.buff_damage
        elif self.card_id == 19:
            return self.deal_damage
        elif self.card_id == 24:
            return self.summoner
        elif self.card_id == 27:
            return self.disable_enemies
        elif self.card_id == 29:
            return self.deal_damage
        else:
            return 0

    def sacrifiedRighteousImmolation(self):
        if self.main_class == 1 or self.gold < 4 :
            return -100
        
        if self.main_class == 2:
            return self.block_enemies_damage * 2
        
        elif self.main_class == 3:
            return self.buff_damage
        
        elif self.main_class == 4:
            return self.deal_damage - 5
        
        elif self.main_class == 5:
            return self.reduce_money

        elif self.main_class == 6:
            return self.deal_damage - 5

        else:
            return -100

    def sacrifiedFinalDuel(self):
        if self.main_class == 1:
            return -100
        
        if self.main_class == 2:
            return self.reduce_enemies_damage
        
        elif self.main_class == 3:
            return self.reduce_enemies_damage
        
        elif self.main_class == 4:
            return self.reduce_money - 20
        
        elif self.main_class == 5:
            return self.card_draw * 6

        elif self.main_class == 6:
            return self.reduce_money - 7
        else:
            return -100
        
    def statusList(self):
        return [self.gold,self.main_class,self.initial_damage,self.initial_life,self.raider,self.champion,self.pioneer,self.warchief,self.guard ,self.deal_damage,self.deal_spell_damage ,self.heal_minion,self.heal_player,self.buff_damage,self.summoner,self.reduce_money,self.card_draw,self.reduce_enemies_damage,self.block_enemies_damage,self.disable_enemies]