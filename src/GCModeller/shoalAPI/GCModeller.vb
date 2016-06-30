Imports LANS.SystemsBiology.AnalysisTools.ProteinTools.Sanger.Pfam.ProteinDomainArchitecture.MPAlignment
Imports LANS.SystemsBiology.Assembly
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.Extensions
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.FileStream.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.ShoalShell
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.DeviceDriver.DriverHandles

<[PackageNamespace]("GCModeller.Assembly.File.IO",
                    Publisher:="xie.guigang@gmail.com")>
Module GCModellerTools

    <InputDeviceHandle("GCML.Csvx")>
    Public Function LoadGCMLCsvx(Path As String) As XmlresxLoader
        Return New XmlresxLoader(CellSystemPath:=Path)
    End Function
End Module
