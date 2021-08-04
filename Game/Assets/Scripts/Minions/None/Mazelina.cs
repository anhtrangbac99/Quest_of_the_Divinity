using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mazelina : Minion, IChampion
{
    [SerializeField] private GameObject piggy = null;


    public override void OnActivateCallback()
    {
        base.OnActivateCallback();
        ActivateChampionEffect();
    }

    public void ActivateChampionEffect()
    {
        List<Card> opponent_card = GameManager.GetInstance().GetOpponentsMinions(owner);

        BoardSide opponent_board_side = GameManager.GetInstance().GetOpponentBoardSide(owner);

        if (opponent_card.Count > 0)
        {
            GameObject pig = Instantiate(piggy);

            // Set this pig's owner to opponent
            Player opponent = GameManager.GetInstance().GetOpponent(owner);
            Minion pig_as_minion = pig.GetComponent<Minion>();

            pig_as_minion.SetOwner(opponent);

            opponent_board_side.ReplaceCard(opponent_card[Random.Range(0, opponent_card.Count)].gameObject, pig);

            pig_as_minion.Activate(false);
        }
    }

    public void DeactivateChampionEffect()
    {

    }
}
