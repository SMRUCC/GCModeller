Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions

Namespace AnalysisAPI

    ''' <summary>
    ''' TSS位点差异性
    ''' </summary>
    Public Class TSSsDifferent
        Public Property GeneID As String
        Public Property TSSs_Condition1 As Long
        Public Property TSSs_Condition2 As Long
        Public Property TTSs_Condition1 As Long
        Public Property TTSs_Condition2 As Long

#Region "产生差异的两个实验条件的描述"

        Public Property Condition1 As String
        Public Property Condition2 As String
#End Region

        Public Property Pathway As String()

        Public Overrides Function ToString() As String
            Return GeneID
        End Function

        Public Function HaveBoth() As Boolean
            Return TSSs_Condition1 <> 0 AndAlso TSSs_Condition2 <> 0 AndAlso TTSs_Condition1 <> 0 AndAlso TTSs_Condition2 <> 0
        End Function

        Public Function HaveTSSs() As Boolean
            Return TSSs_Condition1 <> 0 AndAlso TSSs_Condition2 <> 0
        End Function

        Public Function HaveTTSs() As Boolean
            Return TTSs_Condition1 <> 0 AndAlso TTSs_Condition2 <> 0
        End Function

        Public Function TSSChanged() As Boolean
            If Not HaveTSSs() Then
                Return False
            End If
            Return Math.Abs(TSSs_Condition1 - TSSs_Condition2) > 10
        End Function

        Public Function TTSChnaged() As Boolean
            If Not HaveTTSs() Then
                Return False
            End If
            Return Math.Abs(TTSs_Condition1 - TTSs_Condition2) > 10
        End Function
    End Class

End Namespace