#Region "Microsoft.VisualBasic::0673c9f0cb6e8b77a451e5a21684b6fa, CLI_tools\Solver.FBA\Program.vb"

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

    ' Module Program
    ' 
    '     Function: Main
    ' 
    '     Sub: Test, Test2
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Extensions
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.API.base
Imports RDotNET.Extensions.VisualBasic.API.utils
Imports SMRUCC.genomics.Analysis.FBA.Core
Imports SMRUCC.genomics.Analysis.FBA_DP.v2

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

    Sub Test2()
        Dim matrix As New Matrix With {
            .Compounds = {"C1", "C2", "C3"},
            .Flux = New Dictionary(Of String, DoubleRange) From {
                {"v1", New DoubleRange(0, 100)},
                {"v2", New DoubleRange(0, 100)},
                {"v3", New DoubleRange(0, 100)},
                {"v4", New DoubleRange(0, 100)},
                {"v5", New DoubleRange(0, 100)},
                {"v6", New DoubleRange(0, 100)},
                {"v7", New DoubleRange(0, 100)},
                {"v8", New DoubleRange(0, 100)}
            },
            .Targets = {"v1", "v2", "v3", "v4", "v5", "v6", "v7", "v8"},
            .Matrix = {
                 {1.0, 1.0, 3.0, 1.0, -1.0, -51.0, 10.0, 10.0},
                 {.0, 5.0, 30.0, 10.0, 8.0, 1.0, .0, -10.0},
                 {.0, -2.0, .0, -100.0, .0, -1.0, .0, .0}
            }.ToVectorList
        }
        Dim result = matrix.Rsolver

        Call Console.WriteLine(result.ToString)
    End Sub
End Module
