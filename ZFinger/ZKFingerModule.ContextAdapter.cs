using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ZKFinger
{
    public static partial class ZKFingerModule
    {
        private class ContextAdapter
        {
            private readonly Action<ZKSessionState, object> stateChanged;
            private readonly SynchronizationContext synchronizationContext;

            public void OnStateChanged(ZKSessionState state, object args)
            {
                if (synchronizationContext != null)
                    synchronizationContext.Send(obj => stateChanged(state, obj), args);
                else
                    stateChanged(state, args);
            }

            public ContextAdapter(Action<ZKSessionState, object> stateChanged, SynchronizationContext context)
            {
                if (context == null)
                    synchronizationContext = SynchronizationContext.Current;
                else
                    synchronizationContext = context;

                this.stateChanged = stateChanged;
            }
        }
    }
}
