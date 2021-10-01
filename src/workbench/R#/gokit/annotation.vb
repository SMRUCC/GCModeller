#Region "Microsoft.VisualBasic::e20ad86a293225bd9e81b4a7032e76d7, R#\gokit\annotation.vb"

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

    ' Module annotation
    ' 
    '     Function: CreateKO2GO, mapTop
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports SMRUCC.genomics.Data.GeneOntology.Annotation
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("go.annotation")>
Module annotation

    ''' <summary>
    ''' export ko to go mapping data from the uniprot database.
    ''' </summary>
    ''' <param name="uniprot">the data reader of the uniprot xml database file.</param>
    ''' <param name="threshold">the supports coverage threshold value.</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("uniprot.ko2go")>
    <RApiReturn(GetType(SecondaryIDSolver))>
    Public Function CreateKO2GO(uniprot As pipeline, Optional threshold# = 0.8, Optional env As Environment = Nothing) As Object
        If uniprot Is Nothing Then
            Return debug.stop("the uniprot annotation source can not be nothing!", env)
        ElseIf Not uniprot.elementType Like GetType(entry) Then
            Return debug.stop({
                 $"invalid element type for input data!",
                 $"required: " & GetType(entry).FullName,
                 $"but given: " & uniprot.elementType.ToString
            }, env)
        End If

        Dim idmaps = uniprot.populates(Of entry)(env) _
            .PopulateMappings _
            .GroupBy(Function(a) a.KO)
        Dim mapper As SecondaryIDSolver = SecondaryIDSolver.Create(
            source:=idmaps,
            mainID:=Function(a) a.Key,
            secondaryID:=Function(a)
                             Return a.Select(Function(m) m.GO).mapTop(threshold)
                         End Function,
            skip2ndMaps:=True
        )

        Return mapper
    End Function

    <Extension>
    Private Function mapTop(groups As IEnumerable(Of String()), threshold#) As String()
        Dim allMatrix As String()() = groups.ToArray
        Dim counts = From go_id As String
                     In allMatrix.IteratesALL
                     Group By go_id
                     Into Count

        Return counts _
            .Where(Function(a) (a.Count / allMatrix.Length) >= threshold) _
            .Select(Function(a) a.go_id) _
            .ToArray
    End Function
End Module
