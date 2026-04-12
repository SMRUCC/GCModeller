#Region "Microsoft.VisualBasic::86523512facfda42d994c544403783cf, models\Networks\Network.Regulons\TRN.vb"

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

    '   Total Lines: 28
    '    Code Lines: 19 (67.86%)
    ' Comment Lines: 6 (21.43%)
    '    - Xml Docs: 83.33%
    ' 
    '   Blank Lines: 3 (10.71%)
    '     File Size: 1.08 KB


    ' Module TRN
    ' 
    '     Function: CorrelationNetwork
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports Microsoft.VisualBasic.Data.GraphTheory.SparseGraph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Correlations

Public Module TRN

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="expression"></param>
    ''' <param name="cutoff">An absolute value for the correlation cutoff.</param>
    ''' <returns></returns>
    <Extension>
    Public Function CorrelationNetwork(expression As IEnumerable(Of DataSet), Optional cutoff As Double = 0.65) As IEnumerable(Of Connection)
        Dim matrix As DataSet() = expression.ToArray
        Dim samples As String() = matrix.PropertyNames

        Return matrix _
            .Select(Function(gene)
                        Return gene.CorrelationImpl(matrix, samples, isSelfComparison:=True, skipIndirect:=False, cutoff:=cutoff)
                    End Function) _
            .IteratesALL _
            .Where(Function(cnn)
                       Return Math.Abs(cnn.cor) >= cutoff
                   End Function)
    End Function

    <Extension>
    Public Function CorrelationImpl(gene As DataSet, matrix As DataSet(), sampleNames$(), isSelfComparison As Boolean, skipIndirect As Boolean, cutoff#) As Connection()
        Dim fpkm As Double() = gene(sampleNames)
        Dim links As Connection() = matrix _
            .Where(Function(g)
                       If isSelfComparison Then
                           Return g.ID <> gene.ID
                       Else
                           Return True
                       End If
                   End Function) _
            .AsParallel _
            .Select(Function(g)
                        Dim fpkm2 As Double() = g(sampleNames)
                        Dim cor As Double = GetPearson(fpkm, fpkm2)

                        If Math.Abs(cor) >= cutoff AndAlso skipIndirect Then
                            Return New Connection With {
                                .cor = cor,
                                .gene1 = gene.ID,
                                .gene2 = g.ID,
                                .is_directly = True
                            }
                        Else
                            Return New Connection With {
                                .cor = Spearman(fpkm, fpkm2),
                                .gene1 = gene.ID,
                                .gene2 = g.ID,
                                .is_directly = False
                            }
                        End If
                    End Function) _
            .ToArray

        Call gene.ID.info

        Return links
    End Function
End Module

Public Class Connection
    Implements IInteraction
    Implements INetworkEdge

    Public Property gene1 As String Implements IInteraction.source
    Public Property gene2 As String Implements IInteraction.target
    Public Property is_directly As Boolean
    Public Property cor As Double Implements INetworkEdge.value
    Public Property interaction As String Implements INetworkEdge.Interaction

End Class