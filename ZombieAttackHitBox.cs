using UnityEngine;

public class ZombieAttackHitBox : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("attackhitboxに入ったよ");
    
        {
            // プレイヤーにダメージを与える処理
            fpscontroller player = other.GetComponent<fpscontroller>();
            if (player != null)
            {Debug.Log("プレイヤーを発見");
                player.TakeDamage(damage);
                Debug.Log("ダメージを与えた;");
            }
        }
    }
}
