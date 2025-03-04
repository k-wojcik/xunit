using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit.Internal;
using Xunit.Sdk;

namespace Xunit.Runner.Common;

/// <summary>
/// Caches message metadata for xUnit.net v3 messages. The metadata which is cached depends on
/// the message that is passed (for example, passing a <see cref="TestAssemblyMessage"/> will store
/// and/or return <see cref="IAssemblyMetadata"/>). Storage methods require the Starting versions
/// of messages, as these are the ones which contain the metadata.
/// </summary>
public class MessageMetadataCache
{
	readonly Dictionary<string, object> cache = [];

	/// <summary>
	/// Sets <see cref="IAssemblyMetadata"/> into the cache.
	/// </summary>
	/// <param name="message">The message that contains the metadata.</param>
	public void Set(TestAssemblyStarting message)
	{
		Guard.ArgumentNotNull(message);
		Guard.NotNull(
			() => string.Format(
				CultureInfo.CurrentCulture,
				"{0} cannot be null when setting metadata for {1}",
				nameof(TestAssemblyStarting.AssemblyUniqueID),
				typeof(TestAssemblyStarting).SafeName()
			),
			message.AssemblyUniqueID
		);

		InternalSet(message.AssemblyUniqueID, message);
	}

	/// <summary>
	/// Sets <see cref="ITestCaseMetadata"/> into the cache.
	/// </summary>
	/// <param name="message">The message that contains the metadata.</param>
	public void Set(TestCaseStarting message)
	{
		Guard.ArgumentNotNull(message);
		Guard.NotNull(
			() => string.Format(
				CultureInfo.CurrentCulture,
				"{0} cannot be null when setting metadata for {1}",
				nameof(TestCaseStarting.TestCaseUniqueID),
				typeof(TestCaseStarting).SafeName()
			),
			message.TestCaseUniqueID
		);

		InternalSet(message.TestCaseUniqueID, message);
	}

	/// <summary>
	/// Sets <see cref="ITestClassMetadata"/> into the cache.
	/// </summary>
	/// <param name="message">The message that contains the metadata.</param>
	public void Set(TestClassStarting message)
	{
		Guard.ArgumentNotNull(message);
		Guard.NotNull(
			() => string.Format(
				CultureInfo.CurrentCulture,
				"{0} cannot be null when setting metadata for {1}",
				nameof(TestClassStarting.TestClassUniqueID),
				typeof(TestClassStarting).SafeName()
			),
			message.TestClassUniqueID
		);

		InternalSet(message.TestClassUniqueID, message);
	}

	/// <summary>
	/// Sets <see cref="ITestCollectionMetadata"/> into the cache.
	/// </summary>
	/// <param name="message">The message that contains the metadata.</param>
	public void Set(TestCollectionStarting message)
	{
		Guard.ArgumentNotNull(message);
		Guard.NotNull(
			() => string.Format(
				CultureInfo.CurrentCulture,
				"{0} cannot be null when setting metadata for {1}",
				nameof(TestCollectionStarting.TestCollectionUniqueID),
				typeof(TestCollectionStarting).SafeName()
			),
			message.TestCollectionUniqueID
		);

		InternalSet(message.TestCollectionUniqueID, message);
	}

	/// <summary>
	/// Sets <see cref="ITestMetadata"/> into the cache.
	/// </summary>
	/// <param name="message">The message that contains the metadata.</param>
	public void Set(TestStarting message)
	{
		Guard.ArgumentNotNull(message);
		Guard.NotNull(
			() => string.Format(
				CultureInfo.CurrentCulture,
				"{0} cannot be null when setting metadata for {1}",
				nameof(TestStarting.TestUniqueID),
				typeof(TestStarting).SafeName()
			),
			message.TestUniqueID
		);

		InternalSet(message.TestUniqueID, message);
	}

