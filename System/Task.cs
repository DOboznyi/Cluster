using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    class Task
    {
        double Time_init;
        List<double> Time_execute;
        double Time_end;
        double deadline;
        double time;
        int id;
        double trust;

        public Task(double Time_init, double deadline, double time, int id, double trust)
        {
            this.Time_init = Time_init;
            this.deadline = deadline;
            Time_execute = new List<double>();
            Time_end = -1;
            this.time = time;
            this.id = id;
            this.trust = trust;
        }

        public void execute(double Time)
        {
            Time_execute.Add(Time);
            time = 0;
        }

        public void execute(double Time, double quantum)
        {
            Time_execute.Add(Time);
            time -= quantum;
        }

        public double getTimeInit()
        {
            return Time_init;
        }

        public double getTimeEnd()
        {
            return Time_end;
        }

        public double getDeadline()
        {
            return deadline;
        }

        public List<double> getTimeExecute()
        {
            return Time_execute;
        }

        public int getId()
        {
            return id;
        }

        public void setEnd(double Time_end)
        {
            this.Time_end = Time_end;
        }

        public double getTime()
        {
            return time;
        }

        public double getTrust() {
            return trust;
        }
    }
}
