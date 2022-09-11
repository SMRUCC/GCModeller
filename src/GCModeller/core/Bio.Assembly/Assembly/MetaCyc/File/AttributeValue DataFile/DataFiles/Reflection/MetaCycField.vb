#Region "Microsoft.VisualBasic::7fdb266659f274a0b46b77dea11348a0, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Reflection\MetaCycField.vb"

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

    '   Total Lines: 53
    '    Code Lines: 16
    ' Comment Lines: 33
    '   Blank Lines: 4
    '     File Size: 1.85 KB


    '     Class MetaCycField
    ' 
    '         Properties: Name, Type
    '         Enum Types
    ' 
    '             [Char], [String], TStr
    ' 
    ' 
    ' 
    '  
    ' 
    '     Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Assembly.MetaCyc.File.DataFiles.Reflection

    ''' <summary>
    ''' MetaCyc数据库中的一个对象的一个属性值
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Property, allowmultiple:=False, inherited:=True)>
    Public Class MetaCycField : Inherits Attribute

        ''' <summary>
        ''' 域名称
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Name As String
        ''' <summary>
        ''' 域类型，提供三种类型：字符，字符串以及字符数组
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Type As MetaCycField.Types = MetaCycField.Types.String

        ''' <summary>
        ''' Data field type in a Metacyc database object instance.(MetaCyc数据库中的一个对象实例的数据属性域的数据类型)
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum Types
            ''' <summary>
            ''' A single string type variable.
            ''' </summary>
            ''' <remarks></remarks>
            [String]
            ''' <summary>
            ''' A string array type variable.
            ''' </summary>
            ''' <remarks></remarks>
            TStr
            ''' <summary>
            ''' A single Char type variable.
            ''' </summary>
            ''' <remarks></remarks>
            [Char]
        End Enum

        Public Overrides Function ToString() As String
            If Type = Types.String Then Return String.Format("(String) {0}", Name)
            'Else
            Return String.Format("(String[]) {0}", Name)
        End Function
    End Class
End Namespace
