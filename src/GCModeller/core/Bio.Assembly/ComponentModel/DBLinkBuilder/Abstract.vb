#Region "Microsoft.VisualBasic::e8485093a572db4157f2af76bd478dd6, GCModeller\core\Bio.Assembly\ComponentModel\DBLinkBuilder\Abstract.vb"

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

    '   Total Lines: 22
    '    Code Lines: 14
    ' Comment Lines: 5
    '   Blank Lines: 3
    '     File Size: 685 B


    '     Interface IMetabolite
    ' 
    '         Properties: ChEBI, KEGGCompound
    ' 
    '     Interface IDBLink
    ' 
    '         Properties: DbName, EntryId
    ' 
    '         Function: GetFormatValue
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace ComponentModel.DBLinkBuilder

    Public Interface IMetabolite
        Property ChEBI As String()
        Property KEGGCompound As String
    End Interface

    Public Interface IDBLink
        Property DbName As String
        Property EntryId As String
        ''' <summary>
        ''' 将对象模型转换为含有格式的字符串的值用以写入文件之中
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetFormatValue() As String
    End Interface
End Namespace
