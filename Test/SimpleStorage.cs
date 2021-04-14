using System;
using System.Collections.Generic;
using System.Linq;

namespace TestZFinger
{
   using System.IO;
    using ZKFinger;
    using ZKFinger.SDK;

    class SimpleStorage : IZKTemplatesStorage, IDisposable
    {
        static string FileName { get; set; } = ".\\storage.txt";

        private readonly Dictionary<int, byte[]> memory;
        private readonly HashSet<int> hashIDs;
        private readonly Random rand;

        private int IndexOf(byte[] template)
        {
            try
            {
                // O(N)
                return memory.First(item => ZFingerSDK.Template.Match(item.Value, template) > ZFingerSDK.Template.FP_TMP_MATCH_SCORE).Key;
            }
            catch (InvalidOperationException)
            {
                return -1;
            }
        }

        private int NewID()
        {
            if (hashIDs.Count == ZFingerSDK.MAX_CACHE_COUNT)
                return -1;

            int id;
            do
            {
               id = rand.Next(1, 50000);
            } while (hashIDs.Contains(id));

            return id;
        }

        public ZKStorageResult Check(byte[] template, ref int ID)
        {
            if (template == null && memory.ContainsKey(ID))
                return ZKStorageResult.OK;

            ID = IndexOf(template);
            if (ID != -1)
                return ZKStorageResult.OK;

            return ZKStorageResult.ErrNotFound;
        }

        public ZKStorageResult Remove(byte[] template, ref int ID)
        {
            if (template == null)
            {
                if (!memory.Remove(ID))
                    return ZKStorageResult.ErrNotFound;
            }
            else
            {
                ID = IndexOf(template);
                if (ID == -1)
                    return ZKStorageResult.ErrNotFound;

                memory.Remove(ID);
            }

            return ZKStorageResult.OK;
        }

        public ZKStorageResult Save(byte[] template, out int ID)
        {
            ID = IndexOf(template);
            if (ID != -1)
                return ZKStorageResult.ErrExists;

            ID = NewID();
            memory.Add(ID, template);

            return ZKStorageResult.OK;
        }

        ZKStorageResult IZKTemplatesStorage.Load(int ID, out byte[] template)
        {
            if (!memory.TryGetValue(ID, out template))
                return ZKStorageResult.ErrNotFound;

            return ZKStorageResult.OK;
        }

        public void Dispose()
        {
            using (StreamWriter writer = File.CreateText(FileName))
                foreach (var item in memory)
                {
                    writer.WriteLine($"{item.Key},{Convert.ToBase64String(item.Value)}");
                }
        }

        public SimpleStorage()
        {
            memory = new Dictionary<int, byte[]>();
            hashIDs = new HashSet<int>();
            rand = new Random();

            if (File.Exists(FileName))
                using (StreamReader reader = File.OpenText(FileName))
                    while (!reader.EndOfStream)
                    {
                        string[] items = reader.ReadLine().Split(',');
                        int id = Int32.Parse(items[0]);
                        memory.Add(id, Convert.FromBase64String(items[1]));
                        hashIDs.Add(id);
                    }
        }
    }
}
