namespace AstekUtility.ServiceLocatorTool
{
    /// <summary>
    ///     Base interface for our service locator to work with. Services implementing
    ///     this interface will be retrievable using the locator.
    /// </summary>
    public interface IGameService
	{
		public void RegisterThisService() { }

		public void UnregisterThisService() { }
	}
}