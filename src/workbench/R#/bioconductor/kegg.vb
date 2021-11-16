#Region "Microsoft.VisualBasic::e5fa340bea666773fca1efa909eaa88a, R#\bioconductor\kegg.vb"

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

    ' Module kegg
    ' 
    '     Function: writeKeggMaps, writeKeggReactions
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports RDotNET.Extensions.GCModeller
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("kegg")>
Module kegg

    <ExportAPI("write.keggMap.rds")>
    Public Function writeKeggMaps(<RRawVectorArgument> maps As Object, saveRDS As String, Optional env As Environment = Nothing) As Message
        Dim kegg As pipeline = pipeline.TryCreatePipeline(Of Map)(maps, env)

        If kegg.isError Then
            Return kegg.getError
        Else
            Call kegg.populates(Of Map)(env).WriteMaps(saveRDS)
        End If

        Return Nothing
    End Function

    <ExportAPI("write.keggReaction.rds")>
    Public Function writeKeggReactions(<RRawVectorArgument> reactions As Object, saveRDS As String, Optional env As Environment = Nothing) As Message
        Dim reactionList As pipeline = pipeline.TryCreatePipeline(Of ReactionTable)(reactions, env)

        If reactionList.isError Then
            Return reactionList.getError
        Else
            Call reactionList.populates(Of ReactionTable)(env).WriteReactions(saveRDS)
        End If

        Return Nothing
    End Function
End Module
