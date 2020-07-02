using EXILED;

namespace BetterTranquilizer
{
    public class MainSetting : Plugin
    {
        public override string getName => "BetterTranquilizer";
        private SetEvents setEvents;
        
        public override void OnEnable()
        {
            setEvents = new SetEvents();
            Events.RoundStartEvent += setEvents.OnRoundStart;
            Events.PlayerHurtEvent += setEvents.OnPlayerHurt;
            Events.ShootEvent += setEvents.OnShoot;
            Events.ConsoleCommandEvent += setEvents.OnCallCommand;
            Events.Scp106CreatedPortalEvent += setEvents.On106CreatePortal;
            Events.PlayerHandcuffedEvent += setEvents.OnHandcuffed;
            Events.PlayerDeathEvent += setEvents.OnPlayerDie;
            Events.RemoteAdminCommandEvent += setEvents.BTCommand;
            Events.SetClassEvent += setEvents.OnSetClass;
            Events.WaitingForPlayersEvent += setEvents.OnWaitingForPlayers;
            Log.Info(getName + " on");
        }

        public override void OnDisable()
        {
            Events.RoundStartEvent -= setEvents.OnRoundStart;
            Events.PlayerHurtEvent -= setEvents.OnPlayerHurt;
            Events.ShootEvent -= setEvents.OnShoot;
            Events.ConsoleCommandEvent -= setEvents.OnCallCommand;
            Events.Scp106CreatedPortalEvent -= setEvents.On106CreatePortal;
            Events.PlayerHandcuffedEvent -= setEvents.OnHandcuffed;
            Events.PlayerDeathEvent -= setEvents.OnPlayerDie;
            Events.RemoteAdminCommandEvent -= setEvents.BTCommand;
            Events.SetClassEvent -= setEvents.OnSetClass;
            Events.WaitingForPlayersEvent -= setEvents.OnWaitingForPlayers;
            Log.Info(getName + " off");
        }

        public override void OnReload() { }
    }
}