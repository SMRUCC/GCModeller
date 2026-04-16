#Region "Microsoft.VisualBasic::47169cdec38ffee7230b9a83081c2118, engine\IO\Raw\VCellMatrix\VCellMatrix.vb"

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

    '   Total Lines: 24
    '    Code Lines: 9 (37.50%)
    ' Comment Lines: 11 (45.83%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 4 (16.67%)
    '     File Size: 747 B


    ' Class VCellMatrix
    ' 
    '     Properties: cellularGraph, compartmentIds, expressionData, fluxomics, modules
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

''' <summary>
''' the in-memory model of the virtual cell result data pack
''' </summary>
Public Class VCellMatrix

    ''' <summary>
    ''' the molecule expression data
    ''' </summary>
    ''' <returns></returns>
    Public Property expressionData As DataFrameRow()
    ''' <summary>
    ''' the biological process activity expression data
    ''' </summary>
    ''' <returns></returns>
    Public Property fluxomics As DataFrameRow()

    Public Property cellularGraph As FluxEdge()
    Public Property modules As Dictionary(Of String, String())
    Public Property compartmentIds As String()

End Class

