
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SSystem.Kernel

<Package("S.system")>
Module SSystemKit

    <ExportAPI("kernel")>
    Public Function createKernel() As Kernel

    End Function
End Module
