Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object

<Package("annotation.terms", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gcmodeller.org")>
Module terms

    <ExportAPI("assign.KO")>
    Public Function KOannotations(forward As pipeline, reverse As pipeline, Optional env As Environment = Nothing) As pipeline
        If forward Is Nothing Then
            Return Internal.debug.stop("forward data stream is nothing!", env)
        ElseIf reverse Is Nothing Then
            Return Internal.debug.stop("reverse data stream is nothing!", env)
        ElseIf Not forward.elementType Is GetType(BestHit) Then
            Return Internal.debug.stop($"forward is invalid data stream type: {forward.elementType.fullName}!", env)
        ElseIf Not reverse.elementType Is GetType(BestHit) Then
            Return Internal.debug.stop($"reverse is invalid data stream type: {reverse.elementType.fullName}!", env)
        End If

        Return KOAssignment.KOassignmentBBH(forward.populates(Of BestHit), reverse.populates(Of BestHit)).DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function

    <ExportAPI("assign.COG")>
    Public Function COGannotations()

    End Function

    <ExportAPI("assign.Pfam")>
    Public Function Pfamannotations()

    End Function

    <ExportAPI("assign.GO")>
    Public Function GOannotations()

    End Function

End Module
