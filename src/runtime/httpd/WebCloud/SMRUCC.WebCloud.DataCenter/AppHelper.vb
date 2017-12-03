#Region "Microsoft.VisualBasic::320d87a7a7557487e04090c3fd1c2ea1, ..\httpd\WebCloud\SMRUCC.WebCloud.DataCenter\AppHelper.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Reflection

Public Module AppHelper

    Public Function LoadData(Of T As Structure)() As mysql.app()
        Dim values As FieldInfo() = GetType(T) _
            .GetFields _
            .Where(Function(o) o.FieldType Is GetType(T)) _
            .ToArray
        Dim apps As mysql.app() = values _
            .Select(Function(field)
                        Return New mysql.app With {
                            .name = field.Name,
                            .description = field.Description,
                            .catagory = field.Category,
                            .uid = field.GetValue(Nothing)
                        }
                    End Function) _
            .ToArray

        Return apps
    End Function
End Module

