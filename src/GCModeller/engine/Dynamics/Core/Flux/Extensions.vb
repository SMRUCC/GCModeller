#Region "Microsoft.VisualBasic::c84c800db4023cc963c9fa016f4af905, engine\Dynamics\Core\Extensions.vb"

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

    '     Module Extensions
    ' 
    '         Function: GetProducts, GetReactants, MassToString, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace Core

    <HideModuleName>
    Public Module Extensions

        Friend Function ToString(reaction As Channel) As String
            Dim left = reaction.left.Select(AddressOf MassToString).JoinBy(" + ")
            Dim right = reaction.right.Select(AddressOf MassToString).JoinBy(" + ")
            Dim direct$ = If(reaction.direct = Directions.forward, "=>", "<=")

            Return $"{left} {direct} {right}"
        End Function

        <Extension>
        Private Function MassToString(var As Variable) As String
            If var.IsTemplate Then
                Return $"[{var.Coefficient} {var.Mass.ID}]"
            Else
                Return $"{var.Coefficient} {var.Mass.ID}"
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetReactants(r As Channel) As IEnumerable(Of Variable)
            Return r.left.AsEnumerable
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetProducts(r As Channel) As IEnumerable(Of Variable)
            Return r.right.AsEnumerable
        End Function
    End Module
End Namespace
