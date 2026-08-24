#Region "Microsoft.VisualBasic::674853c760a426767cbe257af12e7025, RNA-Seq\RNA-seq.Data\Quantification\GeneData.vb"

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

    '   Total Lines: 20
    '    Code Lines: 11 (55.00%)
    ' Comment Lines: 3 (15.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 6 (30.00%)
    '     File Size: 566 B


    '     Class GeneData
    ' 
    '         Properties: FPKM, GeneID, Length, RawCount, RPK
    '                     TPM
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel

Namespace GeneQuantification

    ''' <summary>
    ''' gene abundance result
    ''' </summary>
    Public Class GeneData : Implements IExpressionValue

        Public Property GeneID As String Implements IExpressionValue.Identity
        Public Property Length As Double
        Public Property RawCount As Double
        Public Property RPK As Double
        Public Property TPM As Double Implements IExpressionValue.ExpressionValue
        Public Property FPKM As Double

    End Class
End Namespace



