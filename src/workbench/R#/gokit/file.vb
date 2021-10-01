#Region "Microsoft.VisualBasic::4d3bcd78a4f15fdd1bb44a8be84c512b, R#\gokit\file.vb"

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

    ' Module file
    ' 
    '     Function: DAG, ReadGoObo
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.GeneOntology
Imports SMRUCC.genomics.Data.GeneOntology.OBO

<Package("file")>
Public Module file

    ''' <summary>
    ''' read the GO database file
    ''' </summary>
    ''' <param name="goDb"></param>
    ''' <returns></returns>
    <ExportAPI("read.go_obo")>
    Public Function ReadGoObo(goDb As String) As GO_OBO
        Return GO_OBO.LoadDocument(path:=goDb)
    End Function

    <ExportAPI("DAG")>
    Public Function DAG(goDb As GO_OBO) As DAG.Graph
        Return New DAG.Graph(goDb.terms)
    End Function
End Module
