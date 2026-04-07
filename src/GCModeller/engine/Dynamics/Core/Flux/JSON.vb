#Region "Microsoft.VisualBasic::8096b94e1e13d59ed486369aeaa8e3b9, engine\Dynamics\Core\Flux\JSON.vb"

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

    '   Total Lines: 113
    '    Code Lines: 82 (72.57%)
    ' Comment Lines: 15 (13.27%)
    '    - Xml Docs: 86.67%
    ' 
    '   Blank Lines: 16 (14.16%)
    '     File Size: 4.50 KB


    '     Class VariableFactor
    ' 
    '         Properties: compartment_id, factor, id, mass_id
    ' 
    '         Function: (+3 Overloads) GetModel, ToString
    ' 
    '     Class FluxEdge
    ' 
    '         Properties: id, left, name, regulation, right
    ' 
    '         Function: FactorIds, (+2 Overloads) ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq

Namespace Core

    Public Class VariableFactor

        Public Property id As String
        Public Property compartment_id As String
        Public Property factor As Double

        Public ReadOnly Property mass_id As String
            Get
                Return id.Replace("@" & compartment_id, "")
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return id
        End Function

        Public Shared Function GetModel(list As IEnumerable(Of Variable), Optional sign As Double = 1) As IEnumerable(Of VariableFactor)
            Return From m As Variable
                   In list
                   Select New VariableFactor With {
                       .id = m.mass.ID,
                       .compartment_id = m.mass.cellular_compartment,
                       .factor = m.coefficient * sign
                   }
        End Function

        Public Shared Iterator Function GetModel(forwards As Controls, reverse As Controls) As IEnumerable(Of VariableFactor)
            If Not TypeOf forwards Is BaselineControls Then
                For Each f As VariableFactor In GetModel(forwards, 1)
                    Yield f
                Next
            End If
            If Not TypeOf reverse Is BaselineControls Then
                For Each r As VariableFactor In GetModel(reverse, -1)
                    Yield r
                Next
            End If
        End Function

        Private Shared Iterator Function GetModel(reg As Controls, factor As Double) As IEnumerable(Of VariableFactor)
            If TypeOf reg Is AdditiveControls Then
                For Each r As VariableFactor In GetModel(DirectCast(reg, AdditiveControls).activation, factor)
                    Yield r
                Next
            ElseIf TypeOf reg Is KineticsControls Then
                For Each arg As String In DirectCast(reg, KineticsControls).parameters
                    If Not arg.IsNumeric(, includesInteger:=True) Then
                        Dim t = arg.Split("@"c)
                        Dim compart As String = t.Last
                        ' Dim cid As String = t.Take(t.Length - 1).JoinBy("@")

                        Yield New VariableFactor With {
                            .compartment_id = compart,
                            .id = arg,
                            .factor = factor
                        }
                    End If
                Next
            ElseIf TypeOf reg Is KineticsOverlapsControls Then
                For Each kit As KineticsControls In DirectCast(reg, KineticsOverlapsControls).kinetics
                    For Each f As VariableFactor In GetModel(kit, factor)
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
        ''' <summary>
        ''' regulation or enzyme catalysis
        ''' </summary>
        ''' <returns></returns>
        Public Property regulation As VariableFactor()
        Public Property id As String
        ''' <summary>
        ''' the name of this biological process, used for debug used
        ''' </summary>
        ''' <returns></returns>
        Public Property name As String

        Public Overrides Function ToString() As String
            Return left.JoinBy(" + ") & " = " & right.JoinBy(" + ")
        End Function

        Public Overloads Function ToString(symbols As Dictionary(Of String, String)) As String
            Return left.Select(Function(v) symbols.TryGetValue(v.mass_id, [default]:=v.id)).JoinBy(" + ") & " = " & right.Select(Function(v) symbols.TryGetValue(v.mass_id, [default]:=v.id)).JoinBy(" + ")
        End Function

        ''' <summary>
        ''' populate all related factor in current flux model:
        ''' 
        ''' left + right + regulation
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function FactorIds() As IEnumerable(Of String)
            For Each factor As VariableFactor In left.JoinIterates(right).JoinIterates(regulation)
                Yield factor.id
            Next
        End Function

    End Class
End Namespace
