#Region "Microsoft.VisualBasic::6e622edf2500d696d2b80dcf977add70, Shared\Settings.Configuration\Config\Programs\SMART.vb"

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

'     Class SMART
' 
'         Properties: CDD
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.ComponentModel.Settings

Namespace Settings.Programs

    <ClassInterface(ClassInterfaceType.AutoDual)>
    <ComVisible(True)>
    Public Class SMART
        ''' <summary>
        ''' CDD数据库的数据文件夹的路径
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ProfileItem(Name:="cdd")> Public Property CDD As String
    End Class
End Namespace
