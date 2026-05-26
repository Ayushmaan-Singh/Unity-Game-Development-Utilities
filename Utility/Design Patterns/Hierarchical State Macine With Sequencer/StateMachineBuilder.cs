using System.Collections.Generic;
using System.Reflection;
namespace Astek.DesignPattern.HSM_Sequencer
{
    public class StateMachineBuilder
    {
        private readonly State _root;

        public StateMachineBuilder(State root)
        {
            _root = root;
        }

        public StateMachine Build()
        {
            StateMachine m = new StateMachine(_root);
            Wire(_root,m,new HashSet<State>());
            return m;
        }

        private void Wire(State s, StateMachine m, HashSet<State> visited)
        {
            if (s == null) return;
            if (!visited.Add(s)) return; //State is already wired

            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
            FieldInfo machineField = typeof(State).GetField("Machine", flags);
            if (machineField != null) machineField.SetValue(s, m);

            foreach (FieldInfo fld in s.GetType().GetFields(flags))
            {
                if (!typeof(State).IsAssignableFrom(fld.FieldType)) continue; //Only consider fields that are state
                if(fld.Name=="Parent") continue; //Skip back-edge to parent
                State child = (State)fld.GetValue(s);
                if(child==null) continue;
                if (!ReferenceEquals(child.Parent, s)) continue;//Ensure it's our direct child
                
                Wire(child, m, visited);//Recurse into the child
            }
        }
    }
}