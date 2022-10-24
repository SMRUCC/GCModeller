#Region "Microsoft.VisualBasic::25a5f033f393c67e27a4fd7672487b70, Shared\Settings.Configuration\Config\Programs\GCHOST.vb"

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

'     Class GCHOST
' 
'         Properties: DBRoot, DBRootPwd, SDK
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.ComponentModel.Settings

Namespace Settings.Programs

    <ClassInterface(ClassInterfaceType.AutoDual)>
    <ComVisible(True)>
    Public Class GCHOST

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ProfileItem(Name:=".net_sdk", Description:="")> Public Property SDK As String
        <ProfileItem(Name:="db_root")> Public Property DBRoot As String
        <ProfileItem(Name:="db_root_password")> Public Property DBRootPwd As String

    End Class
End Namespace
