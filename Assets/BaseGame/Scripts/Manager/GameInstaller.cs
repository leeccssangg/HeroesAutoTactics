using Game.Core;
using UnityEngine;
using Zenject;

namespace Game.Manager
{
    public class GameInstaller : MonoInstaller
    {
        [field: SerializeField] private Camera MainCamera { get; set; }

        public override void InstallBindings()
        {
            Container.Bind<Camera>().FromInstance(MainCamera).AsSingle();
            Container.Bind<InputManager>().FromInstance(InputManager.Instance).AsSingle();
            Container.Bind<GameManager>().FromInstance(GameManager.Instance).AsSingle();
            Container.Bind<AbilityResolveManager>().FromInstance(AbilityResolveManager.Instance).AsSingle();
            Container.Bind<TargetingManager>().FromInstance(TargetingManager.Instance).AsSingle();
            Container.Bind<FactoryManager>().FromInstance(FactoryManager.Instance).AsSingle();
            Container.Bind<DatabaseManager>().FromInstance(DatabaseManager.Instance).AsSingle();
            Container.Bind<InGameDataManager>().FromInstance(InGameDataManager.Instance).AsSingle();
            Container.Bind<InGameHandleManager>().FromInstance(InGameHandleManager.Instance).AsSingle();

            Container.BindFactory<Object, Hero, Hero.Factory>().FromFactory<PrefabFactory<Hero>>();
            Container.BindFactory<Object, Projectile, Projectile.Factory>().FromFactory<PrefabFactory<Projectile>>();
            Container.BindFactory<Object, CombatText, CombatText.Factory>().FromFactory<PrefabFactory<CombatText>>();
        }
    }
}