Imports SMRUCC.genomics.SequenceModel

Namespace PfamFastaComponentModels

    Public Class PfamCsvRow : Inherits PfamEntryHeader
        Implements IPolymerSequenceModel

        Public Property Start As Integer
        Public Property Ends As Integer
        Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData

        Public Shared Function CreateObject(FastaData As PfamFasta) As PfamCsvRow
            Dim row As PfamCsvRow = FastaData.ShadowCopy(Of PfamCsvRow)()
            row.Start = FastaData.Location.Left
            row.Ends = FastaData.Location.Right

            Return row
        End Function
    End Class
End Namespace