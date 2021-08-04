using UnityEngine;

public class TurnController : MonoBehaviour
{
    public void ChangeTurn()
    {
        GameManager GM = GameManager.GetInstance();

        GM.NotifyTurnOver();
    }
}
