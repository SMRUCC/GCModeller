#Region "Microsoft.VisualBasic::12e6217702ab74a250b1dddb743633d0, annotations\Proteomics\DEP.vb"

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

    '   Total Lines: 25
    '    Code Lines: 16 (64.00%)
    ' Comment Lines: 5 (20.00%)
    '    - Xml Docs: 80.00%
    ' 
    '   Blank Lines: 4 (16.00%)
    '     File Size: 1003 B


    ' Class DEP_iTraq
    ' 
    '     Properties: FDR, foldchange, ID, isDEP, log2FC
    '                 pvalue
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Math.Statistics
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

''' <summary>
''' iTraq的DEP分析结果输出的文件数据读取对象
''' 
''' > https://github.com/xieguigang/GCModeller.cli2R/blob/master/R/iTraq.log2_t-test.R
''' </summary>
Public Class DEP_iTraq : Inherits EntityObject
    Implements IDeg
    Implements IStatPvalue

    Public Overrides Property ID As String Implements IDeg.label

    <Column("FC.avg")> Public Property foldchange As Double
    <Column("p.value")> Public Property pvalue As Double Implements IDeg.pvalue, IStatPvalue.pValue
    <Column("is.DEP")> Public Property isDEP As Boolean
    <Column("log2FC")> Public Property log2FC As Double Implements IDeg.log2FC
    <Column("FDR")> Public Property FDR As Double

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
