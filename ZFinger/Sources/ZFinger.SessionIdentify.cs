using System;

namespace ZKFinger
{
    using SDK;
    public static partial class ZKFingerModule
    {
        private class SessionIdentify : Session
        {
            protected override ZKSessionType SessionType => ZKSessionType.Identify;

            protected override void DoTemplateCaptured(byte[] template)
            {
                if (ZFingerSDK.Cache.IdenditfyTemplate(template, out int id, out int _) == ZFingerSDK.ZKFP_ERR_OK)
                {
                    switch (Storage.Check(null, ref id))
                    {
                        case ZKStorageResult.OK:
                            Context.OnStateChanged(ZKSessionState.FingerIdetified, StateArguments.From(new { ID = id }));
                            return;

                        case ZKStorageResult.ErrNotFound:
                            ZFingerSDK.Cache.RemoveTemplate(id);

                            switch (Storage.Check(template, ref id))
                            {
                                case ZKStorageResult.OK:
                                    if (Storage.Load(id, out template) == ZKStorageResult.OK)
                                        ZFingerSDK.Cache.InsertTemplate(id, template);

                                    Context.OnStateChanged(ZKSessionState.FingerIdetified, StateArguments.From(new { ID = id }));
                                    break;
                            }
                            return;

                        // .....
                    }

                    Context.OnStateChanged(ZKSessionState.FingerNotIdetified, StateArguments.From(new {  }));
                }
                else
                {
                    switch (Storage.Check(template, ref id))
                    {
                        case ZKStorageResult.OK:
                            if (Storage.Load(id, out template) == ZKStorageResult.OK)
                                ZFingerSDK.Cache.InsertTemplate(id, template);

                            Context.OnStateChanged(ZKSessionState.FingerIdetified, StateArguments.From(new { ID = id }));
                            return;

                        // .....
                    }

                    Context.OnStateChanged(ZKSessionState.FingerNotIdetified, StateArguments.From(new {  }));
                }
            }
        }
    }
}
