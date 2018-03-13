using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace Lab6
{
    public class Waiter
    {
        private Action<string> Callback;
        private ConcurrentStack<Glass> DirtyGlassStack;
        private ConcurrentStack<Glass> CleanGlassStack;
        private ConcurrentQueue<Patron> PatronQueue;
        public bool BarIsOpen { get; set; }
        private int waiterSpeed = 1;

        public void Work(Action<string> callback, ConcurrentStack<Glass> dirtyGlassStack, 
            ConcurrentStack<Glass> cleanGlassStack, bool bouncerIsWorking, ConcurrentQueue<Patron> patronQueue, 
            int waiterWashingSec, int waiterPickingGlassesSec, int glasses)
        {
            this.Callback = callback;
            this.DirtyGlassStack = dirtyGlassStack;
            this.CleanGlassStack = cleanGlassStack;
            this.BarIsOpen = bouncerIsWorking;
            this.PatronQueue = patronQueue;

            Task.Run(() =>
            {
                while (BarIsOpen)
                {
                    while (CleanGlassStack.Count() != glasses) 
                    {
                        if (!DirtyGlassStack.IsEmpty)
                        {
                            Callback("The waiter picks up dirty glasses from a table.");
                            Thread.Sleep(waiterWashingSec / waiterSpeed);
                            Callback("The waiter is washing glasses.");
                            Thread.Sleep(waiterPickingGlassesSec / waiterSpeed);
                            Callback("The waiter places the clean glasses back on the shelf.");
                            for (int i = 0; i < DirtyGlassStack.Count(); i++)
                            {
                                DirtyGlassStack.TryPop(out Glass g);
                                CleanGlassStack.Push(new Glass());
                            }
                        }
                    }
                }
                callback("The waiter goes home.");
            });
        }
        public void StopServing()
        {
            BarIsOpen = false;
        }

        public void ChangeSpeed(int speed)
        {
            this.waiterSpeed = speed; 
        }
    }
}
