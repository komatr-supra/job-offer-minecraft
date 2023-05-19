using System;

//this is used in map manager(player digging), its a bit useless
//just dont want trust players
namespace Map
{
    public class Digger
    {
        private CounterSimple destroyCounter;
        private bool stopFlag;
        public Digger()
        {
            destroyCounter = new CounterSimple(() => stopFlag);
        }
        public void StartDigging(float digTime, Action onComplete)
        {
            stopFlag = false;
            destroyCounter.Start(digTime, onComplete);
        }
        public bool StopDigging() => stopFlag = true;

    }
}
