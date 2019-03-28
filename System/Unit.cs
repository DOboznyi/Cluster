using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    class Unit
    {
        bool free;
        double timeUntilBusy;
        int trust;

        public Unit() {
            free = true;
            timeUntilBusy = -1;
            trust = 0;
        }

        public bool getFree(double time) {
            if (time > timeUntilBusy) {
                free = true;
                timeUntilBusy = -1;
            }
            return free;
        }

        public void setTask(double time) {
            free = false;
            timeUntilBusy = time;
        }

        public void addTrust() {
            trust += 1;
        }

        public int getTrust() {
            return trust;
        }
    }
}
