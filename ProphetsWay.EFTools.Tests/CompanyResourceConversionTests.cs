using System;
using System.Linq;
using System.Reflection;

using ProphetsWay.Example.DataAccess;
using ProphetsWay.Example.DataAccess.EF;
using ProphetsWay.Example.DataAccess.Entities;
using ProphetsWay.Example.DataAccess.IDaos;

using Shouldly;

using Xunit;

namespace ProphetsWay.EFTools.Tests
{
	/// <summary>
	/// The structural preconditions of <c>ICompanyResourceDao</c> <b>rule 8</b> — <i>"<c>Get&lt;CompanyResource&gt;(id)</c>
	/// on the generic dispatcher always throws <c>DataAccessConventionException</c> and can never be made to
	/// work"</i> — as they apply to the Entity Framework conversion planned by D17's amendment.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <b>Rule 8's outcome is already asserted upstream and is not re-authored here.</b>
	/// <c>CompanyResourceDataAccessTests.ShouldThrowWhenGenericGetIsUsedOnAnEntityWithNoIdentifier</c> calls the
	/// dispatcher and requires the exception, and it reaches this repository through the seam. D10 declines a
	/// second local copy of an assertion the upstream suite already makes.
	/// </para>
	/// <para>
	/// <b>What upstream cannot assert is why the rule holds.</b> It holds for two independent structural
	/// reasons — the Data Access Object publishes no <c>Get</c>, and the Data Access Layer declares no
	/// <c>Get(CompanyResource)</c> forwarder — and rule 8 itself says <i>which</i> of the two is reported is
	/// unspecified. A conversion that derived from <see cref="BaseNonIdDao{TEntity}"/> instead of
	/// <see cref="RootNonIdDao{TEntity}"/> would publish a <c>Get</c> and remove one of the two, silently, with
	/// every upstream test still green. That is the risk the specification names in terms — <i>"deriving from
	/// <c>BaseNonIdDao</c> instead would publish a <c>Get</c> and put rule 8 at risk"</i> — and it is what this
	/// file guards.
	/// </para>
	/// <para>
	/// <b>Reflection only; no store, no connection, no model.</b> Every assertion here reads types, which is why
	/// it carries <c>Area=Keyless</c> alongside the rest of the lap and needs no local SQL Server.
	/// </para>
	/// <para>
	/// <b>Two things are deliberately not asserted.</b> That the Data Access Object overrides
	/// <c>ApplyStableOrder</c>, and that it overrides <c>Insert</c> to satisfy rule 3. Both are mechanisms rather
	/// than rules: A15's refusal is observable through <c>GetAll</c> and rule 3's no-op through a second
	/// <c>Insert</c>, both of which the upstream suite already exercises, and the specification explicitly offers
	/// a second way to satisfy rule 3 — catching the uniqueness violation — which a structural assertion would
	/// forbid.
	/// </para>
	/// </remarks>
	public class CompanyResourceConversionTests
	{
		#region apparatus

		/// <summary>
		/// The Entity Framework Data Access Object for <see cref="CompanyResource"/>, or <c>null</c> when the
		/// conversion has not landed.
		/// </summary>
		/// <remarks>
		/// <see cref="IExampleDataAccess"/> aggregates <see cref="ICompanyResourceDao"/>, so the Data Access Layer
		/// root implements it too and has to be excluded — the subject is the Data Access Object, not the
		/// aggregate that forwards to it.
		/// </remarks>
		private static Type CompanyResourceDaoType()
		{
			return typeof(ExampleDataAccess).Assembly
				.GetTypes()
				.Where(t => t.IsClass && !t.IsAbstract)
				.Where(t => typeof(ICompanyResourceDao).IsAssignableFrom(t))
				.SingleOrDefault(t => !typeof(IExampleDataAccess).IsAssignableFrom(t));
		}

