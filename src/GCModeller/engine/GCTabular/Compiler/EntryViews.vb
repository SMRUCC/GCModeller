Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic
Imports LANS.SystemsBiology.Assembly
Imports LANS.SystemsBiology.DatabaseServices.SabiorkKineticLaws.TabularDump

Namespace Compiler

    Friend Class EntryViews

        Dim _KEGG_Compounds_EntryView As List(Of KeyValuePair(Of String, FileStream.Metabolite))
        Dim _CheBI_EntryView As List(Of KeyValuePair(Of String, FileStream.Metabolite))
        Dim _PubChem_EntryView As List(Of KeyValuePair(Of String, FileStream.Metabolite))
        Dim _OriginalList As List(Of FileStream.Metabolite)
        Dim _Index As List(Of String)

        Sub New(Metabolites As List(Of FileStream.Metabolite))
            If Metabolites.IsNullOrEmpty Then
                Throw New DataException("[ERROR] There is no metabolites in the input source!!!")
            Else
                Call Console.WriteLine("There is {0} metabolites in the input source.", Metabolites.Count)
            End If
            _KEGG_Compounds_EntryView = CreateKEGGCompoundsEntryView(Metabolites)
            _CheBI_EntryView = CreateCheBIEntryView(Metabolites)
            _PubChem_EntryView = CreatePubChemEntryView(Metabolites)
            _OriginalList = Metabolites
            _Index = (From Metabolite In Metabolites Select Metabolite.Identifier).ToList
        End Sub

        Public Function GetByUnique(UniqueId As String) As FileStream.Metabolite
            Return _OriginalList.GetItem(UniqueId)
        End Function

        Public Function Exists(UniqueId As String) As Boolean
            Return _Index.IndexOf(UniqueId) > -1
        End Function

        Public Sub AddEntry(item As LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Compound)
            Dim Metabolite As FileStream.Metabolite = New FileStream.Metabolite With {.Identifier = item.Entry, .InitialAmount = 1000, .CommonNames = item.CommonNames}
            Dim PubChem As String = If(String.IsNullOrEmpty(item.PUBCHEM), "", New MetaCyc.Schema.DBLinkManager.DBLink() With {.DBName = "PubChem", .AccessionId = item.PUBCHEM}.GetFormatValue)
            Dim CheBI As String() = If(item.CHEBI.IsNullOrEmpty, New String() {}, (From strValue As String In item.CHEBI Select New MetaCyc.Schema.DBLinkManager.DBLink() With {.DBName = "CheBI", .AccessionId = strValue}.GetFormatValue).ToArray)
            Dim DBLinks As List(Of String) = New List(Of String)
            Call DBLinks.Add(New MetaCyc.Schema.DBLinkManager.DBLink() With {.DBName = "KEGG.Compound", .AccessionId = item.Entry}.GetFormatValue)
            Call DBLinks.Add(PubChem)
            Call DBLinks.AddRange(CheBI)
            '     Metabolite.DBLinks = (From strValue As String In DBLinks Where Not String.IsNullOrEmpty(strValue) Select strValue Distinct).ToArray
            Call Update(Metabolite, item.CHEBI, item.Entry, item.PUBCHEM, "")
            Call _OriginalList.Add(Metabolite)
            Call _Index.Add(Metabolite.Identifier)
        End Sub

        Public Sub AddEntry(Metabolite As FileStream.Metabolite)
            If Exists(UniqueId:=Metabolite.Identifier) Then
                Return
            End If

            Dim PubChem As String = If(String.IsNullOrEmpty(Metabolite.PUBCHEM), "", New MetaCyc.Schema.DBLinkManager.DBLink() With {.DBName = "PubChem", .AccessionId = Metabolite.PUBCHEM}.GetFormatValue)
            Dim CheBI As String() = If(Metabolite.ChEBI.IsNullOrEmpty, New String() {}, (From strValue As String In Metabolite.ChEBI Select New MetaCyc.Schema.DBLinkManager.DBLink() With {.DBName = "CheBI", .AccessionId = strValue}.GetFormatValue).ToArray)
            Dim DBLinks As List(Of String) = New List(Of String)
            If Not String.IsNullOrEmpty(Metabolite.KEGGCompound) Then
                Call DBLinks.Add(New MetaCyc.Schema.DBLinkManager.DBLink() With {.DBName = "KEGG.Compound", .AccessionId = Metabolite.KEGGCompound}.GetFormatValue)
            End If
            Call DBLinks.Add(PubChem)
            Call DBLinks.AddRange(CheBI)
            '     Metabolite.DBLinks = (From strValue As String In DBLinks Where Not String.IsNullOrEmpty(strValue) Select strValue Distinct).ToArray
            Call Update(Metabolite, Metabolite.ChEBI, Metabolite.KEGGCompound, Metabolite.PUBCHEM, "")
            Call _OriginalList.Add(Metabolite)
            Call _Index.Add(Metabolite.Identifier)
        End Sub

        Public Sub Update(ByRef Compound As FileStream.Metabolite, NewValue As LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Compound)
            Dim List = Compound.CommonNames.ToList
            Call List.AddRange(NewValue.CommonNames)
            Compound.CommonNames = (From strValue As String In List Where Not String.IsNullOrEmpty(strValue) Select strValue Distinct Order By strValue Ascending).ToArray
            Call Update(Compound, NewValue.CHEBI, NewValue.Entry, NewValue.PUBCHEM, "")
        End Sub

        ''' <summary>
        ''' 在这里假设所添加的目标对象必须是在本列表之中不会存在的对象
        ''' </summary>
        ''' <param name="item"></param>
        ''' <remarks></remarks>
        Public Sub AddEntry(item As CompoundSpecie)
            Dim Metabolite As FileStream.Metabolite = New FileStream.Metabolite With {.Identifier = item.SabiorkId, .InitialAmount = 1000, .CommonNames = item.CommonNames}
            Dim KEGG_Compound As String = If(String.IsNullOrEmpty(item.KEGG_Compound), "", New MetaCyc.Schema.DBLinkManager.DBLink() With {.DBName = "KEGG.Compound", .AccessionId = item.KEGG_Compound}.GetFormatValue)
            Dim PubChem As String = If(String.IsNullOrEmpty(item.PUBCHEM), "", New MetaCyc.Schema.DBLinkManager.DBLink() With {.DBName = "PubChem", .AccessionId = item.PUBCHEM}.GetFormatValue)
            Dim CheBI As String() = If(item.ICompoundObjectCHEBI_values.IsNullOrEmpty, New String() {}, (From strValue As String In item.ICompoundObjectCHEBI_values Select New MetaCyc.Schema.DBLinkManager.DBLink() With {.DBName = "CheBI", .AccessionId = strValue}.GetFormatValue).ToArray)
            Dim DBLinks As List(Of String) = New List(Of String)
            Call DBLinks.Add(KEGG_Compound)
            Call DBLinks.Add(PubChem)
            Call DBLinks.AddRange(CheBI)
            '    Metabolite.DBLinks = (From strValue As String In DBLinks Where Not String.IsNullOrEmpty(strValue) Select strValue Distinct).ToArray
            Call Update(Metabolite, item.ICompoundObjectCHEBI_values, item.KEGG_Compound, item.PUBCHEM, item.SabiorkId)
            Call _OriginalList.Add(Metabolite)
            Call _Index.Add(Metabolite.Identifier)
        End Sub

        Public Sub Update(ByRef Compound As FileStream.Metabolite, NewValue As CompoundSpecie)
            Dim List = Compound.CommonNames.ToList
            Call List.AddRange(NewValue.CommonNames)
            Compound.CommonNames = (From strValue As String In List Where Not String.IsNullOrEmpty(strValue) Select strValue Distinct Order By strValue Ascending).ToArray
            Call Update(Compound, NewValue.ICompoundObjectCHEBI_values, NewValue.KEGG_Compound, NewValue.PUBCHEM, NewValue.SabiorkId)
        End Sub

        Private Sub Update(ByRef Compound As FileStream.Metabolite, CheBI As String(), KEGG_Compound As String, PubChem As String, SabiorkId As String)
            For Each Entry As String In CheBI
                If GetItem(Me._CheBI_EntryView, Entry) Is Nothing Then
                    '   Call Compound.AddDBLinkEntry("CheBI", Entry)
                    Call Me._CheBI_EntryView.Add(New KeyValuePair(Of String, FileStream.Metabolite)(Entry, Compound))
                End If
            Next
            If Not String.IsNullOrEmpty(KEGG_Compound) AndAlso GetItem(Me._KEGG_Compounds_EntryView, KEGG_Compound) Is Nothing Then
                '    Call Compound.AddDBLinkEntry("KEGG.Compound", KEGG_Compound)
                Call Me._KEGG_Compounds_EntryView.Add(New KeyValuePair(Of String, FileStream.Metabolite)(KEGG_Compound, Compound))
            End If
            If Not String.IsNullOrEmpty(PubChem) AndAlso GetItem(Me._PubChem_EntryView, PubChem) Is Nothing Then
                '    Call Compound.AddDBLinkEntry("PubChem", PubChem)
                Call Me._PubChem_EntryView.Add(New KeyValuePair(Of String, FileStream.Metabolite)(PubChem, Compound))
            End If
            ' If Not String.IsNullOrEmpty(SabiorkId) Then Call Compound.AddDBLinkEntry("Sabio-rk", SabiorkId)
        End Sub

        Public Function GetByKeggEntry(EntryId As String) As FileStream.Metabolite
            Return GetItem(_KEGG_Compounds_EntryView, EntryId)
        End Function

        Public Function GetByPubChemEntry(EntryId As String) As FileStream.Metabolite
            Return GetItem(_PubChem_EntryView, EntryId)
        End Function

        Public Function GetByCheBIEntry(EntryIdList As String()) As FileStream.Metabolite
            If EntryIdList.IsNullOrEmpty Then
                Return Nothing
            End If
            For Each EntryId As String In EntryIdList
                Dim item = GetItem(_CheBI_EntryView, EntryId)
                If Not item Is Nothing Then
                    Return item
                End If
            Next
            Return Nothing
        End Function

        Private Shared Function GetItem(Collection As List(Of KeyValuePair(Of String, FileStream.Metabolite)), EntryId As String) As FileStream.Metabolite
            Dim LQuery = (From item In Collection Where String.Equals(item.Key, EntryId, StringComparison.OrdinalIgnoreCase) Select item).ToArray
            If Not LQuery.IsNullOrEmpty Then
                Return LQuery.First.Value
            Else
                Return Nothing
            End If
        End Function

        Private Shared Function CreateKEGGCompoundsEntryView(Metabolites As Generic.IEnumerable(Of FileStream.Metabolite)) As List(Of KeyValuePair(Of String, FileStream.Metabolite))
            Dim LQuery = (From cpd As FileStream.Metabolite In Metabolites
                          Let KEGG_compoundId As String = cpd.KEGGCompound
                          Where Not String.IsNullOrEmpty(KEGG_compoundId)
                          Let item = New KeyValuePair(Of String, FileStream.Metabolite)(KEGG_compoundId, cpd)
                          Select item
                          Order By item.Key Ascending).ToList
            Return LQuery
        End Function

        Private Shared Function CreateCheBIEntryView(Metabolites As Generic.IEnumerable(Of FileStream.Metabolite)) As List(Of KeyValuePair(Of String, FileStream.Metabolite))
            Dim LQuery = (From cpd As FileStream.Metabolite In Metabolites Let CheBIs As String() = cpd.ChEBI Where Not CheBIs.IsNullOrEmpty Select New KeyValuePair(Of String(), FileStream.Metabolite)(CheBIs, cpd)).ToArray
            Dim List As List(Of KeyValuePair(Of String, FileStream.Metabolite)) = New List(Of KeyValuePair(Of String, FileStream.Metabolite))
            For Each Line In LQuery
                Call List.AddRange((From EntryId As String In Line.Key Select New KeyValuePair(Of String, FileStream.Metabolite)(EntryId, Line.Value)).ToArray)
            Next
            Return List
        End Function

        Private Shared Function CreatePubChemEntryView(Metabolites As Generic.IEnumerable(Of FileStream.Metabolite)) As List(Of KeyValuePair(Of String, FileStream.Metabolite))
            Dim LQuery = (From cpd As FileStream.Metabolite In Metabolites
                          Let pubchem As String = cpd.PUBCHEM
                          Where Not String.IsNullOrEmpty(pubchem)
                          Let item = New KeyValuePair(Of String, FileStream.Metabolite)(pubchem, cpd)
                          Select item
                          Order By item.Key Ascending).ToList
            Return LQuery
        End Function
    End Class
End Namespace