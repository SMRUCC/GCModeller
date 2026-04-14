#Region "Microsoft.VisualBasic::667f9d589be82f05d4d2e03547c0f0f6, engine\vcellkit\Modeller\Enzymatic.vb"

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

    '   Total Lines: 83
    '    Code Lines: 60 (72.29%)
    ' Comment Lines: 9 (10.84%)
    '    - Xml Docs: 88.89%
    ' 
    '   Blank Lines: 14 (16.87%)
    '     File Size: 3.02 KB


    ' Module Enzymatic
    ' 
    '     Function: ImportsRhea, openRheaQuery, ParseRhea, QueryReaction
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.SABIORK
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports sbXML = SMRUCC.genomics.Model.SBML.Level3.XmlFile(Of SMRUCC.genomics.Data.SABIORK.SBML.SBMLReaction)

''' <summary>
''' enzymatic reaction network modeller
''' </summary>
<Package("enzymatic")>
Module Enzymatic

    <ExportAPI("query_reaction")>
    Public Function QueryReaction(reaction As Reaction, Optional cache As Object = "./.cache/") As Object
        Dim list As New Dictionary(Of String, sbXML)

        For Each id As String In reaction.enzyme.SafeQuery
            list.Add(id, docuRESTfulWeb.QueryByECNumber(id, cache))
        Next

        Return list
    End Function

    <ExportAPI("open.rhea")>
    <RApiReturn(GetType(RheaNetworkReader))>
    Public Function openRheaQuery(repo As String) As Object
        Return New RheaNetworkReader(repo.Open(FileMode.Open, doClear:=False, [readOnly]:=True))
    End Function

    <ExportAPI("imports_rhea")>
    Public Function ImportsRhea(<RRawVectorArgument>
                                rhea As Object,
                                repo As String,
                                Optional env As Environment = Nothing) As Object

        Dim reactions = pipeline.TryCreatePipeline(Of Reaction)(rhea, env)

        If reactions.isError Then
            Return reactions.getError
        End If

        Using file As Stream = New FileStream(repo, FileMode.OpenOrCreate)
            Dim writer As New RheaNetworkWriter(New StreamPack(file, meta_size:=32 * 1024 * 1024))

            For Each reaction As Reaction In reactions.populates(Of Reaction)(env)
                Call writer.AddReaction(reaction)
            Next

            Call writer.Dispose()
        End Using

        Return Nothing
    End Function
End Module
