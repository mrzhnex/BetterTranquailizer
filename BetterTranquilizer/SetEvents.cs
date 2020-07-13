using Dissonance.Integrations.MirrorIgnorance;
using EXILED;
using EXILED.Extensions;
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

        public void OnRoundStart()
        {
            Global.damageOnFRP = 1.0f;
            Global.can_use_commands = true;
        }

        public string GetUsage()
        {
            return "bt <ник> <выстрелов> <время> (<mtf> or <frp>) | bt <nickname> <remove> (<silent>)";
        }

        public void BTCommand(ref RACommandEvent ev)
        {
            string[] args = ev.Command.Split(' ');
            if (args.Length > 0 && args[0] != "bt")
                return;
            if (args.Length < 3)
            {
                ev.Sender.RAMessage("Out of arguments. Usage: " + GetUsage());
                return;
            }

            ReferenceHub p = Player.GetPlayer(args[1]);

            if (p == null)
            {
                ev.Sender.RAMessage("Player not found");
                return;
            }
            if (args[2] == "remove")
            {
                if (p.gameObject.GetComponent<TranqillMaster>() != null)
                {
                    UnityEngine.Object.Destroy(p.gameObject.GetComponent<TranqillMaster>());
                    bool silent = false;
                    if (args.Length > 3 && args[3] != null)
                    {
                        if (args[3] == "silent")
                        {
                            silent = true;
                        }
                    }
                    if (!silent)
                    {
                        p.ClearBroadcasts();
                        p.Broadcast(10, "<color=#228b22>*вы обнаружили, что у вас закончились транквилизаторы*", true);
                    }

                    ev.Sender.RAMessage(p.nicknameSync.Network_myNickSync + " remove from <tranqillMaster>");
                    return;
                }
                else
                {
                    ev.Sender.RAMessage(p.nicknameSync.Network_myNickSync + " is not <tranqillMaster>");
                    return;
                }
            }

            if (args.Length < 4)
            {
                Log.Info("catch tut");
                ev.Sender.RAMessage("Out of arguments. Usage: " + GetUsage());
                return;
            }
            GameObject tranqMaster = p.gameObject;
            if (tranqMaster.GetComponent<TranqillMaster>() != null)
            {
                ev.Sender.RAMessage(p.nicknameSync.Network_myNickSync + " is already <tranqillMaster>");
                return;
            }
            tranqMaster.AddComponent<TranqillMaster>();
            try
            {
                tranqMaster.GetComponent<TranqillMaster>().shootCount = System.Convert.ToInt32(args[2]);
                tranqMaster.GetComponent<TranqillMaster>().timeSleep = System.Convert.ToInt32(args[3]);
                if (tranqMaster.GetComponent<TranqillMaster>().shootCount <= 0 || tranqMaster.GetComponent<TranqillMaster>().timeSleep <= 0)
                {
                    throw new System.FormatException();
                }
            }
            catch (System.FormatException)
            {
                UnityEngine.Object.Destroy(tranqMaster.GetComponent<TranqillMaster>());
                ev.Sender.RAMessage("Invalid arguments (only positive numbers). Usage: " + GetUsage());
                return;
            }


            tranqMaster.GetComponent<TranqillMaster>().isMTFWeapon = false;
            tranqMaster.GetComponent<TranqillMaster>().isFullRp = true;

            if (args.Length > 4 && args[4] != null)
            {
                if (args[4] == "mtf")
                {
                    tranqMaster.GetComponent<TranqillMaster>().isMTFWeapon = true;
                }
                else if (args[4] == "frp")
                {
                    tranqMaster.GetComponent<TranqillMaster>().isFullRp = true;
                }
            }

            p.ClearBroadcasts();
            p.Broadcast(10, "<color=#228b22>*вам выдан транквилизатор с " + tranqMaster.GetComponent<TranqillMaster>().shootCount + " зарядами и временем действия:" + tranqMaster.GetComponent<TranqillMaster>().timeSleep + "*</color>", true);
            ev.Sender.RAMessage(p.nicknameSync.Network_myNickSync + " add to <tranqillMaster> with " + tranqMaster.GetComponent<TranqillMaster>().shootCount + " shoots and " + tranqMaster.GetComponent<TranqillMaster>().timeSleep + " duration");
            return;
        }

        internal void OnSetClass(EXILED.SetClassEvent ev)
        {
            if (ev.Player.gameObject.GetComponent<BadgeComponent>() != null)
            {
                Object.Destroy(ev.Player.gameObject.GetComponent<BadgeComponent>());
            }
        }

        public void OnPlayerHurt(ref EXILED.PlayerHurtEvent ev)
        {
            if (ev.Player.gameObject.GetComponent<TranqillMaster>() != null)
            {
                if (ev.Player.gameObject.GetComponent<TranqillMaster>().isFullRp)
                {
                    if (ev.DamageType.name == "COM15" || ev.DamageType.name == "USP")
                    {
                        ev.Amount = Global.damageOnFRP;
                    }
                }
            }
        }

        public void OnShoot(ref ShootEvent ev)
        {
            if (ev.Shooter.gameObject.GetComponent<TranqillMaster>() != null)
            {
                bool isTranqill = false;
                GameObject shooter = ev.Shooter.gameObject;
                if (ev.Shooter.GetCurrentItem().id == ItemType.GunE11SR && ev.Shooter.gameObject.GetComponent<TranqillMaster>().isMTFWeapon)
                {
                    shooter.GetComponent<TranqillMaster>().shootCount = shooter.GetComponent<TranqillMaster>().shootCount - 1;

                    if (ev.Target != null)
                        isTranqill = true;

                }
                else if (shooter.GetComponent<TranqillMaster>().isFullRp)
                {
                    if (ev.Shooter.GetCurrentItem().id == ItemType.GunUSP || ev.Shooter.GetCurrentItem().id == ItemType.GunCOM15)
                    {
                        shooter.GetComponent<TranqillMaster>().shootCount = shooter.GetComponent<TranqillMaster>().shootCount - 1;

                        if (ev.Target != null)
                        {
                            if (Vector3.Distance(ev.TargetPos, ev.Shooter.GetPosition()) <= Global.fullRPDistance)
                            {
                                isTranqill = true;
                            }
                            else
                            {
                                ev.Shooter.ClearBroadcasts();
                                ev.Shooter.Broadcast(3, "<color=#228b22>*транквилизатор не подействовал: вы были слишком далеко*</color>", true);
                            }
                        }


                    }
                }
                else if (ev.Shooter.GetCurrentItem().id == ItemType.GunUSP || ev.Shooter.GetCurrentItem().id == ItemType.GunCOM15)
                {
                    shooter.GetComponent<TranqillMaster>().shootCount = shooter.GetComponent<TranqillMaster>().shootCount - 1;

                    if (ev.Target != null)
                        isTranqill = true;
                }

                if (isTranqill)
                {
                    GameObject gameobj = ev.Target;
                    if (ev.Target.GetPlayer().GetRole() == RoleType.Scp049 || ev.Target.GetPlayer().GetRole() == RoleType.Scp93953 || ev.Target.GetPlayer().GetRole() == RoleType.Scp93989)
                    {
                        if (gameobj.GetComponent<TranqillStack>() == null)
                        {
                            gameobj.AddComponent<TranqillStack>();
                            gameobj.GetComponent<TranqillStack>().count = gameobj.GetComponent<TranqillStack>().count + 1;
                        }
                        else
                        {
                            gameobj.GetComponent<TranqillStack>().count = gameobj.GetComponent<TranqillStack>().count + 1;
                            if (gameobj.GetComponent<TranqillStack>().count >= Global.maxTranqillOnSCP)
                            {
                                UnityEngine.Object.Destroy(gameobj.GetComponent<TranqillStack>());

                                CharacterClassManager ccm = gameobj.GetComponent<CharacterClassManager>();
                                PlayerStats.HitInfo newInfo = new PlayerStats.HitInfo(1f, ev.Shooter.nicknameSync.Network_myNickSync, DamageTypes.None, ev.Target.GetPlayer().GetPlayerId());

                                gameobj.GetComponent<RagdollManager>().SpawnRagdoll(gameobj.transform.position, gameobj.transform.rotation, (int)ccm.CurClass, newInfo,
                                    ccm.Classes.SafeGet(ccm.CurClass).team > Team.SCP, gameobj.GetComponent<MirrorIgnorancePlayer>().PlayerId, gameobj.GetComponent<NicknameSync>().MyNick, gameobj.GetComponent<QueryProcessor>().PlayerId);

                                ev.Target.GetPlayer().ClearBroadcasts();
                                ev.Target.GetPlayer().Broadcast(5, "<color=#228b22>*вы транквилизированы и очнетесь через: " + shooter.GetComponent<TranqillMaster>().timeSleep + " секунд*</color>", true);
                                ev.Target.GetPlayer().SetGodMode(true);
                                gameobj.AddComponent<SleepTime>();
                                gameobj.GetComponent<SleepTime>().Info = newInfo;
                                gameobj.GetComponent<SleepTime>().target = ev.Target.GetPlayer();
                                gameobj.GetComponent<SleepTime>().timeSleep = shooter.GetComponent<TranqillMaster>().timeSleep;
                                ev.Target.GetPlayer().SetPosition(saveLocation);
                            }
                        }
                    }
                    else if (ev.Target.GetPlayer().GetRole() == RoleType.ChaosInsurgency || ev.Target.GetPlayer().GetRole() == RoleType.ClassD || ev.Target.GetPlayer().GetRole() == RoleType.FacilityGuard || ev.Target.GetPlayer().GetRole() == RoleType.NtfCadet || ev.Target.GetPlayer().GetRole() == RoleType.NtfCommander || ev.Target.GetPlayer().GetRole() == RoleType.NtfLieutenant || ev.Target.GetPlayer().GetRole() == RoleType.NtfScientist || ev.Target.GetPlayer().GetRole() == RoleType.Scientist)
                    {
                        CharacterClassManager ccm = gameobj.GetComponent<CharacterClassManager>();
                        PlayerStats.HitInfo newInfo = new PlayerStats.HitInfo(1f, ev.Shooter.nicknameSync.Network_myNickSync, DamageTypes.None, ev.Target.GetPlayer().GetPlayerId());
                        gameobj.GetComponent<RagdollManager>().SpawnRagdoll(gameobj.transform.position, gameobj.transform.rotation, (int)ccm.CurClass, newInfo,
                                    ccm.Classes.SafeGet(ccm.CurClass).team > Team.SCP, gameobj.GetComponent<MirrorIgnorancePlayer>().PlayerId, gameobj.GetComponent<NicknameSync>().MyNick, gameobj.GetComponent<QueryProcessor>().PlayerId);

                        ev.Target.GetPlayer().SetGodMode(true);
                        gameobj.AddComponent<SleepTime>();
                        gameobj.GetComponent<SleepTime>().Info = newInfo;
                        gameobj.GetComponent<SleepTime>().target = ev.Target.GetPlayer();
                        ev.Target.GetPlayer().ClearBroadcasts();
                        if (shooter.GetComponent<TranqillMaster>().isFullRp)
                        {
                            gameobj.GetComponent<SleepTime>().timeSleep = shooter.GetComponent<TranqillMaster>().timeSleep * 2;
                            ev.Target.GetPlayer().Broadcast(5, "<color=#228b22>*вы транквилизированы и очнетесь через: " + (shooter.GetComponent<TranqillMaster>().timeSleep * 2).ToString() + " секунд*</color>", true);
                        }
                        else
                        {
                            gameobj.GetComponent<SleepTime>().timeSleep = shooter.GetComponent<TranqillMaster>().timeSleep;
                            ev.Target.GetPlayer().Broadcast(5, "<color=#228b22>*вы транквилизированы и очнетесь через: " + shooter.GetComponent<TranqillMaster>().timeSleep.ToString() + " секунд*</color>", true);
                        }
                        ev.Target.GetPlayer().inventory.ServerDropAll();
                        ev.Target.GetPlayer().SetPosition(saveLocation);
                    }
                }
                if (shooter.GetComponent<TranqillMaster>().shootCount <= 0)
                {
                    UnityEngine.Object.Destroy(shooter.GetComponent<TranqillMaster>());
                    ev.Shooter.ClearBroadcasts();
                    ev.Shooter.Broadcast(5, "<color=#228b22>*у вас закончились транквилизаторы*</color>", true);
                }

            }
        }

        public void OnCallCommand(EXILED.ConsoleCommandEvent ev)
        {
            if (!Global.can_use_commands)
            {
                ev.ReturnMessage = "Дождитесь начала раунда!";
                return;
            }

            if (ev.Command == "bodypickup")
            {
                if ((ev.Player.GetTeam() == Team.SCP && ev.Player.GetRole() != RoleType.Scp049) || ev.Player.GetRole() == RoleType.Spectator)
                {
                    ev.ReturnMessage = Global._iswrongowner;
                    return;
                }
                GameObject gameobj = ev.Player.gameObject;
                if (gameobj.GetComponent<BadgeComponent>() != null)
                {
                    ev.ReturnMessage = Global._isalreadyholder;
                    return;
                }
                if (ev.Player.IsHandCuffed())
                {
                    ev.ReturnMessage = Global._iscuffed;
                    return;
                }

                foreach (Ragdoll rd in UnityEngine.Object.FindObjectsOfType<Ragdoll>())
                {
                    if (Vector3.Distance(gameobj.transform.position, rd.transform.position) < Global.distance_to_pickup && rd.gameObject != null)
                    {
                        gameobj.AddComponent<BadgeComponent>();
                        gameobj.GetComponent<BadgeComponent>().Body = UnityEngine.Object.Instantiate(rd);
                        UnityEngine.Object.Destroy(rd.gameObject, 0.0f);

                        ev.ReturnMessage = Global._successpickup + Player.GetPlayer(gameobj.GetComponent<BadgeComponent>().Body.owner.PlayerId).nicknameSync.Network_myNickSync;
                        return;
                    }
                }
                ev.ReturnMessage = Global._istoolongtopickup;
                return;
            }
            else if (ev.Command == "bodydrop")
            {
                GameObject gameobj = ev.Player.gameObject;
                if (gameobj.GetComponent<BadgeComponent>() != null)
                {
                    ev.ReturnMessage = Global._successdrop;
                    UnityEngine.Object.Destroy(gameobj.GetComponent<BadgeComponent>());
                    return;
                }
                else
                {
                    ev.ReturnMessage = Global._isnotholder;
                    return;
                }
            }

        }

        public void OnWaitingForPlayers()
        {
            try
            {
                Global.IsFullRp = Plugin.Config.GetBool("IsFullRp");
            }
            catch (System.Exception ex)
            {
                Log.Info("Catch an exception while getting boolean value from config file: " + ex.Message);
                Global.IsFullRp = false;
            }
            Global.can_use_commands = false;
        }

        public void On106CreatePortal(EXILED.Scp106CreatedPortalEvent ev)
        {
            scpCustomLock = !scpCustomLock;
            if (scpCustomLock)
            {
                List<GameObject> tempRagdolls = new List<GameObject>();
                foreach (Ragdoll rd in UnityEngine.Object.FindObjectsOfType<Ragdoll>())
                {
                    if (Vector3.Distance(rd.transform.position, new Vector3(0f, -2000f, 0f) + Vector3.up * 1.5f) < 15f)
                    {
                        tempRagdolls.Add(UnityEngine.Object.Instantiate(rd.gameObject));
                        UnityEngine.Object.Destroy(rd.gameObject, 0);
                    }
                }
                foreach (GameObject rdObj in tempRagdolls)
                {
                    rdObj.transform.position = ev.PortalPosition;
                    NetworkServer.Spawn(rdObj);
                }
            }

        }

        public void OnHandcuffed(ref HandcuffEvent ev)
        {
            if (ev.Player.IsHandCuffed())
            {
                if (ev.Player.gameObject.GetComponent<BadgeComponent>() != null)
                {
                    UnityEngine.Object.Destroy(ev.Player.gameObject.GetComponent<BadgeComponent>());
                }
            }
        }

        public void OnPlayerDie(ref PlayerDeathEvent ev)
        {
            if (ev.Player.gameObject.GetComponent<BadgeComponent>() != null)
            {
                UnityEngine.Object.Destroy(ev.Player.gameObject.GetComponent<BadgeComponent>());
            }
        }
    }
}