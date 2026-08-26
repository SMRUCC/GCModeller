Imports System.IO
Imports System.Text

Namespace IO

    Module WriteModel

        ' ==================== 写入网络结构 ====================

        ''' <summary>
        ''' 将网络结构写入 TSV 文件
        ''' 格式：From, To, EdgeType
        ''' </summary>
        Public Sub WriteNetworkStructure(network As Core.BayesianNetwork, filePath As String)
            Dim sb As New StringBuilder()
            sb.AppendLine("From" & vbTab & "To" & vbTab & "EdgeType")

            For Each node In network.Nodes
                For Each parentIdx In node.Parents
                    sb.AppendLine(String.Format("{0}{1}{2}{3}regulation",
                        network.Nodes(parentIdx).Name, vbTab, node.Name, vbTab))
                Next
            Next

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8)
        End Sub

        ' ==================== 写入 CPD 参数 ====================

        ''' <summary>
        ''' 将 CPD 参数写入 TSV 文件
        ''' </summary>
        Public Sub WriteCPDParameters(network As Core.BayesianNetwork, filePath As String)
            Dim sb As New StringBuilder()
            sb.AppendLine("Gene" & vbTab & "Intercept" & vbTab & "Parents" & vbTab &
                          "Coefficients" & vbTab & "ResidualSD" & vbTab & "RSquared")

            For Each node In network.Nodes
                If node.CPD Is Nothing Then Continue For
                Dim cpd As Core.BnCPD = node.CPD

                Dim parentNames As String = ""
                Dim coeffStr As String = ""
                If cpd.ParentIndices IsNot Nothing AndAlso cpd.ParentIndices.Length > 0 Then
                    parentNames = String.Join(";", cpd.ParentIndices.Select(Function(p) network.Nodes(p).Name))
                    coeffStr = String.Join(";", cpd.Coeffs.Select(Function(c) c.ToString("F6")))
                End If

                sb.AppendLine(String.Format("{0}{1}{2:F6}{3}{4}{5}{6}{7}{8:F6}{9}{10:F4}",
                    node.Name, vbTab, cpd.Intercept, vbTab, parentNames, vbTab,
                    coeffStr, vbTab, cpd.ResidualSD, vbTab, cpd.RSquared))
            Next

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8)
        End Sub
    End Module
End Namespace