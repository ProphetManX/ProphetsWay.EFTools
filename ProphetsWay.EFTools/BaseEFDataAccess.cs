using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProphetsWay.BaseDataAccess;
using System;
using System.Collections.Generic;

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
	public abstract class BaseEFDataAccess<TContext> : BaseDataAccess.BaseDataAccess, IBaseDataAccess where TContext : DbContext
	{
		private IDbContextTransaction _transaction;
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

		/// <summary>
		/// Whether <see cref="Dispose"/> has run on this instance. <c>false</c> until it does, and <c>true</c>
		/// from the <b>first statement</b> of <see cref="Dispose"/> onward — so a teardown step that calls back
		/// onto the instance is refused rather than served by a half-torn-down object.
		/// </summary>
		protected bool IsDisposed
		{
			get { return _disposed; }
		}

		/// <summary>
		/// Throws when this instance has been disposed; returns otherwise.
		/// </summary>
		/// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
		/// <remarks>
		/// This library applies the guard to the seven inherited dispatcher members itself. A derived Data Access
		/// Layer <b>must</b> call it as the first statement of every member it declares — each per-entity
		/// forwarder and every custom method — because nothing here dispatches or intercepts on that path. With
		/// <see cref="ContextOwnership.Owned"/> a missing guard is masked, because the disposed context throws of
		/// its own accord; with <see cref="ContextOwnership.Borrowed"/> the context is still alive and the member
		/// silently succeeds. This is what makes the behavior identical in both modes.
		/// </remarks>
		protected void ThrowIfDisposed()
		{
			if (_disposed)
				throw new ObjectDisposedException(GetType().FullName);
		}

		/// <summary>
		/// Begins a transaction on <see cref="Context"/> and retains it for this unit of work.
		/// </summary>
		/// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
		/// <exception cref="InvalidOperationException">
		/// A transaction begun by this instance is already open. Transactions do not nest.
		/// </exception>
		/// <remarks>
		/// This method never consults <c>Context.Database.CurrentTransaction</c> to decide whether to begin one,
		/// so it can never check-and-skip. On a <see cref="ContextOwnership.Borrowed"/> context whose owner has
		/// already begun a transaction directly, Entity Framework Core's own exception propagates unwrapped and
		/// that outer transaction is left in force — this layer does not silently enroll in it.
		/// </remarks>
		public override void TransactionStart()
		{
			ThrowIfDisposed();

			if (_transaction != null)
				throw new InvalidOperationException("A transaction is already open on this Data Access Layer instance; transactions do not nest.");

			_transaction = Context.Database.BeginTransaction();
		}

		/// <summary>
		/// Commits the transaction this instance began, and clears its transaction state.
		/// </summary>
		/// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
		/// <exception cref="InvalidOperationException">
		/// No transaction is open on this instance — including a second call after a commit or a rollback.
		/// </exception>
		/// <remarks>
		/// <b>A failed commit leaves nothing open and discards its writes.</b> The transaction state is cleared
		/// before the commit is attempted and the Entity Framework transaction is disposed either way, which
		/// rolls the batch back at the store; the provider's exception is then rethrown unwrapped. A caller must
		/// not follow a failed commit with <see cref="TransactionRollBack"/> — there is nothing left to roll
		/// back and that call throws.
		/// </remarks>
		public override void TransactionCommit()
		{
			ThrowIfDisposed();

			var transaction = TakeOpenTransaction();

			try
			{
				transaction.Commit();
			}
			catch
			{
				DisposeQuietly(transaction);
				throw;
			}

			transaction.Dispose();
		}

		/// <summary>
		/// Rolls back the transaction this instance began, and clears its transaction state.
		/// </summary>
		/// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
		/// <exception cref="InvalidOperationException">
		/// No transaction is open on this instance — including after a commit, after a rollback, and after a
		/// <i>failed</i> commit.
		/// </exception>
		public override void TransactionRollBack()
		{
			ThrowIfDisposed();

			var transaction = TakeOpenTransaction();

			try
			{
				transaction.Rollback();
			}
			catch
			{
				DisposeQuietly(transaction);
				throw;
			}

			transaction.Dispose();
		}

		// Clears the instance's transaction state before the caller acts on it, so a failed Commit or Rollback
		// still leaves nothing open.
		private IDbContextTransaction TakeOpenTransaction()
		{
			if (_transaction == null)
				throw new InvalidOperationException("No transaction is open on this Data Access Layer instance.");

			var transaction = _transaction;
			_transaction = null;

			return transaction;
		}

		private static void DisposeQuietly(IDbContextTransaction transaction)
		{
			try
			{
				transaction.Dispose();
			}
			catch
			{
				// The failure that brought us here is the one the caller needs to see.
			}
		}

		/// <inheritdoc />
		public override IList<T> GetAll<T>()
		{
			ThrowIfDisposed();
			return base.GetAll<T>();
		}

		/// <inheritdoc />
		public override IList<T> GetPaged<T>(int skip, int take)
		{
			ThrowIfDisposed();
			return base.GetPaged<T>(skip, take);
		}

		/// <inheritdoc />
		public override int GetCount<T>()
		{
			ThrowIfDisposed();
			return base.GetCount<T>();
		}

		/// <inheritdoc />
		public override T Get<T>(object id)
		{
			ThrowIfDisposed();
			return base.Get<T>(id);
		}

		/// <inheritdoc />
		public override void Insert<T>(T item)
		{
			ThrowIfDisposed();
			base.Insert<T>(item);
		}

		/// <inheritdoc />
		public override int Update<T>(T item)
		{
			ThrowIfDisposed();
			return base.Update<T>(item);
		}

		/// <inheritdoc />
		public override int Delete<T>(T item)
		{
			ThrowIfDisposed();
			return base.Delete<T>(item);
		}

		/// <summary>
		/// Rolls back any transaction still open, runs <see cref="DisposeCore"/>, then disposes
		/// <see cref="Context"/> only when this instance owns it.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Idempotent, and never throws, as <see cref="IBaseDataAccess"/> requires. <see cref="IsDisposed"/> is
		/// set before any teardown, and every step is best-effort: a failure in one is swallowed and the next
		/// still runs, so a throwing <see cref="DisposeCore"/> cannot leave an owned context undisposed. A
		/// transaction still open is <b>rolled back</b>, never committed. With
		/// <see cref="ContextOwnership.Borrowed"/> the context is left untouched and stays usable by whoever
		/// supplied it — only the transaction this layer started is cleaned up.
		/// </para>
		/// <para>
		/// <b>Sealed</b>: idempotency and never-throwing are contract terms a derived Data Access Layer must not
		/// be able to break by accident. A derived layer with something of its own to release overrides
		/// <see cref="DisposeCore"/> and inherits both guarantees.
		/// </para>
		/// </remarks>
		public sealed override void Dispose()
		{
			if (_disposed)
				return;

			_disposed = true;

			RollBackRetainedTransaction();

			try
			{
				DisposeCore();
			}
			catch
			{
				// Derived cleanup never escapes disposal, and never causes the step below to be skipped.
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

		/// <summary>
		/// Releases whatever the derived Data Access Layer created for itself.
		/// </summary>
		/// <remarks>
		/// Called once, from <see cref="Dispose"/>, after <see cref="IsDisposed"/> is already <c>true</c> and
		/// after any open transaction has been rolled back, but before an owned <see cref="Context"/> is
		/// disposed. <b>Must not throw</b>; anything it does throw is swallowed and disposal continues. Calling
		/// back onto this instance from here yields <see cref="ObjectDisposedException"/> rather than a
		/// half-torn-down object.
		/// </remarks>
		protected virtual void DisposeCore()
		{
		}

		// Talks to the retained transaction directly rather than through TransactionRollBack, which at this
		// point would throw ObjectDisposedException, and InvalidOperationException in the ordinary case where
		// nothing is open.
		private void RollBackRetainedTransaction()
		{
			var transaction = _transaction;
			_transaction = null;

			if (transaction == null)
				return;

			try
			{
				transaction.Rollback();
			}
			catch
			{
				// An abandoned transaction is rolled back by the database once the connection drops.
			}

			// Disposed even when the rollback failed — a borrowed context is left usable.
			DisposeQuietly(transaction);
		}
	}
}
