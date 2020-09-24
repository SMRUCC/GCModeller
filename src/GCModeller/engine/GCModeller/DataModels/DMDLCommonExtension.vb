#Region "Microsoft.VisualBasic::6bf855f48073f8aded9e7b762eed2b57, engine\GCModeller\DataModels\DMDLCommonExtension.vb"

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

    ' Module DMDLCommonExtension
    ' 
    '     Function: GetTypes
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

''' <summary>
''' Something commonly operation in the datamodel building procedure.
''' </summary>
''' <remarks></remarks>
Module DMDLCommonExtension

    ''' <summary>
    ''' Convert the object types definition from the stirng value into the enumerate value.
    ''' (将一个对象的字符串形式的类型定义值转换为枚举形式的定义值)
    ''' </summary>
    ''' <param name="e">待转换的目标对象</param>
    ''' <returns>
    ''' Return a collection of this type definition value.
    ''' (返回类型枚举值的集合)
    ''' </returns>
    ''' <remarks></remarks>
    <Extension> Public Function GetTypes(e As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Object) As LANS.SystemsBiology.Assembly.MetaCyc.Schema.Slots.Types()
        Dim Query As Generic.IEnumerable(Of LANS.SystemsBiology.Assembly.MetaCyc.Schema.Slots.Types) =
            From s As String In e.Types
            Select CType(s, LANS.SystemsBiology.Assembly.MetaCyc.Schema.Slots.Types) 'Types Converting LINQ Query.
        Return Query.ToArray
    End Function
End Module