	/// <summary>
	/// Sets <see cref="ITestMethodMetadata"/> into the cache.
	/// </summary>
	/// <param name="message">The message that contains the metadata.</param>
	public void Set(TestMethodStarting message)
	{
		Guard.ArgumentNotNull(message);
		Guard.NotNull(
			() => string.Format(
				CultureInfo.CurrentCulture,
				"{0} cannot be null when setting metadata for {1}",
				nameof(TestMethodStarting.TestMethodUniqueID),
				typeof(TestMethodStarting).SafeName()
			),
			message.TestMethodUniqueID
		);

		InternalSet(message.TestMethodUniqueID, message);
	}

	/// <summary>
	/// Attempts to retrieve <see cref="IAssemblyMetadata"/> from the cache (and optionally remove it).
	/// </summary>
	/// <param name="assemblyUniqueID">The unique ID of the assembly to retrieve.</param>
	/// <param name="remove">Set to <c>true</c> to remove the metadata after retrieval.</param>
	/// <returns>The cached metadata, if present; or <c>null</c> if there isn't any.</returns>
	public IAssemblyMetadata? TryGetAssemblyMetadata(
		string assemblyUniqueID,
		bool remove = false) =>
			InternalGetAndRemove(Guard.ArgumentNotNull(assemblyUniqueID), remove) as IAssemblyMetadata;

	/// <summary>
	/// Attempts to retrieve <see cref="IAssemblyMetadata"/> from the cache.
	/// </summary>
	/// <param name="message">The message that indicates which metadata to retrieve.</param>
	/// <returns>The cached metadata, if present; or <c>null</c> if there isn't any.</returns>
	public IAssemblyMetadata? TryGetAssemblyMetadata(TestAssemblyMessage message) =>
		InternalGetAndRemove(Guard.ArgumentNotNull(message).AssemblyUniqueID, false) as IAssemblyMetadata;

	/// <summary>
	/// Attempts to retrieve <see cref="ITestCaseMetadata"/> from the cache (and optionally remove it).
	/// </summary>
	/// <param name="testCaseUniqueID">The unique ID of the test case to retrieve.</param>
	/// <param name="remove">Set to <c>true</c> to remove the metadata after retrieval.</param>
	/// <returns>The cached metadata, if present; or <c>null</c> if there isn't any.</returns>
	public ITestCaseMetadata? TryGetTestCaseMetadata(
		string testCaseUniqueID,
		bool remove = false) =>
			InternalGetAndRemove(Guard.ArgumentNotNull(testCaseUniqueID), remove) as ITestCaseMetadata;

	/// <summary>
	/// Attempts to retrieve <see cref="ITestCaseMetadata"/> from the cache.
	/// </summary>
	/// <param name="message">The message that indicates which metadata to retrieve.</param>
	/// <returns>The cached metadata, if present; or <c>null</c> if there isn't any.</returns>
	public ITestCaseMetadata? TryGetTestCaseMetadata(TestCaseMessage message) =>
		InternalGetAndRemove(Guard.ArgumentNotNull(message).TestCaseUniqueID, false) as ITestCaseMetadata;

	/// <summary>
	/// Attempts to retrieve <see cref="ITestClassMetadata"/> from the cache (and optionally remove it).
	/// </summary>
	/// <param name="testClassUniqueID">The unique ID of the test class to retrieve.</param>
	/// <param name="remove">Set to <c>true</c> to remove the metadata after retrieval.</param>
	/// <returns>The cached metadata, if present; or <c>null</c> if there isn't any.</returns>
	public ITestClassMetadata? TryGetClassMetadata(
		string testClassUniqueID,
		bool remove = false) =>
			InternalGetAndRemove(Guard.ArgumentNotNull(testClassUniqueID), remove) as ITestClassMetadata;

	/// <summary>
	/// Attempts to retrieve <see cref="ITestClassMetadata"/> from the cache.
	/// </summary>
	/// <param name="message">The message that indicates which metadata to retrieve.</param>
	/// <returns>The cached metadata, if present; or <c>null</c> if there isn't any.</returns>
	public ITestClassMetadata? TryGetClassMetadata(TestClassMessage message) =>
		InternalGetAndRemove(Guard.ArgumentNotNull(message).TestClassUniqueID, false) as ITestClassMetadata;

