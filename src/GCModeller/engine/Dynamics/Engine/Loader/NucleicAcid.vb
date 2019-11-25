Imports Microsoft.VisualBasic.Serialization.JSON

Public Class NucleicAcid

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public Property A As String
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public Property U As String
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public Property G As String
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public Property C As String

    Default Public ReadOnly Property Base(compound As String) As String
        Get
            Select Case compound
                Case "A" : Return A
                Case "U", "T" : Return U
                Case "G" : Return G
                Case "C" : Return C
                Case Else
                    Throw New NotImplementedException(compound)
            End Select
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

End Class

