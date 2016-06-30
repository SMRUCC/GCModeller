Imports Microsoft.VisualBasic.CommandLine.Reflection
'Imports Microsoft.VisualBasic.DataVisualization.DocumentFormat.Extensions

Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.Extensions

Imports Microsoft.VisualBasic.Scripting.ShoalShell
Imports LANS.SystemsBiology.AnalysisTools.ProteinTools.Sanger.Pfam.ProteinDomainArchitecture.MPAlignment
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.DeviceDriver.DriverHandles
Imports LANS.SystemsBiology.Assembly
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat
Imports Microsoft.VisualBasic.Scripting.MetaData

Partial Module IO

    <InputDeviceHandle("Gff")>
    <ExportAPI("Read.Gff")>
    Public Function ReadGff(Path As String) As LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.GFF
        Return LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.GFF.LoadDocument(Path)
    End Function

    <IO_DeviceHandle(GetType(LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.GFF))>
    <ExportAPI("Write.Gff")>
    Public Function WriteGff(data As LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.GFF, SaveTo As String) As Boolean
        Return data.Save(SaveTo, System.Text.Encoding.ASCII)
    End Function

End Module