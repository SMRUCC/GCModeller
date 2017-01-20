Imports Microsoft.VisualBasic.Data.csv

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
        Call "C:\Users\xieguigang\OneDrive\1.17\4. analysis\C-T.txt" _
            .ReadAllLines _
            .GenerateAnnotations("C:\Users\xieguigang\OneDrive\1.17\2. annotations\uniprot.xml") _
            .Select(Function(x) x.Item1) _
            .ToArray _
            .SaveDataSet("C:\Users\xieguigang\OneDrive\1.17\4. analysis\C-T.annotations.csv")

        Call "C:\Users\xieguigang\OneDrive\1.17\4. analysis\WT-KO.txt" _
         .ReadAllLines _
         .GenerateAnnotations("C:\Users\xieguigang\OneDrive\1.17\2. annotations\uniprot.xml") _
         .Select(Function(x) x.Item1) _
         .ToArray _
         .SaveDataSet("C:\Users\xieguigang\OneDrive\1.17\4. analysis\WT-KO.annotations.csv")

        Pause()
    End Sub
End Module
