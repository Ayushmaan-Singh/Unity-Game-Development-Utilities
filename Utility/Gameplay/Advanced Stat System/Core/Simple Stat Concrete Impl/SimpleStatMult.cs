namespace Astek.AdvancedStatSys.Core
{
    /// <summary>
    /// For stacking stats
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleStatMult<T> : SimpleStat<T> where T : struct, IStatModifierData<T>
    {
        private int _zeroesCount;
        private float _nonZeroValue;

        public SimpleStatMult(float baseValue = 1) : base(baseValue)
        {
            ClearCachedValues(baseValue);
        }

        protected override bool AddOperation(StatModifier<T> modifier, float baseValue, float currentValue, out float newValue)
        {
            float value = 1 + modifier.Value;
            if (value == 0)
                _zeroesCount++;
            else
                _nonZeroValue *= value;

            newValue = _zeroesCount > 0 ? 0 : _nonZeroValue;
            return false;
        }
        protected override bool RemoveOperation(StatModifier<T> modifier, float baseValue, float currentValue, out float newValue)
        {
            float value = 1 + modifier.Value;
            if (value == 0)
                _zeroesCount--;
            else
                _nonZeroValue /= value;

            newValue = _zeroesCount > 0 ? 0 : _nonZeroValue;
            return false;
        }
        protected override bool SetBaseValue(float newBaseValue, float oldBaseValue, float currentValue, out float newValue)
        {
            if (oldBaseValue == 0)
                _zeroesCount--;
            else
                _nonZeroValue /= oldBaseValue;

            if (newBaseValue == 0)
                _zeroesCount++;
            else
                _nonZeroValue *= newBaseValue;

            newValue = _zeroesCount > 0 ? 0 : _nonZeroValue;
            return false;
        }

        protected sealed override void ClearCachedValues(float baseValue)
        {
            if (baseValue == 0)
            {
                _zeroesCount = 1;
                _nonZeroValue = 1;
            }
            else
            {
                _zeroesCount = 0;
                _nonZeroValue = baseValue;
            }
        }
    }
}