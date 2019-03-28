using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    class Cluster
    {
        List<Unit> units;
        List<double> percent;
        double tetta;
        double eps;
        double crit;

        public Cluster(int n, List<double> percent, double tetta, double eps, double crit) {
            units = new List<Unit>();
            for (int i = 0; i < n; i++) {
                units.Add(new Unit());
            }
            this.percent = percent;
            this.tetta = tetta;
            this.eps = eps;
            this.crit = crit;
        }

        public bool freeCluster(double time) {
            foreach (Unit unit in units) {
                if (unit.getFree(time)) {
                    return true;
                }
            }
            return false;
        }

        public void setTask(double time) {
            foreach (Unit unit in units)
            {
                if (unit.getFree(time))
                {
                    unit.setTask(time);
                    return;
                }
            }
        }

        public void voting() {
            for (int i = 0; i < units.Count; i++) {
                Random r = new Random();
                int[] votes = new int[6] { 0, 0, 0, 0, 0, 0 };
                for (int j = 0; j < units.Count; j++)
                { 
                    if (r.NextDouble() < 0.5) {
                        votes[units.ElementAt(j).getTrust()+3]++;
                    }
                    else votes[units.ElementAt(j).getTrust()]++;
                }
                double betta = (votes[0] - Math.Pow(votes[3], tetta) + 1 / Math.Pow(votes[1], tetta) - Math.Pow(votes[4], tetta)) / (votes[0]+votes[1]+Math.Pow(eps,2)/(votes[0] + votes[1]));
                if (betta > crit) {
                    int perc = 0;
                    for (int j = 0; j < units.Count; j++)
                    {
                        if (units.ElementAt(j).getTrust() == (units.ElementAt(i).getTrust() + 1))
                        {
                            perc++;
                        }
                    }
                    perc++;
                    perc /= units.Count;
                    if (perc >= percent.ElementAt(units.ElementAt(i).getTrust() + 1)) {
                        units.ElementAt(i).addTrust();
                    }
                }
            }
        }
    }
}
