using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Astek.DesignPattern.HSM_Sequencer.Editor
{
    public class HSMExample_StateMachineDriver : MonoBehaviour
    {
        public PlayerContext ctx = new PlayerContext();
        public Transform groundCheck;
        public float groundRadius = 0.2f;
        public LayerMask groundMask;
        public bool drawGizmos = true;

        private string lastPath;
        private Rigidbody rb;
        StateMachine stateMachine;
        private State root;

        private void Awake()
        {
            rb=gameObject.GetOrAdd<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            ctx.Rb = rb;
            ctx.Anim=GetComponentInChildren<Animator>();

            root = new HSMExample_Root(null, ctx);
            StateMachineBuilder builder = new StateMachineBuilder(root);
            stateMachine=builder.Build();
        }

        private void Update()
        {
            float x = 0f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) x -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) x += 1f;
            ctx.JumpPressed = Keyboard.current.spaceKey.wasPressedThisFrame;
            ctx.Move.x = Mathf.Clamp(x, -1f, 1f);

            ctx.Grounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundMask);
            string path = StatePath(stateMachine.Root.Leaf());
            if (path != lastPath)
                lastPath = path;
        }
        private void FixedUpdate()
        {
            Vector3 v = rb.linearVelocity.With(x: ctx.Velocity.x);
            rb.linearVelocity = v;

            ctx.Velocity.x = rb.linearVelocity.x;
        }

        private void OnDrawGizmosSelected()
        {
            if (!drawGizmos || groundCheck == null) return;
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
        static string StatePath(State s) => string.Join(">", s.PathToRoot().Reverse().Select(n => n.GetType().Name));
    }

    [Serializable]
    public class PlayerContext
    {
        public Vector3 Move;
        public Vector3 Velocity;
        public bool Grounded;
        public float MoveSpeed = 6f;
        public float Accel = 40f;
        public float JumpSpeed = 7f;
        public bool JumpPressed;
        public Animator Anim;
        public Rigidbody Rb;
    }
}