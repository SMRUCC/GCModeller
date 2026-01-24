Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Framework
Imports SMRUCC.genomics.ComponentModel

Namespace Kmers.Kraken2

    ''' <summary>
    ''' the bracken abundance table
    ''' </summary>
    Public Class Bracken : Implements IExpressionValue

        Public Property name As String
        Public Property taxonomy_id As Integer
        Public Property taxonomy_lvl As String
        Public Property kraken_assigned_reads As Double
        Public Property added_reads As Double
        Public Property new_est_reads As Double
        Public Property fraction_total_reads As Double Implements IExpressionValue.ExpressionValue

        Private ReadOnly Property uniqueId As String Implements IExpressionValue.Identity
            Get
                Return $"{taxonomy_id}.{name}"
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"{name} [{taxonomy_lvl}]"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function LoadTable(tsvfile As String) As Bracken()
            Return tsvfile.LoadCsv(Of Bracken)(mute:=True, tsv:=True)
        End Function

    End Class
End Namespace