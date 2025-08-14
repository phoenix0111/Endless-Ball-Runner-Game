using UnityEngine;


[AddComponentMenu("ServiceLocator/ ServiceLocator Global")]
public class ServiceLocatorGlobalBootstrapper : Bootstrapper
{
    [SerializeField] private bool dontDestroyOnLoad = false;
    protected override void Bootstrap()
    {
        Container.ConfigureAsGlobal(dontDestroyOnLoad);
    }
}
