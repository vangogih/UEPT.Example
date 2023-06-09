using System;
using DataSakura.AA.Runtime.Utilities;
using DataSakura.AA.Runtime.Utilities.Logging;
using VContainer.Unity;

namespace DataSakura.AA.Runtime.Battle
{
    public class BattleFlow : IStartable, IDisposable
    {
        private readonly LoadingService _loadingService;
        private readonly PlaneFactory _planeFactory;

        public BattleFlow(LoadingService loadingService, PlaneFactory planeFactory)
        {
            _loadingService = loadingService;
            _planeFactory = planeFactory;
        }
        
        public async void Start()
        {
            await _loadingService.BeginLoading(_planeFactory);

            _planeFactory.SpawnOrGetPlayerPlane(RuntimeConstants.Planes.Corncob);
            Log.Battle.D("BattleFlow.Start()");
        }

        public void Dispose()
        {
            Log.Battle.D("BattleFlow.Dispose()");
        }
    }
}