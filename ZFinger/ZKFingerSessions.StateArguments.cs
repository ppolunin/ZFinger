using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKFinger
{
    public static partial class ZKFingerSessions
    {
        private class StateArguments : DynamicObject
        {
            private readonly object _arg;

            private object GetValue(string name)
            {
                return _arg.GetType().GetProperties().First(info => info.Name == name).GetValue(_arg);
            }

            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            {
                try
                {
                    Delegate val = (Delegate)GetValue(binder.Name);
                    result = val.Method.Invoke(val.Target, args);
                }
                catch
                {
                    return base.TryInvokeMember(binder, args, out result);
                }

                return true;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                try
                {
                    result = GetValue(binder.Name);
                }
                catch
                {
                    return base.TryGetMember(binder, out result);
                }

                return true;
            }

            private StateArguments(object arg) { _arg = arg; }

            public static dynamic From(object arg) { return new StateArguments(arg); }
        }
    }
}
