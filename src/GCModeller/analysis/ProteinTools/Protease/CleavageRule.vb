Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default

Public Class CleavageRule

    ''' <summary>
    ''' FullName
    ''' </summary>
    ''' <returns></returns>
    Public Property Name As String
    ''' <summary>
    ''' BriefName
    ''' </summary>
    ''' <returns></returns>
    Public Property Protease As String

#Region "in C-terminal direction"
    Public Property P4 As String
    Public Property P3 As String
    Public Property P2 As String
    Public Property P1 As String
#End Region

#Region "in C-terminal direction"
    Public Property P1C As String
    Public Property P2C As String
#End Region

    Shared ReadOnly Any As DefaultValue(Of String) = "." _
        .AsDefault([If]:=Function(s)
                             Select Case DirectCast(s, String)
                                 Case "-", " ", "*"
                                     Return True
                                 Case Else
                                     Return False
                             End Select
                         End Function)

    Public Iterator Function GetRules() As IEnumerable(Of String)
        Yield P4 Or Any
        Yield P3 Or Any
        Yield P2 Or Any
        Yield P1 Or Any

        Yield P1C Or Any
        Yield P2C Or Any
    End Function
End Class
