Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels

Namespace Assembly.NCBI.GenBank.TabularFormat

    Public Module Extensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function COGs(genes As IEnumerable(Of GeneBrief)) As IEnumerable(Of String)
            Return From gene In genes Select gene.COG
        End Function

        <Extension>
        Public Function AsGenes(features As IEnumerable(Of GFF.Feature)) As IEnumerable(Of GeneBrief)

        End Function
    End Module
End Namespace