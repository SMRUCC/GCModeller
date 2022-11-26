#Region "Microsoft.VisualBasic::063fe183d61f54d94a76ff6364d2fc1a, GCModeller\engine\vcellkit\Modeller\Enzymatic.vb"

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

    '   Total Lines: 64
    '    Code Lines: 48
    ' Comment Lines: 3
    '   Blank Lines: 13
    '     File Size: 2.37 KB


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
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.Rhea
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
    Public Function QueryReaction(reaction As Rhea.Reaction, Optional cache As Object = "./.cache/") As Object
        Dim list As New Dictionary(Of String, sbXML)

        For Each id As String In reaction.enzyme.SafeQuery
            list.Add(id, SMRUCC.genomics.Data.SABIORK.WebRequest.QueryByECNumber(id, cache))
        Next

        Return list
    End Function

    <ExportAPI("read.rhea")>
    Public Function ParseRhea(file As String) As Object
        Return TextParser.ParseReactions(file.ReadAllLines).DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function

    <ExportAPI("open.rhea")>
    Public Function openRheaQuery(repo As String) As Object
        Return New RheaNetworkReader(repo.Open(FileMode.Open, doClear:=False, [readOnly]:=True))
    End Function

    <ExportAPI("imports_rhea")>
    Public Function ImportsRhea(<RRawVectorArgument>
                                rhea As Object,
                                repo As String,
                                Optional env As Environment = Nothing) As Object

        Dim reactions = pipeline.TryCreatePipeline(Of Rhea.Reaction)(rhea, env)

        If reactions.isError Then
            Return reactions.getError
        End If

        Using file As Stream = New FileStream(repo, FileMode.OpenOrCreate)
            Dim writer As New RheaNetworkWriter(New StreamPack(file, meta_size:=32 * 1024 * 1024))

            For Each reaction As Rhea.Reaction In reactions.populates(Of Rhea.Reaction)(env)
                Call writer.AddReaction(reaction)
            Next

            Call writer.Dispose()
        End Using

        Return Nothing
    End Function
End Module
