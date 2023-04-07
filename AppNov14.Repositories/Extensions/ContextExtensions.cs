using System;
using System.Data.Linq;
using System.Threading;

namespace AppNov14.Repositories.Extensions
{
    public static class ContextExtensions
    {
        private const int MaxSubmitAttempts = 10;

        public static void TrySubmitChanges(this DataContext context)
        {
            try
            {
                ContextExtensions.TrySubmitChangesWithResolvingConflicts(context);
            }
            catch (InvalidOperationException)
            {
                ContextExtensions.TrySubmitChangesWithResolvingConflicts(context);
            }
        }

        public static void TrySubmitChangesWithResolvingConflicts(this DataContext context, Action action)
        {
            try
            {
                action();
            }
            catch (ChangeConflictException)
            {
                foreach (var conflict in context.ChangeConflicts)
                {
                    foreach (var member in conflict.MemberConflicts)
                    {
                        member.Resolve(RefreshMode.KeepCurrentValues);
                    }
                }
                action();
            }
        }

        private static void TrySubmitChangesWithResolvingConflicts(DataContext context)
        {
            var currentAttempt = 0;
            while (true)
            {
                try
                {
                    context.SubmitChanges(ConflictMode.ContinueOnConflict);
                    break;
                }
                catch (ChangeConflictException)
                {
                    if (currentAttempt == ContextExtensions.MaxSubmitAttempts)
                    {
                        throw;
                    }
                    foreach (var conflict in context.ChangeConflicts)
                    {
                        foreach (var member in conflict.MemberConflicts)
                        {
                            member.Resolve(RefreshMode.KeepCurrentValues);
                        }
                    }
                    if (currentAttempt > 0)
                    {
                        Thread.Sleep(1000);
                    }
                    currentAttempt++;
                }
            }
        }
    }
}