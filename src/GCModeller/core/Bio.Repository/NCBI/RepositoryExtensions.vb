Imports SMRUCC.genomics.Assembly.NCBI.GenBank

Namespace NCBI

    Public Module RepositoryExtensions

        ''' <summary>
        ''' 将非``plasmid``的基因组序列从指定的Genbank文件<paramref name="gb"/>之中拿出来
        ''' </summary>
        ''' <param name="gb"></param>
        ''' <returns></returns>
        Public Function GetGenomeData(gb As String) As GBFF.File
            Return GBFF.File _
                .LoadDatabase(filePath:=gb) _
                .Where(Function(g)
                           Return InStr(g.Definition.Value, "plasmid", CompareMethod.Text) = 0
                       End Function) _
                .FirstOrDefault
        End Function
    End Module
End Namespace