Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.Uniprot.XML

Public Module UniProtExtensions

    <Extension> Public Function ScanUniProt(UniProtXml As IEnumerable(Of entry)) As TaxonomyRepository
        ' 因为在这里是处理一个非常大的UniProt注释数据库，所以需要首先做一次扫描
        ' 将需要提取的信息先放到缓存之中
        Dim cache$ = UniProtXml.ScanInternal
        ' 之后再将信息提取出来整理为一个XML格式的信息库
        Dim list As New List(Of TaxonomyRef)

        For Each xml As String In ls - l - r - "*.Xml" <= cache
            Dim organism As organism = xml.LoadXml(Of organism)
            Dim KO$() = (xml.TrimSuffix & ".txt") _
                .ReadAllLines _
                .Distinct _
                .OrderBy(Function(id) id) _
                .ToArray


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
            Dim lineage$ = taxonomy.lineage.taxonlist.Select(AddressOf NormalizePathString).JoinBy("/").MD5
            Dim path$

            With taxonomy.lineage.taxonlist
                path = $"{tmp}/{ .First}/{ .ref(1)}/{lineage}/[{taxonomy.dbReference.id}] {taxonomy.scientificName.NormalizePathString}.XML"
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
