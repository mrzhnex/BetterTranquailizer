using Exiled.API.Features;
using Mirror;
using UnityEngine;

namespace BetterTranquilizer
{
    public class BadgeComponent : MonoBehaviour
    {
        public Ragdoll Body;
        private Vector3 PrevPos;
        private float Timer = 0.0f;
        private readonly float TimeIsUp = 0.5f;
        public void Update()
        {
            Timer += Time.deltaTime;
            if (Timer > TimeIsUp)
            {
                Timer = 0f;
                if (Vector3.Distance(transform.position, new Vector3(0f, -2000f, 0f) + Vector3.up * 1.5f) < 1.5f)
                {
                    Destroy(gameObject.GetComponent<BadgeComponent>());
                }
                else
                {
                    PrevPos = transform.position;
                    
                    if (Global.IsFullRp)
                    {
                        if (new ItemType[] { ItemType.GunE11SR, ItemType.GunLogicer, ItemType.GunMP7, ItemType.GunProject90 }.Contains(Player.Get(gameObject).CurrentItem.id))
                        {
                            for (int i = 0; i < Player.Get(gameObject).Inventory.items.Count; i++)
                            {
                                if (Player.Get(gameObject).Inventory.items[i] == Player.Get(gameObject).CurrentItem)
                                {
                                    Player.Get(gameObject).DropItem(Player.Get(gameObject).Inventory.items[i]);
                                    Player.Get(gameObject).ClearBroadcasts();
                                    Player.Get(gameObject).Broadcast(7, "Вы уронили оружие, так как вы несли тело", Broadcast.BroadcastFlags.Normal);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Player.Get(gameObject).CurrentItem.id != ItemType.None)
                        {
                            for (int i = 0; i < Player.Get(gameObject).Inventory.items.Count; i++)
                            {
                                if (Player.Get(gameObject).Inventory.items[i] == Player.Get(gameObject).CurrentItem)
                                {
                                    Player.Get(gameObject).DropItem(Player.Get(gameObject).Inventory.items[i]);
                                    Player.Get(gameObject).ClearBroadcasts();
                                    Player.Get(gameObject).Broadcast(7, "Вы уронили предмет, так как вы несли тело", Broadcast.BroadcastFlags.Normal);
                                    break;
                                }
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