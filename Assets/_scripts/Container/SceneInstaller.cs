using UnityEngine;
using Zenject;
//https://github.com/modesttree/Zenject/blob/master/Documentation/Signals.md
//https://github.com/modesttree/Zenject/blob/master/Documentation/CheatSheet.md
public class SceneInstaller : MonoInstaller<SceneInstaller>
{
    [Header("Managers")]
    [SerializeField] private HexMapManager _hexMapManager;
    [SerializeField] private MapResourceManager _mapResourceManager;
    [SerializeField] private InteractionManager _interactionManager;
    [SerializeField] private CameraManager _cameraManager;

    [Header("LocalData")]
    [SerializeField] private HexTypesContainer _hexTypesContainer;
    
    readonly SignalBus _signalBus;
    
   public override void InstallBindings()
       {
           SignalBusInstaller.Install(Container);
           Container.Bind<SceneInstaller>().AsSingle();
           Container.Bind<HexMapManager>().FromInstance(_hexMapManager).AsSingle();
           Container.Bind<HexTypesContainer>().FromInstance(_hexTypesContainer).AsSingle();
           Container.Bind<MapResourceManager>().FromInstance(_mapResourceManager).AsSingle();
           Container.Bind<InteractionManager>().FromInstance(_interactionManager).AsSingle();
           Container.Bind<CameraManager>().FromInstance(_cameraManager).AsSingle();
           
           Container.Bind<CoreSignals>().AsSingle();

           //SIGNALS >>>>>>>>>>>>>>>>
           Container.DeclareSignal<CoreSignals.HexSelectedSignal>();
           Container.DeclareSignal<CoreSignals.HexTargetedSignal>();
       }
}

public class CoreSignals
{
    public class HexSelectedSignal
    {
        public IInteractable SelectedHex;

        public HexSelectedSignal(IInteractable hex)
        {
            SelectedHex = hex;
        }
    }
    
    public class HexTargetedSignal
    {
        public IInteractable TargetedHex;

        public HexTargetedSignal(IInteractable hex)
        {
            TargetedHex = hex;
        }
    }
}
