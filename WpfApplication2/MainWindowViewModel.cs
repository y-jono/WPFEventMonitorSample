using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace WpfApplication2
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string text_ = "Hoge";
        public string Text
        {
            get
            {
                return text_;
            }
            set
            {
                text_ = value;
                NotifyPropertyChanged();
            }
        }

        BlockingCollection<int> queue_ = new BlockingCollection<int>();
        Task<int> WaitEvent10Async()
        {
            return Task<int>.Run(() =>
            {
                return queue_.Take();
            });
        }

        async internal void Process()
        {
            using (var r = new Resource())
            {
                Text = $"{DateTime.Now}"; // ボタン押下直後に時刻表示

                var t = WaitEvent10Async(); // Event "10" を待つスレッドを用意する

                // 処理を開始する.処理が発行するイベントを受信するリスナーを同時に登録できる。
                Action<int> monitor = new Action<int>((ev) =>
                {
                    if (ev != 10) { return; }
                    queue_.Add(ev);
                });
                var a = await r.StartProcWithMonitor(monitor);

                Text = $"{a}"; // 1秒後に1を表示

                var b = await t; // Event "10" を受信するまでメインスレッドは別のメッセージを処理する
                r.UnregisterMonitor(monitor);

                Text = $"{b}"; // 10秒後に10を表示
            }
        }

    }
}