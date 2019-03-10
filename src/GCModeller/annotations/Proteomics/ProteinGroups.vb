﻿#Region "Microsoft.VisualBasic::2d960b35c06262358fb44e396ffb5460, annotations\Proteomics\ProteinGroups.vb"

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

    ' Module ProteinGroups
    ' 
    '     Function: __applyInternal, ExportAnnotations, (+5 Overloads) GenerateAnnotations, GetKOlist, GetProteinIds
    '               LoadSample, Term2Locus
    ' 
    '     Sub: __apply, (+2 Overloads) ApplyAnnotations, ApplyDEPsAnnotations, ExportColorDEGs, ExportKOList
    '          GetProteinDefs
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.KEGG
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.Uniprot.Web
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports protein = Microsoft.VisualBasic.Data.csv.IO.EntityObject
Imports uniprotProteomics = SMRUCC.genomics.Assembly.Uniprot.XML.UniProtXML
Imports csv = Microsoft.VisualBasic.Data.csv.IO.File
Imports Xlsx = Microsoft.VisualBasic.MIME.Office.Excel.File

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
                                        Optional mappings As Dictionary(Of String, String()) = Nothing) As IEnumerable(Of (protein, String()))
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
                                        Optional scientifcName$ = Nothing) As IEnumerable(Of (protein, String()))

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
                                                 Optional accID As Boolean = False) As IEnumerable(Of (protein, String()))

        If uniprotXML.FileLength > 1024 * 1024 * 1024L Then
            ' ultra large size mode
            Dim idlist As Index(Of String) = ID.Indexing
            Dim seq As VBInteger = 0

            For Each protein As Uniprot.XML.entry In uniprotProteomics.EnumerateEntries(uniprotXML)
                If protein.accessions.Any(Function(acc) acc.IsOneOfA(idlist)) Then
                    Dim uniprot As Dictionary(Of Uniprot.XML.entry) = protein.ShadowCopy.ToDictionary
                    Dim list$() = protein.accessions
                    Dim geneID$

                    If accID Then
                        geneID = list.Where(Function(acc) acc.IsOneOfA(idlist)).First
                    Else
                        geneID = ++seq
                    End If

                    Yield list.__applyInternal(
                        New Dictionary(Of String, String),
                        mappings, uniprot, prefix, geneID,
                        scientifcName,
                        iTraq)
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
                    New Dictionary(Of String, String),
                    mappings, uniprot, prefix, geneID,
                    scientifcName,
                    iTraq)
            Next
        End If
    End Function

    <Extension>
    Public Iterator Function GenerateAnnotations(proteinGroups As IEnumerable(Of Perseus),
                                                 uniprot As Dictionary(Of Uniprot.XML.entry),
                                                 Optional prefix$ = "",
                                                 Optional scientifcName$ = Nothing,
                                                 Optional iTraq As Boolean = False,
                                                 Optional accID As Boolean = False) As IEnumerable(Of (protein, String()))
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
                New Dictionary(Of String, String),
                mappings, uniprot, prefix, i,
                scientifcName,
                iTraq)
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
                                               Optional accID As Boolean = False) As IEnumerable(Of (protein, String()))

        Dim uniprot As Dictionary(Of Uniprot.XML.entry) = uniprotProteomics.LoadDictionary(uniprotXML)
        Dim ID As IEnumerable(Of String) = uniprot.Keys
        Dim geneID$
        Dim mappings As Dictionary(Of String, String()) =
            ID.ToDictionary(
            Function(s) s,
            Function(s) {s})

        For Each Idtags As SeqValue(Of String) In ID.SeqIterator
            Dim list$() = (+Idtags).Split(deli)
            Dim i As Integer = Idtags.i + 1

            If accID Then
                geneID = +Idtags
            Else
                geneID = i
            End If

            Yield list.__applyInternal(
                New Dictionary(Of String, String),
                mappings, uniprot, prefix, geneID,
                scientifcName,
                iTraq)
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
                                     annotations As Dictionary(Of String, String),
                                     mappings As Dictionary(Of String, String()),
                                     uniprot As Dictionary(Of Uniprot.XML.entry),
                                     prefix$, geneID$,
                                     scientifcName$,
                                     iTraq As Boolean) As (protein As protein, mapsId As String())

        Dim maps = source _
            .Select(Function(s) UCase(s)) _
            .Where(Function(ref) mappings.ContainsKey(ref)) _
            .Select(Function(ref) (ref:=ref, ID:=mappings(ref))) _
            .ToArray  ' 从uniprot ref90 转换为标准基因号
        Dim mapsID$() = maps.Select(Function(t) t.ID).IteratesALL.Distinct.ToArray
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
                    uniprots = {protein}  ' 已经找到了目标物种的蛋白注释了，则会抛弃掉其他物种的蛋白注释
                    mapsID = {
                        DirectCast(protein, INamedValue).Key
                    }
                    found = True
