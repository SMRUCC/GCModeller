Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace Assembly.DOOR

    Public Module ViewAPI

        Public Function GetFirstGene(Operon As KeyValuePair(Of String, OperonGene())) As OperonGene
            If Operon.Value.First.Location.Strand = Strands.Forward Then
                Return (From Gene In Operon.Value Select Gene Order By Gene.Location.Left Ascending).First
            Else
                Return (From Gene In Operon.Value Select Gene Order By Gene.Location.Left Descending).First
            End If
        End Function

        Public Function GenerateLstIdString(Operon As KeyValuePairObject(Of String, OperonGene())) As String
            If Operon.Value.Count = 1 Then
                Return Operon.Value.First.Synonym
            End If
            Return String.Join("; ", (From GeneObject In Operon.Value Select GeneObject.Synonym).ToArray)
        End Function

    End Module
End Namespace