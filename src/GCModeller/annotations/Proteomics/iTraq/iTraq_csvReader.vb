#Region "Microsoft.VisualBasic::f4c70cb2dd21e1fc201e63489f21aad1, GCModeller\annotations\Proteomics\iTraq\iTraq_csvReader.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 189
    '    Code Lines: 146
    ' Comment Lines: 20
    '   Blank Lines: 23
    '     File Size: 7.32 KB


    ' Module iTraq_csvReader
    ' 
    '     Function: __mergeHeaders, Combinations, iTraqMatrix, MergeShotgunAnnotations, StripCsv
    '               SymbolReplace
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.Uniprot

Public Module iTraq_csvReader

    <Extension>
    Public Iterator Function Combinations(symbols As IEnumerable(Of iTraqSymbols)) As IEnumerable(Of iTraqSymbols)
        With symbols.ToArray
            For Each symbol1 As iTraqSymbols In .ByRef
                For Each symbol2 As iTraqSymbols In .ByRef
                    Yield New iTraqSymbols With {
                        .Symbol = $"{symbol1.Symbol}/{symbol2.Symbol}",
                        .SampleID = $"{symbol1.SampleID}/{symbol2.SampleID}",
                        .AnalysisID = $"{symbol1.AnalysisID}/{symbol2.AnalysisID}"
                    }
                Next
            Next
        End With
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="symbols">从csv文件之中所读取出来的原始标签数据</param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function iTraqMatrix(data As IEnumerable(Of iTraqReader), symbols As IEnumerable(Of iTraqSymbols)) As IEnumerable(Of DataSet)
        With symbols.Combinations.ToArray
            For Each gene As iTraqReader In data
                Dim groups = gene.GetSampleGroups(.ByRef)
                Dim foldChange = groups _
                    .Values _
                    .ToDictionary(Function(x) x.Group,
                                  Function(x) x.FoldChange)

                Yield New DataSet With {
                    .ID = gene.ID,
                    .Properties = foldChange
                }
            Next
        End With
    End Function

    <Extension>
    Public Iterator Function SymbolReplace(data As IEnumerable(Of iTraqReader), symbols As IEnumerable(Of iTraqSymbols)) As IEnumerable(Of iTraqReader)
        With symbols.Combinations.ToArray
            For Each gene As iTraqReader In data
                Dim groups = gene.GetSampleGroups(.ByRef)

                ' 直接进行赋值似乎会出现bug，在这里使用with代码块进行赋值操作
                With gene
                    .Properties = groups _
                        .Values _
                        .Select(Function(g) g.PopulateData) _
                        .IteratesALL _
                        .ToDictionary

                    Yield .ByRef
                End With
            Next
        End With
    End Function

    Public Function StripCsv(path$, Optional headers% = 2) As File
        Dim [in] As File = File.Load(path)
        Dim headerRows As RowObject() = [in].Take(headers).ToArray
        Dim proteins As New List(Of RowObject)
        Dim row As New Value(Of RowObject)
        Dim i As i32 = headers

        Do While Not (row = [in](++i)).IsNullOrEmpty
            If Not row.Value.First.StringEmpty Then
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
