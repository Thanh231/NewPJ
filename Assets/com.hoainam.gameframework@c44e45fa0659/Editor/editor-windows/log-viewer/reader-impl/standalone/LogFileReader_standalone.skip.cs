
using System.Collections.Generic;
using System.Text.RegularExpressions;

public partial class LogFileReader_standalone
{
    private bool isInIntroduce = true;
    
    private const string lastIntroduceMsg = "<RI> Initialized touch support.";
    private List<string> listSkippedMsg = new List<string>()
    {
        "^UnloadTime: .* ms",
        @"^Unloading \d+ Unused Serialized files \(Serialized files now loaded: \d+\)",
        @"^Unloading \d+ unused Assets \/ \(.*\). Loaded Objects now: \d+.",
        "^Memory consumption went from .* MB to .*MB.",
        @"^Total: .* ms \(FindLiveObjects: .* ms CreateObjectMapping: .* ms MarkObjects: .* ms  DeleteObjects: .* ms\)",
        "^Uploading Crash Report",
        @"^The referenced script on this Behaviour \(Game Object .*\) is missing!",
        "^ShaderLab::GrabPasses::ApplyGrabPassMainThread can't be called from a job thread! This usually happens when GrabPass is used with SRP so make sure you're using Built-in Render Pipeline when using GrabPass."
    };

    private bool SkipLine(string line)
    {
        return SkipLineDueToIntroduce(line) || SkipLineDueToMatchSkippedLine(line);
    }

    private bool SkipLineDueToIntroduce(string line)
    {
        if (!isInIntroduce)
        {
            return false;
        }

        if (line.Equals(lastIntroduceMsg))
        {
            isInIntroduce = false;
        }

        return true;
    }

    private bool SkipLineDueToMatchSkippedLine(string line)
    {
        foreach (var skippedMsg in listSkippedMsg)
        {
            if (Regex.IsMatch(line, skippedMsg))
            {
                return true;
            }
        }
        return false;
    }
}