	/// <summary>
	/// Attempts to retrieve <see cref="ITestCollectionMetadata"/> from the cache (and optionally remove it).
	/// </summary>
	/// <param name="testCollectionUniqueID">The unique ID of the test collection to retrieve.</param>
	/// <param name="remove">Set to <c>true</c> to remove the metadata after retrieval.</param>
	/// <returns>The cached metadata, if present; or <c>null</c> if there isn't any.</returns>
	public ITestCollectionMetadata? TryGetCollectionMetadata(
		string testCollectionUniqueID,
		bool remove = false) =>
			InternalGetAndRemove(Guard.ArgumentNotNull(testCollectionUniqueID), remove) as ITestCollectionMetadata;

	/// <summary>
	/// Attempts to retrieve <see cref="ITestCollectionMetadata"/> from the cache.
	/// </summary>
	/// <param name="message">The message that indicates which metadata to retrieve.</param>
	/// <returns>The cached metadata, if present; or <c>null</c> if there isn't any.</returns>
	public ITestCollectionMetadata? TryGetCollectionMetadata(TestCollectionMessage message) =>
		InternalGetAndRemove(Guard.ArgumentNotNull(message).TestCollectionUniqueID, false) as ITestCollectionMetadata;

	/// <summary>
	/// Attempts to retrieve <see cref="ITestMethodMetadata"/> from the cache (and optionally remove it).
	/// </summary>
	/// <param name="testMethodUniqueID">The unique ID of the test method to retrieve.</param>
	/// <param name="remove">Set to <c>true</c> to remove the metadata after retrieval.</param>
	/// <returns>The cached metadata, if present; or <c>null</c> if there isn't any.</returns>
	public ITestMethodMetadata? TryGetMethodMetadata(
		string testMethodUniqueID,
		bool remove = false) =>
			InternalGetAndRemove(Guard.ArgumentNotNull(testMethodUniqueID), remove) as ITestMethodMetadata;

	/// <summary>
	/// Attempts to retrieve <see cref="ITestMethodMetadata"/> from the cache.
	/// </summary>
	/// <param name="message">The message that indicates which metadata to retrieve.</param>
	/// <returns>The cached metadata, if present; or <c>null</c> if there isn't any.</returns>
	public ITestMethodMetadata? TryGetMethodMetadata(TestMethodMessage message) =>
		InternalGetAndRemove(Guard.ArgumentNotNull(message).TestMethodUniqueID, false) as ITestMethodMetadata;

	/// <summary>
	/// Attempts to retrieve <see cref="ITestMetadata"/> from the cache (and optionally remove it).
	/// </summary>
	/// <param name="testUniqueID">The unique ID of the test to retrieve.</param>
	/// <param name="remove">Set to <c>true</c> to remove the metadata after retrieval.</param>
	/// <returns>The cached metadata, if present; or <c>null</c> if there isn't any.</returns>
	public ITestMetadata? TryGetTestMetadata(
		string testUniqueID,
		bool remove = false) =>
			InternalGetAndRemove(Guard.ArgumentNotNull(testUniqueID), remove) as ITestMetadata;

	/// <summary>
	/// Attempts to retrieve <see cref="ITestMetadata"/> from the cache.
	/// </summary>
	/// <param name="message">The message that indicates which metadata to retrieve.</param>
	/// <returns>The cached metadata, if present; or <c>null</c> if there isn't any.</returns>
	public ITestMetadata? TryGetTestMetadata(TestMessage message) =>
		InternalGetAndRemove(Guard.ArgumentNotNull(message).TestUniqueID, false) as ITestMetadata;

