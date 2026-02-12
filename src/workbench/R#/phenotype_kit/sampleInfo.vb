#Region "Microsoft.VisualBasic::0185baf0eb545a62786d123cb9070a57, R#\phenotype_kit\sampleInfo.vb"

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

'   Total Lines: 427
'    Code Lines: 308 (72.13%)
' Comment Lines: 63 (14.75%)
'    - Xml Docs: 95.24%
' 
'   Blank Lines: 56 (13.11%)
'     File Size: 16.86 KB


' Module DEGSample
' 
'     Function: DesignAnalysis, getSampleId, groupColors, guessSampleGroups, PopulateSampleInfo
'               print, ReadSampleInfo, sample_groups, sampleinfoTable, sampleInfoTable
'               ScanForSampleInfo, shuffle_groups, WriteSampleInfo
' 
'     Sub: Main
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine.ExpressionSymbols.DataSets
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.ConsolePrinter
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions
Imports Rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports REnv = SMRUCC.Rsharp.Runtime
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' GCModeller DEG experiment analysis designer toolkit
''' </summary>
<Package("sampleInfo", Category:=APICategories.ResearchTools)>
<RTypeExport("sample_info", GetType(SampleInfo))>
Module DEGSample

    Sub Main()
        Call printer.AttachConsoleFormatter(Of SampleInfo)(AddressOf print)
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(SampleInfo()), AddressOf sampleinfoTable)
    End Sub

    Private Function sampleinfoTable(samples As SampleInfo(), args As list, env As Environment) As Rdataframe
        Dim data As New Rdataframe With {.columns = New Dictionary(Of String, Array)}

        ' data.columns(NameOf(SampleInfo.ID)) = samples.Select(Function(a) a.ID).ToArray
        data.columns(NameOf(SampleInfo.sample_name)) = samples.Select(Function(a) a.sample_name).ToArray
        data.columns(NameOf(SampleInfo.sample_info)) = samples.Select(Function(a) a.sample_info).ToArray
        data.columns(NameOf(SampleInfo.injectionOrder)) = samples.Select(Function(a) a.injectionOrder).ToArray
        data.columns(NameOf(SampleInfo.batch)) = samples.Select(Function(a) a.batch).ToArray
        data.columns(NameOf(SampleInfo.color)) = samples.Select(Function(a) a.color).ToArray
        data.columns(NameOf(SampleInfo.shape)) = samples.Select(Function(a) a.shape).ToArray
        data.rownames = samples.Select(Function(a) a.ID).ToArray

        Return data
    End Function

    Private Function print(sample As SampleInfo) As String
        Return $" ({sample.sample_info}) {sample.sample_name}"
    End Function

    ''' <summary>
    ''' try to parse the sampleInfo data from the
    ''' sample labels
    ''' </summary>
    ''' <param name="sample_names"></param>
    ''' <param name="raw_list"></param>
    ''' <returns></returns>
    <ExportAPI("guess.sample_groups")>
    <RApiReturn(GetType(list), GetType(SampleInfo))>
    Public Function guessSampleGroups(sample_names As Array,
                                      Optional maxDepth As Boolean = False,
                                      Optional raw_list As Boolean = True) As Object

        Return CLRVector.asCharacter(sample_names) _
            .GuessPossibleGroups(maxDepth) _
            .ToDictionary(Function(group) group.name,
                          Function(group)
                              Return CObj(group.ToArray)
                          End Function) _
            .DoCall(Function(list)
                        If raw_list Then
                            Return New list With {.slots = list}
                        Else
                            Return PopulateSampleInfo(list).ToArray
                        End If
                    End Function)
    End Function

    Private Iterator Function PopulateSampleInfo(list As Dictionary(Of String, Object)) As IEnumerable(Of SampleInfo)
        Dim colors As LoopArray(Of String) = Designer _
            .GetColors("Paper", list.Count) _
            .Select(Function(c) c.ToHtmlColor) _
            .ToArray
        Dim order As i32 = 1

        For Each group As KeyValuePair(Of String, Object) In list
            Dim color As String = colors.Next

            For Each sample As String In DirectCast(group.Value, String())
                Yield New SampleInfo With {
                    .ID = sample,
                    .sample_name = sample,
                    .sample_info = group.Key,
                    .color = color,
                    .injectionOrder = ++order,
                    .batch = 1,
                    .shape = "circle"
                }
            Next
        Next
    End Function

    ''' <summary>
    ''' get/set the group colors
    ''' </summary>
    ''' <param name="sampleinfo"></param>
    ''' <param name="colorSet"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("group.colors")>
    <RApiReturn(GetType(SampleInfo))>
    Public Function groupColors(sampleinfo As SampleInfo(),
                                <RByRefValueAssign>
                                Optional colorSet As Object = Nothing,
                                Optional env As Environment = Nothing) As Object

        If colorSet Is Nothing Then
            ' just get colorset
            Return New list With {
                .slots = sampleinfo _
                    .GroupBy(Function(a) a.sample_info) _
                    .ToDictionary(Function(a) a.Key,
                                  Function(a)
                                      Return CObj(a.First.color)
                                  End Function)
            }
        Else
            ' set colors to the sample group
            Dim groups = sampleinfo _
                .GroupBy(Function(a) a.sample_info) _
                .ToArray
            Dim colors As LoopArray(Of String) = Designer _
                .GetColors(RColorPalette.getColorSet(colorSet, [default]:="paper")) _
                .Select(Function(c) c.ToHtmlColor) _
                .ToArray

            For Each grp In groups
                Dim htmlCode As String = ++colors

                For Each sample In grp
                    sample.color = htmlCode
                Next
            Next

            Return sampleinfo
        End If
    End Function

    ' design(sampleinfo,  A = B+C+D );

    ''' <summary>
    ''' Create new analysis design sample info via formula
    ''' </summary>
    ''' <param name="sampleinfo"></param>
    ''' <param name="designs"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("design")>
    <RApiReturn(GetType(SampleInfo))>
    Public Function DesignAnalysis(<RRawVectorArgument> sampleinfo As Object,
                                   <RListObjectArgument>
                                   <RLazyExpression>
                                   Optional designs As list = Nothing,
                                   Optional env As Environment = Nothing) As Object

        Dim sampleinfos As pipeline = pipeline.TryCreatePipeline(Of SampleInfo)(sampleinfo, env)

        If sampleinfos.isError Then
            Return sampleinfos.getError
        End If

        Dim samplegroups = sampleinfos.populates(Of SampleInfo)(env) _
            .GroupBy(Function(si) si.sample_info) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return a.ToArray
                          End Function)
        Dim new_groups As New List(Of SampleInfo)
        Dim removePending As New List(Of String)

        For Each design In designs.slots
            Dim new_groupID As String = design.Key
            Dim from_groups = FormulaExpression.GetSymbols(DirectCast(design.Value, Expression))
            Dim currents As New List(Of SampleInfo)

            If from_groups Like GetType(Exception) Then
                Return RInternal.debug.stop({
                    $"invalid expression for the formula: {from_groups.TryCast(Of Exception).ToString}",
                    $"new group label: {new_groupID}"
                }, env)
            End If

            For Each label As String In from_groups.TryCast(Of String())
                Call currents.AddRange(samplegroups(label))
            Next

            Call new_groups.AddRange(From si In currents Select New SampleInfo With {
                .batch = si.batch,
                .color = si.color,
                .ID = si.ID,
                .injectionOrder = si.injectionOrder,
                .sample_info = new_groupID,
                .sample_name = si.sample_name,
                .shape = si.shape
            })
            Call removePending.AddRange(from_groups.TryCast(Of String()))
        Next

        For Each label As String In removePending.Distinct
            Call samplegroups.Remove(label)
        Next

        Call new_groups.AddRange(samplegroups.Values.IteratesALL)

        Return new_groups.ToArray
    End Function

    ''' <summary>
    ''' Read the sampleinfo data table from a given csv file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="tsv"></param>
    ''' <param name="exclude_groups"></param>
    ''' <param name="id_makenames"></param>
    ''' <returns></returns>
    <ExportAPI("read.sampleinfo")>
    Public Function ReadSampleInfo(file As String,
                                   Optional tsv As Boolean = False,
                                   Optional exclude_groups As String() = Nothing,
                                   Optional id_makenames As Boolean = False) As SampleInfo()

        Dim firstLine As String() = New RowObject(file.ReadFirstLine, tsv).ToArray
        Dim nameMaps As New NameMapping(New Dictionary(Of String, String) From {
            {firstLine(Scan0), NameOf(SampleInfo.ID)}
        })
        Dim samples As SampleInfo()

        If tsv Then
            samples = file _
                .LoadTsv(Of SampleInfo)(Encodings.UTF8, nameMaps:=nameMaps, mute:=True) _
                .ToArray
        Else
            samples = file _
                .LoadCsv(Of SampleInfo)(maps:=nameMaps, mute:=True) _
                .ToArray
        End If

        If Not exclude_groups Is Nothing Then
            With New Index(Of String)(exclude_groups)
                samples = samples _
                    .Where(Function(sample)
                               Return .IndexOf(sample.sample_info) = -1
                           End Function) _
                    .ToArray
            End With
        End If

        If id_makenames Then
            Dim sampleId As String() = samples _
                .Select(Function(a) a.ID) _
                .ToArray

            sampleId = REnv.Internal.Invokes.base.makeNames(sampleId)

            For i As Integer = 0 To sampleId.Length - 1
                samples(i).ID = sampleId(i)
            Next
        End If

        Return samples
    End Function

    <ExportAPI("shuffle_groups")>
    Public Function shuffle_groups(x As SampleInfo()) As list
        Dim shuffles = x.GroupBy(Function(xi) xi.sample_info) _
            .OrderBy(Function(a) randf.NextDouble) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return CObj(a.ToArray)
                          End Function)

        Return New list(shuffles)
    End Function

    <ExportAPI("sample_groups")>
    Public Function sample_groups(x As SampleInfo()) As list
        Dim groups = x _
            .GroupBy(Function(xi) xi.sample_info) _
            .OrderBy(Function(xi) xi.Key) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return CObj(a.ToArray)
                          End Function)

        Return New list(groups)
    End Function

    ''' <summary>
    ''' save sampleinfo data as csv file
    ''' </summary>
    ''' <param name="sampleinfo"></param>
    ''' <param name="file"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' You also can save the sampleinfo data directly via the ``write.csv`` function.
    ''' </remarks>
    <ExportAPI("write.sampleinfo")>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function WriteSampleInfo(sampleinfo As SampleInfo(), file$) As Boolean
        Return sampleinfo.SaveTo(file)
    End Function

    ''' <summary>
    ''' create ``sample_info`` data table
    ''' </summary>
    ''' <param name="ID">the sample id in the raw data files</param>
    ''' <param name="sample_name">the sample name label for display, this character vector could be nothing, 
    ''' then the generated sample display name will be replaced with the input sample id</param>
    ''' <param name="sample_info">the sample group information.</param>
    ''' <returns></returns>
    ''' <example>
    ''' let group_vec = c("control","control","treat","control","treat","treat");
    ''' let samples = sampleInfo(group_vec, group_vec);
    ''' let analysis = make.analysis(samples, "control","treat");
    ''' let deg = limma(x, analysis);
    ''' 
    ''' # view deg analysis result of control vs treat
    ''' print(as.data.frame(deg));
    ''' </example>
    <ExportAPI("sampleInfo")>
    <RApiReturn(GetType(SampleInfo))>
    Public Function sampleInfoTable(ID As String(), sample_info As String(),
                                    Optional sample_name As String() = Nothing,
                                    Optional color As String() = Nothing,
                                    Optional env As Environment = Nothing) As Object

        If ID.IsNullOrEmpty OrElse sample_info.IsNullOrEmpty Then
            Return Nothing
        ElseIf sample_name.IsNullOrEmpty Then
            sample_name = ID
        End If

        If ID.Length <> sample_name.Length Then
            Return RInternal.debug.stop({
                $"the size of ID should be equals to the size of sample_name!",
                $"sizeof_ID: {ID.Length}",
                $"sizeof_sample_name: {sample_name.Length}"}, env)
        ElseIf sample_info.Length <> ID.Length AndAlso sample_info.Length > 1 Then
            Return RInternal.debug.stop({
                $"invalid sample_info size, the size of sample_info should be 1 or equals to ID",
                $"size of sample_info: {sample_info.Length}"}, env)
        End If

        Dim get_group = GetVectorElement.Create(Of String)(sample_info)
        Dim list As New List(Of SampleInfo)

        For i As Integer = 0 To ID.Length - 1
            list += New SampleInfo With {
                .ID = ID(i),
                .sample_name = sample_name(i),
                .sample_info = get_group(i),
                .color = color.ElementAtOrNull(i),
                .batch = 1,
                .injectionOrder = i + 1,
                .shape = "circle"
            }
        Next

        Return list.ToArray
    End Function

    <ExportAPI("sampleinfo_gsub")>
    <RApiReturn(GetType(SampleInfo))>
    Public Function sampleinfo_gsub(<RRawVectorArgument> sampleinfo As Object,
                                    <RRawVectorArgument> find As Object,
                                    replace_as As String,
                                    Optional env As Environment = Nothing) As Object

        Dim pull = pipeline.TryCreatePipeline(Of SampleInfo)(sampleinfo, env)

        If pull.isError Then
            Return pull.getError
        End If

        Dim clean As New List(Of SampleInfo)
        Dim find_str As String() = CLRVector.asCharacter(find)

        For Each sample As SampleInfo In pull.populates(Of SampleInfo)(env)
            For Each str As String In find_str
                sample.sample_info = sample.sample_info.Replace(str, replace_as)
            Next

            Call clean.Add(sample)
        Next

        Return clean.ToArray
    End Function

    ''' <summary>
    ''' Get sample id collection from a speicifc sample data groups
    ''' </summary>
    ''' <param name="sampleinfo"></param>
    ''' <param name="groups"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("sampleId")>
    <RApiReturn(GetType(String))>
    Public Function getSampleId(<RRawVectorArgument>
                                sampleinfo As Object,
                                groups As String(),
                                Optional env As Environment = Nothing) As Object

        Dim info As pipeline = pipeline.TryCreatePipeline(Of SampleInfo)(sampleinfo, env)

        If info.isError Then
            Return info.getError
        End If

        Dim infoData As SampleInfo() = info.populates(Of SampleInfo)(env).ToArray
        Dim idlist As String() = groups _
            .Select(Function(label) infoData.SampleIDs(label)) _
            .IteratesALL _
            .ToArray

        Return idlist
    End Function

    ''' <summary>
    ''' Create sampleInfo table from text files
    ''' </summary>
    ''' <param name="dir"></param>
    ''' <returns></returns>
    <ExportAPI("sampleinfo.text.groups")>
    Public Function ScanForSampleInfo(dir As String) As SampleInfo()
        Dim sampleInfo As New List(Of SampleInfo)
        Dim samplelist As String()
        Dim groupName$
        Dim index As i32 = 1

        For Each file As String In ls - l - r - "*.txt" <= dir
            groupName = file.BaseName
            samplelist = file.ReadAllLines
            sampleInfo += samplelist _
                .Select(Function(id)
                            Return New SampleInfo With {
                                .ID = id,
                                .sample_name = id,
                                .sample_info = groupName,
                                .injectionOrder = ++index
                            }
                        End Function)
        Next

        Return sampleInfo
    End Function

    <ExportAPI("make.analysis")>
    <RApiReturn(GetType(DataAnalysis))>
    Public Function makeDataAnalysis(sampleinfo As SampleInfo(), control As String, treatment As String) As Object
        sampleinfo = sampleinfo _
            .Where(Function(si) si.sample_info = control OrElse si.sample_info = treatment) _
            .OrderBy(Function(si)
                         If si.sample_info = control Then
                             Return 0
                         Else
                             Return 1
                         End If
                     End Function) _
            .ToArray

        Return New DataAnalysis(sampleinfo)
    End Function

    <ExportAPI("make.MLdataset")>
    Public Function makeMLdataset(x As HTS.DataFrame.Matrix, sampleinfo As SampleInfo()) As Object
        Dim gene_ids As String() = x.rownames
        Dim samples As Dictionary(Of String, Double()) = x.sampleID _
            .ToDictionary(Function(name) name,
                          Function(name)
                              Return x.GetSampleArray(name).ToArray
                          End Function)
        Dim dataset As New List(Of EntityClusterModel)
        Dim missing As New List(Of SampleInfo)

        For Each sample As SampleInfo In sampleinfo
            If samples.ContainsKey(sample.ID) Then
                Dim vec As New Dictionary(Of String, Double)
                Dim vals As Double() = samples(sample.ID)

                For i As Integer = 0 To gene_ids.Length - 1
                    Call vec.Add(gene_ids(i), vals(i))
                Next

                Call dataset.Add(New EntityClusterModel With {
                    .ID = sample.ID,
                    .Cluster = sample.sample_info,
                    .Properties = vec
                })
            Else
                Call missing.Add(sample)
            End If
        Next

        If missing.Any Then
            Call $"found {missing.Count} missing samples from the given expression matrix: {missing.JoinBy(", ")}".warning
        End If

        Return dataset.ToArray
    End Function
End Module
