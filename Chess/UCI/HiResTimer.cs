﻿/*
ChessLib, a chess data structure library

MIT License

Copyright (c) 2017-2018 Rudy Alex Kohn

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

namespace Rudz.Chess.UCI
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Properties;
    using Types;

    /// <summary>
    /// Idea From https://stackoverflow.com/a/41697139/548894
    /// </summary>
    public sealed class HiResTimer : IHiResTimer, IDisposable
    {
        public static readonly double TickLength = 1000f / Stopwatch.Frequency; // ms

        public static readonly double Frequency = Stopwatch.Frequency;

        private readonly object _intervalLock;

        private float _interval;

        private CancellationTokenSource _cancellationTokenSource;

        private Task _executer;

        public HiResTimer() => _intervalLock = new object();

        public HiResTimer(int id)
            : this(1f, id) { }

        public HiResTimer(float interval, int id)
        {
            _intervalLock = new object();
            Interval = interval;
            Id = id;
        }

        ~HiResTimer() { Dispose(false); }

        /// <summary>
        /// Invoked when the timer is elapsed
        /// </summary>
        public event EventHandler<HiResTimerArgs> Elapsed;

        public static bool IsHighResolution => Stopwatch.IsHighResolution;

        public int Id { get; set; }

        public float Interval {
            get {
                lock (_intervalLock)
                    return _interval;
            }

            set {
                if (value < 0f || float.IsNaN(value))
                    throw new ArgumentOutOfRangeException(nameof(value));
                lock (_intervalLock)
                    _interval = value;
            }
        }

        public bool IsRunning => _cancellationTokenSource != null && !_cancellationTokenSource.Token.IsCancellationRequested && _cancellationTokenSource.Token.CanBeCanceled;

        public bool UseHighPriorityThread { get; set; } = true;

        public static implicit operator HiResTimer(int id) => new HiResTimer(id);

        public static bool operator ==(HiResTimer left, HiResTimer right) => Equals(left, right);

        public static bool operator ==([NotNull] HiResTimer left, int right) => left.Id == right;

        public static bool operator ==([NotNull] HiResTimer left, Player right) => left.Id == right.Side;

        public static bool operator !=(HiResTimer left, HiResTimer right) => !Equals(left, right);

        public static bool operator !=([NotNull] HiResTimer left, int right) => left.Id != right;

        public static bool operator !=([NotNull] HiResTimer left, Player right) => left.Id != right.Side;

        public void Start()
        {
            if (IsRunning)
                Stop();

            Debug.Print("Timer Start on thread " + Thread.CurrentThread.ManagedThreadId);

            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.Token.ThrowIfCancellationRequested();
            _executer = Task.Factory.StartNew(() => ExecuteTimer(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();

            if (!IsRunning)
                return;

            Debug.WriteLine("Timer Stop on thread " + Thread.CurrentThread.ManagedThreadId);
            if (_executer.IsCanceled || _executer.IsCompleted)
                return;

            try {
                _executer.Wait(500);
                Debug.WriteLine("Timer Stop called ");
            } catch (AggregateException ae) {
                Debug.Print(ae.Message);
            } catch (ObjectDisposedException ode) {
                Debug.Print(ode.Message);
            }
        }

        public override int GetHashCode() => Id;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == GetType() && Equals((HiResTimer)obj);
        }

        public bool Equals(HiResTimer other) => Id == other.Id;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private static double ElapsedHiRes(Stopwatch stopwatch) => stopwatch.ElapsedTicks * TickLength;

        private void ExecuteTimer(CancellationToken cancellationToken)
        {
            Debug.Print("Timer ExecuteTimer on thread " + Thread.CurrentThread.ManagedThreadId);

            float nextTrigger = 0f;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (!cancellationToken.IsCancellationRequested) {
                nextTrigger += Interval;
                double elapsed;

                while (true) {
                    elapsed = ElapsedHiRes(stopwatch);
                    double diff = nextTrigger - elapsed;
                    if (diff <= 0f)
                        break;

                    if (diff < 1f)
                        Thread.SpinWait(10);
                    else if (diff < 5f)
                        Thread.SpinWait(100);
                    else if (diff < 15f)
                        Thread.Sleep(1);
                    else
                        Thread.Sleep(10);

                    if (cancellationToken.IsCancellationRequested)
                        return;
                }

                double delay = elapsed - nextTrigger;
                Elapsed?.Invoke(this, new HiResTimerArgs(delay));

                if (cancellationToken.IsCancellationRequested)
                    return;

                // restarting the timer in every hour to prevent precision problems
                if (stopwatch.Elapsed.TotalHours < 1d)
                    continue;
                stopwatch.Restart();
                nextTrigger = 0f;
            }

            stopwatch.Stop();
        }

        private void ReleaseUnmanagedResources()
        {
            // TODO release unmanaged resources here
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (!disposing)
                return;
            Debug.Print($"Timer with ID {Id} disposed.");
            _executer?.Dispose();
        }
    }

    // ReSharper disable once StyleCop.SA1402
    public class HiResTimerArgs : EventArgs, IHiResTimerArgs
    {
        internal HiResTimerArgs(double delay) => Delay = delay;

        public double Delay { get; }
    }
}