	/// <summary>
	/// Attempts to retrieve <see cref="IAssemblyMetadata"/> from the cache, and if present,
	/// removes the metadata from the cache.
	/// </summary>
	/// <param name="message">The message that indicates which metadata to retrieve.</param>
	/// <returns>The cached metadata, if present; or <c>null</c> if there isn't any.</returns>
	public IAssemblyMetadata? TryRemove(TestAssemblyFinished message) =>
		InternalGetAndRemove(Guard.ArgumentNotNull(message).AssemblyUniqueID, true) as IAssemblyMetadata;

	/// <summary>
	/// Attempts to retrieve <see cref="ITestCaseMetadata"/> from the cache, and if present,
	/// removes the metadata from the cache.
	/// </summary>
	/// <param name="message">The message that indicates which metadata to retrieve.</param>
	/// <returns>The cached metadata, if present; or <c>null</c> if there isn't any.</returns>
	public ITestCaseMetadata? TryRemove(TestCaseFinished message) =>
		InternalGetAndRemove(Guard.ArgumentNotNull(message).TestCaseUniqueID, true) as ITestCaseMetadata;

	/// <summary>
	/// Attempts to retrieve <see cref="ITestClassMetadata"/> from the cache, and if present,
	/// removes the metadata from the cache.
	/// </summary>
	/// <param name="message">The message that indicates which metadata to retrieve.</param>
	/// <returns>The cached metadata, if present; or <c>null</c> if there isn't any.</returns>
	public ITestClassMetadata? TryRemove(TestClassFinished message) =>
		InternalGetAndRemove(Guard.ArgumentNotNull(message).TestClassUniqueID, true) as ITestClassMetadata;

	/// <summary>
	/// Attempts to retrieve <see cref="ITestCollectionMetadata"/> from the cache, and if present,
	/// removes the metadata from the cache.
	/// </summary>
	/// <param name="message">The message that indicates which metadata to retrieve.</param>
	/// <returns>The cached metadata, if present; or <c>null</c> if there isn't any.</returns>
	public ITestCollectionMetadata? TryRemove(TestCollectionFinished message) =>
		InternalGetAndRemove(Guard.ArgumentNotNull(message).TestCollectionUniqueID, true) as ITestCollectionMetadata;

	/// <summary>
	/// Attempts to retrieve <see cref="ITestMetadata"/> from the cache, and if present,
	/// removes the metadata from the cache.
	/// </summary>
	/// <param name="message">The message that indicates which metadata to retrieve.</param>
	/// <returns>The cached metadata, if present; or <c>null</c> if there isn't any.</returns>
	public ITestMetadata? TryRemove(TestFinished message) =>
		InternalGetAndRemove(Guard.ArgumentNotNull(message).TestUniqueID, true) as ITestMetadata;

	/// <summary>
	/// Attempts to retrieve <see cref="ITestMethodMetadata"/> from the cache, and if present,
	/// removes the metadata from the cache.
	/// </summary>
	/// <param name="message">The message that indicates which metadata to retrieve.</param>
	/// <returns>The cached metadata, if present; or <c>null</c> if there isn't any.</returns>
	public ITestMethodMetadata? TryRemove(TestMethodFinished message) =>
		InternalGetAndRemove(Guard.ArgumentNotNull(message).TestMethodUniqueID, true) as ITestMethodMetadata;

	// Helpers

	object? InternalGetAndRemove(
		string? uniqueID,
		bool remove)
	{
		if (uniqueID is null)
			return null;

		lock (cache)
		{
			if (cache.TryGetValue(uniqueID, out var metadata) && remove)
				cache.Remove(uniqueID);

			return metadata;
		}
	}

	void InternalSet(
		string uniqueID,
		object metadata)
	{
		lock (cache)
		{
#pragma warning disable CA1854 // This isn't getting a value, it's setting one
			if (cache.ContainsKey(uniqueID))
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Key '{0}' already exists in the message metadata cache.{1}Old item: {2}{3}New item: {4}", uniqueID, Environment.NewLine, cache[uniqueID], Environment.NewLine, metadata));
#pragma warning restore CA1854

			cache.Add(uniqueID, metadata);
		}
	}
}
