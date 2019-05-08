
Imports System.Runtime.CompilerServices

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
End Module