		/// <summary>
		/// The methods on <paramref name="dal"/> that the dispatcher's own lookup would accept as the
		/// <paramref name="methodName"/> forwarder for <paramref name="entityType"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Mirrors <c>BaseDataAccessHelper.FindExactMatch</c> clause for clause, because the question being asked
		/// is not "does a member of this shape exist" but "would the dispatcher find one": public and instance,
		/// generic method definitions skipped, ordinal name equality, one parameter, and the parameter type
		/// compared for <b>exact</b> equality. That last clause is the dispatcher's, in terms — it matches
		/// positionally rather than through <c>Type.DefaultBinder</c>, "which would also accept a parameter typed
		/// as a base class or interface of the entity."
		/// </para>
		/// <para>
		/// <b><c>DeclaredOnly</c> is deliberately not applied.</b> The dispatcher applies it per level while
		/// walking <c>BaseType</c> to the root, which is a search of the whole hierarchy, not of one type. A
		/// forwarder declared on a base Data Access Layer would therefore be dispatched to exactly as one declared
		/// on the leaf, and a guard that could not see it would agree with the dispatcher only for as long as the
		/// hierarchy stayed one level deep.
		/// </para>
		/// </remarks>
		private static MethodInfo[] ForwardersFor(Type dal, string methodName, Type entityType)
		{
			return dal
				.GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.Where(m => !m.IsGenericMethodDefinition)
				.Where(m => string.Equals(m.Name, methodName, StringComparison.Ordinal))
				.Where(m => m.GetParameters().Length == 1)
				.Where(m => m.GetParameters()[0].ParameterType == entityType)
				.ToArray();
		}

		#endregion

		/// <summary>
		/// The conversion lands on <see cref="RootNonIdDao{TEntity}"/> — the family that implements no capability
		/// interface — and not on <see cref="BaseNonIdDao{TEntity}"/>.
		/// </summary>
		/// <remarks>
		/// <c>Contract</c>: D17's amendment, and the shape check in <i>`CompanyResourceDao` on this family</i>,
		/// whose rule-8 row reads <i>"preserved … and it must stay that way."</i> The negative half is the whole
		/// point: both bases satisfy rules 1, 2 and 4–10, so a conversion onto the wrong one compiles, passes
		/// every upstream test, and quietly makes rule 8 depend on the Data Access Layer alone.
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldConvertCompanyResourceDaoOntoTheKeylessRootAndNotOntoTheIBaseDaoPublishingBase()
		{
			//setup
			var dao = CompanyResourceDaoType();

			//act & assert
			dao.ShouldNotBeNull(
				"No type in ProphetsWay.Example.DataAccess.EF implements ICompanyResourceDao. The conversion " +
				"D17's amendment plans - CompanyResourceDao on RootNonIdDao<CompanyResource> - has not landed, " +
				"and the three ICompanyResourceDao forwarders on ExampleDataAccess still throw.");

			typeof(RootNonIdDao<CompanyResource>).IsAssignableFrom(dao).ShouldBeTrue();

			typeof(BaseNonIdDao<CompanyResource>).IsAssignableFrom(dao).ShouldBeFalse(
				"CompanyResourceDao derives from BaseNonIdDao<CompanyResource>, which publishes Get and Update. " +
				"ICompanyResourceDao inherits IBaseDao<T> not at all and documents both members as meaningless " +
				"for an entity whose identity is a pair of foreign keys - that is the coercion S5 exists to " +
				"prevent, and rule 8 rests on the Data Access Object publishing no Get.");
		}

		/// <summary>
		/// The Data Access Object publishes neither <c>Get</c> nor <c>Update</c> — the first of rule 8's two
		/// independent reasons.
		/// </summary>
		/// <remarks>
		/// <c>Contract</c>: <c>ICompanyResourceDao</c> rule 8, and its <i>No <c>Update</c></i> / <i>No
		/// <c>Get</c></i> rationale. Asserted on the concrete Data Access Object rather than on the base, because
		/// an override could publish either member on a type whose base does not.
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldPublishNeitherGetNorUpdateOnTheCompanyResourceDao()
		{
			//setup
			var dao = CompanyResourceDaoType();

			//act & assert
			dao.ShouldNotBeNull("The conversion has not landed - see the message on the shape test.");

			dao.GetMethod("Get", new[] { typeof(CompanyResource) }).ShouldBeNull();
			dao.GetMethod("Update", new[] { typeof(CompanyResource) }).ShouldBeNull();
		}

