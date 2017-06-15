Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.Microarray.DEGDesigner

Partial Module CLI

    ''' <summary>
    ''' 生成总蛋白的表达状况的文氏图，主要统计的是蛋白表达与否？
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/proteinGroups.venn")>
    <Usage("/proteinGroups.venn /in <proteinGroups.csv> /designer <designer.csv> [/label <tag label> /deli <delimiter, default=_> /out <out.venn.DIR>]")>
    <Group(CLIGroups.SamplesExpressions_CLI)>
    Public Function proteinGroupsVenn(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim designer$ = args <= "/designer"
        Dim label$ = args <= "/label"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & ".venn/")
        Dim proteins = EntityObject.LoadDataSet([in])
        Dim designers As Designer() = designer.LoadCsv(Of Designer)
        Dim venn As New List(Of EntityObject)
        Dim delimiter$ = args.GetValue("/deli", "_")
        Dim groupLabels = designers.GetExperimentGroupLabels(label, delimiter)

        Call groupLabels.GetJson(True).__DEBUG_ECHO

        For Each protein As EntityObject In proteins
            Dim x As New EntityObject(protein.ID)

            ' 先遍历每一个蛋白
            ' 再遍历每一个实验设计
            ' 如果实验重复超过半数以上都是零，则为空
            For Each group As KeyValuePair(Of String, String()) In groupLabels
                Dim vals#() = group.Value _
                    .Select(Function(l) Val(protein(l))) _
                    .Where(Function(v)
                               Return (Not v.IsNaNImaginary) AndAlso (Not v = 0R)
                           End Function) _
                    .ToArray
                Dim n% = Fix(group.Value.Length / 2)

                If vals.Length >= n Then
                    ' 超过半数，没有表达
                    x.Properties.Add(group.Key, vals.Average)
                Else
                    ' 置为零的时候，venn模型任然会认为有数据，会出错，在这里置为空字符串
                    x.Properties.Add(group.Key, "")
                End If
            Next

            venn += x
        Next

        Dim dataOUT$ = out & "/proteinGroups.venn.csv"

        Call venn.SaveTo(dataOUT)
        Call Apps.VennDiagram.Draw(
            dataOUT,
            "proteinGroups.venn",
            out:=out & "/venn.tiff")

        Return 0
    End Function

    ''' <summary>
    ''' 获取用于从protein数据之中获取表达数据的标签信息
    ''' </summary>
    ''' <param name="designers"></param>
    ''' <param name="label$"></param>
    ''' <param name="delimiter$"></param>
    ''' <returns></returns>
    <Extension> Public Function GetExperimentGroupLabels(designers As Designer(), Optional label$ = Nothing, Optional delimiter$ = "_") As Dictionary(Of String, String())
        Dim groups = designers _
            .GroupBy(Function(d) d.GroupLabel) _
            .ToArray
        Dim groupLabels As New Dictionary(Of String, String())
        Dim gl$
        Dim labels$() = Nothing
        Dim addLabels = Sub()
                            labels = labels _
                                .Select(Function(l)
                                            If label.StringEmpty Then
                                                Return l
                                            Else
                                                Return $"{label}{delimiter}{l}"
                                            End If
                                        End Function) _
                                .Select(Function(l) l.Replace("-", ".")) _
                                .ToArray
                            gl = labels.LongestTag
                            groupLabels.Add(gl, labels)
                        End Sub

        For Each group In groups
            labels = group _
                .Select(Function(d) d.Experiment) _
                .ToArray
            addLabels()
            labels = group _
                .Select(Function(d) d.Control) _
                .ToArray
            addLabels()
        Next

        Return groupLabels
    End Function

    <ExportAPI("/Relative.amount")>
    <Usage("/Relative.amount /in <proteinGroups.csv> /designer <designer.csv> [/label <tag label> /deli <delimiter, default=_> /out <out.csv>]")>
    <Group(CLIGroups.SamplesExpressions_CLI)>
    Public Function RelativeAmount(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim designer$ = args <= "/designer"
        Dim label$ = args <= "/label"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & ".relative_amount/")
        Dim proteins = EntityObject.LoadDataSet([in])
        Dim designers As Designer() = designer.LoadCsv(Of Designer)
        Dim relativeAmounts As New List(Of EntityObject)
        Dim delimiter$ = args.GetValue("/deli", "_")
        Dim groupLabels = designers.GetExperimentGroupLabels(label, delimiter)

        Call groupLabels.GetJson(True).__DEBUG_ECHO

        Return relativeAmounts _
            .SaveTo(out & "/relative_amount.csv") _
            .CLICode
    End Function
End Module