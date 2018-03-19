#Region "Microsoft.VisualBasic::26be6bfa46102a60962c1db86288f11a, CLI_tools\c2\Reconstruction\KineticLaws\EquationEquals.vb"

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

    ' Class EquationEquals
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: ApplyAnalysis
    '     Class EquationEqualsMapping
    ' 
    '         Properties: Comments, CommonNames, KEGG_Equation, KEGG_Id, Metacyc_Equation
    '                     Metacyc_Id, Types
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions

Public Class EquationEquals : Inherits LANS.SystemsBiology.Assembly.MetaCyc.Schema.EquationEquals

    Public Class EquationEqualsMapping
        Public Property KEGG_Id As String
        Public Property KEGG_Equation As String
        Public Property Metacyc_Id As String
        Public Property Metacyc_Equation As String
        Public Property CommonNames As String()
        Public Property Comments As String
        Public Property Types As String()
    End Class

    Dim _MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder

    Sub New(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder,
            KEGGCompounds As LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Compound())
        Call MyBase.New(MetaCyc.GetCompounds, KEGGCompounds)
        _MetaCyc = MetaCyc
    End Sub

    Public Function ApplyAnalysis(KEGGReactions As LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Reaction()) As EquationEqualsMapping()
        Dim LQuery = (From Reaction In LANS.SystemsBiology.Assembly.MetaCyc.Schema.Metabolism.Reaction.DirectCast(_MetaCyc.GetReactions).AsParallel
                      Let EqualsPair = (From item In KEGGReactions Where MyBase.Equals(item.Equation, Reaction.Equation, False) Select item).ToArray
                      Let Generation = Function() As EquationEqualsMapping
                                           Dim EquationEqualsMapping = New EquationEqualsMapping With {.Metacyc_Id = Reaction.Identifier, .Metacyc_Equation = Reaction.Equation, .Types = Reaction.Types.ToArray}
                                           If Not EqualsPair.IsNullOrEmpty Then
                                               Dim EqualsKEGGReaction = EqualsPair.First
                                               EquationEqualsMapping.KEGG_Id = EqualsKEGGReaction.Entry
                                               EquationEqualsMapping.KEGG_Equation = EqualsKEGGReaction.Equation
                                               EquationEqualsMapping.CommonNames = EqualsKEGGReaction.CommonNames
                                               EquationEqualsMapping.Comments = EqualsKEGGReaction.Comments
                                           End If
                                           Return EquationEqualsMapping
                                       End Function Select Generation()).ToArray
        Return LQuery
    End Function
End Class