		/// <summary>
		/// The Data Access Layer forwards the three capabilities the dispatcher must reach and declares
		/// <b>no</b> <c>Get(CompanyResource)</c> — the second of rule 8's two independent reasons.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <c>Contract</c>: <c>ICompanyResourceDao</c> rule 8, and <i>How the dispatcher reaches a Data Access
		/// Object</i> — a capability sitting unforwarded on the Data Access Object base is not
		/// dispatcher-reachable, and a <c>Get</c> forwarder that <i>was</i> declared would be the one way to make
		/// rule 8 stop holding from this side.
		/// </para>
		/// <para>
		/// The three positive assertions are here rather than in a file of their own because they are the same
		/// fact from the other direction: the forwarders that must exist, and the one that must not.
		/// </para>
		/// <para>
		/// <b>The negative half must not be written as <c>GetMethod("Get", new[] { typeof(CompanyResource) })</c>,
		/// and was, and could not pass.</b> That overload binds through <c>Type.DefaultBinder</c>, which accepts a
		/// widening reference conversion. <see cref="BaseEFDataAccess{TContext}"/> declares
		/// <c>public override T Get&lt;T&gt;(object id)</c> — one of the seven inherited dispatcher members it is
		/// required to override so that a disposed layer answers <see cref="ObjectDisposedException"/> — and its
		/// parameter is <c>object</c>. With the six concrete <c>Get(X)</c> forwarders all incompatible, that
		/// generic entry point is the <i>sole</i> compatible candidate, so the call returns
		/// <c>T Get[T](System.Object)</c> and never <c>null</c>. No state of <c>ExampleDataAccess</c> makes such
		/// an assertion pass short of deleting a member the library mandates: the form cannot distinguish the
		/// thing being guarded — a hand-written <c>Get(CompanyResource)</c> — from the dispatcher entry point that
		/// must exist. <b>Do not simplify it back.</b>
		/// </para>
		/// <para>
		/// <see cref="ForwardersFor"/> is used instead, and asks the question the rule actually turns on: would
		/// the dispatcher find a <c>Get</c> forwarder for this entity. It is also self-controlling. No member of
		/// <c>ExampleDataAccess</c> can satisfy every clause of that selector at once — rule 8 is exactly the
		/// statement that none may — so the clauses are held live in pairs: <c>Get</c> against
		/// <see cref="Company"/> shows the parameter-type clause is not what comes back empty, and <c>Insert</c>
		/// against <see cref="CompanyResource"/> shows the name clause is not either. Between them every clause
		/// is exercised, and the empty result that follows is an absence rather than a selector that never
		/// selects anything.
		/// </para>
		/// <para>
		/// <b>The Data Access Object half of this file is asymmetric to this one, and correctly so.</b>
		/// <see cref="ShouldPublishNeitherGetNorUpdateOnTheCompanyResourceDao"/> keeps the plain
		/// <c>GetMethod(name, types)</c> form and passes, because <c>CompanyResourceDao</c> inherits no generic
		/// <c>Get&lt;T&gt;</c> for the binder to widen into. The trap is the Data Access Layer's alone.
		/// </para>
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldForwardInsertDeleteAndGetAllButNeverGetOnTheDataAccessLayer()
		{
			//setup
			var dal = typeof(ExampleDataAccess);

			//act & assert
			dal.GetMethod("Insert", new[] { typeof(CompanyResource) }).ShouldNotBeNull();
			dal.GetMethod("Delete", new[] { typeof(CompanyResource) }).ShouldNotBeNull();
			dal.GetMethod("GetAll", new[] { typeof(CompanyResource) }).ShouldNotBeNull();

			//the two controls for the assertion below, and not facts about Company or about Insert. No member
			//satisfies every clause of the selector at once - that is precisely what rule 8 forbids - so the
			//clauses are covered in pairs: holding the name at Get proves the parameter-type clause is not what
			//is empty, and holding the parameter at CompanyResource proves the name clause is not either
			ForwardersFor(dal, "Get", typeof(Company)).ShouldNotBeEmpty(
				"ExampleDataAccess declares no Get(Company) forwarder, so the selector is not distinguishing " +
				"anything and the assertion below is proving nothing. Fix this before reading it as evidence.");

			ForwardersFor(dal, "Insert", typeof(CompanyResource)).ShouldNotBeEmpty(
				"The selector cannot match a CompanyResource parameter at all, so the assertion below is empty " +
				"for the wrong reason. Fix this before reading it as evidence.");

			ForwardersFor(dal, "Get", typeof(CompanyResource)).ShouldBeEmpty(
				"ExampleDataAccess declares a Get(CompanyResource) forwarder. ICompanyResourceDao rule 8 " +
				"requires Get<CompanyResource>(id) to throw DataAccessConventionException and says it can never " +
				"be made to work; a forwarder is the one thing on this side that could change that.");
		}
	}
}
