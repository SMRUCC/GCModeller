Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Linq
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
            Dim mappsId$() = list _
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
            Dim annotations As New Dictionary(Of String, String)
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

            Dim geneID As String = Idtags.i + 1

            If Not String.IsNullOrEmpty(prefix) Then
                geneID = prefix & "_" & geneID.FormatZero("0000")
            End If

            Yield New protein With {
                .Identifier = geneID,
                .Properties = annotations
            }
        Next
    End Function

    <Extension>
    Public Function LoadSample(path$) As protein()
        Return protein.LoadDataSet(path).ToArray
    End Function
End Module
