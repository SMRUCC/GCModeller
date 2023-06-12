#Region "Microsoft.VisualBasic::d9fc9678b9eb842049f05f3ea4a99aea, Shared\Settings.Configuration\Config\Programs\GCC.vb"

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

'     Class GCC
' 
'         Properties: Filters
'         Structure Replacement
' 
'             Properties: NewReplaced, Old
' 
'             Function: ToString
' 
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.InteropServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Settings

Namespace Settings.Programs

#Disable Warning

    <ClassInterface(ClassInterfaceType.AutoDual)>
    <ComVisible(True)>
    Public Class GCC

        <ClassInterface(ClassInterfaceType.AutoDual)>
        <ComVisible(True)>
        Public Class Replacement

            <XmlAttribute> Public Property NewReplaced As String
            <XmlAttribute> Public Property Old As String

            Public Overrides Function ToString() As String
                Return String.Format("{0} --> {1}", Old, NewReplaced)
            End Function
        End Class

        <ProfileNodeItem> <XmlElement> Public Property Filters As Replacement()
    End Class
End Namespace
