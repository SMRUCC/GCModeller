Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Metagenome

<Package("OTU_table")>
Module OTUTableTools

    <ExportAPI("relative_abundance")>
    Public Function relativeAbundance(x As OTUTable()) As OTUTable()
        Dim sample_ids As String() = x _
            .Select(Function(otu) otu.Properties.Keys) _
            .IteratesALL _
            .Distinct _
            .ToArray
        Dim v As Vector

        For Each name As String In sample_ids
            v = x.Select(Function(otu) otu(name)).AsVector
            v = v / v.Sum

            For i As Integer = 0 To x.Length - 1
                x(i)(name) = v(i)
            Next
        Next

        Return x
    End Function

    <ExportAPI("filter")>
    Public Function filter(x As OTUTable(), relative_abundance As Double) As OTUTable()
        Return x _
            .Where(Function(otu)
                       Return otu.Properties _
                          .Values _
                          .Any(Function(xi)
                                   Return xi > relative_abundance
                               End Function)
                   End Function) _
            .ToArray
    End Function
End Module
