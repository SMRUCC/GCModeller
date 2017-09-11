#Region "Microsoft.VisualBasic::11f4c70f3bf162d002cd62b42381fa3c, ..\CLI_tools\eggHTS\CLI\2. DEP.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.ComponentModel
Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.ChartPlots.Statistics.Heatmap
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Scripting
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.HTS.Proteomics
Imports SMRUCC.genomics.Analysis.KEGG
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports SMRUCC.genomics.Visualize
Imports SMRUCC.genomics.Data.Repository.kb_UniProtKB.UniprotKBEngine
Imports SMRUCC.genomics.Data.Repository.kb_UniProtKB

Partial Module CLI

#Region "DEG data experiment designer"

    <ExportAPI("/edgeR.Designer")>
    <Description("Generates the edgeR inputs table")>
    <Usage("/edgeR.Designer /in <proteinGroups.csv> /designer <designer.csv> [/label <default is empty> /deli <default=-> /out <out.DIR>]")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function edgeRDesigner(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim designer = args <= "/designer"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & $"-{designer.BaseName}-edgeR/")
        Dim designers As Designer() = designer.LoadCsv(Of Designer)
        Dim label$ = args.GetValue("/label", "")
        Dim deli$ = args.GetValue("/deli", "-")

        Call EdgeR_rawDesigner([in], designers, (label, deli), workDIR:=out)

        Return 0
    End Function

    <ExportAPI("/T.test.Designer.iTraq")>
    <Description("Generates the iTraq data t.test DEP method inputs table")>
    <Usage("/T.test.Designer.iTraq /in <proteinGroups.csv> /designer <designer.csv> [/label <default is empty> /deli <default=-> /out <out.DIR>]")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function TtestDesigner(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim designer = args <= "/designer"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & $"-{designer.BaseName}-iTraq_Ttest/")
        Dim designers As Designer() = designer.LoadCsv(Of Designer)
        Dim label$ = args.GetValue("/label", "")
        Dim deli$ = args.GetValue("/deli", "-")

        Call DEGDesigner.TtestDesigner([in], designers, (label, deli), workDIR:=out)

        Return 0
    End Function

    ''' <summary>
    ''' 独立实验变量的t检验构建工具
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/T.test.Designer.LFQ")>
    <Description("Generates the LFQ data t.test DEP method inputs table")>
    <Usage("/T.test.Designer.LFQ /in <proteinGroups.csv> /designer <designer.csv> [/label <default is empty> /deli <default=-> /out <out.DIR>]")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function TtestDesignerLFQ(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim designer = args <= "/designer"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & $"-{designer.BaseName}-LFQ_Ttest/")
        Dim designers As Designer() = designer.LoadCsv(Of Designer)
        Dim label$ = args.GetValue("/label", "")
        Dim deli$ = args.GetValue("/deli", "-")

        Call DEGDesigner.TtestDesignerIndependent([in], designers, (label, deli), workDIR:=out)

        Return 0
    End Function

