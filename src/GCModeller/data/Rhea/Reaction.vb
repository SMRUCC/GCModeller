#Region "Microsoft.VisualBasic::8cb81286ae0a4102262b7c2acc9c9ee5, GCModeller\data\Rhea\Reaction.vb"

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

    '   Total Lines: 30
    '    Code Lines: 22
    ' Comment Lines: 0
    '   Blank Lines: 8
    '     File Size: 924 B


    ' Class Reaction
    ' 
    '     Properties: definition, entry, enzyme, equation
    ' 
    '     Function: EquationParser, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Public Class Reaction : Implements INamedValue

    Public Property entry As String Implements INamedValue.Key
    Public Property definition As String
    Public Property equation As Equation
    Public Property enzyme As String()

    Friend Shared Function EquationParser(text As String) As Equation
        Dim eq As Equation = Equation.TryParse(text)

        For Each cpd As CompoundSpecieReference In eq.Reactants
            If cpd.ID.IndexOf(","c) > -1 Then
                Dim t = cpd.ID.Split(","c)

                cpd.StoiChiometry = t.Length
                cpd.ID = t(Scan0)
            End If
        Next

        Return eq
    End Function

    Public Overrides Function ToString() As String
        Return definition
    End Function

End Class

