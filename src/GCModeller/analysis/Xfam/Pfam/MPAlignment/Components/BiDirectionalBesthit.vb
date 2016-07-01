Imports System.Text
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace ProteinDomainArchitecture.MPAlignment

    Public Class BiDirectionalBesthit : Inherits BBH.BiDirectionalBesthit
        Implements IMPAlignmentResult

        Public Property Similarity As Double Implements IMPAlignmentResult.Similarity
        Public Property Score As Double Implements IMPAlignmentResult.MPScore

        Public Shared Function Upgrade(source As BBH.BiDirectionalBesthit, DomainAlignment As AlignmentOutput) As BiDirectionalBesthit
            If DomainAlignment Is Nothing Then
                Return source.ShadowCopy(Of BiDirectionalBesthit)()
            End If

            Dim Besthit = source.ShadowCopy(Of BiDirectionalBesthit)()
            Besthit.Similarity = DomainAlignment.Similarity
            Besthit.Score = DomainAlignment.Score

            Return Besthit
        End Function

        Public Interface IMPAlignmentResult
            Property MPScore As Double
            Property Similarity As Double
        End Interface
    End Class
End Namespace