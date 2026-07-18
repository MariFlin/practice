using System;

namespace task17;

public class HardStopCommand : ICommand
{
    private readonly ServerThread _targetThread;

    public HardStopCommand(ServerThread targetThread)
    {
        _targetThread = targetThread ?? throw new ArgumentNullException(nameof(targetThread));
    }

    public void Execute()
    {
        if (!_targetThread.IsCurrentWorkerThread())
            throw new InvalidOperationException("HardStopCommand выполнена не в целевом потоке");

        _targetThread.HardStop();
    }
}