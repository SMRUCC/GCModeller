Imports System.Runtime.CompilerServices

Namespace MySQL

    Public Module DbExtensions

        <Extension> Public Function GetConnectedNode(correlation As MySQL.Tables.xcb, nodeFrom As String) As String
            If String.Equals(correlation.g1_entity, nodeFrom, StringComparison.OrdinalIgnoreCase) Then
                Return correlation.g2_entity
            ElseIf String.Equals(correlation.g2_entity, nodeFrom, StringComparison.OrdinalIgnoreCase)
                Return correlation.g1_entity
            Else
                Return ""
            End If
        End Function

        ''' <summary>
        ''' 一般认为WGCNA至少为0.1即可看作为共表达，而在筛选函数之中的过滤参数为0.6，故而WGCNA肯定会被过滤掉，在这里乘以6来避免被过滤
        ''' </summary>
        ''' <param name="pcc"></param>
        ''' <param name="spcc"></param>
        ''' <returns></returns>
        Public Function GetCorrelateScore(pcc As Double, spcc As Double) As Double ', wgcna As Double) As Double
            Dim a = Math.Abs(pcc)
            Dim b = Math.Abs(spcc)

            If a >= b Then
                Return pcc
            Else
                Return spcc
            End If
        End Function

        <Extension> Public Function GetCorrelateScore(correlates As MySQL.Tables.xcb) As Double
            Dim d As Double = GetCorrelateScore(correlates.pcc, correlates.spcc)
#Const DEBUG = 0
#If DEBUG Then
            If d = 0R Then
                Call $"{correlates.ToString} is not valid...".__DEBUG_ECHO
            End If
#End If
            Return d
        End Function
    End Module
End Namespace