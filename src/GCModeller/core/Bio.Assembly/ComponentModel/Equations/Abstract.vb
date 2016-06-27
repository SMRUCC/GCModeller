Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace ComponentModel.EquaionModel

    Public Interface ICompoundSpecies : Inherits sIdEnumerable
        Property StoiChiometry As Double
    End Interface

    Public Interface IEquation(Of TCompound As ICompoundSpecies)
        ''' <summary>
        ''' On the equation left side.
        ''' </summary>
        ''' <returns></returns>
        Property Reactants As TCompound()
        ''' <summary>
        ''' On the equation right side.
        ''' </summary>
        ''' <returns></returns>
        Property Products As TCompound()
        Property Reversible As Boolean
    End Interface
End Namespace