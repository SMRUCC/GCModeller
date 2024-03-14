Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Text

Public Class Hierarchy : Inherits Tree(Of PathwayName)

    Public Shared Function LoadInternal() As Hierarchy
        Dim tree As New Hierarchy With {
            .ID = 0,
            .label = "Reactome",
            .Data = Nothing,
            .Parent = Nothing,
            .Childs = New Dictionary(Of String, Tree(Of PathwayName))
        }
        Dim index As New Dictionary(Of String, Hierarchy)
        Dim t As String()
        Dim ancestor As String
        Dim child As String
        Dim parent As Hierarchy

        For Each line As String In My.Resources.ReactomePathwaysRelation.LineTokens
            t = line.Split(ASCII.TAB)
            ancestor = t(0)
            child = t(1)

            If Not index.ContainsKey(ancestor) Then
                index(ancestor) = New Hierarchy With {
                    .ID = index.Count + 1,
                    .label = ancestor,
                    .Childs = New Dictionary(Of String, Tree(Of PathwayName))
                }
            End If
            If Not index.ContainsKey(child) Then
                index(child) = New Hierarchy With {
                    .ID = index.Count + 1,
                    .label = child,
                    .Childs = New Dictionary(Of String, Tree(Of PathwayName)),
                    .Parent = index(ancestor)
                }
            End If

            parent = index(ancestor)
            parent.Childs.Add(child, index(child))
        Next

        For Each item As Hierarchy In index.Values
            If item.Parent Is Nothing Then
                tree.Childs.Add(item.label, item)
                item.Parent = tree
            End If
        Next

        Return tree
    End Function

End Class
