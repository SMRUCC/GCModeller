#Region "Microsoft.VisualBasic::bfabf28dfa4a776b510f5557ebfee877, core\Bio.Assembly\ComponentModel\Equations\Equation\Default\CompoundSpecieReference.vb"

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

    '   Total Lines: 60
    '    Code Lines: 42 (70.00%)
    ' Comment Lines: 7 (11.67%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 11 (18.33%)
    '     File Size: 1.95 KB


    '     Class CompoundSpecieReference
    ' 
    '         Properties: Compartment, ID, Stoichiometry
    ' 
    '         Constructor: (+4 Overloads) Sub New
    '         Function: AsFactor, Equals, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.TagData

Namespace ComponentModel.EquaionModel.DefaultTypes

    ''' <summary>
    ''' the compound model reference.
    ''' </summary>
    Public Class CompoundSpecieReference : Implements ICompoundSpecies

        ''' <summary>
        ''' 化学计量数
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Stoichiometry As Double Implements ICompoundSpecies.Stoichiometry
        <XmlText> Public Property ID As String Implements ICompoundSpecies.Key
        <XmlAttribute> Public Property Compartment As String

        Sub New()
        End Sub

        Sub New(ref As ICompoundSpecies)
            Stoichiometry = ref.Stoichiometry
            ID = ref.Key
        End Sub

        Sub New(factor As Double, compound As String)
            Stoichiometry = factor
            ID = compound
        End Sub

        Sub New(factor As Double, compound As String, compart As String)
            Me.Stoichiometry = factor
            Me.ID = compound
            Me.Compartment = compart
        End Sub

        Public Overloads Function Equals(b As ICompoundSpecies, strict As Boolean) As Boolean
            Return Equivalence.Equals(Me, b, strict)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function AsFactor() As FactorString(Of Double)
            Return New FactorString(Of Double) With {
                .factor = Stoichiometry,
                .result = ID
            }
        End Function

        Public Overrides Function ToString() As String
            If Stoichiometry > 1 Then
                Return String.Format("{0} {1}", Stoichiometry, ID)
            Else
                Return ID
            End If
        End Function
    End Class

End Namespace
