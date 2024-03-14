Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Text

Public Class Hierarchy : Inherits Tree(Of PathwayName)

    Public Overrides Function ToString() As String
        If Data Is Nothing Then
            Return label
        Else
            Return Data.ToString
        End If
    End Function

    Public Shared Function LoadInternal(Optional tax As String = Nothing) As Hierarchy
        Dim tree As New Hierarchy With {
            .ID = 0,
            .label = "Reactome",
            .Data = Nothing,
            .Parent = Nothing,
            .Childs = New Dictionary(Of String, Tree(Of PathwayName))
        }
        Dim names = PathwayName.LoadInternal.ToDictionary(Function(p) p.id)
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
                    .Childs = New Dictionary(Of String, Tree(Of PathwayName)),
                    .Data = names(.label)
                }
            End If
            If Not index.ContainsKey(child) Then
                index(child) = New Hierarchy With {
                    .ID = index.Count + 1,
                    .label = child,
                    .Childs = New Dictionary(Of String, Tree(Of PathwayName)),
                    .Parent = index(ancestor),
                    .Data = names(.label)
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

        If tax.StringEmpty Then
            Return tree
        Else
            Return FilterTax(tree, taxname:=tax)
        End If
    End Function

    Private Shared Function FilterTax(tree As Hierarchy, taxname As String) As Hierarchy
        For Each key As String In tree.Childs.Keys.ToArray
            If tree(key).Data Is Nothing Then
                tree.Childs.Remove(key)
            ElseIf tree(key).Data.tax_name <> taxname Then
                tree.Childs.Remove(key)
            End If
        Next

        Return tree
    End Function

End Class
