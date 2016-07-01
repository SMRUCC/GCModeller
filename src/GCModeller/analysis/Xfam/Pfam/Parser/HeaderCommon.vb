Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports System.Reflection
Imports SMRUCC.genomics.SequenceModel

Namespace PfamFastaComponentModels

    Public MustInherit Class PfamCommon
        Implements I_PolymerSequenceModel

        Friend Const P1 As Integer = 0
        Friend Const P2 As Integer = 1
        Friend Const P3 As Integer = 2

#Region "p1"
        Public Property UniqueId As String
#End Region

#Region "p2"
        Public Property Uniprot As String
        Public Property ChainId As String
#End Region

#Region "p3"
        Public Property PfamId As String
        Public Property PfamIdAsub As String
        Public Property PfamCommonName As String
#End Region

        Public Property SequenceData As String Implements I_PolymerSequenceModel.SequenceData

        Public Function ShadowCopy(Of T As PfamCommon)() As T
            Dim PfamObject As T = Activator.CreateInstance(Of T)()

            With PfamObject
                .ChainId = ChainId
                .PfamId = PfamId
                .PfamIdAsub = PfamIdAsub
                .PfamCommonName = PfamCommonName
                .SequenceData = SequenceData
                .Uniprot = Uniprot
                .UniqueId = UniqueId
            End With

            Return PfamObject
        End Function
    End Class

    Public Class PfamCsvRow : Inherits PfamCommon

        Public Property Start As Integer
        Public Property Ends As Integer

        Public Shared Function CreateObject(FastaData As PfamFasta) As PfamCsvRow
            Dim PfamCsvRow As PfamCsvRow = FastaData.ShadowCopy(Of PfamCsvRow)()
            PfamCsvRow.Start = FastaData.Location.Left
            PfamCsvRow.Ends = FastaData.Location.Right

            Return PfamCsvRow
        End Function
    End Class
End Namespace