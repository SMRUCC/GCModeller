Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.Microarray.IMMO
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports std = System.Math

''' <summary>
''' IMMO (Integration Model for Incomplete Multi-Omics)
''' </summary>
<Package("IMMO")>
<RTypeExport("immo_pars", GetType(IMMOConfig))>
Module IMMOTool

    <ExportAPI("prepared_data")>
    Public Function preparedDataTool(<RListObjectArgument> data As list, Optional env As Environment = Nothing) As Object
        Dim i As i32 = 1
        Dim matrices As New List(Of Double(,))

        For Each name As String In data.getNames
            Dim val As Matrix = TryCast(data.getByName(name), Matrix)

            If Not val Is Nothing Then
                matrices.Add(val. )
                Console.WriteLine($"  组学{++i} ({name}): {val.sample_count} 个样本 × {val.size} 个特征")
            End If
        Next


        Console.WriteLine($"  组学2 (蛋白质组): {omics2Samples.Length} 个样本 × {omics2Data.GetLength(1)} 个特征")
        Console.WriteLine($"  组学1样本: {String.Join(", ", omics1Samples)}")
        Console.WriteLine($"  组学2样本: {String.Join(", ", omics2Samples)}")
        Console.WriteLine($"  缺失样本: S01, S08 在组学2中缺失")
        Console.WriteLine()

        ' ====================================================================
        ' 3. 数据预处理（处理样本不匹配）
        ' ====================================================================
        Console.WriteLine("[3] 数据预处理（对齐样本、生成掩码、归一化）...")
        Console.WriteLine()

        Dim matrices As New List(Of Double(,)) From {omics1Data, omics2Data}
        Dim sampleIDLists As New List(Of String()) From {omics1Samples, omics2Samples}
        Dim omicsNames As String() = {"Transcriptome", "Proteome"}

        Dim preparedData As PreparedData = DataPrep.PrepareData(
            matrices, sampleIDLists, omicsNames, normalize:=True)

        Console.WriteLine($"  统一样本数: {preparedData.UnifiedSampleIDs.Length}")
        Console.WriteLine($"  统一样本ID: {String.Join(", ", preparedData.UnifiedSampleIDs)}")
        Console.WriteLine()

        For Each omics In preparedData.OmicsList
            Dim observedCount = 0
            For j = 0 To omics.Mask.Length - 1
                If omics.Mask(j) > 0 Then observedCount += 1
            Next
            Dim totalEntries = omics.NumSamples * omics.NumFeatures
            Console.WriteLine($"  {omics.Name}:")
            Console.WriteLine($"    矩阵维度: {omics.NumSamples} × {omics.NumFeatures}")
            Console.WriteLine($"    观测值: {observedCount}/{totalEntries} ({observedCount * 100.0 / totalEntries:F1}%)")
            Console.WriteLine($"    缺失值: {totalEntries - observedCount}/{totalEntries} ({(totalEntries - observedCount) * 100.0 / totalEntries:F1}%)")
        Next
        Console.WriteLine()
    End Function
End Module
