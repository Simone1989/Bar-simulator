using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace Lab6
{
   public class Patron
    {
        public string Name { get; set; }
        Queue<string> patronNameQueue = new Queue<string>();
        Queue<string> tempQueue = new Queue<string>();

        private int patronDrinkingIntervalMin = 10000;
        private int patronDrinkingIntervalMax = 20000;

        public Patron(string name)
        {
            this.Name = name;
            patronNameQueue.Enqueue(Name);
        }

        private Action<string> Callback;
        private ConcurrentStack<Chair> FreeChairStack;
        private ConcurrentQueue<Patron> PatronQueue;
        private ConcurrentStack<Glass> DirtyGlassStack;
        public string BeerDrinkingPatron { get; set; }
        Random random = new Random();

        //Function that tells the Patron to "sit down" and drink the beer before disappearing from the queue
        public void SitDown(Action<string> callback, ConcurrentStack<Glass> dirtyGlassStack, ConcurrentStack<Chair> freeChairStack,
            ConcurrentQueue<Patron> patronQueue, ConcurrentQueue<string> uiPatronCountDeQueue, int speed)
        {
            this.Callback = callback;
            this.DirtyGlassStack = dirtyGlassStack;
            this.FreeChairStack = freeChairStack;
            this.PatronQueue = patronQueue;

            Task.Run(() =>
            {
                tempQueue.Enqueue(PatronQueue.FirstOrDefault().Name);
                BeerDrinkingPatron = tempQueue.First();
                tempQueue.Dequeue();
                PatronQueue.TryDequeue(out Patron p); 

                while (FreeChairStack.IsEmpty)
                {
                    Callback($"{BeerDrinkingPatron} is looking for a place to sit.");
                    Thread.Sleep(1000 / speed);
                }
                FreeChairStack.TryPop(out Chair c);
                Thread.Sleep(4000/ speed);
                Callback($"{BeerDrinkingPatron} sits down.");
                Thread.Sleep(random.Next(patronDrinkingIntervalMin / speed, patronDrinkingIntervalMax / speed));
                uiPatronCountDeQueue.TryDequeue(out string s);
                FreeChairStack.Push(new Chair());
                DirtyGlassStack.Push(new Glass());
                Callback($"{BeerDrinkingPatron} finishes the beer and leaves the bar.");
            });
        }
    }
}
