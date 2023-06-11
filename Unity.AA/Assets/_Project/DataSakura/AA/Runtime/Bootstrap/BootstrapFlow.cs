using DataSakura.Runtime.Utilities;
using DataSakura.Runtime.Utilities.Logging;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace DataSakura.Runtime.Bootstrap
{
    public class BootstrapFlow : IStartable
    {
        private readonly LoadingService _loadingService;
        private readonly ConfigContainer _configContainer;

        public BootstrapFlow(LoadingService loadingService, ConfigContainer configContainer)
        {
            _loadingService = loadingService;
            _configContainer = configContainer;
        }

        public async void Start()
        {
            Log.Default.D("BootstrapFlow.Start()");

            await _loadingService.BeginLoading(new ApplicationConfigurationLoadUnit());
            await _loadingService.BeginLoading(_configContainer);
            SceneManager.LoadScene(RuntimeConstants.Scenes.Battle);
        }
    }
}