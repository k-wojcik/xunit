using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Versioning;
using Xunit.Internal;

namespace Xunit.Sdk;

/// <summary>
/// This message indicates that the execution process is about to start for
/// the requested assembly.
/// </summary>
[JsonTypeID("test-assembly-starting")]
public sealed class TestAssemblyStarting : TestAssemblyMessage, IAssemblyMetadata, IWritableAssemblyMetadata
{
	string? assemblyName;
	string? assemblyPath;
	string? testEnvironment;
	string? testFrameworkDisplayName;
	IReadOnlyDictionary<string, IReadOnlyList<string>>? traits;

	/// <inheritdoc/>
	public string AssemblyName
	{
		get => this.ValidateNullablePropertyValue(assemblyName, nameof(AssemblyName));
		set => assemblyName = Guard.ArgumentNotNullOrEmpty(value, nameof(AssemblyName));
	}

	/// <inheritdoc/>
	public string AssemblyPath
	{
		get => this.ValidateNullablePropertyValue(assemblyPath, nameof(AssemblyPath));
		set => assemblyPath = Guard.ArgumentNotNullOrEmpty(value, nameof(AssemblyPath));
	}

	/// <inheritdoc/>
	public string? ConfigFilePath { get; set; }

	/// <summary>
	/// Gets or sets the seed value used for randomization. If <c>null</c>, then the test framework
	/// does not support getting or setting a randomization seed. (For stock versions of xUnit.net,
	/// support for settable randomization seeds started with v3.)
	/// </summary>
	public int? Seed { get; set; }

	/// <summary>
	/// Gets or sets the date and time when the test assembly execution began.
	/// </summary>
	public DateTimeOffset StartTime { get; set; }

	/// <summary>
	/// Gets or sets the target framework that the assembly was compiled against.
	/// Examples: ".NETFramework,Version=v4.7.2", ".NETCoreApp,Version=v6.0". This information
	/// is read from <see cref="TargetFrameworkAttribute"/> on the test assembly, which
	/// is normally auto-generated (but could be missing or empty).
	/// </summary>
	public string? TargetFramework { get; set; }

	/// <summary>
	/// Gets or sets a display string that describes the test execution environment.
	/// Examples: "32-bit .NET Framework 4.8.4220.0", "64-bit .NET Core 4.6.29220.03"
	/// </summary>
	public string TestEnvironment
	{
		get => this.ValidateNullablePropertyValue(testEnvironment, nameof(TestEnvironment));
		set => testEnvironment = Guard.ArgumentNotNullOrEmpty(value, nameof(TestEnvironment));
	}

	/// <summary>
	/// Gets or sets a display string which describes the test framework and version number.
	/// Examples: "xUnit.net v3 0.1.0-pre.15", "xUnit.net 2.4.1"
	/// </summary>
	public string TestFrameworkDisplayName
	{
		get => this.ValidateNullablePropertyValue(testFrameworkDisplayName, nameof(TestFrameworkDisplayName));
		set => testFrameworkDisplayName = Guard.ArgumentNotNullOrEmpty(value, nameof(TestFrameworkDisplayName));
	}

	/// <inheritdoc/>
	public IReadOnlyDictionary<string, IReadOnlyList<string>> Traits
	{
		get => this.ValidateNullablePropertyValue(traits, nameof(Traits));
		set => traits = Guard.ArgumentNotNull(value, nameof(Traits));
	}

	string IAssemblyMetadata.UniqueID =>
		AssemblyUniqueID;

	/// <inheritdoc/>
	protected override void Deserialize(IReadOnlyDictionary<string, object?> root)
	{
		base.Deserialize(root);

		root.DeserializeAssemblyMetadata(this);

		Seed = JsonDeserializer.TryGetInt(root, nameof(Seed));
		if (JsonDeserializer.TryGetDateTimeOffset(root, nameof(StartTime)) is DateTimeOffset startTime)
			StartTime = startTime;
		TargetFramework = JsonDeserializer.TryGetString(root, nameof(TargetFramework));
		testEnvironment = JsonDeserializer.TryGetString(root, nameof(TestEnvironment));
		testFrameworkDisplayName = JsonDeserializer.TryGetString(root, nameof(TestFrameworkDisplayName));
	}

	/// <inheritdoc/>
	protected override void Serialize(JsonObjectSerializer serializer)
	{
		Guard.ArgumentNotNull(serializer);

		base.Serialize(serializer);

		serializer.SerializeAssemblyMetadata(this);

		serializer.Serialize(nameof(Seed), Seed);
		serializer.Serialize(nameof(StartTime), StartTime);
		serializer.Serialize(nameof(TargetFramework), TargetFramework);
		serializer.Serialize(nameof(TestEnvironment), TestEnvironment);
		serializer.Serialize(nameof(TestFrameworkDisplayName), TestFrameworkDisplayName);
	}

	/// <inheritdoc/>
	public override string ToString() =>
		string.Format(
			CultureInfo.CurrentCulture,
			"{0} name={1} path={2} config={3}{4}",
			base.ToString(),
			assemblyName.Quoted(),
			AssemblyPath.Quoted(),
			ConfigFilePath.Quoted(),
			Seed is null ? "" : string.Format(CultureInfo.CurrentCulture, " seed={0}", Seed)
		);

	/// <inheritdoc/>
	protected override void ValidateObjectState(HashSet<string> invalidProperties)
	{
		base.ValidateObjectState(invalidProperties);

		ValidatePropertyIsNotNull(assemblyName, nameof(AssemblyName), invalidProperties);
		ValidatePropertyIsNotNull(assemblyPath, nameof(AssemblyPath), invalidProperties);
		ValidatePropertyIsNotNull(testEnvironment, nameof(TestEnvironment), invalidProperties);
		ValidatePropertyIsNotNull(testFrameworkDisplayName, nameof(TestFrameworkDisplayName), invalidProperties);
		ValidatePropertyIsNotNull(traits, nameof(Traits), invalidProperties);
	}
}
