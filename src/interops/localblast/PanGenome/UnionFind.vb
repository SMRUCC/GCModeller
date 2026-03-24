''' <summary>
''' 并查集辅助类，用于高效处理基因家族的聚类
''' </summary>
Public Class UnionFind

    ReadOnly parent As New Dictionary(Of String, String)()

    ''' <summary>
    ''' 添加元素
    ''' </summary>
    ''' <param name="element"></param>
    Public Sub AddElement(element As String)
        If Not parent.ContainsKey(element) Then
            parent.Add(element, element)
        End If
    End Sub

    ''' <summary>
    ''' 查找根节点
    ''' </summary>
    ''' <param name="element"></param>
    ''' <returns></returns>
    Public Function Find(element As String) As String
        If Not parent.ContainsKey(element) Then
            Return Nothing
        End If

        ' 路径压缩
        If parent(element) <> element Then
            parent(element) = Find(parent(element))
        End If
        Return parent(element)
    End Function

    ''' <summary>
    ''' 合并两个集合
    ''' </summary>
    ''' <param name="elem1"></param>
    ''' <param name="elem2"></param>
    Public Sub Union(elem1 As String, elem2 As String)
        Dim root1 = Find(elem1)
        Dim root2 = Find(elem2)

        If root1 IsNot Nothing AndAlso root2 IsNot Nothing AndAlso root1 <> root2 Then
            ' 简单合并策略
            parent(root1) = root2
        End If
    End Sub
End Class