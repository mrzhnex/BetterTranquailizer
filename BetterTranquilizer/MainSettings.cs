using EXILED;

namespace BetterTranquilizer
{
    public class MainSettings : Plugin
    {
        public override string getName => nameof(BetterTranquilizer);
        public SetEvents SetEvents { get; set; }
        
        public override void OnEnable()
        {
            SetEvents = new SetEvents();
            Events.RoundStartEvent += SetEvents.OnRoundStart;
            Events.PlayerHurtEvent += SetEvents.OnPlayerHurt;
            Events.ShootEvent += SetEvents.OnShoot;
            Events.ConsoleCommandEvent += SetEvents.OnCallCommand;
            Events.Scp106CreatedPortalEvent += SetEvents.On106CreatePortal;
            Events.PlayerHandcuffedEvent += SetEvents.OnHandcuffed;
            Events.PlayerDeathEvent += SetEvents.OnPlayerDie;
            Events.RemoteAdminCommandEvent += SetEvents.BTCommand;
            Events.SetClassEvent += SetEvents.OnSetClass;
            Events.WaitingForPlayersEvent += SetEvents.OnWaitingForPlayers;
            Log.Info(getName + " on");
        }

        public override void OnDisable()
        {
            Events.RoundStartEvent -= SetEvents.OnRoundStart;
            Events.PlayerHurtEvent -= SetEvents.OnPlayerHurt;
            Events.ShootEvent -= SetEvents.OnShoot;
            Events.ConsoleCommandEvent -= SetEvents.OnCallCommand;
            Events.Scp106CreatedPortalEvent -= SetEvents.On106CreatePortal;
            Events.PlayerHandcuffedEvent -= SetEvents.OnHandcuffed;
            Events.PlayerDeathEvent -= SetEvents.OnPlayerDie;
            Events.RemoteAdminCommandEvent -= SetEvents.BTCommand;
            Events.SetClassEvent -= SetEvents.OnSetClass;
            Events.WaitingForPlayersEvent -= SetEvents.OnWaitingForPlayers;
            Log.Info(getName + " off");
        }

        public override void OnReload() { }
    }
}