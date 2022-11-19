#Region "Microsoft.VisualBasic::0ef9389361f362657a3bba73f1ff4de8, GCModeller\annotations\GSEA\GSEA\KOBAS\Formats\Gct.vb"

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
    '    Code Lines: 65
    ' Comment Lines: 5
    '   Blank Lines: 13
    '     File Size: 3.07 KB


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
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports tsv = Microsoft.VisualBasic.Data.csv.IO.File

Namespace KOBAS

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
                Return genes(Scan0).EnumerateKeys.Length
            End Get
        End Property

        Public Property genes As GeneExpression()

        Public Class GeneExpression : Inherits DataSet

            Public Property Description As String

            Public Overrides Function ToString() As String
                Return MyBase.ToString()
            End Function
        End Class

        Public Shared Function LoadFile(path As String) As Gct
            Dim table = tsv.LoadTsv(path, Encodings.UTF8)
            Dim version = table.First()(Scan0).Trim("#"c)
            Dim size = table(1)
            Dim geneNumber As Integer = size(0)
            Dim sampleNumber As Integer = size(1)
            Dim sampleNames = table(2).Skip(2).ToArray
            Dim getGenes As GeneExpression() =
                Iterator Function() As IEnumerable(Of GeneExpression)
                    For Each row As RowObject In table.Skip(3)
                        Yield New GeneExpression With {
                            .ID = row(Scan0),
                            .Description = row(1),
                            .Properties = row _
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
