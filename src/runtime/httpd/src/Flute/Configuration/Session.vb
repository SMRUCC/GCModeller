#Region "Microsoft.VisualBasic::449eb1f8193f2b84d0bccdec5c3425c9, G:/GCModeller/src/runtime/httpd/src/Flute//Configuration/Session.vb"

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

    '   Total Lines: 19
    '    Code Lines: 13
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 606 B


    '     Class Session
    ' 
    '         Properties: session_enable, session_id_prefix, session_store
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Settings.Inf

Namespace Configurations

    <ClassName("session")>
    Public Class Session

        <Description("the prefix for the user session id.")>
        Public Property session_id_prefix As String = "flute_www_"

        <Description("the directory folder path for save the session data as files.")>
        Public Property session_store As String = "/tmp/flute_sessions/"

        <Description("enable the session?")>
        Public Property session_enable As Boolean = True

    End Class
End Namespace
