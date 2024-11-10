using Com.UnBocal.Rush.Properties;
using Com.UnBocal.Rush.Tickables;
using DG.Tweening;
using UnityEngine;

namespace Com.UnBocal.Rush.Debugs
{
    public class DebugTick : Tickable
    {

        int test = 0;

        protected override void Tick()
        {
            if (++test < 4) return;
            test = 0;
            m_transform.DOScale(Vector3.one * .5f, Game.Properties.TickInterval * 2f).From(Vector3.one * 1.1f);
        }
    }
}