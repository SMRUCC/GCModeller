#Region "Microsoft.VisualBasic::10834bc783d588db1f6c0ebe660dad1d, visualize\Phylip\CLI\CLI.vb"

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

    ' Module PhylipCLI
    ' 
    '     Function: Gendist, Initialize
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("Phylip",
Cites:="",
Publisher:="evolution.gs.washington.edu/phylip/credits.html",
Url:="http://evolution.genetics.washington.edu/phylip.html",
Description:="PHYLIP is a free package of programs for inferring phylogenies. All of the methods in this namespace required the phylip software was install on your computer.")>
Module PhylipCLI

    Dim _innerInvoker As PhylipInvoker

    <ExportAPI("Init()")>
    Public Function Initialize(bin As String) As Boolean
        _innerInvoker = New PhylipInvoker(bin)
        Return True
    End Function

    <ExportAPI("Gendist")>
    Public Function Gendist(Matrix As MatrixFile.MatrixFile, ResultSaved As String) As Boolean
        Return _innerInvoker.Gendist(Matrix, ResultSaved)
    End Function
End Module
