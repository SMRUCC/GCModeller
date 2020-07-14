Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace Assembly.NCBI.GenBank.TabularFormat

    Public Module PTTFileWriter

        <Extension>
        Public Sub WriteGeneLines(genes As IEnumerable(Of GeneBrief), output As TextWriter)
            For Each line As String In From gene As GeneBrief
                                       In genes
                                       Let strandCode As String = If(gene.Location.Strand = Strands.Forward, "+", "-")
                                       Select String.Format("{0}..{1}	{2}	{3}	{4}	{5}	{6}	{7}	{8}	{9}",
                                           gene.Location.left,
                                           gene.Location.right,
                                           strandCode,
                                           gene.Length,
                                           gene.PID,
                                           gene.Gene,
                                           gene.Synonym,
                                           gene.Code,
                                           gene.COG,
                                           gene.Product
                                       )

                Call output.WriteLine(line)
            Next
        End Sub

        <Extension>
        Public Sub WriteDocument(genomics As PTT, output As StringBuilder)
            Call genomics.WriteDocument(New StringWriter(output))
        End Sub

        <Extension>
        Public Sub WriteDocument(genomics As PTT, output As TextWriter)
            Call output.WriteLine(genomics.Title & String.Format(" - 1..{0}", genomics.Size))

            Call output.WriteLine()
            Call output.WriteLine(genomics.NumOfProducts & " proteins")
            Call output.WriteTabular(genomics.GeneObjects)
            Call output.Flush()
        End Sub

        <Extension>
        Public Sub WriteTabular(output As TextWriter, genes As IEnumerable(Of GeneBrief))
            Call output.WriteLine({"Location", "Strand", "Length", "PID", "Gene", "Synonym", "Code", "COG", "Product"}.JoinBy(vbTab))
            Call genes.WriteGeneLines(output)
        End Sub

        <Extension>
        Public Function GetText(genomics As PTT) As String
            With New StringBuilder
                Call .DoCall(AddressOf genomics.WriteDocument)
                Return .ToString
            End With
        End Function
    End Module
End Namespace