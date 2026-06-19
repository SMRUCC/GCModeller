Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
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
    <RApiReturn(GetType(PreparedData))>
    Public Function preparedDataTool(<RListObjectArgument> data As list, Optional env As Environment = Nothing) As Object
        Dim i As i32 = 1
        Dim matrices As New List(Of Double(,))
        Dim sampleIDLists As New List(Of String())
        Dim omicsNames As New List(Of String)

        ' ====================================================================
        ' 3. 数据预处理（处理样本不匹配）
        ' ====================================================================
        Console.WriteLine("[1] 数据预处理（对齐样本、生成掩码、归一化）...")
        Console.WriteLine()

        For Each name As String In data.getNames
            Dim val As Matrix = TryCast(data.getByName(name), Matrix)

            If Not val Is Nothing Then
                matrices.Add(val.AsTensorArray)
                sampleIDLists.Add(val.sampleID)
                omicsNames.Add(name)

                Console.WriteLine($"  组学{++i} ({name}): {val.sample_count} 个样本 × {val.size} 个特征")
            End If
        Next

        Dim unionSampleIDs As String() = sampleIDLists.IteratesALL.Distinct.OrderBy(Function(id) id).ToArray
        Dim missingIDs As New List(Of String())

        For idx As Integer = 0 To sampleIDLists.Count - 1
            Dim currentSamples = sampleIDLists(idx)
            Console.WriteLine($"  组学{idx + 1}样本: {sampleIDLists(i).JoinBy(", ")}")
            missingIDs.Add(unionSampleIDs.Where(Function(id) Array.IndexOf(currentSamples, id) < 0).ToArray)
        Next

        i = 1

        For Each row In missingIDs
            If row.Any Then
                Console.WriteLine($"  缺失样本: {row.JoinBy(", ")} 在组学{i}中缺失")
            End If

            i = i + 1
        Next

        Console.WriteLine()

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

        Return preparedData
    End Function
End Module
