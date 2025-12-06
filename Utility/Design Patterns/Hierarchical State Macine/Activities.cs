using System;
using System.Threading;
using System.Threading.Tasks;
namespace AstekUtility.DesignPattern.HSM
{
    public enum ActivityMode { Inactive, Activating, Active, Deactivating }

    public interface IActivity
    {
        ActivityMode Mode { get; }
        Task ActivateAsync(CancellationToken cancellationToken);
        Task DeactivateAsync(CancellationToken cancellationToken);
    }

    public abstract class Activity : IActivity
    {
        public ActivityMode Mode { get; protected set; } = ActivityMode.Inactive;
        public virtual async Task ActivateAsync(CancellationToken cancellationToken)
        {
            if (Mode != ActivityMode.Inactive) return;

            Mode = ActivityMode.Activating;
            await Task.CompletedTask;
            Mode = ActivityMode.Active;
        }
        public virtual async Task DeactivateAsync(CancellationToken cancellationToken)
        {
            if (Mode != ActivityMode.Active) return;

            Mode = ActivityMode.Deactivating;
            await Task.CompletedTask;
            Mode = ActivityMode.Inactive;
        }
    }

    #region Some Activity Variants Presets

    public class DelayActivationActivity : Activity
    {
        public float Seconds = 0.2f;

        public override async Task ActivateAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(Seconds), cancellationToken);
            await base.ActivateAsync(cancellationToken);
        }
    }

    #endregion
}