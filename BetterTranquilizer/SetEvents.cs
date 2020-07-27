using Dissonance.Integrations.MirrorIgnorance;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Mirror;
using RemoteAdmin;
using System.Collections.Generic;
using UnityEngine;

namespace BetterTranquilizer
{
    public class SetEvents
    {
        private readonly Vector3 saveLocation = new Vector3(0f, -5000f, 0f);
        private bool scpCustomLock = false;

        internal void OnRoundStarted()
        {
            Global.can_use_commands = true;
        }

        internal void OnShooting(ShootingEventArgs ev)
        {
            if (ev.Shooter.GameObject.GetComponent<TranqillMaster>() != null)
            {
                bool isTranqill = false;
                if (ev.Shooter.CurrentItem.id == ItemType.GunE11SR && ev.Shooter.GameObject.GetComponent<TranqillMaster>().isMTFWeapon)
                {
                    ev.Shooter.GameObject.GetComponent<TranqillMaster>().shootCount = ev.Shooter.GameObject.GetComponent<TranqillMaster>().shootCount - 1;

                    if (ev.Target != null)
                        isTranqill = true;

                }
                else if (ev.Shooter.GameObject.GetComponent<TranqillMaster>().isFullRp)
                {
                    if (ev.Shooter.CurrentItem.id == ItemType.GunUSP || ev.Shooter.CurrentItem.id == ItemType.GunCOM15)
                    {
                        ev.Shooter.GameObject.GetComponent<TranqillMaster>().shootCount = ev.Shooter.GameObject.GetComponent<TranqillMaster>().shootCount - 1;

                        if (ev.Target != null)
                        {
                            if (Vector3.Distance(ev.Target.transform.position, ev.Shooter.Position) <= Global.fullRPDistance)
                            {
                                isTranqill = true;
                            }
                            else
                            {
                                ev.Shooter.ClearBroadcasts();
                                ev.Shooter.Broadcast(5, "<color=#228b22>*транквилизатор не подействовал: вы были слишком далеко*</color>", Broadcast.BroadcastFlags.Normal);
                            }
                        }
                    }
                }
                else if (ev.Shooter.CurrentItem.id == ItemType.GunUSP || ev.Shooter.CurrentItem.id == ItemType.GunCOM15)
                {
                    ev.Shooter.GameObject.GetComponent<TranqillMaster>().shootCount = ev.Shooter.GameObject.GetComponent<TranqillMaster>().shootCount - 1;

                    if (ev.Target != null)
                        isTranqill = true;
                }

                if (isTranqill)
                {
                    Player target = Player.Get(ev.Target);
                    if (target.Role == RoleType.Scp049 || target.Role == RoleType.Scp93953 || target.Role == RoleType.Scp93989)
                    {
                        if (ev.Target.GetComponent<TranqillStack>() == null)
                        {
                            ev.Target.AddComponent<TranqillStack>();
                            ev.Target.GetComponent<TranqillStack>().count = ev.Target.GetComponent<TranqillStack>().count + 1;
                        }
                        else
                        {
                            ev.Target.GetComponent<TranqillStack>().count = ev.Target.GetComponent<TranqillStack>().count + 1;
                            if (ev.Target.GetComponent<TranqillStack>().count >= Global.maxTranqillOnSCP)
                            {
                                Object.Destroy(ev.Target.GetComponent<TranqillStack>());

                                CharacterClassManager ccm = ev.Target.GetComponent<CharacterClassManager>();
                                PlayerStats.HitInfo newInfo = new PlayerStats.HitInfo(1f, ev.Shooter.Nickname, DamageTypes.None, target.Id);

                                ev.Target.GetComponent<RagdollManager>().SpawnRagdoll(target.Position, ev.Target.transform.rotation, Vector3.zero, (int)ccm.CurClass, newInfo,
                                    ccm.Classes.SafeGet(ccm.CurClass).team > Team.SCP, ev.Target.GetComponent<MirrorIgnorancePlayer>().PlayerId, ev.Target.GetComponent<NicknameSync>().MyNick, ev.Target.GetComponent<QueryProcessor>().PlayerId);

                                target.ClearBroadcasts();
                                target.Broadcast(5, "<color=#228b22>*вы транквилизированы и очнетесь через: " + ev.Shooter.GameObject.GetComponent<TranqillMaster>().timeSleep + " секунд*</color>", Broadcast.BroadcastFlags.Normal);
                                target.IsGodModeEnabled = true;
                                ev.Target.AddComponent<SleepTime>();
                                ev.Target.GetComponent<SleepTime>().Info = newInfo;
                                ev.Target.GetComponent<SleepTime>().target = target;
                                ev.Target.GetComponent<SleepTime>().timeSleep = ev.Shooter.GameObject.GetComponent<TranqillMaster>().timeSleep;
                                target.Position = saveLocation;
                            }
                        }
                    }
                    else if (target.Role == RoleType.ChaosInsurgency || target.Role == RoleType.ClassD || target.Role == RoleType.FacilityGuard || target.Role == RoleType.NtfCadet || target.Role == RoleType.NtfCommander || target.Role == RoleType.NtfLieutenant || target.Role == RoleType.NtfScientist || target.Role == RoleType.Scientist)
                    {
                        CharacterClassManager ccm = ev.Target.GetComponent<CharacterClassManager>();
                        PlayerStats.HitInfo newInfo = new PlayerStats.HitInfo(1f, ev.Shooter.Nickname, DamageTypes.None, target.Id);
                        ev.Target.GetComponent<RagdollManager>().SpawnRagdoll(ev.Target.transform.position, ev.Target.transform.rotation, Vector3.zero, (int)ccm.CurClass, newInfo,
                                    ccm.Classes.SafeGet(ccm.CurClass).team > Team.SCP, ev.Target.GetComponent<MirrorIgnorancePlayer>().PlayerId, ev.Target.GetComponent<NicknameSync>().MyNick, ev.Target.GetComponent<QueryProcessor>().PlayerId);

                        target.IsGodModeEnabled = true;
                        ev.Target.AddComponent<SleepTime>();
                        ev.Target.GetComponent<SleepTime>().Info = newInfo;
                        ev.Target.GetComponent<SleepTime>().target = target;
                        target.ClearBroadcasts();
                        if (ev.Shooter.GameObject.GetComponent<TranqillMaster>().isFullRp)
                        {
                            ev.Target.GetComponent<SleepTime>().timeSleep = ev.Shooter.GameObject.GetComponent<TranqillMaster>().timeSleep * 2;
                            target.Broadcast(5, "<color=#228b22>*вы транквилизированы и очнетесь через: " + (ev.Shooter.GameObject.GetComponent<TranqillMaster>().timeSleep * 2).ToString() + " секунд*</color>", Broadcast.BroadcastFlags.Normal);
                        }
                        else
                        {
                            ev.Target.GetComponent<SleepTime>().timeSleep = ev.Shooter.GameObject.GetComponent<TranqillMaster>().timeSleep;
                            target.Broadcast(5, "<color=#228b22>*вы транквилизированы и очнетесь через: " + ev.Shooter.GameObject.GetComponent<TranqillMaster>().timeSleep.ToString() + " секунд*</color>", Broadcast.BroadcastFlags.Normal);
                        }
                        target.Inventory.ServerDropAll();
                        target.Position = saveLocation;
                    }
                }
                if (ev.Shooter.GameObject.GetComponent<TranqillMaster>().shootCount <= 0)
                {
                    Object.Destroy(ev.Shooter.GameObject.GetComponent<TranqillMaster>());
                    ev.Shooter.ClearBroadcasts();
                    ev.Shooter.Broadcast(5, "<color=#228b22>*у вас закончились транквилизаторы*</color>", Broadcast.BroadcastFlags.Normal);
                }
            }
        }

