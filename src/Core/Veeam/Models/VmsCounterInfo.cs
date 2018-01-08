namespace LMS.Veeam.Models
{
    public class VmsCounterInfo
    {
        public VmsCounterInfo(int nonTrialVmsCount, int trialVmsCount)
        {
            NonTrialVmsCount = nonTrialVmsCount;
            TrialVmsCount = trialVmsCount;
        }

        public int NonTrialVmsCount { get; set; }

        public int TotalVmsCount => TrialVmsCount + NonTrialVmsCount;

        public int TrialVmsCount { get; set; }
    }
}