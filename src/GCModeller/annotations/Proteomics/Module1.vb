Imports Microsoft.VisualBasic.Data.csv
Imports SMRUCC.genomics.Analysis.Microarray

Module Module1

    Sub Main()

        ' 1. 总蛋白注释
        'Call "C:\Users\xieguigang\OneDrive\1.17\2. annotations\uniprot.txt" _
        '    .ReadAllLines _
        '    .GenerateAnnotations("C:\Users\xieguigang\OneDrive\1.17\2. annotations\uniprot.xml") _
        '    .Select(Function(x) x.Item1) _
        '    .ToArray _
        '    .SaveDataSet("C:\Users\xieguigang\OneDrive\1.17\2. annotations\sample-annotation.csv")

        ' Pause()

        ' 2. DEP注释
        'Call "C:\Users\xieguigang\OneDrive\1.17\4. analysis\C-T.txt" _
        '    .ReadAllLines _
        '    .GenerateAnnotations("C:\Users\xieguigang\OneDrive\1.17\2. annotations\uniprot.xml") _
        '    .Select(Function(x) x.Item1) _
        '    .ToArray _
        '    .SaveDataSet("C:\Users\xieguigang\OneDrive\1.17\4. analysis\C-T.annotations.csv")

        'Call "C:\Users\xieguigang\OneDrive\1.17\4. analysis\WT-KO.txt" _
        ' .ReadAllLines _
        ' .GenerateAnnotations("C:\Users\xieguigang\OneDrive\1.17\2. annotations\uniprot.xml") _
        ' .Select(Function(x) x.Item1) _
        ' .ToArray _
        ' .SaveDataSet("C:\Users\xieguigang\OneDrive\1.17\4. analysis\WT-KO.annotations.csv")

        'Pause()
        ' 3. heatmap绘图

        'Call DEGDesigner _
        '    .MergeMatrix("C:\Users\xieguigang\OneDrive\1.17\3. DEP\heatmap", "*.csv", 1.25, 0.05, "FC.avg", 1 / 1.25, "p.value") _
        '    .SaveDataSet("C:\Users\xieguigang\OneDrive\1.17\3. DEP\DEP.heatmap.csv", blank:=0)

        ' 4。 导出KOBAS结果
        Call KOBAS.SplitData("C:\Users\xieguigang\OneDrive\1.17\4. analysis\enrichment\KOBAS\C-T.txt")
        Call KOBAS.SplitData("C:\Users\xieguigang\OneDrive\1.17\4. analysis\enrichment\KOBAS\WT-KO.txt")
    End Sub
End Module