        internal void OnRemoteAdminCommandEvent(SendingRemoteAdminCommandEventArgs ev)
        {
            if (ev.Name != "bt")
                return;
            ev.IsAllowed = false;
            if (ev.Arguments.Count < 2)
            {
                ev.Sender.RemoteAdminMessage("Out of arguments. Usage: " + GetUsage());
                return;
            }

            Player player = Player.Get(ev.Arguments[0]);

            if (player == null)
            {
                ev.Sender.RemoteAdminMessage("Player not found");
                return;
            }
            if (ev.Arguments[1] == "remove")
            {
                if (player.GameObject.GetComponent<TranqillMaster>() != null)
                {
                    Object.Destroy(player.GameObject.GetComponent<TranqillMaster>());
                    bool silent = false;
                    if (ev.Arguments.Count > 2 && ev.Arguments[2] != null)
                    {
                        if (ev.Arguments[2] == "silent")
                        {
                            silent = true;
                        }
                    }
                    if (!silent)
                    {
                        player.ClearBroadcasts();
                        player.Broadcast(10, "<color=#228b22>*вы обнаружили, что у вас закончились транквилизаторы*</color>", Broadcast.BroadcastFlags.Normal);
                    }

                    ev.Sender.RemoteAdminMessage(player.Nickname + " remove from <tranqillMaster>");
                    return;
                }
                else
                {
                    ev.Sender.RemoteAdminMessage(player.Nickname + " is not <tranqillMaster>");
                    return;
                }
            }

            if (ev.Arguments.Count < 3)
            {
                ev.Sender.RemoteAdminMessage("Out of arguments. Usage: " + GetUsage());
                return;
            }
            if (player.GameObject.GetComponent<TranqillMaster>() != null)
            {
                ev.Sender.RemoteAdminMessage(player.Nickname + " is already <tranqillMaster>");
                return;
            }
            player.GameObject.AddComponent<TranqillMaster>();
            try
            {
                player.GameObject.GetComponent<TranqillMaster>().shootCount = System.Convert.ToInt32(ev.Arguments[1]);
                player.GameObject.GetComponent<TranqillMaster>().timeSleep = System.Convert.ToInt32(ev.Arguments[2]);
                if (player.GameObject.GetComponent<TranqillMaster>().shootCount <= 0 || player.GameObject.GetComponent<TranqillMaster>().timeSleep <= 0)
                {
                    throw new System.FormatException();
                }
            }
            catch (System.FormatException)
            {
                Object.Destroy(player.GameObject.GetComponent<TranqillMaster>());
                ev.Sender.RemoteAdminMessage("Invalid arguments (only positive numbers). Usage: " + GetUsage());
                return;
            }


            player.GameObject.GetComponent<TranqillMaster>().isMTFWeapon = false;
            player.GameObject.GetComponent<TranqillMaster>().isFullRp = true;

            if (ev.Arguments.Count > 3 && ev.Arguments[3] != null)
            {
                if (ev.Arguments[3] == "mtf")
                {
                    player.GameObject.GetComponent<TranqillMaster>().isMTFWeapon = true;
                }
                else if (ev.Arguments[3] == "frp")
                {
                    player.GameObject.GetComponent<TranqillMaster>().isFullRp = true;
                }
            }

            player.ClearBroadcasts();
            player.Broadcast(10, "<color=#228b22>*вам выдан транквилизатор с " + player.GameObject.GetComponent<TranqillMaster>().shootCount + " зарядами и временем действия:" + player.GameObject.GetComponent<TranqillMaster>().timeSleep + "*</color>", Broadcast.BroadcastFlags.Normal);
            ev.Sender.RemoteAdminMessage(player.Nickname + " add to <tranqillMaster> with " + player.GameObject.GetComponent<TranqillMaster>().shootCount + " shoots and " + player.GameObject.GetComponent<TranqillMaster>().timeSleep + " duration");
            return;
        }

