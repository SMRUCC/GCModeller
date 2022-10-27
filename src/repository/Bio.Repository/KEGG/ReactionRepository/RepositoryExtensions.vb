#Region "Microsoft.VisualBasic::04adec1c15b1ca149f1bf3dcfef9b637, Bio.Repository\KEGG\ReactionRepository\RepositoryExtensions.vb"

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

'     Module RepositoryExtensions
' 
'         Function: FetchCompoundRepository, FetchPathwayRepository, FetchReactionRepository
'         Class TermKeys
' 
'             Constructor: (+1 Overloads) Sub New
' 
' 
' 
' 
' /********************************************************************************/

#End Region

#If NETCOREAPP Then
Imports System.Data
#End If
Imports System.Runtime.CompilerServices

Namespace KEGG.Metabolism

    Public Module RepositoryExtensions

        Public NotInheritable Class TermKeys

            Private Sub New()
            End Sub

            Public Const KEGG$ = "kegg"
            Public Const ChEBI$ = "chebi"
            Public Const HMDB$ = "hmdb"
            Public Const metlin$ = "metlin"
            Public Const Name$ = "name"
            Public Const CAS$ = "CAS"

        End Class

        <Extension>
        Public Function FetchReactionRepository(resource As String) As ReactionRepository
            If resource.ExtensionSuffix.TextEquals("Xml") AndAlso resource.FileExists Then
                Return resource.LoadXml(Of ReactionRepository)
            ElseIf resource.DirectoryExists Then
                Return ReactionRepository.ScanModel(resource)
            Else
                Throw New InvalidExpressionException($"{resource}")
            End If
        End Function

        <Extension>
        Public Function FetchCompoundRepository(resource As String) As CompoundRepository
            If resource.ExtensionSuffix.TextEquals("Xml") AndAlso resource.FileExists Then
                Return resource.LoadXml(Of CompoundRepository)
            ElseIf resource.DirectoryExists Then
                Return CompoundRepository.ScanModels(resource, False)
            Else
                Throw New InvalidExpressionException($"{resource}")
            End If
        End Function

        <Extension>
        Public Function FetchPathwayRepository(resource As String) As PathwayRepository
            If resource.ExtensionSuffix.TextEquals("Xml") AndAlso resource.FileExists Then
                Return resource.LoadXml(Of PathwayRepository)
            ElseIf resource.DirectoryExists Then
                Return PathwayRepository.ScanModels(resource)
            Else
                Throw New InvalidExpressionException($"{resource}")
            End If
        End Function
    End Module
End Namespace
