#Region "Microsoft.VisualBasic::6555d5491914ecf46b030a35d3fb324d, annotations\GSEA\GSEA\KOBAS\Formats\Gct.vb"

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

    '   Total Lines: 83
    '    Code Lines: 65 (78.31%)
    ' Comment Lines: 5 (6.02%)
    '    - Xml Docs: 80.00%
    ' 
    '   Blank Lines: 13 (15.66%)
    '     File Size: 3.16 KB


    '     Class Gct
    ' 
    '         Properties: genes, numberOfgenes, numberOfsamples, version
    ' 
    '         Function: GetEnumerator, IEnumerable_GetEnumerator, LoadFile
    ' 
    '         Class GeneExpression
    ' 
    '             Properties: Description
    ' 
    '             Function: ToString
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text

Namespace Formats

    ''' <summary>
    ''' #### GCT: Gene Cluster Text file format (``*.gct``)
    ''' 
    ''' The GCT format is a tab delimited file format that describes an expression dataset.
    ''' </summary>
    Public Class Gct : Implements IEnumerable(Of GeneExpression)

        Public Property version As String

        Public ReadOnly Property numberOfgenes As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return genes.Length
            End Get
        End Property

        Public ReadOnly Property numberOfsamples As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return genes(Scan0).Expression.Count
            End Get
        End Property

        Public Property genes As GeneExpression()

        Public ReadOnly Property sampleSet As String()
            Get
                Return genes.SelectMany(Function(e) e.Expression.Keys).Distinct.ToArray
            End Get
        End Property

        Public Class GeneExpression

            Public Property Id As String
            Public Property Description As String
            Public Property Expression As Dictionary(Of String, Double)

            Public Overrides Function ToString() As String
                Return MyBase.ToString()
            End Function
        End Class

        Public Function AsMatrix() As Matrix
            Dim sampleSet As String() = Me.sampleSet

            Return New Matrix With {
                .tag = version,
                .sampleID = sampleSet,
                .expression = genes _
                    .Select(Function(e)
                                Return New DataFrameRow With {
                                    .geneID = e.Id,
                                    .experiments = e.Expression _
                                        .Selects(sampleSet) _
                                        .ToArray
                                }
                            End Function) _
                    .ToArray
            }
        End Function

        Public Shared Function LoadFile(path As String) As Gct
            Dim table = Document.ReadTable(path, ASCII.TAB, Encodings.UTF8).ToArray
            Dim version = table.First()(Scan0).Trim("#"c)
            Dim size = table(1)
            Dim geneNumber As Integer = size(0)
            Dim sampleNumber As Integer = size(1)
            Dim sampleNames = table(2).Skip(2).ToArray
            Dim getGenes As GeneExpression() = Iterator Function() As IEnumerable(Of GeneExpression)
                                                   For Each row As String() In table.Skip(3)
                                                       Yield New GeneExpression With {
                                                            .Id = row(Scan0),
                                                            .Description = row(1),
                                                            .Expression = row _
                                                                .Skip(2) _
                                                                .SeqIterator _
                                                                .ToDictionary(Function(i) sampleNames(i),
                                                                              Function(x)
                                                                                  Return Val(x.value)
                                                                              End Function)
                                                        }
                                                   Next
                                               End Function().ToArray
            Return New Gct With {
                .version = version,
                .genes = getGenes
            }
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of GeneExpression) Implements IEnumerable(Of GeneExpression).GetEnumerator
            For Each gene As GeneExpression In genes
                Yield gene
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace
