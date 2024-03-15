Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.Text

Public Class HierarchyLink

    Public Property pathway As PathwayName
    Public Property id As String
    Public Property childs As String()
    Public Property parent As String

    Public ReadOnly Property text As String
        Get
            Return pathway.name
        End Get
    End Property

    Public Shared Iterator Function CalculateRoots(tree As Dictionary(Of String, HierarchyLink)) As IEnumerable(Of String)
        Dim allChilds = tree.Values.Select(Function(a) a.childs.Indexing).ToArray

        For Each key As String In tree.Keys
            If allChilds.All(Function(i) Not key Like i) Then
                Yield key
            End If
        Next
    End Function

    ''' <summary>
    ''' generates the list data for load in jstree
    ''' </summary>
    ''' <param name="tax"></param>
    ''' <returns></returns>
    Public Shared Function LoadInternal(Optional tax As String = Nothing) As Dictionary(Of String, HierarchyLink)
        Dim names = PathwayName.LoadInternal.ToDictionary(Function(p) p.id)
        Dim index As New Dictionary(Of String, HierarchyLink)
        Dim childs As New Dictionary(Of String, List(Of String))
        Dim t As String()
        Dim ancestor As String
        Dim child As String

        For Each line As String In My.Resources.ReactomePathwaysRelation.LineTokens
            t = line.Split(ASCII.TAB)
            ancestor = t(0)
            child = t(1)

            If Not index.ContainsKey(ancestor) Then
                index(ancestor) = New HierarchyLink With {.id = ancestor, .pathway = names(.id)}
                childs(ancestor) = New List(Of String)
            End If
            If Not index.ContainsKey(child) Then
                index(child) = New HierarchyLink With {.id = child, .pathway = names(.id)}
                childs(child) = New List(Of String)
            End If

            childs(ancestor).Add(child)
            index(child).parent = ancestor
        Next

        For Each key As String In childs.Keys
            index(key).childs = childs(key).ToArray
        Next

        If Not tax.StringEmpty Then
            index = FilterTax(index, tax)
        End If

        index("Reactome") = New HierarchyLink With {
            .id = "Reactome",
            .childs = CalculateRoots(index).ToArray,
            .parent = "#",
            .pathway = New PathwayName With {
                .id = "Reactome",
                .name = "Reactome",
                .tax_name = "Reactome"
            }
        }

        For Each id As String In index("Reactome").childs
            index(id).parent = "Reactome"
        Next

        Return index
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Shared Function FilterTax(tree As Dictionary(Of String, HierarchyLink), taxname As String) As Dictionary(Of String, HierarchyLink)
        Return tree.Values _
            .Where(Function(a) a.pathway.tax_name = taxname) _
            .ToDictionary(Function(a)
                              Return a.id
                          End Function)
    End Function

End Class

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

    Public Shared Function TreeJSON(tree As Hierarchy) As String
        Call BreakParentLoop(tree)
        Return JSONSerializer.GetJson(tree)
    End Function

    Private Shared Sub BreakParentLoop(ByRef tree As Hierarchy)
        tree.Parent = Nothing

        For Each key As String In tree.Childs.Keys.ToArray
            BreakParentLoop(tree(key))
        Next
    End Sub

End Class
