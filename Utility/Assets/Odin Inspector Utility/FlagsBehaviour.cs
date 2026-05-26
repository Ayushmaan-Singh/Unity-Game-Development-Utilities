using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Astek.Odin
{
	public interface IFlagsBehaviour<in T> where T : Enum
	{
		bool HasFlag(T tag);
		void AddFlag(T tag);
		void RemoveFlag(T tag);
	}

	[HideMonoScript]
	public abstract class FlagsBehaviour<T> : MonoBehaviour, IFlagsBehaviour<T> where T : Enum
	{
		[SerializeField, HideInInspector]
		private T flags;

		[ShowInInspector, ShowIf("IsFlags"), HideLabel, EnumToggleButtons]
		public T Flags
		{
			get => flags;
			set => flags = value;
		}

		[ShowInInspector, HideIf("IsFlags"), HideLabel]
		private T Enum
		{
			get => Flags;
			set => Flags = value;
		}

		private bool IsFlags()
		{
			return typeof(T).IsDefined(typeof(FlagsAttribute), false);
		}

		public bool HasFlag(T flag)
		{
			return Flags.Has(flag);
		}

		public void AddFlag(T flag)
		{
			if (!HasFlag(flag)) Flags = Flags.Add<T>(flag);
		}

		public void RemoveFlag(T flag)
		{
			Flags = Flags.Remove<T>(flag);
		}
	}

	public static class FlagsExtensions
	{
		public static bool Has(this Enum self, Enum value)
		{
			try
			{
				return ((int)(object)self & (int)(object)value) == (int)(object)value;
			}
			catch
			{
				return false;
			}
		}

		public static T Add<T>(this Enum self, Enum value) where T : Enum
		{
			try
			{
				return (T)(object)((int)(object)self | (int)(object)value);
			}
			catch (Exception ex)
			{
				throw new ArgumentException($"Could not append value from enumerated type '{typeof(T).Name}'.", ex);
			}
		}

		public static T Remove<T>(this Enum self, Enum value) where T : Enum
		{
			try
			{
				return (T)(object)((int)(object)self & ~(int)(object)value);
			}
			catch (Exception ex)
			{
				throw new ArgumentException($"Could not remove value from enumerated type '{typeof(T).Name}'.", ex);
			}
		}

		private static bool TryGetComponent<T>(this GameObject self, out IFlagsBehaviour<T> comp) where T : Enum
		{
			comp = self.GetComponent<IFlagsBehaviour<T>>();
			return (comp != null);
		}

		public static bool HasFlag<T>(this GameObject self, T flag) where T : Enum
		{
			return self.TryGetComponent(out IFlagsBehaviour<T> comp) && comp.HasFlag(flag);
		}

		public static void AddFlag<T>(this GameObject self, T flag) where T : Enum
		{
			if (self.TryGetComponent(out IFlagsBehaviour<T> comp)) comp.AddFlag(flag);
		}

		public static void RemoveFlag<T>(this GameObject self, T flag) where T : Enum
		{
			if (self.TryGetComponent(out IFlagsBehaviour<T> comp)) comp.RemoveFlag(flag);
		}

		private static bool TryGetComponent<T>(this Component self, out IFlagsBehaviour<T> comp) where T : Enum
		{
			comp = self.GetComponent<IFlagsBehaviour<T>>();
			return (comp != null);
		}

		public static T Flags<T>(this Component self) where T:Enum
		{
			if (!self.TryGetComponent(out FlagsBehaviour<T> comp))
				throw new NullReferenceException($"{self.gameObject.name} does not have any flag behaviour");

			return comp.Flags;
		}
		public static bool HasFlag<T>(this Component self, T flag) where T : Enum
		{
			return self.TryGetComponent(out IFlagsBehaviour<T> comp) && comp.HasFlag(flag);
		}

		public static void AddFlag<T>(this Component self, T flag) where T : Enum
		{
			if (self.TryGetComponent(out IFlagsBehaviour<T> comp)) comp.AddFlag(flag);
		}

		public static void RemoveFlag<T>(this Component self, T flag) where T : Enum
		{
			if (self.TryGetComponent(out IFlagsBehaviour<T> comp)) comp.RemoveFlag(flag);
		}
	}
}