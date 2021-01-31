﻿#Region "Microsoft.VisualBasic::72b04d54da84619a13ba75b7e2adad09, annotations\Proteomics\DEP.vb"

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

    ' Class DEP_iTraq
    ' 
    '     Properties: FCavg, FDR, ID, isDEP, log2FC
    '                 pvalue
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Visualize

''' <summary>
''' iTraq的DEP分析结果输出的文件数据读取对象
''' 
''' > https://github.com/xieguigang/GCModeller.cli2R/blob/master/R/iTraq.log2_t-test.R
''' </summary>
Public Class DEP_iTraq : Inherits EntityObject
    Implements IDeg

    Public Overrides Property ID As String Implements IDeg.label

    <Column("FC.avg")> Public Property FCavg As Double
    <Column("p.value")> Public Property pvalue As Double Implements IDeg.pvalue
    <Column("is.DEP")> Public Property isDEP As Boolean
    <Column("log2FC")> Public Property log2FC As Double Implements IDeg.log2FC
    <Column("FDR")> Public Property FDR As Double

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
