Imports SMRUCC.genomics.SequenceModel

Namespace PfamFastaComponentModels

    Public Class PfamCsvRow : Inherits PfamCommon
        Implements IPolymerSequenceModel

        Public Property Start As Integer
        Public Property Ends As Integer
        Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData

        Public Shared Function CreateObject(FastaData As PfamFasta) As PfamCsvRow
            Dim PfamCsvRow As PfamCsvRow = FastaData.ShadowCopy(Of PfamCsvRow)()
            PfamCsvRow.Start = FastaData.Location.Left
            PfamCsvRow.Ends = FastaData.Location.Right

            Return PfamCsvRow
        End Function
    End Class
End Namespace