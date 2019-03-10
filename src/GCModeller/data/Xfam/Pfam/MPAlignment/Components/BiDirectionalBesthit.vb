#Region "Microsoft.VisualBasic::1ca047bf04802d8af725d9a8a3c01c3b, data\Xfam\Pfam\MPAlignment\Components\BiDirectionalBesthit.vb"

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

    '     Class BiDirectionalBesthit
    ' 
    '         Properties: Score, Similarity
    ' 
    '         Function: Upgrade
    '         Interface IMPAlignmentResult
    ' 
    '             Properties: MPScore, Similarity
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application

Namespace ProteinDomainArchitecture.MPAlignment

    Public Class BiDirectionalBesthit : Inherits BBH.BiDirectionalBesthit
        Implements IMPAlignmentResult

        Public Property Similarity As Double Implements IMPAlignmentResult.Similarity
        Public Property Score As Double Implements IMPAlignmentResult.MPScore

        Public Shared Function Upgrade(source As BBH.BiDirectionalBesthit, DomainAlignment As AlignmentOutput) As BiDirectionalBesthit
            If DomainAlignment Is Nothing Then
                Return source.ShadowCopy(Of BiDirectionalBesthit)()
            End If

            Dim Besthit = source.ShadowCopy(Of BiDirectionalBesthit)()
            Besthit.Similarity = DomainAlignment.Similarity
            Besthit.Score = DomainAlignment.Score

            Return Besthit
        End Function

        Public Interface IMPAlignmentResult
            Property MPScore As Double
            Property Similarity As Double
        End Interface
    End Class
End Namespace
