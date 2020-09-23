#Region "Microsoft.VisualBasic::d8519a5c6ed0113bd6a1224556f51b37, models\Networks\Network.Regulons\TRN.vb"

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

    ' Module TRN
    ' 
    '     Function: CorrelationNetwork
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.Microarray

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
End Module
