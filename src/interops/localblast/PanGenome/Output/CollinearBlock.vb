#Region "Microsoft.VisualBasic::806b5bff00352fec7bbcedb13355c686, localblast\PanGenome\Output\CollinearBlock.vb"

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

    '   Total Lines: 33
    '    Code Lines: 18 (54.55%)
    ' Comment Lines: 11 (33.33%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 4 (12.12%)
    '     File Size: 884 B


    ' Class CollinearBlock
    ' 
    '     Properties: Chr1, Chr2, Genome1, Genome2, OrthologyLinks
    '                 Score
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' 共线性区块定义
''' </summary>
Public Class CollinearBlock

    Public Property Genome1 As String
    Public Property Genome2 As String
    Public Property Chr1 As String
    Public Property Chr2 As String
    ''' <summary>
    ''' 区块包含的基因对
    ''' </summary>
    ''' <returns></returns>
    Public Property OrthologyLinks As OrthologyLink()
    ''' <summary>
    ''' TODO: 评估指标：得分或E-value
    ''' </summary>
    ''' <returns></returns>
    Public Property Score As Double

    Sub New()
    End Sub

    Friend Sub New(source As CollinearBlock, links As IEnumerable(Of OrthologyLink))
        Genome1 = source.Genome1
        Genome2 = source.Genome2
        Chr1 = source.Chr1
        Chr2 = source.Chr2
        OrthologyLinks = links.ToArray
        Score = source.Score
    End Sub

End Class
