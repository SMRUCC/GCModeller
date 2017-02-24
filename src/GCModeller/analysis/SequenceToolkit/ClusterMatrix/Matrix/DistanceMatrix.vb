Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module DistanceMatrix

    <Extension>
    Public Function NeedlemanWunsch(locis As IEnumerable(Of FastaToken), Optional out As StreamWriter = Nothing) As DataSet()
        If out Is Nothing Then
            out = App.NullDevice
        End If


    End Function
End Module
