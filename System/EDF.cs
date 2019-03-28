using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    class EDF
    {
        ListQueue<Task> queue;
        ListQueue<Task> listQ;
        ListQueue<Task> ready;
        double quantum;

        List<int> number;
        double free;
        double busy;

        Cluster cluster;
        double a;
        double b;
        public EDF(double lambda, List<double> t, List<int> k, int n, double quantum, int clusterNum, double a, double b, List<double> percent, double tetta, double eps, double crit)
        {
            queue = new ListQueue<Task>();
            ListQueue<Task> listQueue = new ListQueue<Task>();
            Random random = new Random();
            int id = 0;
            for (int i = 0; i < t.Count; i++)
            {
                double time = 0;
                double timeDeadline = k.ElementAt(i) * t.ElementAt(i);
                for (int j = 0; j < n; j++)
                {
                    time += generateEvent(lambda);
                    double timeTaskInit = time;
                    double trust = random.NextDouble();
                    listQueue.Enqueue(new Task(timeTaskInit, timeDeadline, t.ElementAt(i), id, trust));
                    id++;
                }
            }
            // Надо добавить идентификаторы что бы потом удобно переносить
            while (listQueue.Count != 0)
            {
                double initTime = double.MaxValue;
                int ptr = 0;
                for (int i = 0; i < listQueue.Count; i++)
                {
                    if (listQueue.ElementAt(i).getTimeInit() < initTime)
                    {
                        ptr = i;
                        initTime = listQueue.ElementAt(i).getTimeInit();
                    }
                }
                queue.Enqueue(listQueue.ElementAt(ptr));
                listQueue.RemoveAt(ptr);
            }
            listQ = new ListQueue<Task>();
            ready = new ListQueue<Task>();
            //overdue = new ListQueue<Task>();
            this.quantum = quantum;
            cluster = new Cluster(clusterNum, percent, tetta, eps, crit);
            this.a = a;
            this.b = b;
        }

        public EDF(ListQueue<Task> queue)
        {
            this.queue = queue;
        }

        public void run()
        {
            number = new List<int>();
            free = 0;
            busy = 0;
            double time = 0;
            while ((listQ.Count != 0) || (queue.Count != 0))
            {
                checkQueue(time);
                /*//Загружаем задачи из плана
                for (int i = 0;i<queue.Count;i++) {
                    Task t = queue.ElementAt(i);
                    if (time > t.getTimeInit()) {
                        listQ.Add(t);
                    }
                }
                //Удаляем просроченные задачи
                List<int> ptr = new List<int>();
                for (int i = 0; i < queue.Count; i++) {
                    Task t = listQ.ElementAt(i);
                    if (time > (t.getTimeInit()+t.getDeadline()))
                    {
                        ptr.Add(t.getId());
                    }
                }
                foreach (int p in ptr) {
                    Task t=null;
                    foreach (Task t1 in listQ) {
                        if (t1.getId() == p) {
                            t = t1;
                            break;
                        }
                    }
                    t.setEnd(time);
                    ready.Add(t);
                    listQ.Remove(t);
                }
                */
                cluster.voting();
                if ((listQ.Count > 0) || (cluster.freeCluster(time)))
                {
                    number.Add(listQ.Count);
                    double nextTime = double.MaxValue;
                    while ((listQ.Count > 0) && (cluster.freeCluster(time)))
                    {
                        double T = 0;
                        double score = double.MaxValue;
                        int ptr = -1;
                        for (int i = 0; i < listQ.Count; i++)
                        {
                            if (getScore(listQ.ElementAt(i),time)<score)
                            {
                                score = getScore(listQ.ElementAt(i), time);
                                ptr = i;
                            }
                        }
                        T = listQ.ElementAt(ptr).getTime();
                        listQ.ElementAt(ptr).execute(time);
                        Task t = listQ.ElementAt(ptr);
                        listQ.Remove(t);
                        //В список решенных
                        ready.Enqueue(t);
                        //time += T;
                        //busy += T;
                        if (T < nextTime) {
                            nextTime = T;
                        }
                    }
                    time += nextTime;
                }
                else
                {
                    double initTime = double.MaxValue;
                    int ptr = 0;
                    for (int i = 0; i < queue.Count; i++)
                    {
                        if (queue.ElementAt(i).getTimeInit() < initTime)
                        {
                            ptr = i;
                            initTime = queue.ElementAt(i).getTimeInit();
                        }
                    }
                    if (initTime == double.MaxValue)
                    {
                        time = double.MaxValue;
                    }
                    else
                    {
                        free += (initTime + 0.00000000000001) - time;
                        time = initTime + 0.00000000000001;
                    }
                }
            }
        }

        public void checkQueue(double time)
        {
            List<int> ptr = new List<int>();
            //Загружаем задачи из плана
            for (int i = 0; i < queue.Count; i++)
            {
                Task t = queue.ElementAt(i);
                if (time > t.getTimeInit())
                {
                    listQ.Enqueue(t);
                    ptr.Add(t.getId());
                }
            }
            foreach (int p in ptr)
            {
                Task t = null;
                foreach (Task t1 in queue)
                {
                    if (t1.getId() == p)
                    {
                        t = t1;
                        break;
                    }
                }
                queue.Remove(t);
            }
            //Удаляем просроченные задачи
            ptr = new List<int>();
            for (int i = 0; i < listQ.Count; i++)
            {
                Task t = listQ.ElementAt(i);
                if (time > (t.getTimeInit() + t.getDeadline()))
                {
                    ptr.Add(t.getId());
                }
            }
            foreach (int p in ptr)
            {
                Task t = null;
                foreach (Task t1 in listQ)
                {
                    if (t1.getId() == p)
                    {
                        t = t1;
                        break;
                    }
                }
                t.setEnd(time);
                ready.Enqueue(t);
                listQ.Remove(t);
            }
        }

        double generateEvent(double lamda)
        {
            Random rand = new Random();
            double result = -1.0 / lamda * Math.Log(rand.NextDouble());
            return result;
        }

        public Dictionary<int, int> getTimeN()
        {
            Dictionary<int, int> d = new Dictionary<int, int>();
            foreach (Task t in ready)
            {
                int time = 0;
                if (t.getTimeExecute().Count != 0) time = (int)(t.getTimeExecute().ElementAt(0) - t.getTimeInit());
                else time = (int)t.getDeadline();
                if (d.ContainsKey(time)) d[time]++;
                else d.Add(time, 1);
            }
            return d;
        }

        public double getTimeM()
        {
            double d = 0;
            foreach (Task t in ready)
            {
                double time = 0;
                if (t.getTimeExecute().Count != 0) time = t.getTimeExecute().ElementAt(0) - t.getTimeInit();
                else time = t.getDeadline();
                d += time / ready.Count;
            }
            return d;
        }

        public double getQueueSize()
        {
            double d = 0;
            foreach (int a in number)
            {
                d += ((double)a) / number.Count;
            }
            return d;
        }

        public int getOverdueNum()
        {
            int d = 0;
            foreach (Task t in ready)
            {
                if (t.getTime() > 0) d += 1;
            }
            return d;
        }

        public double getOverduePercent()
        {
            return ((double)getOverdueNum()) / ready.Count;
        }

        public double getFreePercent()
        {
            return free / (free + busy);
        }

        public double getScore(Task t, double time) {
            double dead = time - (t.getTimeInit() + t.getDeadline());
            double trust = t.getDeadline();
            return dead * a + trust * b;
        }
    }
}