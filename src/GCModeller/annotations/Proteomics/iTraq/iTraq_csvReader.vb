Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.Uniprot

Public Module iTraq_csvReader

    <Extension>
    Public Iterator Function Combinations(signs As IEnumerable(Of iTraqSigns)) As IEnumerable(Of iTraqSigns)
        With signs.ToArray
            For Each symbol1 As iTraqSigns In .ref
                For Each symbol2 As iTraqSigns In .ref
                    Yield New iTraqSigns With {
                        .Sign = $"{symbol1.Sign}/{symbol2.Sign}",
                        .SampleID = $"{symbol1.SampleID}/{symbol2.SampleID}"
                    }
                Next
            Next
        End With
    End Function

    Public Function StripCsv(path$, Optional headers% = 2) As File
        Dim [in] As File = File.Load(path)
        Dim headerRows As RowObject() = [in].Take(headers).ToArray
        Dim proteins As New List(Of RowObject)
        Dim row As New Value(Of RowObject)
        Dim i As int = headers

        Do While Not (row = [in](++i)).IsNullOrEmpty
            If Not row.value.First.StringEmpty Then
                Dim h = UniprotFasta.SimpleHeaderParser(header:=(+row)(1))
                proteins += (+row)
                Call (+row).Insert(1, h("UniprotID"))
            End If
        Loop

        Dim out As New File
        out += headerRows.__mergeHeaders
        out += proteins.ToArray
        Return out
    End Function

    <Extension>
    Private Function __mergeHeaders(headers As RowObject()) As RowObject
        Dim out As New RowObject From {"ID", "UniprotID"}

        For i As Integer = 1 To headers(Scan0).NumbersOfColumn - 1
            Dim index% = i
            Dim t$() = headers.Select(Function(r) r(index)) _
                .Where(Function(str) Not str.StringEmpty) _
                .ToArray
            Dim s$ = t.JoinBy(" ")

            out += s
        Next

        Return out
    End Function

    <Extension>
    Public Function MergeShotgunAnnotations(proteins As EntityObject(),
                                            ta As NamedCollection(Of EntityObject),
                                            tb As NamedCollection(Of EntityObject),
                                            Optional mappings As Dictionary(Of String, String()) = Nothing) As EntityObject()
        ' 将注释数据放进去
        Dim table As Dictionary(Of NamedCollection(Of EntityObject)) = proteins.GroupByKey.ToDictionary
        Dim A$ = ta.Name, B$ = tb.Name
        Dim insertKey$
        'Dim reverseMappings As Dictionary(Of String, String()) = mappings _
        '    .SafeQuery _
        '    .Select(Function(k)
        '                Return k.Value.Select(Function(kv)
        '                                          Return (key:=kv, Value:=k.Key)
        '                                      End Function)
        '            End Function) _
        '    .IteratesALL _
        '    .GroupBy(Function(t) t.key) _
        '    .ToDictionary(Function(k) k.Key,
        '                  Function(g) g.Select(Function(v) v.Value).ToArray)
        Dim array As EntityObject()

        Const refID$ = "ref.ID"

        ' shotgun使用Pep开始的肽段数作为表达量的近似值
        For Each prot As EntityObject In ta
            For Each k In prot.EnumerateKeys
                If InStr(k, "Pep") > 0 Then
                    insertKey = A & "." & k
                Else
                    insertKey = k
                End If

                If mappings Is Nothing Then
                    array = table(prot.ID).ToArray
                Else
                    array = mappings(prot.ID) _
                        .Select(Function(key) table(key)) _
                        .IteratesAll
                End If

                For Each o As EntityObject In array.Distinct
                    Call o.Properties.Add(insertKey, prot(k))
                    If Not o.Properties.ContainsKey(refID) Then
                        Call o.Properties.Add(refID, prot.ID)
                    End If
                Next
            Next
        Next

        For Each prot As EntityObject In tb
            For Each k In prot.EnumerateKeys
                If InStr(k, "Pep") > 0 Then
                    insertKey = B & "." & k
                Else
                    insertKey = k
                End If

                If mappings Is Nothing Then
                    array = table(prot.ID).ToArray
                Else
                    array = mappings(prot.ID) _
                        .Select(Function(key) table(key)) _
                        .IteratesAll
                End If

                For Each o As EntityObject In array
                    With o.Properties
                        If Not .ContainsKey(insertKey) Then
                            .Add(insertKey, prot(k))
                        End If
                        If Not .ContainsKey(refID) Then
                            .Add(refID, prot.ID)
                        End If
                    End With
                Next
            Next
        Next

        Return table.Values.IteratesAll.ToArray
    End Function
End Module
