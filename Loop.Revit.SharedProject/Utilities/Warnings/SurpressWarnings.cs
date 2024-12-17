using Autodesk.Revit.DB;

<<<<<<< Updated upstream
namespace Loop.Revit.Utilities.Warnings;

public class SuppressAllWarnings : IFailuresPreprocessor
{
    public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
    {
        var failures = failuresAccessor.GetFailureMessages();

        foreach (var failure in failures)
        {
            var severity = failure.GetSeverity();

            if (severity == FailureSeverity.Warning)
            {
                failuresAccessor.DeleteWarning(failure);
            }
            else
            {
                failuresAccessor.ResolveFailure(failure);
                return FailureProcessingResult.ProceedWithCommit;
            }
        }

        return FailureProcessingResult.Continue;
=======
namespace Loop.Revit.Utilities.Warnings
{
    public class SuppressAllWarnings : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            var failures = failuresAccessor.GetFailureMessages();

            foreach (var failure in failures)
            {
                var severity = failure.GetSeverity();

                if (severity == FailureSeverity.Warning)
                {
                    failuresAccessor.DeleteWarning(failure);
                }
                else
                {
                    failuresAccessor.ResolveFailure(failure);
                    return FailureProcessingResult.ProceedWithCommit;
                }
            }

            return FailureProcessingResult.Continue;
        }
>>>>>>> Stashed changes
    }
}