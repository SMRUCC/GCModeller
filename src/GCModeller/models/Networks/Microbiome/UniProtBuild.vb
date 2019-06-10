#Region "Microsoft.VisualBasic::751b3b037e673d01a8e601306771bc87, models\Networks\Microbiome\UniProtBuild.vb"

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

    ' Module UniProtBuild
    ' 
    '     Function: ko00000Provider, ScanInternal, (+2 Overloads) ScanModels, ScanUniProt, skipUntil
    ' 
    '     Sub: CopyTo
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel
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

    <Extension>
    Public Function ScanUniProt(UniProtXml As IEnumerable(Of entry), Optional ByRef cache As (KO_list$, taxonomy$, counts$) = Nothing) As TaxonomyRepository
        ' 因为在这里是处理一个非常大的UniProt注释数据库，所以需要首先做一次扫描
        ' 将需要提取的信息先放到缓存之中
        Dim tmp$ = App.GetAppSysTempFile(, App.PID)
        Dim model As TaxonomyRepository

        cache = UniProtXml.ScanInternal(tmp)
        model = ScanModels(cache)

        Return model
    End Function

    <Extension>
    Public Sub CopyTo(cache As (KO_list$, taxonomy$, counts$), destination$)
        With destination & "/"
            Call cache.KO_list.FileCopy(.ByRef)
            Call cache.taxonomy.FileCopy(.ByRef)
            Call cache.counts.FileCopy(.ByRef)
        End With
    End Sub

    ''' <summary>
    ''' 从一个已经缓存有数据的文件夹之中构建模型
    ''' </summary>
    ''' <param name="cache"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function ScanModels(cache As String) As TaxonomyRepository
        Return (cache & "/" & KO_list, cache & "/" & Taxonomy_data, cache & "/" & gene_counts).ScanModels
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="cache"></param>
    ''' <param name="export">因为UniProt数据库可能达到1TB的数量级,所以在这里必须要使用这个参数来导出数据文件,否则内存会溢出</param>
    ''' <returns></returns>
    <Extension>
    Public Function ScanModels(cache As (KO_list$, taxonomy$, counts$), export$) As TaxonomyRepository
        Dim ko00000 = ko00000Provider()
        Dim organismKO As New Dictionary(Of String, List(Of String))
        Dim counts As New Dictionary(Of String, Counter)
        Dim repository As New TaxonomyRepository With {
            .taxonomy = New Dictionary(Of String, Metagenomics.Taxonomy)
        }

        DirectCast(repository, IFileReference).FilePath = $"{export}/main.json"

        For Each line As String In cache.KO_list.IterateAllLines
            With line.Split(ASCII.TAB)
                If Not organismKO.ContainsKey(.First) Then
                    Call organismKO.Add(.First, New List(Of String))
                End If

                Call organismKO(.First).Add(.Last)
            End With
        Next

        For Each genomeID As String In cache.counts.IterateAllLines
            If Not counts.ContainsKey(genomeID) Then
                Call counts.Add(genomeID, New Counter)
            End If

            Call counts(genomeID).Hit()
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

            ' 因为要了解覆盖度，所以这里需要计数一下KO的数目
            ' 因为有些基因的功能可能会重复，即KO相同，所以要在Distinct之前做计数
            Dim annotated% = KO.Count
            Dim terms As XmlProperty() = KO _
                .Distinct _
                .OrderBy(Function(id) id) _
                .Select(Function(id)
                            If ko00000.ContainsKey(id) Then
                                Dim term As NamedValue(Of String) = ko00000(id)

                                Return New XmlProperty With {
                                    .name = id,
                                    .value = term.Name,
                                    .comment = term.Value
                                }
                            Else
                                Return New XmlProperty With {
                                    .name = id
                                }
                            End If
                        End Function) _
                .ToArray

            Dim refModel As New TaxonomyRef With {
                .organism = organism,
                .taxonID = taxon,
                .genome = New OrthologyTerms With {
                    .Terms = terms
                },
                .coverage = annotated / counts(taxon)
            }

            Call repository.taxonomy.Add(taxon, refModel.TaxonomyString)
            Call refModel _
                .GetXml _
                .SaveTo(repository.StorageReference(refModel.TaxonomyString, relative:=False))
        Next

        Return repository
    End Function

    ''' <summary>
    ''' 任务意外中断可以使用这个函数来进行继续执行
    ''' </summary>
    ''' <param name="UniProtXml"></param>
    ''' <param name="acc$"></param>
    ''' <returns></returns>
    <Extension>
    Private Iterator Function skipUntil(UniProtXml As IEnumerable(Of entry), acc$) As IEnumerable(Of entry)
        Dim skip As Boolean = True

        If acc.StringEmpty Then
            skip = False
        End If

        For Each protein As entry In UniProtXml
            If skip Then
                ' 假设在执行数据库构建任务的时候，acc编号是在成功写入数据之后才会被记录下来的
                ' 那么也就是说当前的这个protein的数据已经被建立索引了
                ' 所以不需要再yield返回了，这里只需要设置一下skip开关即可
                If protein.accessions.IndexOf(acc) > -1 Then
                    skip = False
                End If
            Else
                Yield protein
            End If
        Next
    End Function

    Const KO_list$ = "KO.list"
    Const Taxonomy_data$ = "taxonomy.txt"
    Const gene_counts$ = "gene.counts"
    Const blank$ = "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++"
    Const blankPattern$ = "\+{10,}\n"

    ''' <summary>
    ''' 函数返回来的是临时文件夹的路径位置
    ''' </summary>
    ''' <param name="UniProtXml"></param>
    ''' <param name="tmp">工作区缓存文件夹</param>
    ''' <returns></returns>
    <Extension>
    Private Function ScanInternal(UniProtXml As IEnumerable(Of entry), tmp$) As (KO_list$, taxonomy$, counts$)
        ' KO 缓存文件示例
        ' 
        ' taxonomy_id KO
        ' taxonomy_id KO
        ' taxonomy_id KO
        '
        ' 
        Dim taxonomyIndex As New Dictionary(Of String, String)

        Using KO As StreamWriter = $"{tmp}/{KO_list}".OpenWriter,
            taxon As StreamWriter = $"{tmp}/{Taxonomy_data}".OpenWriter,
            counts As StreamWriter = $"{tmp}/{gene_counts}".OpenWriter

            For Each protein As entry In UniProtXml _
                .Where(Function(org)
                           Return Not org.organism Is Nothing AndAlso
                                  Not org.organism.dbReference Is Nothing
                       End Function)

                Dim taxonomy As organism = protein.organism
                Dim taxonID$ = taxonomy.dbReference.id

                ' 为了计算出覆盖度，需要知道基因组之中有多少个基因的记录
                ' 在这里直接记录下物种的编号，在后面分组计数就可以了解基因组
                ' 之中的基因的总数了
                Call counts.WriteLine(taxonID)

                ' 如果已经存在index了，则不会写入taxo文件之中
                If Not taxonomyIndex.ContainsKey(taxonID) Then
                    Call taxon.WriteLine(taxonomy.GetXml)
                    Call taxon.WriteLine(blank)
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

            Call KO.Flush()
            Call counts.Flush()
            Call taxon.Flush()
        End Using

        Return (
            $"{tmp}/{KO_list}",
            $"{tmp}/{Taxonomy_data}",
            $"{tmp}/{gene_counts}"
        )
    End Function
End Module
