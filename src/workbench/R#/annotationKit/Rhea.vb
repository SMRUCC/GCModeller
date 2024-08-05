Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("rhea")>
Module Rhea

    ''' <summary>
    ''' open the rdf data pack of Rhea database
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' https://ftp.expasy.org/databases/rhea/rdf/rhea.rdf.gz
    ''' </remarks>
    <ExportAPI("open.rdf")>
    Public Function openRDF(file As String) As Object

    End Function
End Module
