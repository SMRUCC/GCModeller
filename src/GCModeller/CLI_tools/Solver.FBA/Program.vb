#Region "Microsoft.VisualBasic::9efb922f4c725e41124c7e1939f4d50e, ..\GCModeller\CLI_tools\Solver.FBA\Program.vb"

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

Imports System.IO
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.Settings
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Terminal.STDIO
Imports RDotNET
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.API.utils
Imports RDotNET.Extensions.VisualBasic.API.base

''' <summary>
''' 本程序集模块是所有的基于FBA模型的模型的求解方法的集合
''' </summary>
''' <remarks></remarks>
Module Program

    Public Profile As Settings.File = Settings.Session.Initialize

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Function Main() As Integer
        Return GetType(CLI).RunCLI(App.Command)
    End Function

    Public Sub Test()
        Call TryInit(Program.Profile.R_HOME)

        If installed.packages("lpSolve") Is Nothing Then
            install.packages("lpSolve")
        End If

        library("lpSolve")

        SyncLock R
            With R
                Dim out As String() = .WriteLine("p <- lp(objective.in=c(5, 8),
const.mat= Matrix(c(1, 1, 1, 2), nrow = 2),
const.rhs= c(2, 3),
const.dir=c(""<="", ""=""), direction=""max"");
p$solution;")
                Call out.JoinBy(vbCrLf).__DEBUG_ECHO
            End With
        End SyncLock
    End Sub
End Module
