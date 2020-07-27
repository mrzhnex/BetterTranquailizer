using Exiled.API.Features;

namespace BetterTranquilizer
{
    public class MainSettings : Plugin<Config>
    {
        public override string Name => nameof(BetterTranquilizer);
        public SetEvents SetEvents { get; set; }
        
        public override void OnEnabled()
        {
            try
            {
                Global.IsFullRp = Config.IsFullRp;
            }
            catch (System.Exception ex)
            {
                Log.Info("Catch an exception while getting boolean value from config file: " + ex.Message);
                Global.IsFullRp = false;
            }
            SetEvents = new SetEvents();
            Exiled.Events.Handlers.Server.RoundStarted += SetEvents.OnRoundStarted;
            Exiled.Events.Handlers.Player.Hurting += SetEvents.OnHurting;
            Exiled.Events.Handlers.Player.Shooting += SetEvents.OnShooting;
            Exiled.Events.Handlers.Server.SendingConsoleCommand += SetEvents.OnSendingConsoleCommand;
            Exiled.Events.Handlers.Scp106.CreatingPortal += SetEvents.OnCreatingPortal;
            Exiled.Events.Handlers.Player.Handcuffing += SetEvents.OnHandcuffing;
            Exiled.Events.Handlers.Player.ChangingRole += SetEvents.OnChangingRole;
            Exiled.Events.Handlers.Server.SendingRemoteAdminCommand += SetEvents.OnRemoteAdminCommandEvent;
            Exiled.Events.Handlers.Server.WaitingForPlayers += SetEvents.OnWaitingForPlayers;
            Log.Info(Name + " on");
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= SetEvents.OnRoundStarted;
            Exiled.Events.Handlers.Player.Hurting -= SetEvents.OnHurting;
            Exiled.Events.Handlers.Player.Shooting -= SetEvents.OnShooting;
            Exiled.Events.Handlers.Server.SendingConsoleCommand -= SetEvents.OnSendingConsoleCommand;
            Exiled.Events.Handlers.Scp106.CreatingPortal -= SetEvents.OnCreatingPortal;
            Exiled.Events.Handlers.Player.Handcuffing -= SetEvents.OnHandcuffing;
            Exiled.Events.Handlers.Player.ChangingRole -= SetEvents.OnChangingRole;
            Exiled.Events.Handlers.Server.SendingRemoteAdminCommand -= SetEvents.OnRemoteAdminCommandEvent;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= SetEvents.OnWaitingForPlayers;
            Log.Info(Name + " off");
        }
    }
}