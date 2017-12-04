#Region "Microsoft.VisualBasic::2302b03a90afe5fbf0a3bad1f0b93c87, ..\workbench\devenv\Strings.vb"

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

Module Strings
    Public ReadOnly AppPath As String = My.Application.Info.DirectoryPath
    Public ReadOnly LocalStorage As String = AppPath & "/LocalStorage/"

    ''' <summary>
    ''' .../projs/
    ''' </summary>
    ''' <remarks></remarks>
    Public ReadOnly Project As String = AppPath & "/Projs/"

    Public ReadOnly LicenseFile As String = AppPath & "/License.rtf"

    Public ReadOnly LogFile As String = Settings.LogDIR & "/dev2.log"

    Public Const BlankFlush As String = "                                                                                                                                                                                                                                                                   "
    ''' <summary>
    ''' (Logging Object) GCModeller
    ''' </summary>
    ''' <remarks></remarks>
    Public Const Modeller As String = "GCModeller"
    ''' <summary>
    ''' (Logging Object) LibMySQL
    ''' </summary>
    ''' <remarks></remarks>
    Public Const LibMySQL As String = "LibMySQL"
    ''' <summary>
    ''' (Logging Object) IDE
    ''' </summary>
    ''' <remarks></remarks>
    Public Const IDE As String = "IDE"

    Public ReadOnly Languages As String() = {"zh-CN", "en-US", "fr-FR"}
End Module
