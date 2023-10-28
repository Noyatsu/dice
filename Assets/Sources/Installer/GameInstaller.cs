using SSTraveler.Game;
using Zenject;

namespace SSTraveler.Installer
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IGameProcessManager>().To<GameProcessManager>().AsSingle();
        }
    }
}