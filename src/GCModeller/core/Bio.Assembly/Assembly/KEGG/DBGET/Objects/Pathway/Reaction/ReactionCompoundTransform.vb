#Region "Microsoft.VisualBasic::3af735fdf8ffc177d4a6edd9f534d205, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\Reaction\ReactionCompoundTransform.vb"

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

    '   Total Lines: 26
    '    Code Lines: 10
    ' Comment Lines: 11
    '   Blank Lines: 5
    '     File Size: 765 B


    '     Class ReactionCompoundTransform
    ' 
    '         Properties: [to], from
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace Assembly.KEGG.DBGET.bGetObject

    ''' <summary>
    ''' 反应左端代谢物在经过了代谢反应之后结果上的转换变化的结果（反应的右端）
    ''' </summary>
    Public Class ReactionCompoundTransform

        ''' <summary>
        ''' the kegg compound id
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Overridable Property from As String

        ''' <summary>
        ''' the kegg compound id
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Overridable Property [to] As String

        Public Overrides Function ToString() As String
            Return $"{from}->{[to]}"
        End Function
    End Class
End Namespace
