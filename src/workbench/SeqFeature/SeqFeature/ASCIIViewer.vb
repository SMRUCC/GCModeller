Imports System.IO
Imports System.Runtime.CompilerServices

Public Module ASCIIViewer

    '                           ProtK 
    'Ch_lo_Pn1.3_Pn2_ProtK_Therm| 
    '           Glu_ProtK_Staph|| 
    '            AspGluN_ProtK||| 
    '  ArgC_Clost_Therm_Tryps|||| 
    '        Glu_ProtK_Staph||||| 
    '               AspGluN|||||| 

    '                     ||||||| 
    '                     SERVELAT
    '                 1   --------   8

    <Extension>
    Public Sub DisplayOn(sites As IEnumerable(Of Site), seq$, Optional dev As TextWriter = Nothing)

    End Sub
End Module

Public Class Site
    Public Property Name As String
    Public Property Left As Integer
End Class