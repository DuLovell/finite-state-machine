using UnityEngine;

namespace FSM
{
    public abstract class State
    {
        public abstract void OnEnter();
        public abstract void OnExit();

        public virtual void OnUpdate()
        {
        }

        public virtual void OnFixedUpdate()
        {
        }

        public virtual void OnTriggerEnter2D(Collider2D collider)
        {
        }
        
        public abstract string Name { get; }
    }
}
