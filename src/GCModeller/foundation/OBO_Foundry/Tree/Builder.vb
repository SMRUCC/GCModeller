
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Public Module Builder

    <Extension>
    Public Function BuildTree(terms As IEnumerable(Of RawTerm)) As Dictionary(Of String, GenericTree)
        Dim vertex As Dictionary(Of String, GenericTree) = terms _
            .Where(Function(t) t.Type = "[Term]") _
            .Select(Function(t)
                        Dim data = t.GetData
                        Dim id = (data!id).First
                        Return (id:=id, term:=t, data:=data)
                    End Function) _
            .ToDictionary(Function(t) t.id,
                          Function(k)
                              Dim name = k.data!name.First
                              Return New GenericTree With {
                                  .ID = k.id,
                                  .data = k.data,
                                  .name = name
                              }
                          End Function)

        For Each v As GenericTree In vertex.Values
            If Not v.data.ContainsKey("is_a") Then
                v.is_a = {}
            Else
                Dim is_a = v.data!is_a _
                    .Select(Function(value)
                                Return value.StringSplit("\s*!\s*").First.Trim
                            End Function) _
                    .ToArray

                v.is_a = is_a _
                    .Select(Function(id) vertex(id)) _
                    .ToArray
            End If
        Next

        Return vertex
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="node">Tree was created by <see cref="Builder.BuildTree(IEnumerable(Of RawTerm))"/> function.</param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function GetTermsByLevel(node As GenericTree, level%) As GenericTree()
        Dim chains As New List(Of List(Of GenericTree))

        If node.is_a.IsNullOrEmpty Then
            chains = New List(Of List(Of GenericTree)) From {New List(Of GenericTree) From {node}}
        Else
            For Each parent As GenericTree In node.is_a
                For Each chain In GetTermsLineage(parent, {node, parent})
                    chains.Add(chain)
                Next
            Next
        End If

        chains = chains _
            .Select(Function(chain)
                        Return chain.With(Sub(c) Call c.Reverse())
                    End Function) _
            .AsList

        Return chains _
            .Select(Function(chain) chain.ElementAtOrDefault(level)) _
            .Where(Function(lineNode) Not lineNode Is Nothing) _
            .Distinct _
            .ToArray
    End Function

    <Extension>
    Private Iterator Function GetTermsLineage(node As GenericTree, parentChain As IEnumerable(Of GenericTree)) As IEnumerable(Of List(Of GenericTree))
        Dim chainList = parentChain.AsList

        If node.is_a.IsNullOrEmpty Then
            Yield chainList
        Else
            For Each parent As GenericTree In node.is_a
                For Each chain In GetTermsLineage(parent, New List(Of GenericTree)(chainList).Join(parent))
                    Yield chain
                Next
            Next
        End If
    End Function
End Module
