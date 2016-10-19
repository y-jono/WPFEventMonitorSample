using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfApplication2
{
    class Resource : IDisposable
    {
        Timer timer_ = null;

        internal Resource()
        {
            Console.WriteLine($"new {nameof(Resource)}");
        }

        public void Dispose()
        {
            timer_.Dispose();
            Console.WriteLine($"Dispose {nameof(Resource)}");
        }

        public static volatile Int32 counter_ = 0;

        private event Action<int> listener_ = null;

        internal Task<int> StartProcWithMonitor(Action<int> listener)
        {
            if (listener != null)
            {
                listener_ = listener;
            }

            return Task<int>.Run(() =>
            {
                // イベント発行スレッド開始
                timer_ = new Timer(
                    (o) => {
                        counter_ = (Int32)counter_ + 1;

                        // TODO: delegateを登録できるようにする。
                        listener?.Invoke(counter_);
                        //queue_.Add(counter_);
                    },
                    null,
                    TimeSpan.FromMilliseconds(0),
                    TimeSpan.FromMilliseconds(1000)
                    );

                // 時間がかかる処理
                Thread.Sleep(TimeSpan.FromSeconds(1));

                return counter_;
            });
        }

        internal void UnregisterMonitor(Action<int> listener)
        {
            if (listener != null)
            {
                listener_ -= listener;
            }
        }
    }
}
