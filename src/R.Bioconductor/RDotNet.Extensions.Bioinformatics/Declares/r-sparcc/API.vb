#Region "Microsoft.VisualBasic::b6ea3dc56d4ac0b88e493d9b05161510, RDotNet.Extensions.Bioinformatics\Declares\r-sparcc\API.vb"

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

    '     Module API
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: sparcc
    ' 
    '     Class Correlations
    ' 
    '         Properties: CORR, COV, VBASIS
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNet.Extensions.VisualBasic.RSystem
Imports RDotNet.Extensions.VisualBasic
Imports Microsoft.VisualBasic.Language

Namespace sparcc

    ''' <summary>
    ''' R package computes correlation for relative abundances
    ''' </summary>
    Public Module API

        ''' <summary>
        ''' 装载计算脚本
        ''' </summary>
        Sub New()
            ' Call R.Evaluate(My.Resources.sparcc)
        End Sub

        ''' <summary>
        ''' count matrix x should be samples on the rows and OTUs on the colums,
        ''' 
        ''' ```R
        ''' assuming dim(x) -> samples by OTUs
        ''' ```
        ''' </summary>
        ''' <param name="x">The data file path</param>
        ''' <param name="maxIter"></param>
        ''' <param name="th"></param>
        ''' <param name="exiter"></param>
        ''' <returns></returns>
        Public Function sparcc(x As String, Optional maxIter As Integer = 20, Optional th As Double = 0.1, Optional exiter As Double = 10) As Correlations
            Dim out As String() = R.WriteLine($"tab <- read.table(""{x.UnixPath}"",header=T);")
            Dim result = R.Evaluate($"thisX <- sparcc(tab, {maxIter}, {th}, {exiter});")
            Dim list = result.AsList.ToArray
            Dim i As New Pointer
            Dim corr As Double() = list(++i).AsNumeric.ToArray
            Dim cov As Double() = list(++i).AsNumeric.ToArray
            Dim vbasis As Double() = list(++i).AsNumeric.ToArray

            Return New Correlations With {
                .CORR = corr.Split(vbasis.Length),
                .COV = cov.Split(vbasis.Length),
                .VBASIS = vbasis
            }
        End Function
    End Module

    Public Class Correlations

        Public Property CORR As Double()()
        Public Property COV As Double()()
        Public Property VBASIS As Double()
    End Class
End Namespace
