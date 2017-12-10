Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports KO = SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.KOCatalog
Imports XmlProperty = Microsoft.VisualBasic.Text.Xml.Models.Property

Public Module UniProtExtensions

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
        Dim cache$ = UniProtXml.ScanInternal
        Dim model As TaxonomyRepository = ScanModels(cache)
        Return model
    End Function

    Private Function ScanModels(cache As String) As TaxonomyRepository
        Dim ko00000 = ko00000Provider()
        Dim list As New List(Of TaxonomyRef)

        ' 之后再将信息提取出来整理为一个XML格式的信息库
        For Each xml As String In ls - l - r - "*.Xml" <= cache
            Dim organism As organism = xml.LoadXml(Of organism)
            Dim KO$() = (xml.TrimSuffix & ".txt") _
                .ReadAllLines _
                .Distinct _
                .Where(Function(s) Not s.StringEmpty) _
                .OrderBy(Function(id) id) _
                .ToArray
            Dim terms As XmlProperty() = KO _
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
                .TaxonID = organism.dbReference.id,
                .genome = New OrthologyTerms With {
                    .Terms = terms
                }
            }
        Next

        Return New TaxonomyRepository With {
            .Taxonomy = list
        }
    End Function

    ''' <summary>
    ''' 函数返回来的是临时文件夹的路径位置
    ''' </summary>
    ''' <param name="UniProtXml"></param>
    ''' <returns></returns>
    <Extension>
    Private Function ScanInternal(UniProtXml As IEnumerable(Of entry)) As String
        Dim tmp$ = App.GetAppSysTempFile(, App.PID)

        For Each protein As entry In UniProtXml
            Dim taxonomy = protein.organism
            Dim lineage$ = taxonomy _
                .lineage _
                .taxonlist _
                .Select(AddressOf NormalizePathString) _
                .JoinBy("/") _
                .MD5
            Dim path$

            With taxonomy
                Dim name$ = $"[{ .dbReference.id}] { .scientificName.NormalizePathString}"

                With .lineage.taxonlist
                    path = $"{ .FirstOrDefault }/{ .ElementAtOrDefault(1)}/{lineage}"
                    path = $"{tmp}/{path}/{name}.XML"
                End With
            End With

            Dim KO$() = protein.Xrefs _
                .TryGetValue("KO") _
                .SafeQuery _
                .Select(Function(KEGG) KEGG.id) _
                .ToArray

            If path.FileExists Then
                ' 追加
                ' 因为append并不会换行，所以在这里需要额外的追加一个LF换行
                Call (KO _
                    .JoinBy(ASCII.LF) & vbLf) _
                    .SaveTo(path.TrimSuffix & ".txt", append:=True, throwEx:=False)
            Else
                ' 写入新的数据
                Call taxonomy.GetXml.SaveTo(path)
                Call KO.FlushAllLines(path.TrimSuffix & ".txt")
            End If
        Next

        Return tmp
    End Function
End Module
