Imports System.IO
Imports HMMER3
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.IO.HDF5.struct
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("hmmer")>
Module hmmer

    <ExportAPI("parse_hmmer_model")>
    Public Function parse_hmmer_model(x As String) As ProfileHMM
        Return HMMER3Parser.ParseContent(x.SolveStream)
    End Function

    ''' <summary>
    ''' Parse the kofamscan table output
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("parse_kofamscan")>
    <RApiReturn(GetType(KOFamScan))>
    Public Function parse_kofamscan(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim s = SMRUCC.Rsharp.GetFileStream(file, IO.FileAccess.Read, env)

        If s Like GetType(Message) Then
            Return s.TryCast(Of Message)
        End If

        Return pipeline.CreateFromPopulator(KOFamScan.ParseTable(s.TryCast(Of Stream)))
    End Function

End Module
