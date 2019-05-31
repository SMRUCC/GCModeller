Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas.NtProps

Namespace Colors

    ''' <summary>
    ''' Maps the colors based on the nt property
    ''' </summary>
    Public Module NtPropsMapsExtensions

        <Extension>
        Public Function FromGC(source As IEnumerable(Of GeneObjectGC)) As Mappings()
            Dim GC As Double() = source.Select(Function(x) x.value).ToArray
            Return GradientMaps.GradientMappings(GC)
        End Function

        <Extension>
        Public Function FromAT(source As IEnumerable(Of GeneObjectGC)) As Mappings()
            Dim AT As Double() = source.Select(Function(x) x.value).ToArray
            Return GradientMaps.GradientMappings(AT)
        End Function

        <Extension>
        Public Function PropertyMaps(source As IEnumerable(Of FastaSeq)) As NtPropsMaps
            Dim genome As New FastaFile(source)
            Dim props As GeneObjectGC() = GCProps.GetGCContentForGenes(genome)
            Dim AT As Mappings() = props.FromAT
            Dim GC As Mappings() = props.FromGC

            Return New NtPropsMaps With {
                .source = genome,
                .props = (From x As GeneObjectGC
                          In props
                          Select x
                          Group x By x.Title Into Group) _
                               .ToDictionary(Function(x) x.Title,
                                             Function(x) x.Group.First),
                .AT = (From x As Mappings
                       In AT
                       Select x
                       Group x By x.value Into Group) _
                            .ToDictionary(Function(x) x.value,
                                          Function(x) x.Group.First.CircosColor),
                .GC = (From x As Mappings
                       In GC
                       Select x
                       Group x By x.value Into Group) _
                            .ToDictionary(Function(x) x.value,
                                          Function(x) x.Group.First.CircosColor)
            }
        End Function
    End Module
End Namespace