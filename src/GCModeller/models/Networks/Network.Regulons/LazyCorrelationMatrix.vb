#Region "Microsoft.VisualBasic::a96e32bcbdbf6828836c4ecb2685700e, models\Networks\Network.Regulons\LazyCorrelationMatrix.vb"

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

    '   Total Lines: 46
    '    Code Lines: 27 (58.70%)
    ' Comment Lines: 10 (21.74%)
    '    - Xml Docs: 80.00%
    ' 
    '   Blank Lines: 9 (19.57%)
    '     File Size: 1.43 KB


    ' Class LazyCorrelationMatrix
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Correlation
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Math.Correlations
Imports Microsoft.VisualBasic.Math.Matrix
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Public Class LazyCorrelationMatrix

    ''' <summary>
    ''' the normalized expression matrix data
    ''' </summary>
    ReadOnly expr As Matrix

    ReadOnly cor As New NamedSparseMatrix
    ReadOnly pval As New NamedSparseMatrix

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="expr">
    ''' the normalized expression matrix data
    ''' </param>
    Sub New(expr As Matrix)
        Me.expr = expr
    End Sub

    Public Function Correlation(gene1 As String, gene2 As String) As (cor As Double, pval As Double)
        If Not cor.CheckElement(gene1, gene2) Then
            Dim c As Double, p As Double
            Dim v1 = expr(gene1)
            Dim v2 = expr(gene2)

            ' no correlation result for missing data
            If v1 Is Nothing OrElse v2 Is Nothing Then
                Return (0, 1)
            End If

            c = Correlations.GetPearson(v1.experiments, v2.experiments, p, throwMaxIterError:=False)

            Call cor.SetValue(gene1, gene2, c)
            Call cor.SetValue(gene2, gene1, c)
            Call pval.SetValue(gene1, gene2, p)
            Call pval.SetValue(gene2, gene1, p)
        End If

        Return (cor(gene1, gene2), pval(gene1, gene2))
    End Function
End Class

