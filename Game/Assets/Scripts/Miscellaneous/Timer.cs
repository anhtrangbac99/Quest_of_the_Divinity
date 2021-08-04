using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour, ITurnUpdatable
{
    private Vector3 base_position = Vector3.zero;

    [SerializeField] private float turn_time_limit = 60f;
    [SerializeField] private float time_left = 0f;

    private int current_turn_id = 0;

    private bool end_turn_switching = false;
    public bool started = true;

    void Start()
    {
        base_position = transform.position;

        GameManager.GetInstance().RegisterTimer(this);
        time_left = turn_time_limit;
    }

    void Update()
    {
        if (started)
        {
            time_left -= Time.deltaTime;
            if (!end_turn_switching)
            {
                EvaluateRotateAndPos();
            }

            if (time_left <= 0f)
            {
                GameManager.GetInstance().NotifyTurnOver();
            }
        }        
    }

    public float GetRemainingTime() => time_left;

    /************************************
    *                                   *
    *       IUPDATABLE SECTION          *
    *                                   *
    ************************************/
    public void RegisterTurnUpdatable()
    {
        GameManager.GetInstance().RegisterTurnUpdatable(this);
    }

    public void UnregisterTurnUpdatable()
    {
        GameManager.GetInstance().UnregisterTurnUpdatable(this);
    }

    public virtual void OnNewTurnUpdate()
    {
        time_left = turn_time_limit;
        current_turn_id = GameManager.GetInstance().GetCurrentPlayerID();

        StartCoroutine(TurnToCurrentPlayer(0.5f));
    }

    IEnumerator TurnToCurrentPlayer(float time)
    {
        end_turn_switching = true;

        float elapsed_time = 0;

        Vector3 current_position = transform.position;
        Vector3 id_zero_pos = new Vector3(base_position.x, 0, base_position.z);
        Vector3 final_postion = current_turn_id == 0 ? id_zero_pos : base_position;

        float current_z_rotation = current_turn_id == 0 ? 90f : -90f;
        float final_z_rotation = current_turn_id == 0 ? 180f : 0f;
        
        while (elapsed_time <= time)
        {
            transform.position = Vector3.Lerp(current_position, final_postion, elapsed_time / time);

            transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Lerp(current_z_rotation, final_z_rotation, elapsed_time / time)));

            elapsed_time += Time.deltaTime;
            yield return null;
        }

        transform.position = final_postion;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, final_z_rotation));

        end_turn_switching = false;
        yield return 0;
    }

    private void EvaluateRotateAndPos()
    {
        float initial_z_rotation = current_turn_id == 0 ? 180f : 0;
        float final_z_rotation = initial_z_rotation + 90f;

        Vector3 id_zero_pos = new Vector3(base_position.x, 0, base_position.z);
        Vector3 initial_position = current_turn_id == 0 ? id_zero_pos : base_position;
        Vector3 final_position = new Vector3(base_position.x, base_position.y / 2f, base_position.z);

        transform.position = Vector3.Lerp(initial_position, final_position, 1f - time_left / turn_time_limit);

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Lerp(initial_z_rotation, final_z_rotation, 1f - time_left / turn_time_limit)));
    }
}
