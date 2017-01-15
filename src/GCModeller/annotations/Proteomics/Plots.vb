Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Analysis.GO
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Public Module Plots

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="input"></param>
    ''' <param name="obo$"></param>
    ''' <param name="p#">Default cutoff is ``-<see cref="Math.Log10(Double)"/>(<see cref="EnrichmentTerm.Pvalue"/>) &lt;= 0.05``</param>
    ''' <returns></returns>
    <Extension>
    Public Function GOEnrichmentPlot(input As IEnumerable(Of EnrichmentTerm), obo$, Optional p# = 0.05) As Bitmap
        Dim GO_terms = GO_OBO.Open(obo).ToDictionary(Function(x) x.id)
        Dim getData = Function(gene As EnrichmentTerm) (gene.ID, gene.number)
        Return input.Where(Function(x) x.P <= p#).Plot(getData, GO_terms)
    End Function

    <Extension>
    Public Function VennData(files As IEnumerable(Of String)) As DocumentStream.File
        Dim datas = files.ToDictionary(Function(f) f.BaseName, Function(f) f.LoadSample.ToDictionary)
        Dim out As New DocumentStream.File
        Dim keys$() = datas.Keys.ToArray
        Dim ALL_locus$() = datas _
            .Select(Function(x) x.Value.Select(Function(o) o.Value.ID)) _
            .Unlist _
            .Distinct _
            .OrderBy(Function(s) s) _
            .ToArray

        Call out.AppendLine(keys)

        For Each ID As String In ALL_locus
            Dim row As New List(Of String)

            For Each sample In keys
                If datas(sample).ContainsKey(ID) Then
                    row += ID
                Else
                    row += ""
                End If
            Next

            Call out.Add(row)
        Next

        Return out
    End Function
End Module
