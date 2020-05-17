Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal
Imports SMRUCC.Rsharp.Runtime.Interop
Imports IContext = SMRUCC.genomics.ContextModel.Context

<Package("annotation.genomics_context", Category:=APICategories.ResearchTools)>
Module context

    Sub New()

    End Sub

    <ExportAPI("location")>
    Public Function location(left As Integer, right As Integer, Optional strand As Object = Nothing) As Object
        Dim strVal As Strands

        If strand Is Nothing Then
            strVal = Strands.Unknown
        ElseIf TypeOf strand Is Strands Then
            strVal = strand
        Else
            strVal = Scripting.ToString(strand).GetStrand
        End If

        Return New NucleotideLocation(left, right, strVal)
    End Function

    <ExportAPI("context")>
    <RApiReturn(GetType(IContext))>
    Public Function context(loci As Object, distance As Integer, Optional note As String = Nothing, Optional env As Environment = Nothing) As Object
        Dim pos As NucleotideLocation

        If loci Is Nothing Then
            Return Nothing
        ElseIf TypeOf loci Is IGeneBrief Then
            pos = DirectCast(loci, IGeneBrief).Location
            note = If(note, loci.ToString)
        ElseIf TypeOf loci Is Contig Then
            pos = DirectCast(loci, Contig).MappingLocation
            note = If(note, loci.ToString)
        ElseIf TypeOf loci Is NucleotideLocation Then
            pos = loci
        Else
            Return debug.stop(New InvalidCastException(loci.GetType.FullName), env)
        End If

        Return New IContext(pos, distance, note)
    End Function
End Module
