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

    <Extension>
    Public Iterator Function GenerateAnnotations(ID As IEnumerable(Of String), idMapping$, uniprotXML$, Optional deli As Char = ";"c) As IEnumerable(Of protein)
        Dim mappings As Dictionary(Of String, String()) = Retrieve_IDmapping.MappingReader(idMapping)
        Dim uniprot As Dictionary(Of String, Uniprot.XML.entry) = SMRUCC.genomics.Assembly.Uniprot.XML.UniprotXML _
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

            Call annotations.Add("name", names.JoinBy("; "))
            Call annotations.Add("GO", GO.JoinBy("; "))
            Call annotations.Add("EC", EC.JoinBy("; "))
            Call annotations.Add("KO", KO.JoinBy("; "))

            Yield New protein With {
                .Identifier = Idtags.i + 1,
                .Properties = annotations
            }
        Next
    End Function

    <Extension>
    Public Function LoadSample(path$) As protein()
        Return protein.LoadDataSet(path).ToArray
    End Function
End Module
