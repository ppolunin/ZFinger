using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKFinger
{
    public enum ZKStorageResult : int
    {
        OK = 0,
        ErrNotFound = 1,
        ErrExists = 2
        // .....
    }

    public interface IZKTemplatesStorage
    {
        ZKStorageResult Load(int ID, out byte[] template);
        ZKStorageResult Save(byte[] template, out int ID);
        ZKStorageResult Check(byte[] template, ref int ID);
        ZKStorageResult Remove(byte[] template, ref int ID);
    }
}
