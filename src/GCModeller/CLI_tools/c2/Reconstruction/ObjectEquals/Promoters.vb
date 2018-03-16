#Region "Microsoft.VisualBasic::0832fb3f5d1834c3b5b5815c25ca8a28, CLI_tools\c2\Reconstruction\ObjectEquals\Promoters.vb"

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

    '     Class Promoters
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Create, Initialize
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports c2.Reconstruction.Operation

Namespace Reconstruction.ObjectEquals

    Friend Class Promoters : Inherits c2.Reconstruction.ObjectEquals.EqualsOperation

        Sub New(Session As OperationSession, Promoters As c2.Reconstruction.Promoters)
            Call MyBase.New(Session)
            Dim LQuery = From Item In Promoters.ReconstructList Select New KeyValuePair(Of String, String())(Item.Key, New String() {Item.Value.Identifier}) '
            Me.EqualsList = LQuery.ToArray
        End Sub

        Public Overrides Function Initialize() As Integer
            Return 0
        End Function

        Public Shared Function Create(Promoters As c2.Reconstruction.Promoters) As Promoters
            Return New Promoters(Nothing, Promoters)
        End Function
    End Class
End Namespace
