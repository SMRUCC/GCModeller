#Region "Microsoft.VisualBasic::17e2aef8a2d46b746ed7bf2b4acca783, core\Bio.Repository\KEGG\ReactionRepository\RepositoryExtensions.vb"

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
    ' 
    '         Class TermKeys
    ' 
    '             Sub: New
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
    End Module
End Namespace