#If DEBUG Then
                    Call $"[{protein.organism.scientificName}] {protein.name}".__DEBUG_ECHO
#End If
                    Exit For
                End If
            Next   ' 假若找不到，才会使用其他的物种的注释

            If Not found Then
                For j As Integer = 0 To uniprots.Length - 1
                    If uniprots(j).Xrefs.ContainsKey("KO") Then
                        uniprots = {uniprots(j)}  ' 假若使用指定的物种的话，对于其他的找不到的基因，也只用一个基因，否则做富集的时候会出问题
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
                        Dim pfam$ = u.features _
                            .SafeQuery _
                            .Where(Function(f) f.type = "domain") _
                            .Select(Function(d)
                                        Return $"{d.description}({d.location.begin.position}|{d.location.end.position})"
                                    End Function) _
                            .JoinBy("+")
                        Return (ID:=u.accessions.JoinBy("|"), s:=pfam)
                    End Function) _
            .Where(Function(s) Not s.s.StringEmpty) _
            .Select(Function(u) $"{u.ID}:{u.s}") _
            .JoinBy("; ")

        Call annotations.Add("geneName", geneNames)
        Call annotations.Add("ORF", ORF)
        Call annotations.Add("Entrez", Entrez.JoinBy("; "))
        Call annotations.Add("fullName", names.JoinBy("; "))
        Call annotations.Add("uniprot", mapsID.JoinBy("; "))
        Call annotations.Add("GO", GO.JoinBy("; "))
        Call annotations.Add("EC", EC.JoinBy("; "))
        Call annotations.Add("KO", KO.JoinBy("; "))
        Call annotations.Add("pfam", pfamString)
        Call annotations.Add("organism", orgNames)

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
            geneID = annotations("uniprot")
        End If

        Return (New protein With {
            .ID = geneID,
            .Properties = annotations
        }, mapsID)
    End Function

    <Extension>
    Public Iterator Function GenerateAnnotations(genes As IEnumerable(Of protein),
                                                 mappings As Dictionary(Of String, String()),
                                                 uniprot As Dictionary(Of Uniprot.XML.entry),
                                                 fields$(),
                                                 Optional [where] As Func(Of protein, Boolean) = Nothing,
                                                 Optional prefix$ = "",
                                                 Optional deli As Char = ";"c,
                                                 Optional geneList As List(Of String) = Nothing,
                                                 Optional scientifcName$ = Nothing,
                                                 Optional iTraq As Boolean = False) As IEnumerable(Of protein)
        If where Is Nothing Then
            where = Function(prot) True
        End If

        For Each gene As SeqValue(Of protein) In genes.SeqIterator
            Dim list$() = (+gene).ID.Split(deli)
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
    Public Sub ApplyAnnotations(files As IEnumerable(Of String),
                                idMapping$, uniprotXML$, idlistField$,
                                Optional prefix$ = "",
                                Optional deli As Char = ";"c,
                                Optional ByRef geneList$() = Nothing)
        Call files.__apply(False, idMapping, uniprotXML, idlistField, prefix, deli, geneList)
    End Sub

    <Extension>
    Private Sub __apply(files As IEnumerable(Of String),
                        DEGsMode As Boolean,
                        idMapping$,
                        uniprotXML$,
                        idlistField$,
                        prefix$,
                        deli As Char,
                        ByRef geneList$())

        Dim mappings As Dictionary(Of String, String()) = Retrieve_IDmapping.MappingReader(idMapping)
        Dim uniprot As Dictionary(Of Uniprot.XML.entry) = SMRUCC.genomics.Assembly.Uniprot.XML.UniProtXML.LoadDictionary(uniprotXML)
        Dim edgeRfields$() = {"logFC", "logCPM", "F", "PValue"}
        Dim suffix$ = If(DEGsMode, "-DEGs-annotations.csv", "-proteins-annotations.csv")
        Dim __where As Func(Of protein, Boolean)
        Dim diffCut = Math.Log(1.5, 2)  ' 蛋白质只需要1.5倍，mRNA才需要2倍

        If DEGsMode Then
            __where = Function(gene) Math.Abs(gene("logFC").ParseNumeric) >= diffCut
        Else
            __where = Nothing
        End If

        Dim list As New List(Of String)
        Dim outList As New List(Of String)

        For Each file As String In files
            Dim proteins = protein.LoadDataSet(file, uidMap:=idlistField)
            Dim DEPs = proteins.GenerateAnnotations(
                mappings, uniprot, edgeRfields,
                where:=__where,
                prefix:=prefix,
                deli:=deli,
                geneList:=list).ToArray
            Dim out$ = file.ParentPath & "/" & file.ParentDirName & "-" & file.BaseName & suffix

            Call DEPs.SaveDataSet(out,, "geneID")
            Call list.SaveTo(out.TrimSuffix & "-uniprot.txt")
            Call outList.AddRange(list)
            Call list.Clear()
        Next

        geneList = list.Distinct.ToArray
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
    Public Sub ApplyDEPsAnnotations(files As IEnumerable(Of String),
                                    idMapping$, uniprotXML$, idlistField$,
                                    Optional prefix$ = "",
                                    Optional deli As Char = ";"c,
                                    Optional ByRef geneList$() = Nothing)

        Call files.__apply(True, idMapping, uniprotXML, idlistField, prefix, deli, geneList)
    End Sub

    ''' <summary>
    ''' <see cref="protein.LoadDataSet"/>的快捷方式
    ''' </summary>
    ''' <param name="path$"></param>
    ''' <returns></returns>
    <Extension>
    Public Function LoadSample(path$, Optional geneIDField$ = Nothing, Optional sheetName$ = "Sheet1") As protein()
        Select Case path.ExtensionSuffix.ToLower
            Case "csv"

                Return protein _
                    .LoadDataSet(path, uidMap:=geneIDField) _
                    .ToArray

            Case "xlsx"

                Dim csv As csv = Xlsx.Open(path).GetTable(sheetName)
                Dim out As protein() = protein _
                    .LoadDataSet(Of protein)(stream:=csv) _
                    .ToArray

                Return out

            Case Else

                Throw New NotSupportedException("File type with suffix: " & path.ExtensionSuffix & " is not support!")
        End Select
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

    <Extension>
    Public Function Term2Locus(proteins As IEnumerable(Of protein), field$, Optional deli$ = Nothing, Optional oneTag As Boolean = False) As NamedValue(Of String)()
        proteins = proteins.Where(Function(x) Not x(field).StringEmpty)

        If deli Is Nothing Then
            Return proteins _
                .Select(Function(x) New NamedValue(Of String)(x(field), x.ID)) _
                .ToArray
        Else
            Dim out As New List(Of NamedValue(Of String))

            For Each prot As protein In proteins
                Dim tags$() = Strings.Split(prot(field), deli) _
                    .Select(AddressOf Trim) _
                    .ToArray

                For Each tag$ In tags
                    out += New NamedValue(Of String) With {
                        .Name = tag,
                        .Value = prot.ID
                    }

                    If oneTag Then
                        Exit For
                    End If
                Next
            Next

            Return out
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="files"></param>
    ''' <param name="geneID$">基因ID所在的列名</param>
    ''' <param name="annotation"></param>
    ''' 
    <Extension>
    Public Sub ApplyAnnotations(files As IEnumerable(Of String), geneID$, fields$(), getAnnotations$(), annotation As Dictionary(Of protein))
        For Each file As String In files
            Dim genes = file.LoadSample(geneID)
            Dim ALL As New List(Of protein)
            Dim DEPs As New List(Of protein)

            For Each gene As protein In genes
                Dim annotations As New Dictionary(Of String, String)

                For Each field In fields
                    annotations.Add(field, gene(field))
                Next

                With annotation(gene.ID)
                    For Each k As String In getAnnotations
                        Call annotations.Add(k, .ItemValue(k))
                    Next
                End With

                ALL += New protein With {
                    .ID = gene.ID,
                    .Properties = annotations
                }

                Dim logFC = Math.Abs(gene("logFC").ParseNumeric)
                ' Dim Pvalue As Double = gene("PValue").ParseNumeric

                If logFC >= Math.Log(1.5, 2) Then 'AndAlso Pvalue <= 0.05 Then
                    DEPs += ALL.Last
                End If
            Next

            Call ALL.SaveDataSet(file.ParentPath & "/" & file.ParentDirName & "-" & file.BaseName & "-proteins-annotations.csv",, "geneID")
            Call DEPs.SaveDataSet(file.ParentPath & "/" & file.ParentDirName & "-" & file.BaseName & "-DEPs-annotations.csv",, "geneID")
        Next
    End Sub
End Module
