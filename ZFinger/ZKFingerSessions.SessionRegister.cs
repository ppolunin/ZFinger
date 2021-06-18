using System;
using System.Collections.Generic;

namespace ZKFinger
{
    using SDK;

    public static partial class ZKFingerSessions
    {
        private class SessionRegister : Session
        {
            private readonly List<byte[]> _templates;

            public int Remained => SDK.ZFingerSDK.REGISTER_FINGER_COUNT - _templates.Count;

            protected override ZKSessionType SessionType => ZKSessionType.Register;

            protected override void DoTemplateCaptured(byte[] template)
            {
                if (_templates.Count == 0)
                {
                    if (ZFingerSDK.Cache.IdenditfyTemplate(template, out int id, out int _) == ZFingerSDK.ZKFP_ERR_OK)
                    {
                        Context.OnStateChanged(ZKSessionState.FingerAlreadyRegistered, StateArguments.From(new { ID = id }));
                        return; // такой пальчик уже есть
                    }
                    else if (Storage.Check(template, ref id) == ZKStorageResult.OK)
                    {
                        if (Storage.Load(id, out byte[] regtemp) == ZKStorageResult.OK)
                            ZFingerSDK.Cache.InsertTemplate(id, regtemp);

                        Context.OnStateChanged(ZKSessionState.FingerAlreadyRegistered, StateArguments.From(new { ID = id }));
                        return; // такой пальчик уже есть
                    }

                    _templates.Add(template);
                    Context.OnStateChanged(ZKSessionState.FingerIsMatched, StateArguments.From(new { @Remained = this.Remained }));
                } 
                else if (ZFingerSDK.Template.MatchWithList(template, _templates))
                {
                    _templates.Add(template);
                    Context.OnStateChanged(ZKSessionState.FingerIsMatched, StateArguments.From(new { @Remained = this.Remained }));
                }
                else
                {
                    Context.OnStateChanged(ZKSessionState.FingerIsNotMatched, StateArguments.From(new { @Remained = this.Remained })); 
                    return; // неудача приложите еще раз пальчик
                }

                if (Remained == 0)
                {
                    try
                    {
                        int result;
                        if ((result = ZFingerSDK.Template.MergeListInto(_templates, out byte[] regtemp)) == ZFingerSDK.ZKFP_ERR_OK)
                            switch (Storage.Save(regtemp, out int savedID))
                            {
                                case ZKStorageResult.OK:
                                    ZFingerSDK.Cache.InsertTemplate(savedID, regtemp);
                                    Context.OnStateChanged(ZKSessionState.FingerRegisterSuccess, StateArguments.From(new { ID = savedID }));
                                    break;
                                // идеальный вариант...

                                case ZKStorageResult.ErrExists: 
                                    Context.OnStateChanged(ZKSessionState.FingerAlreadyRegistered, StateArguments.From(new { ID = savedID }));
                                    break; // такого не должно случиться
                            }
                        else
                            Context.OnStateChanged(ZKSessionState.FingerMergeError, StateArguments.From(new { SDKErr = result }));
                    }
                    finally
                    {
                        _templates.Clear();
                    }
                }
            }

            public SessionRegister()
                : base()
            {
                _templates = new List<byte[]>();
            }
        }
    }
}
