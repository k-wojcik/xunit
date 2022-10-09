using System;

namespace Xunit;

/// <summary>
/// Used to decorate xUnit.net test classes and collections to indicate a test which has
/// per-test-collection fixture data. An instance of the fixture data is initialized just before
/// the first test in the collection is run (including <see cref="IAsyncLifetime.InitializeAsync"/>
/// if it's implemented). After all the tests in the collection have been run, it is cleaned up
/// by calling <see cref="IAsyncDisposable.DisposeAsync"/> if it's implemented, or it falls back
/// to <see cref="IDisposable.Dispose"/> if that's implemented. Collection fixtures may have
/// a public constructor which is either empty, or accepts one or more assembly fixture objects
/// as constructor arguments. To gain access to the fixture data from inside the test, a constructor
/// argument should be added to the test class which exactly matches the <typeparamref name="TFixture"/>.
/// </summary>
/// <typeparam name="TFixture">The type of the fixture.</typeparam>
public interface ICollectionFixture<TFixture>
	where TFixture : class
{ }
