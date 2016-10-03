#Region "Microsoft.VisualBasic::92a7b7f0ce0c2944ffdbebdc29f15c34, ..\GCModeller\CLI_tools\c2\Reconstruction\ObjectEquals\Session.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Namespace Reconstruction.ObjectEquals

    Friend Class Session

        Public ProteinEquals As c2.Reconstruction.ObjectEquals.Proteins
        Public PromoterEquals As c2.Reconstruction.ObjectEquals.Promoters
        Public ReactionEquals As c2.Reconstruction.ObjectEquals.Reactions

        Sub New(Session As c2.Reconstruction.Operation.OperationSession)
            ProteinEquals = New Proteins(Session)
            ReactionEquals = New Reactions(Session)
        End Sub

        Public Sub Initialize()
            Call ProteinEquals.Initialize()
            Call ReactionEquals.Initialize()
        End Sub
    End Class
End Namespace
