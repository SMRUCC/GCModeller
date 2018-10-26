Imports Microsoft.VisualBasic.ComponentModel.TagData

Public Structure Reaction

    ''' <summary>
    ''' 反应过程编号
    ''' </summary>
    Public ID As String

    ''' <summary>
    ''' 代谢底物编号
    ''' </summary>
    Public substrates As FactorString(Of Double)()
    ''' <summary>
    ''' 代谢产物编号
    ''' </summary>
    Public products As FactorString(Of Double)()
    ''' <summary>
    ''' 酶编号
    ''' </summary>
    Public enzyme As String()

    Public Overrides Function ToString() As String
        Return ID
    End Function

End Structure