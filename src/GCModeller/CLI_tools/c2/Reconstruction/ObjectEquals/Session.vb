#Region "Microsoft.VisualBasic::3f4d4117e411bdeeda8d8979061cb817, CLI_tools\c2\Reconstruction\ObjectEquals\Session.vb"

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

    '     Class Session
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: Initialize
    ' 
    ' 
    ' /********************************************************************************/

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
