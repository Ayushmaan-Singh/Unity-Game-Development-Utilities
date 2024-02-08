#===== Usage =====#

#=== Defining Injectable Fields, Methods and Properties ===#
#Use the [Inject] attribute on fields, methods, or properties to mark them as targets for injection.

using DependencyInjection;
using UnityEngine;

public class ClassA : MonoBehaviour 
{
    [Inject] IServiceA serviceA;
    
    IServiceB serviceB;
    
    [Inject]
    public void Init(ServiceB service) {
        this.serviceB = service;
    }
    
    [Inject]
    public IServiceC Service { get; private set; }
}

#=== Creating Providers ===#
#Implement IDependencyProvider and use the [Provide] attribute on methods to define how dependencies are created.

using DependencyInjection;
using UnityEngine;

public class Provider : MonoBehaviour, IDependencyProvider 
{
    [Provide]
    public IServiceA ProvideServiceA() {
        return new ServiceA();
    }
}

#=== Example of Using Multiple Dependencies ===#

using DependencyInjection;
using UnityEngine;

public class ClassB : MonoBehaviour 
{
    [Inject] IServiceA serviceA;
    
    IServiceB serviceB;
    IFactoryA factoryA;
        
    [Inject] // Method injection supports multiple dependencies
    public void Init(IFactoryA factoryA, IServiceB serviceB) {
        this.factoryA = factoryA;
        this.serviceB = serviceB;
    }

    void Start() {
        serviceA.Initialize("ServiceA initialized from ClassB");
        serviceB.Initialize("ServiceB initialized from ClassB");
        factoryA.CreateServiceA().Initialize("ServiceA initialized from FactoryA");
    }
}

Setup
1.Include the Dependency Injection System: Add the provided DependencyInjection namespace and its classes to your project.
2.Add the Injector Component: Attach the Injector MonoBehaviour to a GameObject in your scene.
3.Define Providers: Create provider MonoBehaviours and attach them to GameObjects.
4.Mark Providers: Use [Provide] in your MonoBehaviours to provide a dependency of a particular type.
5.Mark Dependencies: Use [Inject] in your MonoBehaviours to satifsy dependencies.