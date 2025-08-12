#Region "Microsoft.VisualBasic::e973e7a4d4912ff456d3fefff57647b9, engine\Dynamics\Core\Flux\Extensions.vb"

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


    ' Code Statistics:

    '   Total Lines: 37
    '    Code Lines: 30 (81.08%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 7 (18.92%)
    '     File Size: 1.28 KB


    '     Module Extensions
    ' 
    '         Function: GetProducts, GetReactants, MassToString, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Core

    <HideModuleName>
    Public Module Extensions

        ''' <summary>
        ''' string debug view
        ''' </summary>
        ''' <param name="reaction"></param>
        ''' <returns></returns>
        Public Function ToString(reaction As Channel) As String
            Dim left = reaction.left.Select(AddressOf MassToString).JoinBy(" + ")
            Dim right = reaction.right.Select(AddressOf MassToString).JoinBy(" + ")
            Dim direct$ = If(reaction.direct = Directions.forward, "=>", "<=")

            Return $"[{reaction.ID}] {left} {direct} {right}"
        End Function

        <Extension>
        Private Function MassToString(var As Variable) As String
            If var.isTemplate Then
                Return $"*[{var.coefficient} {var.mass.ID}({var.mass.Value.ToString("G3")} unit)]"
            Else
                Return $"{var.coefficient} {var.mass.ID}({var.mass.Value.ToString("G3")} unit)"
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

        <Extension>
        Public Function jsonView(flux As Channel) As String
            Dim left As VariableFactor() = VariableFactor.GetModel(flux.left).ToArray
            Dim right As VariableFactor() = VariableFactor.GetModel(flux.right).ToArray
            Dim model As New FluxEdge With {
                .left = left,
                .right = right,
                .regulation = VariableFactor _
                    .GetModel(flux.forward, flux.reverse) _
                    .ToArray
            }

            Return model.GetJson
        End Function

    End Module

    Public Class VariableFactor

        Public Property id As String
        Public Property compartment_id As String

        Public Shared Function GetModel(list As IEnumerable(Of Variable)) As IEnumerable(Of VariableFactor)
            Return From m As Variable
                   In list
                   Select New VariableFactor With {
                       .id = m.mass.ID,
                       .compartment_id = m.mass.cellular_compartment
                   }
        End Function

        Public Shared Iterator Function GetModel(forwards As Controls, reverse As Controls) As IEnumerable(Of VariableFactor)
            If Not TypeOf forwards Is BaselineControls Then
                For Each f As VariableFactor In GetModel(forwards)
                    Yield f
                Next
            End If
            If Not TypeOf reverse Is BaselineControls Then
                For Each r As VariableFactor In GetModel(reverse)
                    Yield r
                Next
            End If
        End Function

        Private Shared Iterator Function GetModel(reg As Controls) As IEnumerable(Of VariableFactor)
            If TypeOf reg Is AdditiveControls Then
                For Each r As VariableFactor In GetModel(DirectCast(reg, AdditiveControls).activation)
                    Yield r
                Next
            ElseIf TypeOf reg Is KineticsControls Then
                For Each arg As String In DirectCast(reg, KineticsControls).parameters
                    If Not arg.IsNumeric(, includesInteger:=True) Then
                        Dim t = arg.Split("@"c)
                        Dim compart As String = t.Last
                        Dim cid As String = t.Take(t.Length - 1).JoinBy("@")

                        Yield New VariableFactor With {
                            .compartment_id = compart,
                            .id = cid
                        }
                    End If
                Next
            ElseIf TypeOf reg Is KineticsOverlapsControls Then
                For Each kit As KineticsControls In DirectCast(reg, KineticsOverlapsControls).kinetics
                    For Each f As VariableFactor In GetModel(kit)
                        Yield f
                    Next
                Next
            Else
                Throw New NotImplementedException(reg.GetType.FullName)
            End If
        End Function

    End Class

    Public Class FluxEdge

        Public Property left As VariableFactor()
        Public Property right As VariableFactor()
        Public Property regulation As VariableFactor()

    End Class
End Namespace
