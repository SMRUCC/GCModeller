﻿#Region "Microsoft.VisualBasic::39ecd54cbe85f8969e06822c5a8bb59f, Microsoft.VisualBasic.Core\src\ApplicationServices\Application.vb"

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

    '     Class Application
    ' 
    '         Properties: ExecutablePath, ProductName, ProductVersion, StartupPath
    ' 
    '         Sub: DoEvents
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Reflection
Imports Microsoft.VisualBasic.ApplicationServices.Development
Imports AssemblyMeta = Microsoft.VisualBasic.ApplicationServices.Development.AssemblyInfo

Namespace ApplicationServices

    Public Class Application

        Shared ReadOnly main As Assembly = Assembly.GetEntryAssembly()
        Shared ReadOnly meta As AssemblyMeta = main.FromAssembly

        ''' <summary>
        ''' Gets the path for the executable file that started the application, 
        ''' not including the executable name.
        ''' </summary>
        ''' <returns></returns>
        Public Shared ReadOnly Property StartupPath As String
            Get
                Return Path.GetDirectoryName(main.Location)
            End Get
        End Property

        Public Shared ReadOnly Property ExecutablePath As String
            Get
                Return main.Location
            End Get
        End Property

        Public Shared ReadOnly Property ProductName As String
            Get
                Return meta.AssemblyProduct
            End Get
        End Property

        Public Shared ReadOnly Property ProductVersion As String
            Get
                Return meta.AssemblyVersion
            End Get
        End Property

        Public Shared Sub DoEvents()
#If netcore5 = 0 Then
#If UNIX = False Then
            Try
                Call Parallel.DoEvents()
            Catch ex As Exception
                Call ex.PrintException
            End Try
#End If
#End If
        End Sub
    End Class
End Namespace
