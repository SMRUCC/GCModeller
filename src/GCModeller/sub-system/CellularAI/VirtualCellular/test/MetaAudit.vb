Imports System
Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports SMRUCC.genomics.Analysis.VirtualCellular.Data
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

''' <summary>
''' 细胞元数据审计：<c>SampleInfo</c> 反序列化、字段可达性、期别统计与"NA 是否等于正常"的判定。
''' </summary>
Public Module MetaAudit

    Private Const ReportName As String = "virtualcellular-metadata-report.json"

    ''' <summary>默认的元数据路径（可用命令行参数覆盖）。</summary>
    Public Const DefaultPath As String = "E:\hyperomics_24samples\sampleinfo.csv"

    ''' <summary>审计入口。</summary>
    Public Function Run(Optional metaPathArg As String = Nothing) As Integer
        Dim metaPath As String = If(String.IsNullOrWhiteSpace(metaPathArg), DefaultPath, metaPathArg)
        Dim reportPath As String = Path.Combine(AppContext.BaseDirectory, ReportName)

        Call DebugAudit.Reset()
        DebugAudit.MaxEvents = 4096
        Call DebugAudit.Mark("metadata audit started")
        Call DebugAudit.Mark("metadata.path", metaPath)

        Try
            If Not File.Exists(metaPath) Then
                Call DebugAudit.Test(False, $"the metadata file does not exist: {metaPath}")
            Else
                Call CheckMetadata(metaPath)
            End If
        Catch ex As Exception
            Call DebugAudit.Mark("metadata.audit.crash", ex.ToString())
            Call DebugAudit.Test(False, $"the metadata audit should not throw unhandled exceptions: {ex.Message}")
        End Try

        Call DebugAudit.SaveReport(reportPath)
        Call Console.WriteLine($"   [audit] report -> {reportPath}")
        Call Console.WriteLine($"   [audit] {DebugAudit.FailureCount} failure(s)")

        Return DebugAudit.FailureCount
    End Function

    Private Sub CheckMetadata(metaPath As String)
        Dim clock As Diagnostics.Stopwatch = Diagnostics.Stopwatch.StartNew()
        Dim metadata As CellMetadata = CellMetadata.Load(metaPath)

        clock.Stop()

        Call DebugAudit.Mark("metadata.rows", metadata.Count)
        Call DebugAudit.Mark("metadata.elapsed_ms", clock.ElapsedMilliseconds)

        DebugAudit.Test(metadata.Count > 100000,
                        $"the NPC metadata should hold every cell, got {metadata.Count}")

        ' 强类型字段与 metadata 字典都要可达
        Dim first As SampleInfo = metadata.Samples(0)

        DebugAudit.Test(Not String.IsNullOrEmpty(first.ID), "the ID column maps onto SampleInfo.ID")
        DebugAudit.Test(Not String.IsNullOrEmpty(first.sample_name), "the sample_name column maps onto SampleInfo.sample_name")

        Dim stage As String = metadata.StageOf(first.ID)
        Dim patient As String = metadata.Field(first.ID, "patient_id")

        Call DebugAudit.Mark("metadata.first_barcode", first.ID)
        Call DebugAudit.Mark("metadata.first_stage", stage)
        Call DebugAudit.Mark("metadata.first_patient", patient)

        DebugAudit.Test(Not String.IsNullOrEmpty(patient), "the patient_id column lands in the metadata dictionary")
        DebugAudit.Test(CellMetadata.IsMissingStage(stage), $"the first cell of NPC41 is un-staged, got '{stage}'")

        ' 期别分组与交叉统计
        Dim stages As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

        For Each info As SampleInfo In metadata.Samples
            Dim key As String = If(metadata.Field(info.ID, "stage"), "")

            If String.IsNullOrEmpty(key) Then
                key = "<empty>"
            End If

            Dim count As Integer = 0

            Call stages.TryGetValue(key, count)
            stages(key) = count + 1
        Next

        For Each pair As KeyValuePair(Of String, Integer) In stages
            Call DebugAudit.Mark($"metadata.stage[{pair.Key}]", pair.Value)
        Next

        Dim naive As Boolean = stages.ContainsKey("NA")
        Dim selector As String = metadata.NormalSelector

        Call DebugAudit.Mark("metadata.normal_selector", selector)
        DebugAudit.Test(selector = "stage=NA" OrElse selector = "type=normal",
                        $"the normal selector must be one of the two documented modes, got '{selector}'")

        If naive Then
            DebugAudit.Test(stages("NA") > 0, "the NA stage carries cells")
        End If

        Dim crossTab As String = metadata.CrossTab()

        Call DebugAudit.Mark("metadata.crosstab_lines", crossTab.Split(ControlChars.Lf).Length)
        Call Console.WriteLine("   [metadata] stage x type x group")
        For Each line As String In crossTab.Split(ControlChars.Lf)
            Call Console.WriteLine("     " & line)
        Next

        DebugAudit.Test(metadata.NormalBarcodes().Length > 0, "the normal reference is not empty")
        Call DebugAudit.Mark("metadata.normal_cells", metadata.NormalBarcodes().Length)
        Call DebugAudit.Mark("metadata.stage3_cells", metadata.BarcodesOfStage("III").Length)
        Call DebugAudit.Mark("metadata.stage4b_cells", metadata.BarcodesOfStage("IVB").Length)
        Call DebugAudit.Mark("metadata.stage4c_cells", metadata.BarcodesOfStage("IVC").Length)
        Call DebugAudit.Mark("metadata.missing_probe", metadata.MissingBarcodes({"not-a-barcode"}).Length)

        Call DebugAudit.AreEqual(1, metadata.MissingBarcodes({"not-a-barcode"}).Length,
                                 "an unknown barcode is reported as missing metadata")
    End Sub

End Module
