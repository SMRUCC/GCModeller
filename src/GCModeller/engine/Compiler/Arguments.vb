﻿#Region "Microsoft.VisualBasic::e4d4305c2ced80971dee4147170e8aac, Compiler\Arguments.vb"

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

    ' Class RepositoryArguments
    ' 
    '     Function: GetCompounds, GetPathways, GetReactions
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.KEGG.Metabolism

Public Class RepositoryArguments

    Public KEGGCompounds As String
    Public KEGGReactions As String
    Public KEGGPathway As String

    Dim compoundsRepository As CompoundRepository
    Dim reactionsRepository As ReactionRepository
    Dim pathwayRepository As PathwayRepository

    Public Function GetCompounds() As CompoundRepository
        If compoundsRepository Is Nothing Then
            compoundsRepository = KEGGCompounds.FetchCompoundRepository
        End If

        Return compoundsRepository
    End Function

    Public Function GetReactions() As ReactionRepository
        If reactionsRepository Is Nothing Then
            reactionsRepository = KEGGReactions.FetchReactionRepository
        End If

        Return reactionsRepository
    End Function

    Public Function GetPathways() As PathwayRepository
        If pathwayRepository Is Nothing Then
            pathwayRepository = KEGGPathway.FetchPathwayRepository
        End If

        Return pathwayRepository
    End Function

End Class

