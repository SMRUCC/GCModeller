Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels

Namespace Assembly.NCBI.GenBank.TabularFormat

    Public Module Extensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function COGs(genes As IEnumerable(Of GeneBrief)) As IEnumerable(Of String)
            Return From gene In genes Select gene.COG
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function AsGenes(features As IEnumerable(Of GFF.Feature)) As IEnumerable(Of GeneBrief)
            Return features _
                .SafeQuery _
                .Select(Function(feature) feature.ToGeneBrief)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function ToGeneBrief(feature As GFF.Feature) As GeneBrief
            Return New GeneBrief With {
                .Code = feature.ID,
                .COG = feature.COG,
                .Gene = feature.ID,
                .IsORF = True,
                .Length = feature.Length,
                .Location = feature.Location,
                .PID = feature.ProteinId,
                .Product = feature.Product,
                .Synonym = feature.Synonym
            }
        End Function
    End Module
End Namespace