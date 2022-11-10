Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data

<Package("sabiork")>
Public Module sabiork_repository

    <ExportAPI("new")>
    Public Function createNewRepository(file As String) As SabiorkRepository
        Return New SabiorkRepository(file.Open(FileMode.OpenOrCreate, doClear:=True))
    End Function

    <ExportAPI("query")>
    Public Function query(ec_number As String, cache As SabiorkRepository) As Object
        Return cache.GetByECNumber(ec_number)
    End Function
End Module
