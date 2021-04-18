using System;
using System.Drawing;
using System.Threading;

namespace ZKFinger
{
    using SDK;
    using System.Collections.Generic;
    using System.Linq;

    public enum ZKSessionType : int
    {
        Register,
        Identify,
        Unregister
    }

    public enum ZKSessionState : int
    {
        Opened,
        FingerAcquired,
        FingerAlreadyRegistered,
        FingerIdetified,
        FingerIsMatched,
        FingerIsNotMatched,
        FingerMergeError,
        FingerNotIdetified,
        FingerRegisterSuccess,
        FingerUnregistered,
        FingerWarnUnregistered,
        Exception,
        Closed
    }

    public static partial class ZKFingerModule
    {
        private static ContextAdapter Context;
        private static IZKTemplatesStorage Storage;

        private abstract class Session
        {
            private Device _device;
            private volatile bool _closing;
            private readonly Thread _thread;
            private readonly Thread currentThread;

            protected abstract ZKSessionType SessionType { get; }

            private void ThreadProc()
            {
                Context.OnStateChanged(ZKSessionState.Opened, StateArguments.From(new { @Session = SessionType, Device = StateArguments.From(_device) }));
                try
                {
                    while (!_closing)
                    {
                        if (_device.AcquireFingerprint(out byte[] template))
                        {
                            Context.OnStateChanged(ZKSessionState.FingerAcquired, StateArguments.From(new { GetImage = new Func<Bitmap>(_device.GetImage), Template = template }));
                            DoTemplateCaptured(template);
                        }

                        Thread.Sleep(100);
                    }
                }
                finally
                {
                    Context.OnStateChanged(ZKSessionState.Closed, StateArguments.From(new { @Session = SessionType }));
                }
            }

            protected abstract void DoTemplateCaptured(byte[] template);

            static Session()
            {
                if (ZFingerSDK.InitSDKAndDevices() < ZFingerSDK.ZKFP_ERR_OK)
                    throw new InvalidOperationException("The library initialization error occurred");
            }

            public Session()
            {
                _device = new Device();
                currentThread = Thread.CurrentThread;
                _thread = new Thread(ThreadProc) { IsBackground = true };
                _thread.Start();
            }

            public virtual void Dispose()
            {
                _closing = true;
                if (currentThread != Thread.CurrentThread &&
                    _thread != Thread.CurrentThread)
                {
                    _thread.Join();
                }
                _device.Dispose();
            }
        }

        private static readonly object _lock = new object();
        private static Session _session;

        public static void OpenSession(ZKSessionType type, IZKTemplatesStorage storage,
            Action<ZKSessionState, object> stateChanged, SynchronizationContext context = null)
        {
            lock (_lock)
            {
                if (_session != null)
                    throw new InvalidOperationException($"The another inastance of session ({_session.GetType().Name}) is already opened");
                else
                {
                    if (storage == null)
                        throw new ArgumentNullException("A \"storage\" parameter is NULL");

                    if (stateChanged == null)
                        throw new ArgumentNullException("A \"stateChanged\" parameter is NULL");

                    Context = new ContextAdapter(stateChanged, context);
                    Storage = storage;

                    switch (type)
                    {
                        case ZKSessionType.Register:
                            _session = new SessionRegister();
                            break;

                        case ZKSessionType.Unregister:
                            _session = new SessionUnregister();
                            break;

                        default:
                            _session = new SessionIdentify();
                            break;
                    }
                }
            }
        }

        public static bool SessionIsActive
        {
            get
            {
                lock (_lock)
                {
                    return _session != null;
                }
            }
        }

        public static void CloseSession()
        {
            lock(_lock)
            {
                if (_session != null)
                {
                    _session.Dispose();
                    _session = null;
                }
            }
        }
    }
}