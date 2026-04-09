namespace Astek
{
    public static class ArrayExtension
    {
        public static void AddRange<T>(this T[] array, params T[] items)
        {
            T[] newArray = new T[array.Length + items.Length];
            array.CopyTo(newArray, 0);
            items.CopyTo(newArray, array.Length);
        }
    }
}