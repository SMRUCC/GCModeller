Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports KO = SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.KOCatalog
Imports r = System.Text.RegularExpressions.Regex
Imports XmlProperty = Microsoft.VisualBasic.Text.Xml.Models.Property

''' <summary>
''' 这个模块之中包含了如何构建UniProt参考库的方法
''' </summary>
Public Module UniProtBuild

    ''' <summary>
    ''' 得到的都是相同的定义，区别只在于代谢途径不同，则只取出第一个对象即可
    ''' </summary>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function ko00000Provider() As Dictionary(Of String, NamedValue(Of String))
        Return KO.ko00000 _
            .ToDictionary(Function(term) term.Key,
                          Function(term)
                              Return term.Value _
                                  .First _
                                  .Description _
                                  .GetTagValue(";", trim:=True)
                          End Function)
    End Function

    <Extension> Public Function ScanUniProt(UniProtXml As IEnumerable(Of entry)) As TaxonomyRepository
        ' 因为在这里是处理一个非常大的UniProt注释数据库，所以需要首先做一次扫描
        ' 将需要提取的信息先放到缓存之中
        Dim tmp$ = App.GetAppSysTempFile(, App.PID)
        Dim cache = UniProtXml.ScanInternal(tmp)
        Dim model As TaxonomyRepository = ScanModels(cache)
        Return model
    End Function

    Private Function ScanModels(cache As (KO_list$, taxonomy$)) As TaxonomyRepository
        Dim ko00000 = ko00000Provider()
        Dim list As New List(Of TaxonomyRef)
        Dim organismKO As New Dictionary(Of String, List(Of String))

        For Each line As String In cache.KO_list.IterateAllLines
            With line.Split(ASCII.TAB)
                If Not organismKO.ContainsKey(.First) Then
                    Call organismKO.Add(.First, New List(Of String))
                End If

                Call organismKO(.First).Add(.Last)
            End With
        Next

        ' 之后再将信息提取出来整理为一个XML格式的信息库
        For Each xml As String In r.Split(
                input:=cache.taxonomy.ReadAllText,
                pattern:=blankPattern,
                options:=RegexOptions.Singleline
            ) _
            .Where(Function(str)
                       Return InStr(str, "<") > 0 AndAlso InStr(str, ">") > 0
                   End Function)

            Dim organism As organism = xml.LoadFromXml(Of organism)
            Dim taxon$ = organism.dbReference.id
            Dim KO As IEnumerable(Of String)

            If organismKO.ContainsKey(taxon) Then
                KO = organismKO(taxon)
            Else
                KO = {}
            End If

            Dim terms As XmlProperty() = KO _
                .Distinct _
                .OrderBy(Function(id) id) _
                .Select(Function(id)
                            If ko00000.ContainsKey(id) Then
                                Dim term As NamedValue(Of String) = ko00000(id)

                                Return New XmlProperty With {
                                    .name = id,
                                    .value = term.Name,
                                    .Comment = term.Value
                                }
                            Else
                                Return New XmlProperty With {
                                    .name = id
                                }
                            End If
                        End Function) _
                .ToArray

            list += New TaxonomyRef With {
                .organism = organism,
                .TaxonID = taxon,
                .genome = New OrthologyTerms With {
                    .Terms = terms
                }
            }
        Next

        Return New TaxonomyRepository With {
            .Taxonomy = list
        }
    End Function

    <Extension>
    Private Iterator Function skipUntil(UniProtXml As IEnumerable(Of entry), acc$) As IEnumerable(Of entry)

    End Function

    Const KO_list$ = "KO.list"
    Const Taxonomy_data$ = "taxonomy.txt"
    Const blank$ = "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++"
    Const blankPattern$ = "\+{10,}\n"

    ''' <summary>
    ''' 函数返回来的是临时文件夹的路径位置
    ''' </summary>
    ''' <param name="UniProtXml"></param>
    ''' <param name="tmp">工作区缓存文件夹</param>
    ''' <returns></returns>
    <Extension>
    Private Function ScanInternal(UniProtXml As IEnumerable(Of entry), tmp$) As (KO_list As String, taxonomy As String)
        ' KO 缓存文件示例
        ' 
        ' taxonomy_id KO
        ' taxonomy_id KO
        ' taxonomy_id KO
        '
        ' 
        Dim taxonomyIndex As New Dictionary(Of String, String)

        Using KO As StreamWriter = $"{tmp}/{KO_list}".OpenWriter,
            taxo As StreamWriter = $"{tmp}/{Taxonomy_data}".OpenWriter

            For Each protein As entry In UniProtXml _
                .Where(Function(org)
                           Return Not org.organism Is Nothing AndAlso
                                  Not org.organism.dbReference Is Nothing
                       End Function)

                Dim taxonomy As organism = protein.organism
                Dim taxonID$ = taxonomy.dbReference.id

                ' 如果已经存在index了，则不会写入taxo文件之中
                If Not taxonomyIndex.ContainsKey(taxonID) Then
                    Call taxo.WriteLine(taxonomy.GetXml)
                    Call taxo.WriteLine(blank)
                    Call taxonomyIndex.Add(taxonID, "null")
                End If

                Dim KOlist$() = protein.Xrefs _
                    .TryGetValue("KO") _
                    .SafeQuery _
                    .Select(Function(KEGG) KEGG.id) _
                    .ToArray

                If KOlist.Length > 0 Then
                    Call KO.WriteLine(
                        value:=KOlist _
                            .Select(Function(id)
                                        Return taxonID & vbTab & id
                                    End Function) _
                            .JoinBy(KO.NewLine)
                    )
                End If
            Next
        End Using

        Return ($"{tmp}/{KO_list}", $"{tmp}/{Taxonomy_data}")
    End Function
End Module
