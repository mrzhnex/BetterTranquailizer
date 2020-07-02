using EXILED.Extensions;
using Mirror;
using UnityEngine;

namespace BetterTranquilizer
{
    public class BadgeComponent : MonoBehaviour
    {
        public Ragdoll Body;
        private Vector3 PrevPos;
        private float Timer = 0.0f;

        public void Update()
        {
            Timer += Time.deltaTime;
            if (Timer > 0.5f)
            {
                Timer = 0f;
                if (Vector3.Distance(transform.position, new Vector3(0f, -2000f, 0f) + Vector3.up * 1.5f) < 1.5f)
                {
                    Destroy(gameObject.GetComponent<BadgeComponent>());
                }
                else
                {
                    PrevPos = transform.position;
                    
                    if (!Global.IsFullRp && Player.GetPlayer(gameObject).GetCurrentItem().id != ItemType.None)
                    {
                        for (int i = 0; i < Player.GetPlayer(gameObject).inventory.items.Count; i++)
                        {
                            if (Player.GetPlayer(gameObject).inventory.items[i] == Player.GetPlayer(gameObject).GetCurrentItem())
                            {
                                Map.SpawnItem(Player.GetPlayer(gameObject).GetCurrentItem().id, Player.GetPlayer(gameObject).GetCurrentItem().durability, transform.position, transform.rotation, Player.GetPlayer(gameObject).GetCurrentItem().modSight, Player.GetPlayer(gameObject).GetCurrentItem().modBarrel, Player.GetPlayer(gameObject).GetCurrentItem().modOther);
                                Player.GetPlayer(gameObject).inventory.items.Remove(Player.GetPlayer(gameObject).GetCurrentItem());
                                Player.GetPlayer(gameObject).ClearBroadcasts();
                                Player.GetPlayer(gameObject).Broadcast(7, "Вы уронили предмет, так как вы несли тело", true);
                                break;
                            }
                        }
                    }
                    
                    if (Body == null || Body.gameObject == null)
                    {
                        Destroy(GetComponent<BadgeComponent>());
                    }
                    else
                    {
                        Body.gameObject.transform.position = new Vector3(PrevPos.x, PrevPos.y, PrevPos.z) - Vector3.up;
                    }
                }
            }
        }

        public void OnDestroy()
        {
            if (Body != null && Body.gameObject != null)
            {
                Body.gameObject.transform.position = new Vector3(PrevPos.x, PrevPos.y, PrevPos.z) - Vector3.up;
                NetworkServer.Spawn(Body.gameObject);
            }
        }
    }
}
