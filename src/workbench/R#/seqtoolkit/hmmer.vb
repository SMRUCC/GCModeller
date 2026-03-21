Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.IO.HDF5.struct
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.HMMER
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

<Package("hmmer")>
Module hmmer

    <ExportAPI("parse_hmmer_model")>
    Public Function parse_hmmer_model(x As String) As ProfileHMM
        Return HMMER3Parser.ParseContent(x.SolveStream)
    End Function

    <ExportAPI("load_hmmer")>
    Public Function load_hmmer(<RRawVectorArgument> x As Object) As ProteinAnnotator
        Dim list = CLRVector.asCharacter(x)

        If list.IsNullOrEmpty Then
            Return Nothing
        End If

        Dim hmmer As New ProteinAnnotator

        If list.Length = 1 AndAlso list(0).DirectoryExists Then
            Call hmmer.LoadModelsFromDirectory(list(0))
        Else
            For Each file As String In list
                Call hmmer.LoadModel(file)
            Next
        End If

        Return hmmer
    End Function

    <ExportAPI("hmmer_search")>
    <RApiReturn(GetType(AnnotationResult))>
    Public Function hmmer_search(hmmer As ProteinAnnotator, <RRawVectorArgument> x As Object, Optional env As Environment = Nothing) As Object
        Dim seqs = GetFastaSeq(x, env)

        If seqs Is Nothing Then
            Return Nothing
        End If

        Return pipeline.CreateFromPopulator(seqs.Select(Function(fa) hmmer.Annotate(fa)).IteratesALL)
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
