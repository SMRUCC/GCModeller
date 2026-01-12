#Region "Microsoft.VisualBasic::e9bc85b8550aa2f1a5e9138045d90826, data\Rhea\Reaction.vb"

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

'   Total Lines: 47
'    Code Lines: 30 (63.83%)
' Comment Lines: 8 (17.02%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 9 (19.15%)
'     File Size: 1.51 KB


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
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Namespace ComponentModel.EquaionModel

    ''' <summary>
    ''' A general reaction model
    ''' </summary>
    Public Class Reaction : Implements INamedValue

        <XmlAttribute> Public Property entry As String Implements INamedValue.Key

        ''' <summary>
        ''' the reaction equation in character string type
        ''' </summary>
        ''' <returns></returns>
        Public Property definition As String
        ''' <summary>
        ''' the parsed reaction equaltion object based on the definition property
        ''' </summary>
        ''' <returns></returns>
        Public Property equation As Equation

        ''' <summary>
        ''' should be the EC number
        ''' </summary>
        ''' <returns></returns>
        <XmlElement>
        Public Property enzyme As String()
        Public Property compounds As SideCompound()
        <XmlAttribute>
        Public Property isTransport As Boolean
        Public Property db_xrefs As NamedValue()
        Public Property comment As String

        Public Shared Function EquationParser(text As String) As Equation
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

        Public Shared Function FromKeggReaction(r As DBGET.bGetObject.Reaction) As Reaction
            Dim model As Equation = r.ReactionModel
            Dim left As SideCompound() = model.Reactants.Select(Function(a) New SideCompound With {.side = "left", .compound = New CompoundSpecies(a.ID)}).ToArray
            Dim right As SideCompound() = model.Products.Select(Function(a) New SideCompound With {.side = "right", .compound = New CompoundSpecies(a.ID)}).ToArray

            Return New Reaction With {
                .comment = If(r.Comments.StringEmpty, r.Definition, r.Comments),
                .entry = r.ID,
                .definition = If(r.CommonNames.DefaultFirst, r.Definition),
                .enzyme = r.Enzyme,
                .equation = model,
                .compounds = left.JoinIterates(right).ToArray
            }
        End Function

    End Class
End Namespace