Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module SILVA_OTU

    <Extension>
    Public Function ParseOTUrep(fasta As IEnumerable(Of FastaToken)) As Dictionary(Of String, NamedValue(Of Integer))
        Dim table As New Dictionary(Of String, NamedValue(Of Integer))
        Dim OTU$()

        For Each kseq As FastaToken In fasta
            With kseq.Title.Split(ASCII.TAB)
                OTU = .Last.Split("|"c)
                table(.First) = New NamedValue(Of Integer) With {
                    .Name = OTU(0),
                    .Value = Val(OTU(1))
                }
            End With
        Next

        Return table
    End Function
End Module
