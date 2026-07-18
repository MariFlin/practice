using System;
using System.Threading;

namespace task17;

public class SoftStopCommand : ICommand
{
    private readonly ServerThread _targetThread;

    public SoftStopCommand(ServerThread targetThread)
    {
        _targetThread = targetThread ?? throw new ArgumentNullException(nameof(targetThread));
    }

    public void Execute()
    {
        if (!_targetThread.IsCurrentWorkerThread())
            throw new InvalidOperationException("SoftStopCommand выполнена не в целевом потоке");

        _targetThread.SoftStop();
    }
}