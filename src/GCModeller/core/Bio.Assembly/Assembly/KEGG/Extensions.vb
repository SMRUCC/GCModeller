Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.ComponentModel

Namespace Assembly.KEGG

    Public Module Extensions

        ''' <summary>
        ''' 这个主要是应用于ID mapping操作的拓展函数
        ''' </summary>
        ''' <param name="text$">
        ''' Example as the text data in the kegg drugs or kegg disease object:
        ''' 
        ''' ```
        ''' E2A-PBX1 (translocation) [HSA:6929 5087] [KO:K09063 K09355]
        ''' ```
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function GetIDpairedList(text$) As Dictionary(Of String, String())
            Dim ids$() = Regex _
                .Matches(text, "\[.+?\]", RegexICSng) _
                .ToArray(Function(s) s.GetStackValue("[", "]"))

            ' 可能还会存在多重数据，所以在这里不能够直接生成字典
            Dim table As New Dictionary(Of String, List(Of String))

            For Each id As String In ids
                Dim k = id.GetTagValue(":")

                If Not table.ContainsKey(k.Name) Then
                    table.Add(
                        k.Name,
                        New List(Of String))
                End If

                Call table(k.Name).AddRange(k.Value.Split)
            Next

            Dim out As Dictionary(Of String, String()) =
                table.ToDictionary(
                Function(k) k.Key,
                Function(k) k.Value.ToArray)

            Return out
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="tag$"></param>
        ''' <param name="list$"></param>
        ''' <returns>
        ''' Example as:
        ''' 
        ''' ```
        ''' HSA:6929 5087
        ''' ```
        ''' </returns>
        <Extension>
        Public Function IDlistStrings(tag$, list$()) As String
            Return $"{tag}:{list.JoinBy(" ")}"
        End Function

        ''' <summary>
        ''' 检测判断所输入的字符串是否是符合格式要求的？
        ''' </summary>
        ''' <param name="s$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ValidateEntryFormat(s$) As Boolean
            Return s.MatchPattern("[a-z]+\d+")
        End Function

        ''' <summary>
        ''' Example as:
        ''' 
        ''' ```
        ''' Same as: C00001
        ''' ```
        ''' </summary>
        ''' <param name="s$"></param>
        ''' <returns></returns>
        <Extension> Public Function TheSameAs(s$()) As String
            Dim tags As NamedValue(Of String) = s _
                .Select(Function(l) l.GetTagValue(":", trim:=True)) _
                .Where(Function(v) v.Name.TextEquals("Same as")) _
                .FirstOrDefault
            Return tags.Value
        End Function

        <Extension>
        Public Function TheSameAs(Of T As IKEGGRemarks)(o As T) As String
            If Not o.Remarks.IsNullOrEmpty Then
                Return o.Remarks.TheSameAs
            Else
                Return ""
            End If
        End Function

        Public Interface IKEGGRemarks
            Property Remarks As String()
        End Interface

        ''' <summary>
        ''' 这个函数会自动将物种的KEGG前缀去除掉，从而能够直接匹配字典之中的键名
        ''' </summary>
        ''' <param name="table"></param>
        ''' <param name="ID$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function GetPathwayBrite(table As Dictionary(Of String, Pathway), ID$) As Pathway
            ID = Regex.Match(ID, "\d+").Value
            Return table.TryGetValue(ID)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="mappings">``{geneID -> KO}`` mapping data collection.</param>
        ''' <param name="keepsZERO">默认不保存计数为零的分类</param>
        ''' <returns>这个函数所返回去的数据一般是用作于绘图操作的</returns>
        <Extension>
        Public Function LevelAKOStatics(mappings As IEnumerable(Of NamedValue(Of String)),
                                        Optional ByRef KO_counts As KOCatalog() = Nothing,
                                        Optional keepsZERO As Boolean = False) _
                                        As Dictionary(Of String, NamedValue(Of Integer)())
            Dim brites As htext = htext.ko00001
            Dim KOTable As Dictionary(Of String, BriteHText) = brites.GetEntryDictionary
            Dim counts = mappings _
                .GroupBy(Function(gene) gene.Value) _
                .Select(Function(x)
                            ' 对每一个KO进行数量上的统计分析
                            If KOTable.ContainsKey(x.Key) Then
                                Return New KOCatalog With {
                                    .Catalog = x.Key,
                                    .IDs = x.Select(Function(gene) gene.Name).ToArray,
                                    .Description = KOTable(.Catalog).Description,
                                    .Class = KOTable(.Catalog).Class
                                }
                            Else
                                Return New KOCatalog With {
                                    .Catalog = x.Key,
                                    .IDs = x.Select(Function(gene) gene.Name).ToArray,
                                    .Description = "No hits in KEGG KO database",
                                    .Class = "Unclassified"
                                }
                            End If
                        End Function) _
                .ToArray
            Dim out As New Dictionary(Of String, NamedValue(Of Integer)())

            KO_counts = counts

            For Each [class] As BriteHText In brites.Hierarchical.CategoryItems
                Dim profile As New List(Of NamedValue(Of Integer))

                For Each levelACatalog As BriteHText In [class].CategoryItems
                    ' 在这里统计levelA的分布情况
                    Dim KO As IndexOf(Of String) = levelACatalog _
                        .GetEntries _
                        .Where(Function(s) Not s.StringEmpty) _
                        .Indexing
                    profile += New NamedValue(Of Integer) With {
                        .Name = levelACatalog.ClassLabel,
                        .Description = levelACatalog.Description,
                        .Value = counts _
                            .Where(Function(tag) KO(tag.Catalog) > -1) _
                            .Count
                    }
                Next

                out([class].ClassLabel) = If(
                    keepsZERO,
                    profile,
                    profile.Where(Function(x) x.Value > 0).ToArray)
            Next

            Return out
        End Function
    End Module
End Namespace