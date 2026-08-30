#Region "Microsoft.VisualBasic::794467cb5c8fffb85ce5b5eaf69c5da7, R#\phenotype_kit\sampleInfo.vb"

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

    '   Total Lines: 535
    '    Code Lines: 387 (72.34%)
    ' Comment Lines: 75 (14.02%)
    '    - Xml Docs: 92.00%
    ' 
    '   Blank Lines: 73 (13.64%)
    '     File Size: 21.53 KB


    ' Module DEGSample
    ' 
    '     Function: DesignAnalysis, getSampleId, groupColors, guessSampleGroups, makeDataAnalysis
    '               makeMLdataset, PopulateSampleInfo, print, ReadSampleInfo, sample_groups
    '               sampleinfo_gsub, sampleinfoTable, sampleInfoTable, ScanForSampleInfo, shuffle_groups
    '               WriteSampleInfo
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
''' 
''' <remarks>
''' This R# package module provides the toolkit for create and manipulate the 
''' experiment sample information data(<see cref="SampleInfo"/>), which is the 
''' experiment design data of the different expression analysis:
''' 
''' + create the sample information data: ``sampleInfo``, 
'''   ``guess.sample_groups``, ``sampleinfo.text.groups``, ``read.sampleinfo``;
''' + manipulate the sample group data: ``design``, ``sample_groups``, 
'''   ``shuffle_groups``, ``group.colors``, ``sampleinfo_gsub``, ``sampleId``;
''' + build the analysis model for run the different expression analysis: 
'''   ``make.analysis``, ``make.MLdataset``.
''' 
''' The sample information data object in R# environment can be saved as a csv 
''' table file via the ``write.sampleinfo`` api, or be converted to a data frame 
''' via the ``as.data.frame`` api.
''' </remarks>
<Package("sampleInfo", Category:=APICategories.ResearchTools)>
<RTypeExport("sample_info", GetType(SampleInfo))>
Module DEGSample

    ''' <summary>
    ''' Initialize the internal environment of this R# package module
    ''' </summary>
    ''' 
    ''' <remarks>
    ''' this function is invoked automatically at the start of the R# runtime 
    ''' environment, it registers:
    ''' 
    ''' 1. the console formatter of the <see cref="SampleInfo"/> data object;
    ''' 2. the data frame cast handler of the sample information collection, so 
    '''    that the sample information data can be converted to a data frame via 
    '''    the ``as.data.frame`` api.
    ''' </remarks>
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
    ''' <param name="sample_names">
    ''' a character vector of the sample labels, the sample group information will 
    ''' be guessed from the common tag prefix of these sample labels, example as 
    ''' the sample labels ``iBAQ-AAA-1``, ``iBAQ-AAA-2``, ``iBAQ-BBB-1`` will be 
    ''' grouped as the ``AAA`` and ``BBB`` groups.
    ''' </param>
    ''' <param name="maxDepth">
    ''' extends the group label to the max depth? if this parameter is FALSE, then 
    ''' only the first different tag token will be used as the group label, 
    ''' otherwise the group label will be extended until the last common tag token.
    ''' </param>
    ''' <param name="raw_list">
    ''' returns the group result as a raw tuple list object(the slot key of the 
    ''' list is the group label and the slot value is a character vector of the 
    ''' sample label)? if this parameter is FALSE, then a vector of the 
    ''' <see cref="SampleInfo"/> object will be returned.
    ''' </param>
    ''' <returns>
    ''' a tuple list of the guessed sample groups, or a vector of the 
    ''' <see cref="SampleInfo"/> object when the ``raw_list`` parameter is FALSE.
    ''' 
    ''' the generated <see cref="SampleInfo"/> object will be assigned with a 
    ''' default color from the ``Paper`` color set, and the ``shape`` property is 
    ''' set as ``circle``, the ``batch`` property is set as 1 and the 
    ''' ``injectionOrder`` property is the index order of the sample in the 
    ''' generated sample collection.
    ''' </returns>
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
    ''' <param name="sampleinfo">
    ''' a vector of the <see cref="SampleInfo"/> sample information data.
    ''' </param>
    ''' <param name="colorSet">
    ''' a new color set for assign to each sample group, which can be a character 
    ''' vector of the html color code or the color palette name, the ``Paper`` 
    ''' color set will be used if this color set parameter can not be recognized.
    ''' 
    ''' if this parameter is not specified, then this function works as a getter: 
    ''' the current color of each sample group will be returned.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' this function returns a tuple list of the color of each sample group when 
    ''' the ``colorSet`` parameter is not specified(the slot key of the list is the 
    ''' sample group label and the slot value is the html color code of the 
    ''' corresponding sample group), otherwise the input sample information 
    ''' collection that the color of each sample group has been modified will be 
    ''' returned.
    ''' </returns>
    ''' 
    ''' <remarks>
    ''' the colors of the color set will be assigned to the sample groups in a 
    ''' loop manner, so that the color set is not required to have the same size 
    ''' as the sample group numbers.
    ''' 
    ''' this api can be used as a property setter in R# environment: the color of 
    ''' each sample group can be overwritten via the value assign syntax:
    ''' 
    ''' ```r
    ''' group.colors(samples) &lt;- "Set1:c8";
    ''' ```
    ''' </remarks>
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
    ''' <param name="sampleinfo">
    ''' the sample information data, which can be a vector of the 
    ''' <see cref="SampleInfo"/> object or a pipeline object that produces a set of 
    ''' the sample information data.
    ''' </param>
    ''' <param name="designs">
    ''' a tuple list of the experiment design formula: the slot key of the list is 
    ''' the label of the new sample group and the slot value is a formula 
    ''' expression that describes the merge of the original sample groups, example 
    ''' as ``list(A = B + C + D)`` means that the sample groups ``B``, ``C`` and 
    ''' ``D`` will be merged into a new sample group ``A``.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a new vector of the <see cref="SampleInfo"/> object: the sample groups that 
    ''' are described in the given design formula will be replaced with the new 
    ''' generated sample groups, and the other sample groups that are not 
    ''' referenced in the design formula will be kept as is;
    ''' 
    ''' this function returns a R# error message object if the input data can not 
    ''' be cast to a collection of the sample information data, or the given design 
    ''' formula is invalid.
    ''' </returns>
    ''' 
    ''' <remarks>
    ''' the sample information data(the ``ID``, ``sample_name``, ``color``, 
    ''' ``shape``, ``batch`` and ``injectionOrder`` property) of the merged samples 
    ''' will be kept as is, only the ``sample_info`` group label will be replaced 
    ''' with the new group label.
    ''' 
    ''' the design formula expression is a lazy expression, so that the sample group 
    ''' label in the formula is not required to be a R# symbol.
    ''' </remarks>
    ''' 
    ''' <example>
    ''' imports "sampleInfo" from "phenotype_kit";
    ''' 
    ''' # merge the sample groups of "B", "C" and "D" into 
    ''' # a new sample group "A"
    ''' let samples = design(samples, list(A = B + C + D));
    ''' </example>
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
    ''' <param name="file">
    ''' the file path of the sample information table file.
    ''' </param>
    ''' <param name="tsv">
    ''' is the target table file a TSV format table file? by default is FALSE means 
    ''' that the target table file is a CSV format table file.
    ''' </param>
    ''' <param name="exclude_groups">
    ''' a character vector of the sample group label for exclude from the loaded 
    ''' sample information data.
    ''' </param>
    ''' <param name="id_makenames">
    ''' rename the sample id via the generic make names function? this parameter is 
    ''' helpful for make the sample id as a valid R# symbol name.
    ''' </param>
    ''' <returns>
    ''' a vector of the <see cref="SampleInfo"/> object that is loaded from the 
    ''' given table file.
    ''' 
    ''' NOTE: the first column of the table file will be used as the ``ID`` 
    ''' property of the generated sample information data, and the sample data rows 
    ''' that the ``ID`` or the ``sample_info`` data is empty will be removed 
    ''' automatically with a warning message.
    ''' </returns>
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

        ' make filter of the possible missing data
        Dim beforeFilterMissing = samples.Length

        samples = samples _
            .Where(Function(a)
                       Return Not (a.ID.StringEmpty OrElse a.sample_info.StringEmpty)
                   End Function) _
            .ToArray

        If beforeFilterMissing <> samples.Length Then
            Call $"there are {beforeFilterMissing - samples.Length} missing sample data has been filter from the table file input!".warning
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

    ''' <summary>
    ''' shuffle the sample group order in a random manner
    ''' </summary>
    ''' <param name="x">
    ''' a vector of the <see cref="SampleInfo"/> sample information data.
    ''' </param>
    ''' <returns>
    ''' a tuple list of the sample groups in a random order: the slot key of the 
    ''' list is the sample group label and the slot value is a vector of the 
    ''' <see cref="SampleInfo"/> object that belongs to the corresponding sample 
    ''' group.
    ''' </returns>
    ''' 
    ''' <remarks>
    ''' unlike the ``sample_groups`` api, which sorts the sample groups by the group 
    ''' label in ascending order, this function shuffles the sample group order in a 
    ''' random manner, which is helpful for the random color assignment or the 
    ''' permutation test of the sample groups.
    ''' </remarks>
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

    ''' <summary>
    ''' group the sample information data by the sample group label
    ''' </summary>
    ''' <param name="x">
    ''' a vector of the <see cref="SampleInfo"/> sample information data.
    ''' </param>
    ''' <returns>
    ''' a tuple list of the sample groups: the slot key of the list is the sample 
    ''' group label and the slot value is a vector of the <see cref="SampleInfo"/> 
    ''' object that belongs to the corresponding sample group, the sample groups in 
    ''' the generated list object are sorted by the group label in ascending order.
    ''' </returns>
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
                                    Optional batch As Integer() = Nothing,
                                    Optional inject_order As Integer() = Nothing,
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
                .batch = batch.ElementAtOrDefault(i, default:=1),
                .injectionOrder = inject_order.ElementAtOrDefault(i, default:=i + 1),
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
