using EXILED.Extensions;
using UnityEngine;

namespace BetterTranquilizer
{
    public class SleepTime : MonoBehaviour
    {
        public float timeSleep;
        public ReferenceHub target;
        public PlayerStats.HitInfo Info;

        private float currentTimeSleep = 0f;
        private float timer = 0f;
        private readonly float timeIsUp = 1.0f;

        public void Update()
        {
            currentTimeSleep += Time.deltaTime;
            timer += Time.deltaTime;
            if (timer >= timeIsUp)
            {
                timer = 0f;
                target.ClearBroadcasts();
                target.Broadcast(1, "<color=#228b22>*вы транквилизированы и очнетесь через: " + System.Math.Round((timeSleep - currentTimeSleep), 1) + " секунд*</color>", true);
                if (currentTimeSleep >= timeSleep)
                {
                    Destroy(gameObject.GetComponent<SleepTime>());
                }
            }
        }

        public void OnDestroy()
        {
            foreach (Ragdoll rd in FindObjectsOfType<Ragdoll>())
            {
                if (rd.owner.PlayerId == Info.PlyId && rd.owner.DeathCause.GetDamageType().name == Info.GetDamageType().name)
                {
                    Destroy(rd.gameObject, 0.0f);
                    target.SetPosition(rd.gameObject.transform.position + Vector3.up);
                    target.SetGodMode(false);
                    target.ClearBroadcasts();
                    target.Broadcast(10, "<color=#42aaff>*вы очнулись от транквилизатора*</color>", true);
                    break;
                }
            }
        }

    }
}