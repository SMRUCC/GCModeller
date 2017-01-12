Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.KEGG
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.Uniprot.Web
Imports protein = Microsoft.VisualBasic.Data.csv.DocumentStream.EntityObject

Public Module ProteinGroups

    Public Function GetProteinIds(path$, Optional column$ = "Protein IDs") As String()
        Dim idlist$() = path _
            .LoadCsv _
            .GetColumnObjects(column, Function(s) s.Split(";"c)) _
            .Unlist _
            .Distinct _
            .ToArray
        Return idlist
    End Function

    Public Sub GetProteinDefs(path$, Optional save$ = Nothing, Optional column$ = "Protein IDs")
        Dim idData$() = GetProteinIds(path, column)
        Dim gz$ = If(save.IsBlank, path.TrimSuffix & ".xml.gz", save)

        Call Retrieve_IDmapping.Mapping(idData, IdTypes.NF90, IdTypes.ACC, gz)
        Call idData.SaveTo(gz.TrimSuffix.TrimSuffix & "-proteins.txt")
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="ID">
    ''' 对于蛋白组分析而言，这里的每一个元素都是一组基因号的集合，基因号之间通过<paramref name="deli"/>来区分
    ''' </param>
    ''' <param name="idMapping$"></param>
    ''' <param name="uniprotXML$"></param>
    ''' <param name="deli"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function GenerateAnnotations(ID As IEnumerable(Of String), idMapping$, uniprotXML$, Optional prefix$ = "", Optional deli As Char = ";"c) As IEnumerable(Of protein)
        Dim mappings As Dictionary(Of String, String()) = Retrieve_IDmapping.MappingReader(idMapping)
        Dim uniprot As Dictionary(Of String, Uniprot.XML.entry) =
            SMRUCC.genomics.Assembly.Uniprot.XML.UniprotXML _
            .Load(uniprotXML) _
            .entries _
            .GroupBy(Function(x) x.accession) _
            .ToDictionary(Function(x) x.Key,
                          Function(x) x.First)

        For Each Idtags As SeqValue(Of String) In ID.SeqIterator
            Dim list$() = (+Idtags).Split(deli)
            Dim i As Integer = Idtags.i + 1

            Yield list.__applyInternal(
                New Dictionary(Of String, String), mappings, uniprot, prefix, i)
        Next
    End Function

    <Extension>
    Private Function __applyInternal(source As String(),
                                     annotations As Dictionary(Of String, String),
                                     mappings As Dictionary(Of String, String()),
                                     uniprot As Dictionary(Of String, Uniprot.XML.entry),
                                     prefix$, i%) As protein

        Dim mappsId$() = source _
            .Select(Function(s) UCase(s)) _
            .Where(Function(ref) mappings.ContainsKey(ref)) _
            .Select(Function(ref) mappings(ref)) _
            .Unlist _
            .Distinct _
            .ToArray
        Dim uniprots As Uniprot.XML.entry() = mappsId _
            .Where(Function(acc) uniprot.ContainsKey(acc)) _
            .Select(Function(acc) uniprot(acc)) _
            .ToArray
        Dim names = uniprots _
            .Select(Function(prot) prot.protein) _
            .Where(Function(x) Not x Is Nothing AndAlso Not x.recommendedName Is Nothing) _
            .Select(Function(x) x.recommendedName.fullName.value) _
            .Distinct _
            .ToArray
        Dim geneNames = uniprots _
            .Select(Function(prot) prot.gene) _
            .Where(Function(x) Not x Is Nothing AndAlso Not x.name Is Nothing) _
            .Select(Function(x) x.name.value) _
            .Where(Function(s) Not String.IsNullOrEmpty(s)) _
            .Distinct _
            .OrderBy(Function(s) Len(s)) _
            .FirstOrDefault
        Dim getKeyValue = Function(key$)
                              Return uniprots _
                                .Where(Function(x) x.Xrefs.ContainsKey(key)) _
                                .Select(Function(x) x.Xrefs(key)) _
                                .Unlist _
                                .Select(Function(x) x.id) _
                                .Distinct _
                                .ToArray
                          End Function
        Dim GO As String() = getKeyValue("GO")
        Dim EC As String() = getKeyValue("EC")
        Dim KO As String() = getKeyValue("KO")

        Call annotations.Add("geneName", geneNames)
        Call annotations.Add("fullName", names.JoinBy("; "))
        Call annotations.Add("GO", GO.JoinBy("; "))
        Call annotations.Add("EC", EC.JoinBy("; "))
        Call annotations.Add("KO", KO.JoinBy("; "))

        getKeyValue = Function(key)
                          Return uniprots _
                            .Where(Function(x) x.CommentList.ContainsKey(key)) _
                            .Select(Function(x) x.CommentList(key)) _
                            .Unlist _
                            .Select(Function(x) x.text.value) _
                            .Distinct _
                            .ToArray
                      End Function

        Dim functions = getKeyValue("function")
        Dim pathways = getKeyValue("pathway")

        Call annotations.Add("functions", functions.JoinBy("; "))
        Call annotations.Add("pathways", pathways.JoinBy("; "))

        Dim geneID As String = i

        If Not String.IsNullOrEmpty(prefix) Then
            geneID = prefix & "_" & geneID.FormatZero("0000")
        End If

        Return New protein With {
            .Identifier = geneID,
            .Properties = annotations
        }
    End Function

    <Extension>
    Public Iterator Function GenerateAnnotations(genes As IEnumerable(Of protein),
                                                 mappings As Dictionary(Of String, String()),
                                                 uniprot As Dictionary(Of String, Uniprot.XML.entry),
                                                 fields$(),
                                                 Optional [where] As Func(Of protein, Boolean) = Nothing,
                                                 Optional prefix$ = "",
                                                 Optional deli As Char = ";"c) As IEnumerable(Of protein)
        If where Is Nothing Then
            where = Function(prot) True
        End If

        For Each gene As SeqValue(Of protein) In genes.SeqIterator
            Dim list$() = (+gene).Identifier.Split(deli)
            Dim i As Integer = gene.i + 1
            Dim annotations As New Dictionary(Of String, String)
            Dim g As protein = (+gene)

            If False = where(+gene) Then
                Continue For
            Else
                For Each key In fields
                    Call annotations.Add(key, g(key))
                Next
            End If

            Yield list.__applyInternal(
                annotations, mappings, uniprot, prefix, i)
        Next
    End Function

    ''' <summary>
    ''' 不筛选出DEGs，直接导出注释数据
    ''' </summary>
    ''' <param name="files"></param>
    ''' <param name="idMapping$"></param>
    ''' <param name="uniprotXML$"></param>
    ''' <param name="idlistField$"></param>
    ''' <param name="prefix$"></param>
    ''' <param name="deli"></param>
    <Extension>
    Public Sub ApplyAnnotations(files As IEnumerable(Of String), idMapping$, uniprotXML$, idlistField$, Optional prefix$ = "", Optional deli As Char = ";"c)
        Call files.__apply(False, idMapping, uniprotXML, idlistField, prefix, deli)
    End Sub

    <Extension>
    Private Sub __apply(files As IEnumerable(Of String), DEGsMode As Boolean, idMapping$, uniprotXML$, idlistField$, Optional prefix$ = "", Optional deli As Char = ";"c)
        Dim mappings As Dictionary(Of String, String()) = Retrieve_IDmapping.MappingReader(idMapping)
        Dim uniprot As Dictionary(Of String, Uniprot.XML.entry) =
            SMRUCC.genomics.Assembly.Uniprot.XML.UniprotXML _
            .Load(uniprotXML) _
            .entries _
            .GroupBy(Function(x) x.accession) _
            .ToDictionary(Function(x) x.Key,
                          Function(x) x.First)
        Dim edgeRfields$() = {"logFC", "logCPM", "F", "PValue"}
        Dim suffix$ = If(DEGsMode, "-DEGs-annotations.csv", "-proteins-annotations.csv")
        Dim __where As Func(Of protein, Boolean)

        If DEGsMode Then
            __where = Function(gene) Math.Abs(gene("logFC").ParseNumeric) >= 1
        Else
            __where = Nothing
        End If

        For Each file As String In files
            Dim proteins = protein.LoadDataSet(file, uidMap:=idlistField)
            Dim DEPs = proteins.GenerateAnnotations(
                mappings, uniprot, edgeRfields,
                where:=__where,
                prefix:=prefix,
                deli:=deli).ToArray
            Dim out$ = file.ParentPath & "/" & file.ParentDirName & "-" & file.BaseName & suffix

            Call DEPs.SaveDataSet(out,, "geneID")
        Next
    End Sub

    ''' <summary>
    ''' 处理蛋白组数据的函数
    ''' </summary>
    ''' <param name="files">edgeR DEGs结果</param>
    ''' <param name="idMapping$"></param>
    ''' <param name="uniprotXML$"></param>
    ''' <param name="idlistField$">读取质谱结果的标号域的标签列表</param>
    ''' <param name="prefix$"></param>
    ''' <param name="deli"></param>
    <Extension>
    Public Sub ApplyDEPsAnnotations(files As IEnumerable(Of String), idMapping$, uniprotXML$, idlistField$, Optional prefix$ = "", Optional deli As Char = ";"c)
        Call files.__apply(True, idMapping, uniprotXML, idlistField, prefix, deli)
    End Sub

    <Extension>
    Public Function LoadSample(path$) As protein()
        Return protein.LoadDataSet(path).ToArray
    End Function

    <Extension>
    Public Function GetKOlist(proteins As IEnumerable(Of protein), Optional KO$ = "KO") As String()
        Dim list As String() = proteins _
            .Where(Function(x) x.HasProperty(KO)) _
            .Select(Function(x) x(KO).Split(";"c)) _
            .Unlist _
            .Select(AddressOf Trim) _
            .Distinct _
            .Where(Function(s) Not String.IsNullOrEmpty(s)) _
            .SeqIterator _
            .Select(Function(k) $"gene{k.i + 1}{ASCII.TAB}{+k}") _
            .ToArray
        Return list
    End Function

    Public Sub ExportKOList(DIR$, Optional KO$ = "KO")
        For Each file As String In ls - l - r - "*.csv" <= DIR
            Call file.LoadSample _
                .GetKOlist(KO) _
                .SaveTo(file.TrimSuffix & "-KO.txt")
        Next
    End Sub

    Public Sub ExportColorDEGs(DIR$, Optional KO$ = "KO")
        For Each file As String In ls - l - r - "*.csv" <= DIR
            Call file.LoadSample _
                .DEGsPathwayMap() _
                .SaveTo(file.TrimSuffix & "-DEGs-KO.txt")
        Next
    End Sub
End Module
