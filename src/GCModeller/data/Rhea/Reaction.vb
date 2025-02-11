#Region "Microsoft.VisualBasic::bccdee45c03d1532ef46a8aeb3c7e271, data\Rhea\Reaction.vb"

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

    '   Total Lines: 36
    '    Code Lines: 28 (77.78%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 8 (22.22%)
    '     File Size: 1.22 KB


    ' Class Reaction
    ' 
    '     Properties: comment, compounds, db_xrefs, definition, entry
    '                 enzyme, equation, isTransport
    ' 
    '     Function: EquationParser, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Public Class Reaction : Implements INamedValue

    <XmlAttribute> Public Property entry As String Implements INamedValue.Key
    Public Property definition As String
    Public Property equation As Equation
    <XmlElement> Public Property enzyme As String()
    Public Property compounds As SideCompound()
    <XmlAttribute> Public Property isTransport As Boolean
    Public Property db_xrefs As NamedValue()
    Public Property comment As String

    Friend Shared Function EquationParser(text As String) As Equation
        Dim eq As Equation = Equation.TryParse(text)

        For Each cpd As CompoundSpecieReference In eq.Reactants
            If cpd.ID.IndexOf(","c) > -1 Then
                Dim t = cpd.ID.Split(","c)

                cpd.Stoichiometry = t.Length
                cpd.ID = t(Scan0)
            End If
        Next

        Return eq
    End Function

    Public Overrides Function ToString() As String
        Return definition
    End Function

End Class
