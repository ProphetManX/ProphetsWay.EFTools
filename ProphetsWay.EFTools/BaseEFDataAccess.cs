using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProphetsWay.BaseDataAccess;
using System;

namespace ProphetsWay.EFTools
{
	/// <summary>
	/// The Entity Framework Core root of an <see cref="IBaseDataAccess"/> Data Access Layer: it holds the
	/// context every Data Access Object on the layer shares, owns the transaction, and owns disposal.
	/// </summary>
	/// <typeparam name="TContext">
	/// The consumer's context type. Constrained to <see cref="DbContext"/> rather than to
	/// <see cref="BaseEFContext"/>, so a consumer with an existing context — one registered in a
	/// dependency-injection container, for instance — is not required to re-parent it.
	/// </typeparam>
	/// <remarks>
	/// <para>
	/// <b>This class selects no database provider and constructs no context.</b> The derived Data Access
	/// Layer builds a configured <typeparamref name="TContext"/> and passes it in, which is what makes the
	/// library relational-provider-neutral.
	/// </para>
	/// <para>
	/// <b>Not thread-safe</b>, and <b>not shareable</b>: one instance, one unit of work, one thread, and one
	/// <typeparamref name="TContext"/> that backs no other live instance of this class. A transaction in
	/// Entity Framework Core lives on the context, so two layers over one context would silently share one.
	/// </para>
	/// </remarks>
	public class BaseEFDataAccess<TContext> : BaseDataAccess.BaseDataAccess, IBaseDataAccess where TContext : DbContext
	{
		private bool _disposed;

		/// <summary>
		/// The one context every Data Access Object on this layer receives. Typed
		/// <typeparamref name="TContext"/> so a derived layer constructs its Data Access Objects without a cast.
		/// </summary>
		protected TContext Context { get; }

		/// <summary>
		/// Whether this layer disposes <see cref="Context"/> along with itself.
		/// </summary>
		protected ContextOwnership Ownership { get; }

		/// <summary>
		/// Captures a context the caller has already configured, together with an explicit statement of who
		/// disposes it.
		/// </summary>
		/// <param name="context">
		/// A configured context. This class neither builds nor configures one, and does not detect a context
		/// that has already been disposed — the first member reaching the store fails with Entity Framework
		/// Core's own exception, naming the context type rather than this layer.
		/// </param>
		/// <param name="ownership">
		/// <see cref="ContextOwnership.Owned"/> when the derived layer created the context and disposal should
		/// dispose it, <see cref="ContextOwnership.Borrowed"/> when it was injected and someone else disposes it.
		/// There is no default; see <see cref="ContextOwnership"/>.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="ownership"/> is neither member of the enumeration. There is no
		/// "treat anything unknown as borrowed" fallback; a silently misread ownership is the bug this
		/// parameter exists to prevent.
		/// </exception>
		/// <remarks>
		/// Has no side effect beyond capturing the two values — no query, no connection, no transaction, and no
		/// Entity Framework model initialization.
		/// </remarks>
		protected BaseEFDataAccess(TContext context, ContextOwnership ownership)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			if (ownership != ContextOwnership.Borrowed && ownership != ContextOwnership.Owned)
				throw new ArgumentOutOfRangeException(nameof(ownership), ownership, "Context ownership must be stated as either Borrowed or Owned.");

			Context = context;
			Ownership = ownership;
		}

		public override void TransactionCommit()
		{
			Context.Database.CurrentTransaction.Commit();
		}

		public override void TransactionRollBack()
		{
			Context.Database.CurrentTransaction.Rollback();
		}

		public override void TransactionStart()
		{
			Context.Database.BeginTransaction();
		}

		/// <summary>
		/// Rolls back any transaction still open, then disposes <see cref="Context"/> only when this instance
		/// owns it.
		/// </summary>
		/// <remarks>
		/// Idempotent, and never throws: a failed rollback and a failed disposal are each swallowed rather than
		/// propagated, as <see cref="IBaseDataAccess"/> requires. A transaction still open is <b>rolled back</b>,
		/// never committed. With <see cref="ContextOwnership.Borrowed"/> the context is left untouched and stays
		/// usable by whoever supplied it — only the transaction this layer started is cleaned up.
		/// </remarks>
		public override void Dispose()
		{
			if (_disposed)
				return;

			_disposed = true;

			IDbContextTransaction transaction = null;

			try
			{
				transaction = Context.Database.CurrentTransaction;
			}
			catch
			{
				// A context that cannot report a transaction has none this instance can clean up.
			}

			if (transaction != null)
			{
				try
				{
					transaction.Rollback();
				}
				catch
				{
					// An abandoned transaction is rolled back by the database once the connection drops.
				}

				try
				{
					// Disposed even when the rollback failed — a borrowed context is left usable.
					transaction.Dispose();
				}
				catch
				{
					// Disposal never throws.
				}
			}

			if (Ownership != ContextOwnership.Owned)
				return;

			try
			{
				Context.Dispose();
			}
			catch
			{
				// Disposal never throws.
			}
		}
	}
}
