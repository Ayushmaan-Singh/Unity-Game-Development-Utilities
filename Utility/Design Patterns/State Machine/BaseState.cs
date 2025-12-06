using System;
namespace AstekUtility.DesignPattern.StateMachine
{
    public interface IState
    {
        void OnStateEnter();
        void Update(float deltaTime);
        void OnStateExit();
    }

    /// <summary>
    /// Used in the class has its own data alongside state logic
    /// </summary>
    public class BaseState : IState
    {
        public virtual void OnStateEnter() { }
        public virtual void Update(float deltaTime) { }
        public virtual void OnStateExit() { }
    }
    /// <summary>
    /// Used in the case the class doesn't have any data and just state logic
    /// </summary>
    public class BaseState_Event : IState
    {
        public event Action OnStateEnter_Event = delegate { };
        public event Action<float> OnUpdate_Event = delegate { };
        public event Action OnStateExit_Event = delegate { };
        
        public void OnStateEnter() => OnStateEnter_Event.Invoke();
        public void Update(float deltaTime) => OnUpdate_Event.Invoke(deltaTime);
        public void OnStateExit() => OnStateExit_Event.Invoke();
    }
}