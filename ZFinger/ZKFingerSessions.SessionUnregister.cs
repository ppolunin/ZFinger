using System;

namespace ZKFinger
{
    using SDK;
    public static partial class ZKFingerSessions
    {
        private class SessionUnregister : Session
        {
            protected override ZKSessionType SessionType => ZKSessionType.Unregister;

            protected override void DoTemplateCaptured(byte[] template)
            {
                if (ZFingerSDK.Cache.RemoveTemplate(template, out int id) == ZFingerSDK.ZKFP_ERR_OK)
                {
                    switch (Storage.Remove(null, ref id))
                    {
                        case ZKStorageResult.OK:
                            Context.OnStateChanged(ZKSessionState.FingerUnregistered, StateArguments.From(new { ID = id }));
                            return;

                        case ZKStorageResult.ErrNotFound:
                            switch (Storage.Remove(template, ref id))
                            {
                                case ZKStorageResult.OK:
                                    Context.OnStateChanged(ZKSessionState.FingerUnregistered, StateArguments.From(new { ID = id }));
                                    break;
                            }
                            return;

                        // .....
                    }

                    Context.OnStateChanged(ZKSessionState.FingerNotIdetified, StateArguments.From(new { }));
                }
                else
                {
                    switch (Storage.Remove(template, ref id))
                    {
                        case ZKStorageResult.OK:
                            Context.OnStateChanged(ZKSessionState.FingerUnregistered, StateArguments.From(new { ID = id }));
                            return;
                    }

                    Context.OnStateChanged(ZKSessionState.FingerNotIdetified, StateArguments.From(new { }));
                }
            }
        }
    }
}
