#Region "Microsoft.VisualBasic::bec452f37cec1ad361945efc7dd5231f, analysis\RNA-Seq\Toolkits.RNA-Seq\Correlations\IWeight.vb"

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

    ' Interface IWeightPaired
    ' 
    '     Function: GetValue
    ' 
    ' /********************************************************************************/

#End Region

Public Interface IWeightPaired
    ''' <summary>
    ''' 获取两个指定编号的基因的相关度
    ''' </summary>
    ''' <param name="Id1"></param>
    ''' <param name="Id2"></param>
    ''' <param name="Parallel"></param>
    ''' <returns></returns>
    Function GetValue(Id1 As String, Id2 As String, Optional Parallel As Boolean = True) As Double
End Interface
