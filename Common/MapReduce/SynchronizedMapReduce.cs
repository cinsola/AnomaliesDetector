using Common.Providers;
using System.Threading.Tasks;

namespace Common.MapReduce
{
    public class SynchronizedMapReduce : ETLBaseMapReduce
    {
        private readonly IRankingManager _rankingManager;

        public SynchronizedMapReduce(ILogRowProvider fileFormatProvider, IRankingManager rankingManager) : base(fileFormatProvider)
        {
            _rankingManager = rankingManager;
        }

        public override Task<DeviceDetails> ProcessBatch(LogFile measures)
        {
            using (var stream = measures.ToSingleLogReadingsStream())
            {
                var singleLog = _rankingManager.FromSingleSensorReadings(stream);
                var ranking = _rankingManager.GetRanking(singleLog);
                return Task.FromResult(ranking);
            }
        }

    }
}
