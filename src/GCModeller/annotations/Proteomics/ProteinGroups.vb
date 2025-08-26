#Region "Microsoft.VisualBasic::1768c3c1bd794933e197d1a8f032c81d, annotations\Proteomics\ProteinGroups.vb"

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

    '   Total Lines: 471
    '    Code Lines: 356 (75.58%)
    ' Comment Lines: 60 (12.74%)
    '    - Xml Docs: 63.33%
    ' 
    '   Blank Lines: 55 (11.68%)
    '     File Size: 19.81 KB


    ' Module ProteinGroups
    ' 
    '     Function: __applyInternal, ExportAnnotations, (+5 Overloads) GenerateAnnotations, GetKOlist, GetProteinIds
    '               LoadSample
    ' 
    '     Sub: GetProteinDefs
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Annotation
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.Uniprot.Web
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports csv = Microsoft.VisualBasic.Data.Framework.IO.File
Imports uniprotProteomics = SMRUCC.genomics.Assembly.Uniprot.XML.UniProtXML
Imports Xlsx = Microsoft.VisualBasic.MIME.Office.Excel.XLSX.File

''' <summary>
''' Label Free/iTraq/TMT结果注释数据处理
''' </summary>
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
        Dim gz$ = If(save.StringEmpty, path.TrimSuffix & ".xml.gz", save)

        Call Retrieve_IDmapping.Mapping(idData, ID_types.NF90, ID_types.ACC_ID, gz)
        Call idData.SaveTo(gz.TrimSuffix.TrimSuffix & "-proteins.txt")
    End Sub

    ''' <summary>
    ''' 不需要进行mapping
    ''' </summary>
    ''' <param name="ID">直接是uniprot编号</param>
    ''' <param name="uniprotXML$"></param>
    ''' <param name="iTraq">
    ''' 会直接使用原来的<paramref name="ID"/>编号来代替后面的系统自动生成的数字编号
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function GenerateAnnotations(ID As IEnumerable(Of String),
                                        uniprotXML$,
                                        Optional iTraq As Boolean = False,
                                        Optional accID As Boolean = False,
                                        Optional mappings As Dictionary(Of String, String()) = Nothing) As IEnumerable(Of (AnnotationTable, String()))
        Dim list$() = ID.ToArray
        Dim prefix$

        If mappings.IsNullOrEmpty Then
            prefix = "uniprot"
        Else
            prefix = ""
        End If
        If mappings.IsNullOrEmpty Then
            mappings = list.ToDictionary(
                Function(x) x,
                Function(x) {x})
        End If

        Return list.GenerateAnnotations(mappings, uniprotXML, prefix,, iTraq:=iTraq, accID:=accID)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="ID">
    ''' 对于蛋白组分析而言，这里的每一个元素都是一组基因号的集合，基因号之间通过<paramref name="deli"/>来区分
    ''' </param>
    ''' <param name="idMapping$">``*.tsv``, ``*.tab``文件的文件路径</param>
    ''' <param name="uniprotXML$"></param>
    ''' <param name="deli"></param>
    ''' <returns></returns>
    <Extension>
    Public Function GenerateAnnotations(ID As IEnumerable(Of String),
                                        idMapping$,
                                        uniprotXML$,
                                        Optional prefix$ = "",
                                        Optional deli As Char = ";"c,
                                        Optional scientifcName$ = Nothing) As IEnumerable(Of (AnnotationTable, String()))

        Dim mappings As Dictionary(Of String, String()) = Retrieve_IDmapping.MappingReader(idMapping)
        Return ID.GenerateAnnotations(mappings, uniprotXML, prefix, deli, scientifcName)
    End Function

    <Extension>
    Public Iterator Function GenerateAnnotations(ID As IEnumerable(Of String),
                                                 mappings As Dictionary(Of String, String()),
                                                 uniprotXML$,
                                                 Optional prefix$ = "",
                                                 Optional deli As Char = ";"c,
                                                 Optional scientifcName$ = Nothing,
                                                 Optional iTraq As Boolean = False,
                                                 Optional accID As Boolean = False) As IEnumerable(Of (AnnotationTable, String()))

        If uniprotXML.FileLength > 1024 * 1024 * 1024L Then
            ' ultra large size mode
            Dim idlist As Index(Of String) = ID.Indexing
            Dim seq As i32 = 0

            For Each protein As Uniprot.XML.entry In uniprotProteomics.EnumerateEntries(uniprotXML)
                If protein.accessions.Any(Function(acc) acc Like idlist) Then
                    Dim uniprot As Dictionary(Of Uniprot.XML.entry) = protein.ShadowCopy.ToDictionary
                    Dim list$() = protein.accessions
                    Dim geneID$

                    If accID Then
                        geneID = list.Where(Function(acc) acc Like idlist).First
                    Else
                        geneID = ++seq
                    End If

                    Yield list.__applyInternal(
                        New AnnotationTable,
                        mappings, uniprot, prefix, geneID,
                        scientifcName,
                        iTraq
                    )
                End If
            Next
        Else
            Dim uniprot As Dictionary(Of Uniprot.XML.entry) =
                uniprotProteomics _
                .LoadDictionary(uniprotXML)
            Dim geneID$

            For Each Idtags As SeqValue(Of String) In ID.SeqIterator
                Dim list$() = (+Idtags).Split(deli)
                Dim i As Integer = Idtags.i + 1

                If accID Then
                    geneID = +Idtags
                Else
                    geneID = i
                End If

                Yield list.__applyInternal(
                    New AnnotationTable,
                    mappings, uniprot, prefix, geneID,
                    scientifcName,
                    iTraq
                )
            Next
        End If
    End Function

    <Extension>
    Public Iterator Function GenerateAnnotations(proteinGroups As IEnumerable(Of Perseus),
                                                 uniprot As Dictionary(Of Uniprot.XML.entry),
                                                 Optional prefix$ = "",
                                                 Optional scientifcName$ = Nothing,
                                                 Optional iTraq As Boolean = False,
                                                 Optional accID As Boolean = False) As IEnumerable(Of (AnnotationTable, String()))
        Dim geneID$

        For Each Idtags As SeqValue(Of Perseus) In proteinGroups.SeqIterator
            Dim list$() = (+Idtags).ProteinIDs _
                .Select(Function(id) id.Split("|"c, ":"c)(1)) _
                .Distinct _
                .ToArray
            Dim i As Integer = Idtags.i + 1
            Dim mappings = list.ToDictionary(Function(id) id, Function(id) {id})

            If accID Then
                geneID = (+Idtags).ProteinIDs.JoinBy("; ")
            Else
                geneID = i
            End If

            Yield list.__applyInternal(
                New AnnotationTable,
                mappings, uniprot, prefix, i,
                scientifcName,
                iTraq
            )
        Next
    End Function

    ''' <summary>
    ''' 导出指定的uniprot.xml文件之中的所有蛋白质的注释
    ''' </summary>
    ''' <param name="uniprotXML$"></param>
    ''' <param name="prefix$"></param>
    ''' <param name="deli"></param>
    ''' <param name="scientifcName$"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function ExportAnnotations(uniprotXML$,
                                               Optional prefix$ = "",
                                               Optional deli As Char = ";"c,
                                               Optional scientifcName$ = Nothing,
                                               Optional iTraq As Boolean = False,
                                               Optional accID As Boolean = False) As IEnumerable(Of (AnnotationTable, String()))

        Dim uniprot As Dictionary(Of Uniprot.XML.entry) = uniprotProteomics.LoadDictionary(uniprotXML)
        Dim ID As IEnumerable(Of String) = uniprot.Keys
        Dim geneID$
        Dim mappings As Dictionary(Of String, String()) = ID.ToDictionary(Function(s) s, Function(s) {s})

        For Each Idtags As SeqValue(Of String) In ID.SeqIterator
            Dim list$() = (+Idtags).Split(deli)
            Dim i As Integer = Idtags.i + 1

            If accID Then
                geneID = +Idtags
            Else
                geneID = i
            End If

            Yield list.__applyInternal(
                New AnnotationTable,
                mappings, uniprot, prefix, geneID,
                scientifcName,
                iTraq
            )
        Next
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="source">这个列表其实都是指代的一个基因</param>
    ''' <param name="annotations"></param>
    ''' <param name="mappings"></param>
    ''' <param name="uniprot"></param>
    ''' <param name="prefix$"></param>
    ''' <param name="scientifcName">假若这个参数不为空，则会优先考虑该物种的基因注释信息，假若找不到，才会找其他的基因的信息</param>
    ''' <returns></returns>
    <Extension>
    Private Function __applyInternal(source As String(),
                                     annotations As AnnotationTable,
                                     mappings As Dictionary(Of String, String()),
                                     uniprot As Dictionary(Of Uniprot.XML.entry),
                                     prefix$, geneID$,
                                     scientifcName$,
                                     iTraq As Boolean) As (protein As AnnotationTable, mapsId As String())

        Dim maps = source _
            .Select(Function(s) UCase(s)) _
            .Where(Function(ref) mappings.ContainsKey(ref)) _
            .Select(Function(ref)
                        ' 从uniprot ref90 转换为标准基因号
                        Return (ref:=ref, ID:=mappings(ref))
                    End Function) _
            .ToArray
        Dim mapsID$() = maps.Select(Function(t) t.ID) _
            .IteratesALL _
            .Distinct _
            .ToArray
        Dim refIDs = maps.Select(Function(t) t.ref).Distinct.JoinBy("; ")
        Dim uniprots As Uniprot.XML.entry() = mapsID _
            .Where(Function(acc) uniprot.ContainsKey(acc)) _
            .Select(Function(acc) uniprot(acc)) _
            .ToArray

        If Not scientifcName Is Nothing Then
            Dim found As Boolean = False

            ' 会进行优先查找筛选
            For j As Integer = 0 To uniprots.Length - 1
                Dim protein As Uniprot.XML.entry = uniprots(j)

                If Not protein.organism Is Nothing AndAlso protein.organism.scientificName = scientifcName Then
                    ' 已经找到了目标物种的蛋白注释了，则会抛弃掉其他物种的蛋白注释
                    uniprots = {protein}
                    mapsID = {
                        DirectCast(protein, INamedValue).Key
                    }
                    found = True
#If DEBUG Then
                    Call $"[{protein.organism.scientificName}] {protein.name}".debug
#End If
                    Exit For
                End If
            Next

            ' 假若找不到，才会使用其他的物种的注释
            If Not found Then
                For j As Integer = 0 To uniprots.Length - 1
                    If uniprots(j).xrefs.ContainsKey("KO") Then
                        ' 假若使用指定的物种的话，对于其他的找不到的基因，
                        ' 也只用一个基因， 否则做富集的时候会出问题
                        uniprots = {uniprots(j)}
                        Exit For
                    End If
                Next

                uniprots = {uniprots(Scan0)}
                mapsID = {
                    DirectCast(uniprots(Scan0), INamedValue).Key
                }
            End If
        End If

        Dim names = uniprots _
            .Select(Function(prot) prot.proteinFullName) _
            .Where(Function(x) Not x.StringEmpty) _
            .Distinct _
            .ToArray
        Dim geneNames = uniprots _
            .Select(Function(prot) prot.gene) _
            .Where(Function(x) Not x Is Nothing AndAlso Not x.names.IsNullOrEmpty) _
            .Select(Function(x) x.names.First.value) _
            .Where(Function(s) Not String.IsNullOrEmpty(s)) _
            .Distinct _
            .OrderBy(Function(s) Len(s)) _
            .FirstOrDefault ' 会首先使用基因名，当没有基因名才会使用基因号
        Dim getKeyValue = Function(key$)
                              Return uniprots _
                                .Where(Function(x) x.xrefs.ContainsKey(key)) _
                                .Select(Function(x) x.xrefs(key)) _
                                .Unlist _
                                .Select(Function(x) x.id) _
                                .Distinct _
                                .ToArray
                          End Function
        Dim GO As String() = getKeyValue("GO")
        Dim EC As String() = getKeyValue("EC")
        Dim KO As String() = getKeyValue("KO")
        Dim Entrez$() = getKeyValue("GeneID")
        Dim ORF$ = uniprots _
            .Select(Function(prot) prot.gene) _
            .Where(Function(x) Not x Is Nothing) _
            .Select(Function(gene) gene.ORF) _
            .Where(Function(s) Not s.IsNullOrEmpty) _
            .IteratesALL _
            .Distinct _
            .FirstOrDefault
        Dim orgNames$ = uniprots _
            .Select(Function(prot) prot.OrganismScientificName) _
            .Distinct _
            .JoinBy("; ")
        Dim pfamString = uniprots _
            .Select(Function(u)
                        Return (ID:=u.accessions.JoinBy("|"), s:=AnnotationReader.Pfam(u).JoinBy("+"))
                    End Function) _
            .Where(Function(s) Not s.s.StringEmpty) _
            .Select(Function(u) $"{u.ID}:{u.s}") _
            .JoinBy("; ")

        annotations.geneName = geneNames
        annotations.ORF = ORF
        annotations.Entrez = Entrez
        annotations.fullName = names
        annotations.uniprot = mapsID
        annotations.GO = GO
        annotations.EC = EC
        annotations.KO = KO
        annotations.pfam = pfamString
        annotations.organism = orgNames

        'getKeyValue = Function(key)
        '                  Return uniprots _
        '                    .Where(Function(x) x.CommentList.ContainsKey(key)) _
        '                    .Select(Function(x) x.CommentList(key)) _
        '                    .Unlist _
        '                    .Select(Function(x) x.text.value) _
        '                    .Distinct _
        '                    .ToArray
        '              End Function

        'Dim functions = getKeyValue("function")
        'Dim pathways = getKeyValue("pathway")

        'Call annotations.Add("functions", functions.JoinBy("; "))
        'Call annotations.Add("pathways", pathways.JoinBy("; "))      

        If Not String.IsNullOrEmpty(prefix) Then
            geneID = prefix & "_" & geneID.FormatZero("0000")
        Else
            geneID = refIDs
        End If

        If iTraq Then
            annotations.ID = annotations.uniprot.JoinBy("; ")
        End If

        Return (annotations, mapsID)
    End Function

    <Extension>
    Public Iterator Function GenerateAnnotations(genes As IEnumerable(Of AnnotationTable),
                                                 mappings As Dictionary(Of String, String()),
                                                 uniprot As Dictionary(Of Uniprot.XML.entry),
                                                 fields$(),
                                                 Optional [where] As Func(Of AnnotationTable, Boolean) = Nothing,
                                                 Optional prefix$ = "",
                                                 Optional deli As Char = ";"c,
                                                 Optional geneList As List(Of String) = Nothing,
                                                 Optional scientifcName$ = Nothing,
                                                 Optional iTraq As Boolean = False) As IEnumerable(Of AnnotationTable)
        If where Is Nothing Then
            where = Function(prot) True
        End If

        For Each gene As SeqValue(Of AnnotationTable) In genes.SeqIterator
            Dim list$() = (+gene).ID.Split(deli)
            Dim i As Integer = gene.i + 1
            Dim annotations As New AnnotationTable
            Dim g As AnnotationTable = (+gene)

            If False = where(+gene) Then
                Continue For
            Else
                For Each key In fields
                    Call annotations.Add(key, g(key))
                Next
            End If

            With list.__applyInternal(
                annotations, mappings, uniprot,
                prefix, i,
                scientifcName, iTraq)

                If Not geneList Is Nothing Then
                    Call geneList.AddRange(.mapsId)
                End If

                Yield .protein
            End With
        Next
    End Function

    <Extension>
    Public Function LoadSample(path$, Optional geneIDField$ = Nothing, Optional sheetName$ = "Sheet1") As EntityObject()
        Select Case path.ExtensionSuffix.ToLower
            Case "csv"

                Return EntityObject _
                    .LoadDataSet(path, uidMap:=geneIDField) _
                    .ToArray

            Case "xlsx"

                Dim csv As csv = Xlsx.Open(path).GetTable(sheetName)
                Dim out As EntityObject() = EntityObject _
                    .LoadDataSet(Of EntityObject)(stream:=csv) _
                    .ToArray

                Return out

            Case Else

                Throw New NotSupportedException("File type with suffix: " & path.ExtensionSuffix & " is not support!")
        End Select
    End Function

    <Extension>
    Public Function GetKOlist(proteins As IEnumerable(Of AnnotationTable)) As String()
        Dim list As String() = proteins _
            .Where(Function(x) Not x.KO.IsNullOrEmpty) _
            .Select(Function(x) x.KO) _
            .Unlist _
            .Select(AddressOf Strings.Trim) _
            .Distinct _
            .Where(Function(s) Not String.IsNullOrEmpty(s)) _
            .SeqIterator _
            .Select(Function(k) $"gene{k.i + 1}{ASCII.TAB}{+k}") _
            .ToArray
        Return list
    End Function
End Module
