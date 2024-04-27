﻿#Region "Microsoft.VisualBasic::d4711b6fb8dd3c3c672150f5748815a0, G:/GCModeller/src/workbench/R#/gokit//file.vb"

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

    '   Total Lines: 103
    '    Code Lines: 78
    ' Comment Lines: 10
    '   Blank Lines: 15
    '     File Size: 3.65 KB


    ' Module file
    ' 
    '     Function: DAG, is_a, ReadGoObo, synonym, xrefs
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.GeneOntology
Imports SMRUCC.genomics.Data.GeneOntology.DAG
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

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

    ''' <summary>
    ''' parse the term is_a relationship
    ''' </summary>
    ''' <param name="x"></param>
    ''' <returns></returns>
    <ExportAPI("is_a")>
    Public Function is_a(<RRawVectorArgument> x As Object, Optional env As Environment = Nothing) As dataframe
        Dim links As is_a() = Nothing
        Dim df As New dataframe With {.columns = New Dictionary(Of String, Array)}

        If TypeOf x Is Term Then
            links = DirectCast(x, Term).is_a _
                .SafeQuery _
                .Select(Function(si) New is_a(si)) _
                .ToArray
        Else
            links = CLRVector.asCharacter(x) _
                .SafeQuery _
                .Select(Function(si) New is_a(si)) _
                .ToArray
        End If

        Call df.add("id", links.Select(Function(a) a.term_id))
        Call df.add("name", links.Select(Function(a) a.name))

        Return df
    End Function

    <ExportAPI("synonym")>
    Public Function synonym(<RRawVectorArgument> x As Object, Optional env As Environment = Nothing) As dataframe
        Dim names As synonym() = Nothing
        Dim df As New dataframe With {.columns = New Dictionary(Of String, Array)}

        If TypeOf x Is Term Then
            names = DirectCast(x, Term).synonym _
                .SafeQuery _
                .Select(Function(si) New synonym(si)) _
                .ToArray
        Else
            names = CLRVector.asCharacter(x) _
                .SafeQuery _
                .Select(Function(si) New synonym(si)) _
                .ToArray
        End If

        Call df.add("name", names.Select(Function(a) a.name))
        Call df.add("type", names.Select(Function(a) a.type))
        Call df.add("id", names.Select(Function(a) a.synonym.Name))
        Call df.add("term", names.Select(Function(a) a.synonym.Value))

        Return df
    End Function

    <ExportAPI("term_xrefs")>
    Public Function xrefs(<RRawVectorArgument> x As Object, Optional env As Environment = Nothing) As dataframe
        Dim links As NamedValue(Of String)()
        Dim df As New dataframe With {.columns = New Dictionary(Of String, Array)}

        If TypeOf x Is Term Then
            links = DirectCast(x, Term).GetTermXrefs
        Else
            links = CLRVector.asCharacter(x) _
                .SafeQuery _
                .Select(Function(si) TermXrefParser(si)) _
                .ToArray
        End If

        Call df.add("db_name", links.Select(Function(a) a.Name))
        Call df.add("xref_id", links.Select(Function(a) a.Value))

        Return df
    End Function

    <ExportAPI("DAG")>
    Public Function DAG(goDb As GO_OBO) As DAG.Graph
        Return New DAG.Graph(goDb.terms)
    End Function
End Module
