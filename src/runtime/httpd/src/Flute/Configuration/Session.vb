#Region "Microsoft.VisualBasic::19b2ce7477ce5d2de8a9049955152dc6, src\Flute\Configuration\Session.vb"

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

    '   Total Lines: 33
    '    Code Lines: 25 (75.76%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 8 (24.24%)
    '     File Size: 1.22 KB


    '     Class Session
    ' 
    '         Properties: session_enable, session_id_prefix, session_store, sessionStorePath
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.Settings.Inf
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Configurations

    ''' <summary>
    ''' the session related configuration section of the http server, controls
    ''' how user sessions are identified, persisted and enabled.
    ''' </summary>
    <ClassName("session")>
    Public Class Session

        ''' <summary>
        ''' the prefix that is prepended to every generated user session id.
        ''' </summary>
        <Description("the prefix for the user session id.")>
        Public Property session_id_prefix As String = "flute_www_"

        ''' <summary>
        ''' the directory folder path where the session data is saved as files.
        ''' a null/empty value resolves to a cross-platform default under App.HOME.
        ''' </summary>
        <Description("the directory folder path for save the session data as files. A null/empty value resolves to a cross-platform default under App.HOME.")>
        Public Property session_store As String = Nothing

        ''' <summary>
        ''' the resolved, cross-platform session store directory. falls back to
        ''' <c>flute_sessions</c> under the temp directory when
        ''' <see cref="session_store"/> is empty.
        ''' </summary>
        <Description("resolved session store directory (cross-platform).")>
        Public ReadOnly Property sessionStorePath As String
            Get
                Return If(session_store.StringEmpty, Path.Combine(TempFileSystem.TempDir, "flute_sessions"), session_store)
            End Get
        End Property

        ''' <summary>
        ''' a logical value that turns the session subsystem on or off.
        ''' </summary>
        <Description("enable the session?")>
        Public Property session_enable As Boolean = True

        ''' <summary>
        ''' serialize the session configuration to its json representation.
        ''' </summary>
        ''' <returns>the json string of this session configuration.</returns>
        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class
End Namespace