#End Region

    ''' <summary>
    ''' 因为绘制云图需要表达量的实际值，对于iTraq结果而言是没有表达量的，所以这里就仅限于iBAQ结果了
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/iBAQ.Cloud")>
    <Usage("/iBAQ.Cloud /in <expression.csv> /annotations <annotations.csv> /DEPs <DEPs.csv> /tag <expression> [/out <out.png>]")>
    <Description("Cloud plot of the iBAQ DEPs result.")>
    <Argument("/tag", Description:="The field name in the ``/in`` matrix that using as the expression value.")>
    Public Function DEPsCloudPlot(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim annotations$ = args <= "/annotations"
        Dim DEPs$ = args <= "/DEPs"
        Dim tag$ = args <= "/tag"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & "-" & tag.NormalizePathString & ".png")
        Dim expressions = EntityObject.LoadDataSet([in]).ToArray
        Dim annotationData = annotations.LoadCsv(Of UniprotAnnotations)
        Dim DEPsResult = EntityObject.LoadDataSet(DEPs).ToArray
        Dim plot As GraphicsData = CloudPlot.Plot(expressions, annotationData, DEPsResult, tag)
        Return plot.Save(out).CLICode
    End Function

    ''' <summary>
    ''' 使用这个函数来处理iTraq实验结果之中与分析需求单的FC比对方式颠倒的情况
    ''' 
    ''' 假设所输入的文件的第一列为标识符
    ''' 最后的三列为结果数据
    ''' 则中间的剩余的列数据都是FC值
    ''' 
    ''' ``Accession	T1.C1	T1.C2	T2.C1	T2.C2	FC.avg	p.value	is.DEP``
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/iTraq.Reverse.FC",
               Info:="Reverse the FC value from the source result.",
               Usage:="/iTraq.Reverse.FC /in <data.csv> [/out <Reverse.csv>]")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function iTraqInvert(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".reverse.csv")
        Dim data As File = File.Load([in])
        Dim start = 1
        Dim ends = (data.Width - 1) - 3

        For Each row As RowObject In data.Skip(1)
            For i As Integer = start To ends
                Dim s$ = row(i)

                If s = "NA" OrElse s.TextEquals("NaN") Then
                    Continue For
                Else
                    row(i) = 1 / Val(s)
                End If
            Next
        Next

        Return data _
            .Save(out, Encodings.ASCII) _
            .CLICode
    End Function

    <ExportAPI("/DEP.uniprot.list",
               Usage:="/DEP.uniprot.list /DEP <log2-test.DEP.csv> /sample <sample.csv> [/out <out.txt>]")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function DEPUniprotIDlist(args As CommandLine) As Integer
        Dim DEP As String = args("/DEP")
        Dim sample As String = args("/sample")
        Dim out As String = args.GetValue("/out", DEP.TrimSuffix & "-uniprot.ID.list.txt")
        Dim DEP_data As IEnumerable(Of DEP_iTraq) = EntityObject _
            .LoadDataSet(Of DEP_iTraq)(path:=DEP) _
            .Where(Function(d) d.isDEP) _
            .ToArray
        Dim sampleData As Dictionary(Of String, String()) =
            sample _
            .LoadCsv(Of UniprotAnnotations) _
            .GroupBy(Function(p) p.ORF) _
            .ToDictionary(Function(p) p.Key,
                          Function(g) g.ToArray(
                          Function(p) p.ID))
        Dim list$() = DEP_data _
            .Select(Function(d) sampleData(d.ID)) _
            .IteratesALL _
            .Distinct _
            .ToArray

        Return list.SaveTo(out).CLICode
    End Function

    <ExportAPI("/DEP.uniprot.list2",
               Usage:="/DEP.uniprot.list2 /in <log2.test.csv> [/DEP.Flag <is.DEP?> /uniprot.Flag <uniprot> /species <scientifcName> /uniprot <uniprotXML> /out <out.txt>]")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function DEPUniprotIDs2(args As CommandLine) As Integer
        Dim [in] = args("/in")
        Dim DEPFlag As String = args.GetValue("/DEP.flag", "is.DEP?")
        Dim uniprot As String = args.GetValue("/uniprot.Flag", "uniprot")
        Dim data = EntityObject.LoadDataSet([in])
        Dim DEPs = data.Where(Function(prot) prot(DEPFlag).ParseBoolean).ToArray
        Dim uniprotIDs$() = DEPs _
            .Select(Function(prot) prot(uniprot).Split(";"c)) _
            .Unlist _
            .Distinct _
            .Select(AddressOf Trim) _
            .ToArray
        Dim sciName$ = args("/species")
        Dim out As String = args.GetValue(
            "/out",
            [in].TrimSuffix & $"DEPs={DEPs.Length}.uniprotIDs.txt")

        uniprot$ = args("/uniprot")

        If Not sciName.StringEmpty AndAlso uniprot.FileExists(True) Then
            ' 将结果过滤为指定的物种的编号
            Dim table As Dictionary(Of entry) = UniProtXML.LoadDictionary(uniprot)
            uniprotIDs = uniprotIDs _
                .Where(Function(ID) table.ContainsKey(ID) AndAlso
                                    table(ID).organism.scientificName = sciName) _
                .ToArray
        End If

        Return uniprotIDs.SaveTo(out).CLICode
    End Function

    <ExportAPI("/DEP.venn",
               Info:="Generate the VennDiagram plot data and the venn plot tiff. The default parameter profile is using for the iTraq data.",
               Usage:="/DEP.venn /data <Directory> [/level <1.25> /FC.tag <FC.avg> /title <VennDiagram title> /pvalue <p.value> /out <out.DIR>]")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function VennData(args As CommandLine) As Integer
        Dim DIR$ = args("/data")
        Dim FCtag$ = args.GetValue("/FC.tag", "FC.avg")
        Dim pvalue$ = args.GetValue("/pvalue", "p.value")
        Dim out As String = args.GetValue("/out", DIR.TrimDIR & ".venn/")
        Dim dataOUT = out & "/DEP.venn.csv"
        Dim title$ = args.GetValue("/title", "VennDiagram title")
        Dim level# = args.GetValue("/level", 1.25)

        Call DEGDesigner _
            .MergeMatrix(DIR, "*.csv", level, 0.05, FCtag, 1 / level, pvalue) _
            .SaveDataSet(dataOUT)
        Call Apps.VennDiagram.Draw(dataOUT, title, out:=out & "/venn.tiff")

        Return 0
    End Function

    <ExportAPI("/DEPs.union", Usage:="/DEPs.union /in <csv.DIR> [/FC <default=logFC> /out <out.csv>]")>
    Public Function DEPsUnion(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim FC$ = args.GetValue("/FC", "logFC")
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & "-" & FC.NormalizePathString & ".union.csv")
        Dim datas = (ls - l - r - "*.csv" <= [in]) _
            .ToDictionary(Function(csv) csv.BaseName,
                          Function(csv)
                              Return EntityObject _
                                  .LoadDataSet(csv) _
                                  .ToDictionary
                          End Function)
        Dim allIDs = datas.Values _
            .Select(Function(sample) sample.Values) _
            .IteratesALL _
            .Select(Function(x) x.ID) _
            .Distinct _
            .ToArray
        Dim union As New List(Of EntityObject)

        For Each id As String In allIDs
            Dim protein As New EntityObject(id)

            For Each sample In datas
                Dim value = sample.Value.TryGetValue(id)
                If value Is Nothing Then
                    protein.Properties.Add(sample.Key, 1)
                Else
                    protein.Properties.Add(sample.Key, value(FC))
                End If
            Next

            union += protein
        Next

        Return union _
            .SaveTo(out) _
            .CLICode
    End Function

    ''' <summary>
    ''' ``/iTraq``开关表示是够是iTraq的分析输出？
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/DEP.heatmap")>
    <Description("Generates the heatmap plot input data. The default label profile is using for the iTraq result.")>
    <Usage("/DEP.heatmap /data <Directory> [/KO.class /annotation <annotation.csv> /sampleInfo <sampleinfo.csv> /iTraq /non_DEP.blank /size <size, default=2000,3000> /log2FC <log2FC> /is.DEP <is.DEP> /out <out.DIR>]")>
    <Argument("/non_DEP.blank", True, CLITypes.Boolean,
              Description:="If this parameter present, then all of the non-DEP that bring by the DEP set union, will strip as blank on its foldchange value, and set to 1 at finally. Default is reserve this non-DEP foldchange value.")>
    <Argument("/KO.class", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="If this argument was set, then the KO class information for uniprotID will be draw on the output heatmap.")>
    <Argument("/sampleInfo", True, CLITypes.File,
              AcceptTypes:={GetType(SampleInfo)},
              Description:="Describ the experimental group information")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function Heatmap_DEPs(args As CommandLine) As Integer
        Dim DIR$ = args <= "/data"
        Dim log2FC As String = args.GetValue("/log2FC", "log2FC")
        Dim isDEP As String = args.GetValue("/is.DEP", "is.DEP")
        Dim out As String = args.GetValue("/out", DIR.TrimDIR & ".heatmap/")
        Dim dataOUT = out & "/DEP.heatmap.csv"
        Dim nonDEP_blank As Boolean = args.GetBoolean("/non_DEP.blank")
        Dim iTraq As Boolean = args.GetBoolean("/iTraq")
        Dim size$ = args.GetValue("/size", "2000,3000")
        Dim data As Dictionary(Of String, Dictionary(Of DEP_iTraq)) = (ls - l - r - "*.csv" <= DIR).ToDictionary(Function(path) path.BaseName, Function(path) EntityObject.LoadDataSet(Of DEP_iTraq)(path).ToDictionary)
        Dim allDEPs = data.Values.IteratesALL.Where(Function(x) x.Value(isDEP).TextEquals("TRUE")).Keys.Distinct.ToArray
        Dim matrix As New List(Of DataSet)

        For Each id In allDEPs
            Dim FClog2 As New Dictionary(Of String, Double)

            For Each group In data
                With group.Value
                    If .ContainsKey(id) Then
                        With .ref(id)
                            If .ref(isDEP).TextEquals("TRUE") Then
                                For Each prop In .ref.Properties
                                    FClog2.Add(prop.Key, Val(Math.Log(prop.Value, 2)))
                                Next
                            Else
                                If nonDEP_blank Then
                                    For Each prop In .ref.Properties
                                        FClog2.Add(prop.Key, 0) 'log2(1) = 0
                                    Next
                                Else
                                    For Each prop In .ref.Properties
                                        FClog2.Add(prop.Key, Val(Math.Log(prop.Value, 2)))
                                    Next
                                End If
                            End If
                        End With
                    Else
                        For Each key In data(group.Key).Values.First.Properties.Keys
                            FClog2.Add(key, 0)
                        Next
                    End If
                End With
            Next

            matrix += New DataSet With {
                .ID = id,
                .Properties = FClog2
            }
        Next

        Call matrix.SaveTo(dataOUT)

        If args.IsTrue("/KO.class") Then
            Dim groupInfo As SampleInfo() = (args <= "/sampleInfo").LoadCsv(Of SampleInfo)
            Dim KOinfo As Dictionary(Of String, String) = matrix _
                .Keys _
                .GetKOTable(MySQLExtensions.GetMySQLClient(DBName:=UniprotKBEngine.DbName))

            Call DEPsKOHeatmap _
                .Plot(matrix, groupInfo.SampleGroupInfo, groupInfo.SampleGroupColor, KOInfo:=KOinfo) _
                .Save(out & "/plot.png")
        Else
            ' 绘制普通的热图
            Call Heatmap.Plot(matrix, size:=size).Save(out & "/plot.png")
        End If

        Return 0
    End Function

    ''' <summary>
    ''' 获取DEPs的原始数据的热图数据
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <ExportAPI("/DEP.heatmap.raw")>
    <Description("All of the NA value was replaced by value ``1``, as the FC value when it equals 1, then ``log2(1) = 0``, which means it has no changes.")>
    <Usage("/DEP.heatmap.raw /DEPs <DEPs.csv.folder> [/DEP.tag <default=is.DEP> /out <out.csv>]")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function DEPsHeatmapRaw(args As CommandLine) As Integer
        Dim in$ = args <= "/DEPs"
        Dim raw$ = args <= "/raw"
        Dim DEPTag$ = args.GetValue("/DEP.tag", "is.DEP")
        Dim out As String = args.GetValue("/out", [in].TrimDIR & ".heatmap.raw/")
        Dim dataOUT = out & "/DEP.heatmap.raw.csv"

        Return DEGDesigner _
            .GetDEPsRawValues([in], DEPTag) _
            .SaveDataSet(dataOUT) _
            .CLICode
    End Function

    ''' <summary>
    ''' 如果data参数不存在则默认只取出DEP的输入数据之中的is.DEP为真的部分
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/DEPs.takes.values")>
    <Description("")>
    <Usage("/DEPs.takes.values /in <DEPs.csv> [/boolean.tag <default=is.DEP> /by.FC <tag=value, default=logFC=log2(1.5)> /by.p.value <tag=value, p.value=0.05> /data <data.csv> /out <out.csv>]")>
    Public Function TakeDEPsValues(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim isDEP$ = args.GetValue("/boolean.tag", "is.DEP")
        Dim FC$ = args <= "/by.FC"
        Dim pvalue = args <= "/by.p.value"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & "-DEPs.values.csv")
        Dim data As Dictionary(Of EntityObject)
        Dim source = EntityObject.LoadDataSet([in])
        Dim DEPs = source _
            .Where(Function(protein)
                       Return protein(isDEP).TextEquals("True")
                   End Function) _
            .ToArray

        With args <= "/data"
            If .FileLength > 0 Then
                data = EntityObject _
                    .LoadDataSet(.ref) _
                    .ToDictionary
            Else
                data = source.ToDictionary
            End If
        End With

        Dim values = DEPs _
            .Where(Function(protein)
                       Return data.ContainsKey(protein.ID)
                   End Function) _
            .Select(Function(protein) data(protein.ID)) _
            .ToArray
        Return values _
            .SaveTo(out) _
            .CLICode
    End Function

    <ExportAPI("/Venn.Functions",
               Usage:="/Venn.Functions /venn <venn.csv> /anno <annotations.csv> [/out <out.csv>]")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function VennFunctions(args As CommandLine) As Integer
        Dim in$ = args <= "/venn"
        Dim anno$ = args <= "/anno"
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-functions.csv")
        Dim venn As EntityObject() = EntityObject.LoadDataSet([in]).ToArray
        Dim annoData As Dictionary(Of String, EntityObject) = EntityObject _
            .LoadDataSet(anno) _
            .ToDictionary(Function(prot) prot("uniprot"))
        Dim list As New List(Of EntityObject)

        For Each prot As EntityObject In venn
            prot.Properties.Add("geneName", annoData(prot.ID)("geneName"))
            prot.Properties.Add("fullName", annoData(prot.ID)("fullName"))
            prot.Properties.Add("functions", annoData(prot.ID)("functions"))

            list += prot
        Next

        Return list.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' 这个函数要求的列名称能够和raw之中的列名称可以一一对应，假若raw参数存在的话
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Merge.DEPs",
               Info:="Usually using for generates the heatmap plot matrix of the DEPs. This function call will generates two dataset, one is using for the heatmap plot and another is using for the venn diagram plot.",
               Usage:="/Merge.DEPs /in <*.csv,DIR> [/log2 /threshold ""log(1.5,2)"" /raw <sample.csv> /out <out.csv>]")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function MergeDEPs(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim log2 As Boolean = args.GetBoolean("/log2")
        Dim out As String = args.GetValue("/out", [in].TrimDIR & $".data{If(log2, "-log2", "")}.csv")
        Dim raw As String = args("/raw")
        Dim threshold As String = args.GetValue("/threshold", "log(1.5,2)")
        Dim data = (ls - l - r - "*.csv" <= [in]) _
            .Select(Function(x) DataSet.LoadDataSet(path:=x)) _
            .IteratesALL _
            .GroupBy(Function(gene) gene.ID) _
            .ToArray
        Dim allKeys$() = data _
            .Select(Function(g) g.Select(Function(o) o.Properties.Keys)) _
            .IteratesALL _
            .IteratesALL _
            .Distinct _
            .ToArray
        Dim t = Function(n#)
                    If log2 Then
                        Return System.Math.Log(n, newBase:=2)
                    Else
                        Return n
                    End If
                End Function
        Dim output As New File

        If raw.FileExists(True) Then
            Dim sample As Dictionary(Of DataSet) = DataSet.LoadDataSet(path:=raw).ToDictionary
            Dim row As New RowObject({"ID"}.JoinIterates(allKeys))

            output += row

            For Each gene In data
                Dim gData = sample(gene.Key)

                row = New RowObject()
                row += gene.Key
                row += allKeys.Select(Function(k) t(n:=gData(k)).ToString)
                output += row
            Next

            Call output.Save(out.TrimSuffix & "-heatmapData.csv")
        Else

        End If

        ' 在下面生成文氏图的数据
        Dim cut# = (New Expression).Evaluation(threshold)

        For Each row In output.Skip(1)
            For i As Integer = 1 To row.Width - 1
                row(i) = If(Math.Abs(Val(row(i))) >= cut, row(i), "")
            Next
        Next

        Call output.Save(out.TrimSuffix & "-vennData.csv")

        Return 0
    End Function

    ''' <summary>
    ''' 当没有任何生物学重复的时候，就只能够使用这个函数进行FoldChange的直方图的绘制了
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/DEP.logFC.hist",
               Info:="Using for plots the FC histogram when the experiment have no biological replicates.",
               Usage:="/DEP.logFC.hist /in <log2test.csv> [/step <0.5> /tag <logFC> /legend.title <Frequency(logFC)> /x.axis ""(min,max),tick=0.25"" /color <lightblue> /size <1600,1200> /out <out.png>]")>
    <Argument("/tag", True, CLITypes.String,
              AcceptTypes:={GetType(String)},
              Description:="Which field in the input dataframe should be using as the data source for the histogram plot? Default field(column) name is ""logFC"".")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function logFCHistogram(args As CommandLine) As Integer
        Dim [in] = args("/in")
        Dim tag As String = args.GetValue("/tag", "logFC")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & $".{tag.NormalizePathString}.histogram.png")
        Dim data = EntityObject.LoadDataSet([in])
        Dim xAxis As String = args("/x.axis")
        Dim step! = args.GetValue("/step", 0.5!)
        Dim lTitle$ = args.GetValue("/legend.title", "Frequency(logFC)")
        Dim color$ = args.GetValue("/color", "lightblue")

        Return data _
            .logFCHistogram(tag,
                            size:=args.GetValue("/size", New Size(1600, 1200)),
                            [step]:=[step],
                            xAxis:=xAxis,
                            serialTitle:=lTitle,
                            color:=color) _
            .Save(out) _
            .CLICode
    End Function

    <ExportAPI("/DEP.logFC.Volcano", Usage:="/DEP.logFC.Volcano /in <DEP.qlfTable.csv> [/tag.pvalue <default=p.value> /size <1920,1440> /out <plot.csv>]")>
    <Description("Volcano plot of the DEPs' analysis result.")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function logFCVolcano(args As CommandLine) As Integer
        Dim in$ = args("/in")
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & ".DEP.vocano.plot.png")
        Dim sample = EntityObject.LoadDataSet([in])
        Dim size$ = args.GetValue("/size", "1920,1440")
        Dim pvalueTag$ = args.GetValue("/tag.pvalue", "p.value")

        Return Volcano.PlotDEGs(sample, pvalue:=pvalueTag,
                                padding:="padding: 50 50 150 150",
                                displayLabel:=LabelTypes.None,
                                size:=size) _
            .Save(out) _
            .CLICode
    End Function

    <ExportAPI("/DEPs.stat",
               Info:="https://github.com/xieguigang/GCModeller.cli2R/blob/master/GCModeller.cli2R/R/log2FC_t-test.R",
               Usage:="/DEPs.stat /in <log2.test.csv> [/log2FC <default=log2FC> /out <out.stat.csv>]")>
    <Argument("/log2FC", True, CLITypes.String, Description:="The field name that stores the log2FC value of the average FoldChange")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function DEPStatics(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & ".DEPs.stat.csv")
        Dim log2FC$ = args.GetValue("/log2FC", "log2FC")
        Dim DEPs As EntityObject() = EntityObject _
           .LoadDataSet(path:=in$) _
           .Where(Function(d) d("is.DEP").TextEquals("TRUE")) _
           .ToArray
        Dim result As New File

        result += {"上调", "下调", "总数"}
        result += {
            DEPs _
                .Where(Function(prot) Val(prot(log2FC)) > 0) _
                .Count _
                .ToString,
            DEPs _
                .Where(Function(prot) Val(prot(log2FC)) < 0) _
                .Count _
                .ToString,
            CStr(DEPs.Length)
        }

        Return result.Save(out).CLICode
    End Function
End Module
