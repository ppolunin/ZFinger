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
            private class ContextArgs
            {
                public ZKSessionState State { get; private set; }
                public object Args { get; private set; }

                public ContextArgs(ZKSessionState state, object args)
                {
                    State = state;
                    Args = args;
                }
            }

            private readonly Action<ZKSessionState, object> stateChanged;
            private readonly SynchronizationContext synchronizationContext;

            private void ContextProc(object obj)
            {
                ContextArgs args = obj as ContextArgs;
                stateChanged(args.State, args.Args);
            }

            public void OnStateChanged(ZKSessionState state, object args)
            {
                if (synchronizationContext != null)
                    synchronizationContext.Send(ContextProc, new ContextArgs(state, args));
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
