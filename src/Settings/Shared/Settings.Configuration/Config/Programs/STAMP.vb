#Region "Microsoft.VisualBasic::6904bbf591b192c235069709b214a8db, Shared\Settings.Configuration\Config\Programs\STAMP.vb"

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

'     Class STAMP
' 
'         Properties: MEME
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.ComponentModel.Settings

Namespace Settings.Programs

#Disable Warning

    <ClassInterface(ClassInterfaceType.AutoDual)>
    <ComVisible(True)>
    Public Class STAMP
        <ProfileItem(Name:="meme_bin")> Public Property MEME As String
    End Class
End Namespace
