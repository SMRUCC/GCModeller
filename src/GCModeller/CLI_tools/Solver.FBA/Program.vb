#Region "Microsoft.VisualBasic::227f4fd8e40d4de25a1c1ab1aed61866, ..\GCModeller\CLI_tools\Solver.FBA\Program.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports Microsoft.VisualBasic.Terminal.STDIO
Imports Microsoft.VisualBasic
Imports System.IO
Imports System.Text.RegularExpressions

Imports RDotNET.Extensions.VisualBasic.RSystem
Imports RDotNET

Imports Microsoft.VisualBasic.ComponentModel.Settings
Imports Microsoft.VisualBasic.Extensions
Imports RDotNET.Extensions.VisualBasic

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

        If Not Installed.Packages("lpSolve") Then
            Install.Packages("lpSolve")
        End If

        Library("lpSolve")

        Dim out As String() = RServer.WriteLine("p <- lp(objective.in=c(5, 8),
const.mat= Matrix(c(1, 1, 1, 2), nrow = 2),
const.rhs= c(2, 3),
const.dir=c(""<="", ""=""), direction=""max"");
p$solution;")

        Call out.JoinBy(vbCrLf).__DEBUG_ECHO
    End Sub
End Module