        internal void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.Player.GameObject.GetComponent<BadgeComponent>() != null)
            {
                Object.Destroy(ev.Player.GameObject.GetComponent<BadgeComponent>());
            }
        }

        internal void OnCreatingPortal(CreatingPortalEventArgs ev)
        {
            scpCustomLock = !scpCustomLock;
            if (scpCustomLock)
            {
                List<GameObject> tempRagdolls = new List<GameObject>();
                foreach (Ragdoll rd in Object.FindObjectsOfType<Ragdoll>())
                {
                    if (Vector3.Distance(rd.transform.position, new Vector3(0f, -2000f, 0f) + Vector3.up * 1.5f) < 15f)
                    {
                        tempRagdolls.Add(Object.Instantiate(rd.gameObject));
                        Object.Destroy(rd.gameObject, 0);
                    }
                }
                foreach (GameObject rdObj in tempRagdolls)
                {
                    rdObj.transform.position = ev.Position;
                    NetworkServer.Spawn(rdObj);
                }
            }
        }

        internal void OnHandcuffing(HandcuffingEventArgs ev)
        {
            if (!ev.Target.IsCuffed)
            {
                if (ev.Target.GameObject.GetComponent<BadgeComponent>() != null)
                {
                    Object.Destroy(ev.Target.GameObject.GetComponent<BadgeComponent>());
                }
            }
        }

        internal void OnSendingConsoleCommand(SendingConsoleCommandEventArgs ev)
        {
            if (!Global.can_use_commands)
            {
                ev.ReturnMessage = "Дождитесь начала раунда!";
                return;
            }

            if (ev.Name == "bodypickup")
            {
                if ((ev.Player.Team == Team.SCP && ev.Player.Role != RoleType.Scp049) || ev.Player.Role == RoleType.Spectator)
                {
                    ev.ReturnMessage = Global._iswrongowner;
                    return;
                }

                if (ev.Player.GameObject.GetComponent<BadgeComponent>() != null)
                {
                    ev.ReturnMessage = Global._isalreadyholder;
                    return;
                }
                if (ev.Player.IsCuffed)
                {
                    ev.ReturnMessage = Global._iscuffed;
                    return;
                }

                foreach (Ragdoll rd in Object.FindObjectsOfType<Ragdoll>())
                {
                    if (Vector3.Distance(ev.Player.Position, rd.transform.position) < Global.distance_to_pickup && rd.gameObject != null)
                    {
                        ev.Player.GameObject.AddComponent<BadgeComponent>();
                        ev.Player.GameObject.GetComponent<BadgeComponent>().Body = Object.Instantiate(rd);
                        Object.Destroy(rd.gameObject, 0.0f);

                        ev.ReturnMessage = Global._successpickup + Player.Get(ev.Player.GameObject.GetComponent<BadgeComponent>().Body.owner.PlayerId).Nickname;
                        return;
                    }
                }
                ev.ReturnMessage = Global._istoolongtopickup;
                return;
            }
            else if (ev.Name == "bodydrop")
            {
                if (ev.Player.GameObject.GetComponent<BadgeComponent>() != null)
                {
                    ev.ReturnMessage = Global._successdrop;
                    Object.Destroy(ev.Player.GameObject.GetComponent<BadgeComponent>());
                    return;
                }
                else
                {
                    ev.ReturnMessage = Global._isnotholder;
                    return;
                }
            }
        }

        internal void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker.GameObject.GetComponent<TranqillMaster>() != null)
            {
                if (ev.Attacker.GameObject.GetComponent<TranqillMaster>().isFullRp)
                {
                    if (ev.DamageType.name == "COM15" || ev.DamageType.name == "USP")
                    {
                        ev.Amount = Global.damageOnFRP;
                    }
                }
            }
        }

        public string GetUsage()
        {
            return "bt <ник> <выстрелов> <время> (<mtf> or <frp>) | bt <nickname> <remove> (<silent>)";
        }

        public void OnWaitingForPlayers()
        {
            Global.can_use_commands = false;
        }
    }
}