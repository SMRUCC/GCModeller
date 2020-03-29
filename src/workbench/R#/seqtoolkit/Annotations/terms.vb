#Region "Microsoft.VisualBasic::fa305348b133420b3a28f1743e529893, R#\seqtoolkit\Annotations\terms.vb"

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

    ' Module terms
    ' 
    '     Function: COGannotations, GOannotations, KOannotations, Pfamannotations
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object

<Package("annotation.terms", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gcmodeller.org")>
Module terms

    <ExportAPI("assign.KO")>
    Public Function KOannotations(forward As pipeline, reverse As pipeline, Optional env As Environment = Nothing) As pipeline
        If forward Is Nothing Then
            Return Internal.debug.stop("forward data stream is nothing!", env)
        ElseIf reverse Is Nothing Then
            Return Internal.debug.stop("reverse data stream is nothing!", env)
        ElseIf Not forward.elementType.raw Is GetType(BestHit) Then
            Return Internal.debug.stop($"forward is invalid data stream type: {forward.elementType.fullName}!", env)
        ElseIf Not reverse.elementType.raw Is GetType(BestHit) Then
            Return Internal.debug.stop($"reverse is invalid data stream type: {reverse.elementType.fullName}!", env)
        End If

        Return KOAssignment.KOassignmentBBH(forward.populates(Of BestHit), reverse.populates(Of BestHit)).DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function

    <ExportAPI("assign.COG")>
    Public Function COGannotations()

    End Function

    <ExportAPI("assign.Pfam")>
    Public Function Pfamannotations()

    End Function

    <ExportAPI("assign.GO")>
    Public Function GOannotations()

    End Function

End Module

