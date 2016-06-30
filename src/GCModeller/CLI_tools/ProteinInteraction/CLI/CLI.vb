Imports LANS.SystemsBiology.AnalysisTools.ProteinTools.Interactions.SwissTCS
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic

<PackageNamespace("Protein.Interactions.Tools", Category:=APICategories.CLI_MAN,
                  Description:="Tools for analysis the protein interaction relationship.",
                  Publisher:="xie.guigang@gcmodeller.org",
                  Url:="http://gcmodeller.org")>
Public Module CLI

    <ExportAPI("--interact.TCS", Usage:="--interact.TCS /door <door.opr> /MiST2 <mist2.xml> /swiss <tcs.csv.DIR> /out <out.DIR>")>
    Public Function TCSParser(args As CommandLine.CommandLine) As Integer
        Dim MiST2 = args("/mist2").LoadXml(Of LANS.SystemsBiology.Assembly.MiST2.MiST2)  ' 主要是从这个模块之中获取TCS的基因定义
        Dim Door = LANS.SystemsBiology.Assembly.Door.Load(args("/door"))
        Dim cTkDIR As String = args("/swiss")
        Dim outDIR As String = args.GetValue("/out", App.CurrentDirectory)
        Dim CrossTalks = FileIO.FileSystem.GetFiles(cTkDIR, FileIO.SearchOption.SearchAllSubDirectories, "*.csv") _
            .ToArray(Function(csv) csv.LoadCsv(Of CrossTalks)).MatrixToList

        For Each rep As Assembly.MiST2.Replicon In MiST2.MajorModules

            Dim lstHisk As String() = rep.TwoComponent.get_HisKinase
            Dim lstRR As String() = rep.TwoComponent.GetRR

            For Each HisK As String In lstHisk
                Dim lstChunk As New List(Of CrossTalks)

                For Each RR As String In lstRR

                    Dim p As Double = CrossTalks.CrossTalk(HisK, RR)

                    If Door.SameOperon(HisK, RR) Then  ' 同源的？？？
                        If Not p > 0 Then
                            p = 1
                        End If

                        Call lstChunk.Add(New CrossTalks With {.Kinase = HisK, .Regulator = RR, .Probability = p})
                    Else
                        If p > 0 Then
                            Call lstChunk.Add(New CrossTalks With {.Kinase = HisK, .Regulator = RR, .Probability = p})
                        End If
                    End If
                Next

                If Not lstChunk.IsNullOrEmpty Then
                    Call lstChunk.SaveTo(outDIR & $"/{HisK}.csv")
                End If
            Next
        Next

        Return 0
    End Function
End Module
