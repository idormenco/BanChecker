using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanCheckerWPF.Classes
{
    public class Step
    {
        public Step() { }

        public Step(int currentStep, int firstStep, int secondStep)
        {
            this.currentStep = currentStep;
            this.firstParentStep = firstStep;
            this.secondParentStep = secondStep;
        }

        public Step(int currentStep, int firstStep)
        {
            this.currentStep = currentStep;
            this.firstParentStep = firstStep;
            this.secondParentStep = 0;
        }

        public Step(int currentStep) {
            this.currentStep = currentStep;
            this.firstParentStep = 0;
            this.secondParentStep = 0;
        }

        private int currentStep;

        public int CurrentStep
        {
            get { return currentStep; }
            set { currentStep = value; }
        }
        private int firstParentStep;

        public int FirstParentStep
        {
            get { return firstParentStep; }
            set { firstParentStep = value; }
        }
        private int secondParentStep;

        public int SecondParentStep
        {
            get { return secondParentStep; }
            set { secondParentStep = value; }
        }
    }
}
