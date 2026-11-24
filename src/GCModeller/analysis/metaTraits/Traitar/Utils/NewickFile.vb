Imports SMRUCC.genomics.Analysis.metaTraits.Traitar.Models

Namespace Traitar

    Public Module NewickFile

        ' ================================================================
        ' 5. Newick 系统发育树解析
        ' ================================================================

        ''' <summary>
        ''' 解析Newick格式系统发育树
        ''' 简化版，仅支持基本结构
        ''' </summary>
        Public Function ParseNewick(newickStr As String) As PhyloTreeNode
            newickStr = newickStr.Trim()
            If newickStr.EndsWith(";") Then
                newickStr = newickStr.Substring(0, newickStr.Length - 1)
            End If

            Dim pos As Integer = 0
            Return ParseNewickRecursive(newickStr, pos)
        End Function

        Private Function ParseNewickRecursive(s As String, ByRef pos As Integer) As PhyloTreeNode
            Dim node As New PhyloTreeNode()

            If pos < s.Length AndAlso s(pos) = "(" Then
                pos += 1  ' 跳过 "("
                Do
                    Dim child As PhyloTreeNode = ParseNewickRecursive(s, pos)
                    child.Parent = node
                    node.Children.Add(child)
                    If pos < s.Length AndAlso s(pos) = "," Then
                        pos += 1  ' 跳过 ","
                    Else
                        Exit Do
                    End If
                Loop

                If pos < s.Length AndAlso s(pos) = ")" Then
                    pos += 1  ' 跳过 ")"
                End If
            End If

            ' 解析节点名
            Dim nameBuilder As New System.Text.StringBuilder()
            Do While pos < s.Length
                Dim c As Char = s(pos)
                If c = ","c OrElse c = ")"c OrElse c = "("c OrElse c = ";"c Then
                    Exit Do
                End If
                nameBuilder.Append(c)
                pos += 1
            Loop

            ' 处理分支长度（如 name:0.123）
            Dim nameStr As String = nameBuilder.ToString()
            Dim colonIdx As Integer = nameStr.IndexOf(":"c)
            If colonIdx >= 0 Then
                node.Name = nameStr.Substring(0, colonIdx).Trim()
                Dim blStr As String = nameStr.Substring(colonIdx + 1).Trim()
                Dim bl As Double
                If Double.TryParse(blStr, bl) Then
                    node.BranchLength = bl
                End If
            Else
                node.Name = nameStr.Trim()
            End If

            Return node
        End Function
    End Module
End Namespace