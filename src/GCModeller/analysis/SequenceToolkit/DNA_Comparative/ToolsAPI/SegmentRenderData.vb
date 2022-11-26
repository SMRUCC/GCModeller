#Region "Microsoft.VisualBasic::f8a922e792b3f362c7793dec32bc3a65, GCModeller\analysis\SequenceToolkit\DNA_Comparative\ToolsAPI\SegmentRenderData.vb"

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

    '   Total Lines: 18
    '    Code Lines: 6
    ' Comment Lines: 12
    '   Blank Lines: 0
    '     File Size: 687 B


    ' Class SegmentRenderData
    ' 
    '     Properties: Identities, Positive, QueryId, SubjectId
    ' 
    ' /********************************************************************************/

#End Region

Public Class SegmentRenderData : Inherits SiteSigma
    ''' <summary>
    ''' 当前位点<see cref="Site"></see>上面的Query的基因号
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property QueryId As String()
    ''' <summary>
    ''' 与当前位点上面的<see cref="QueryId"></see>比对上的蛋白质的编号，假若没有比对上的记录，则为空字符串
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SubjectId As String()
    Public Property Identities As Double
    Public Property Positive As Double
End Class
