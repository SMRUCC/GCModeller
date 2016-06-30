Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.ShoalShell
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions

<[Namespace]("Doc.SBML")>
Module SBML

    <Runtime.DeviceDriver.DriverHandles.IO_DeviceHandle(GetType(Generic.IEnumerable(Of LANS.SystemsBiology.AnalysisTools.CARMEN.Reaction)))>
    <ExportAPI("Write.Csv.CarmenImports")>
    Public Function WriteCARMEN(data As Generic.IEnumerable(Of LANS.SystemsBiology.AnalysisTools.CARMEN.Reaction), path As String) As Boolean
        Return data.SaveTo(path, False)
    End Function

    <ExportAPI("Read.Xml.SBML2")>
    Public Function LoadSBML2(path As String) As LANS.SystemsBiology.Assembly.SBML.Level2.XmlFile
        Return LANS.SystemsBiology.Assembly.SBML.Level2.XmlFile.Load(path)
    End Function
End Module
