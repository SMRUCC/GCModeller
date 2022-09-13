#Region "Microsoft.VisualBasic::340c13951bb5e68b2d202cafcd2f7cd1, GCModeller\models\Networks\KEGG\Dunnart\Components.vb"

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

    '   Total Lines: 78
    '    Code Lines: 32
    ' Comment Lines: 36
    '   Blank Lines: 10
    '     File Size: 2.21 KB


    '     Class Node
    ' 
    '         Properties: dunnartid, height, index, label, rx
    '                     ry, width, x, y
    ' 
    '     Class Link
    ' 
    '         Properties: source, target
    ' 
    '     Class Group
    ' 
    '         Properties: label, leaves, padding, style
    ' 
    '     Class Constraint
    ' 
    '         Properties: axis, offsets, type
    ' 
    '     Class NodeOffset
    ' 
    '         Properties: node, offset
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Dunnart

    Public Class Node

        ''' <summary>
        ''' 代谢物名称标签文本
        ''' </summary>
        ''' <returns></returns>
        Public Property label As String

        ''' <summary>
        ''' 与<see cref="index"/>几乎是等价的
        ''' </summary>
        ''' <returns></returns>
        Public Property dunnartid As String
        ''' <summary>
        ''' 与<see cref="dunnartid"/>几乎是等价的
        ''' </summary>
        ''' <returns></returns>
        Public Property index As Integer
        Public Property width As Double
        Public Property height As Double
        Public Property x As Double
        Public Property y As Double
        Public Property rx As Double
        Public Property ry As Double
    End Class

    Public Class Link

        ''' <summary>
        ''' <see cref="Node.index"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property source As Integer
        ''' <summary>
        ''' <see cref="Node.index"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property target As Integer
    End Class

    Public Class Group

        ''' <summary>
        ''' an array of <see cref="Node.index"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property leaves As Integer()
        Public Property style As String
        Public Property padding As Double
        Public Property label As String

    End Class

    Public Class Constraint
        ''' <summary>
        ''' always 'alignment'?
        ''' </summary>
        ''' <returns></returns>
        Public Property type As String = "alignment"
        ''' <summary>
        ''' value should be x or y
        ''' </summary>
        ''' <returns></returns>
        Public Property axis As String
        Public Property offsets As NodeOffset()
    End Class

    Public Class NodeOffset
        ''' <summary>
        ''' <see cref="Dunnart.Node.index"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property node As Integer
        Public Property offset As Double
    End Class
End Namespace
