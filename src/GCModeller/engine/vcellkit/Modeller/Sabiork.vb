Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.SABIORK
Imports SMRUCC.genomics.Data.SABIORK.SBML

<Package("sabiork")>
Public Module sabiork_repository

    <ExportAPI("new")>
    Public Function createNewRepository(file As String) As SabiorkRepository
        Return New SabiorkRepository(file.Open(FileMode.OpenOrCreate, doClear:=True))
    End Function

    <ExportAPI("open")>
    Public Function openRepository(file As String) As SabiorkRepository
        Return New SabiorkRepository(file.Open(FileMode.OpenOrCreate, doClear:=False))
    End Function

    <ExportAPI("query")>
    Public Function query(ec_number As String, cache As SabiorkRepository) As Object
        Return cache.GetByECNumber(ec_number)
    End Function

    <ExportAPI("parseSbml")>
    Public Function parseSbml(data As String) As SbmlDocument
        Dim xml As String = data.LineIterators.JoinBy(vbLf)
        Dim model As SbmlDocument = ModelQuery.parseSBML(xml)
        Return model
    End Function
End Module
