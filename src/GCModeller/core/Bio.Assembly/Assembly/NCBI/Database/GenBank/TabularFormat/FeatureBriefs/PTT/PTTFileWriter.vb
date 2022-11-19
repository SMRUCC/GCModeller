#Region "Microsoft.VisualBasic::173362a1c56d5b8b76fc8047f9cc7b79, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\TabularFormat\FeatureBriefs\PTT\PTTFileWriter.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 63
    '    Code Lines: 54
    ' Comment Lines: 0
    '   Blank Lines: 9
    '     File Size: 2.61 KB


    '     Module PTTFileWriter
    ' 
    '         Function: GetText
    ' 
    '         Sub: (+2 Overloads) WriteDocument, WriteGeneLines, WriteTabular
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
