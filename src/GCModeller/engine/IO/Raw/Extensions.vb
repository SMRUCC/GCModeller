#Region "Microsoft.VisualBasic::3745fe50a91753189ad633d77a498ba7, GCModeller\engine\IO\Raw\Extensions.vb"

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

    '   Total Lines: 27
    '    Code Lines: 18
    ' Comment Lines: 5
    '   Blank Lines: 4
    '     File Size: 756 B


    ' Module Extensions
    ' 
    '     Function: GetMatrix, LoadCDF
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.DataStorage.netCDF

<HideModuleName>
Public Module Extensions

    <Extension>
    Public Iterator Function GetMatrix(raw As Raw.Reader, module$) As IEnumerable(Of DataSet)
        For Each time As Double In raw.AllTimePoints
            Yield New DataSet With {
                .ID = time,
                .Properties = raw.Read(time, [module])
            }
        Next
    End Function

    ''' <summary>
    ''' 读取netCDF格式的模拟计算结果数据文件
    ''' </summary>
    ''' <param name="cdf"></param>
    ''' <returns></returns>
    <Extension>
    Public Function LoadCDF(cdf As netCDFReader)

    End Function
End Module
