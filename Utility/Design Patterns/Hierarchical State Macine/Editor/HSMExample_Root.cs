using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
namespace AstekUtility.DesignPattern.HSM.Editor
{
    public class HSMExample_Root : State
    {
        public readonly Grounded Grounded;
        public readonly Airborne Airborne;
        private readonly PlayerContext ctx;

        public HSMExample_Root(StateMachine m, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Grounded = new Grounded(m, this, ctx);
            Airborne = new Airborne(m, this, ctx);
        }

        protected override State GetInitialState() => Grounded;
        protected override State GetTransition() => ctx.Grounded ? null : Airborne;
    }

    public class Grounded : State
    {
        readonly PlayerContext ctx;
        public readonly Idle Idle;
        public readonly Move Move;

        public Grounded(StateMachine machine, State parent, PlayerContext ctx) : base(machine, parent)
        {
            this.ctx = ctx;
            Idle = new Idle(machine, this, ctx);
            Move = new Move(machine, this, ctx);
            Add(new DelayActivationActivity() { Seconds = 0.50f });
        }

        protected override State GetInitialState() => Idle;
        protected override State GetTransition()
        {
            if (ctx.JumpPressed)
            {
                ctx.JumpPressed = false;
                Rigidbody rb = ctx.Rb;

                if (rb != null)
                {
                    Vector3 v = rb.linearVelocity;
                    v.y = ctx.JumpSpeed;
                    rb.linearVelocity = v;
                }
                return ((HSMExample_Root)Parent).Airborne;
            }
            return ctx.Grounded ? null : ((HSMExample_Root)Parent).Airborne;
        }
    }
    public class Idle : State
    {
        readonly PlayerContext ctx;

        public Idle(StateMachine m, State parent, PlayerContext ctx) : base(m, parent)
        {
            this.ctx = ctx;
        }

        protected override State GetTransition() => Mathf.Abs(ctx.Move.x) > 0.01f ? ((Grounded)Parent).Move : null;

        protected override void OnEnter()
        {
            ctx.Velocity.x = 0f;
        }
    }
    public class Move : State
    {
        readonly PlayerContext ctx;


        public Move(StateMachine machine, State parent, PlayerContext ctx) : base(machine, parent)
        {
            this.ctx = ctx;
        }

        protected override State GetTransition()
        {
            if (!ctx.Grounded)
                return ((HSMExample_Root)Parent).Airborne;

            return Mathf.Abs(ctx.Move.x) <= 0.01f ? ((Grounded)Parent).Idle : null;
        }

        protected override void OnUpdate(float deltaTime)
        {
            float target = ctx.Move.x * ctx.MoveSpeed;
            ctx.Velocity.x = Mathf.MoveTowards(ctx.Velocity.x, target, ctx.Accel * deltaTime);
        }
    }

    public class Airborne : State
    {
        readonly PlayerContext ctx;

        public Airborne(StateMachine machine, State parent, PlayerContext ctx) : base(machine, parent)
        {
            this.ctx = ctx;
        }

        protected override State GetTransition() => ctx.Grounded ? ((HSMExample_Root)Parent).Grounded : null;

        protected override void OnEnter()
        {
            //Update animator like going from locomotion to jump animation
        }
    }
}