#Region "Microsoft.VisualBasic::89fd37a7ee8e180d0c91017bb2f1eae7, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Files\Reactions.vb"

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

    '   Total Lines: 85
    '    Code Lines: 60
    ' Comment Lines: 16
    '   Blank Lines: 9
    '     File Size: 4.62 KB


    '     Class Reactions
    ' 
    '         Properties: AttributeList
    ' 
    '         Function: getECDictionary, GetTransportReactions, ToString, (+2 Overloads) Trim
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.File.DataFiles

    ''' <summary>
    ''' Frames within the Reactions class describe properties of a biochemical reaction independent
    ''' of any enzyme or enzymes that catalyze that reaction. A reaction is a biochemical
    ''' transformation that interconverts two sets of chemical compounds (which includes small
    ''' metabolites, proteins, and DNA regions), and may translocate compounds from one cellular
    ''' compartment to another. Most reactions are written in a conventional direction that
    ''' has been assigned by the Enzyme Nomenclature Commission, but that direction may or
    ''' may not be the predominate physiological direction of the reaction. Reaction substrates
    ''' can include small-molecular-weight compounds (for metabolic reactions), proteins (such
    ''' as in signaling pathways), and DNA sites (such as for reactions involving binding of transcription
    ''' factors to DNA).
    ''' </summary>
    ''' <remarks>
    ''' This file lists all chemical reactions in the PGDB.
    ''' (本数据库文件中记录了本菌种内所有的化学反应)
    ''' </remarks>
    Public Class Reactions : Inherits DataFile(Of Slots.Reaction)

        Public Overrides ReadOnly Property AttributeList As String()
            Get
                Return {
                    "UNIQUE-ID", "TYPES", "COMMON-NAME", "ATOM-MAPPINGS", "CANNOT-BALANCE?",
                    "CITATIONS", "COMMENT", "COMMENT-INTERNAL", "CREDITS", "DATA-SOURCE",
                    "DBLINKS", "DELTAG0", "DOCUMENTATION", "EC-NUMBER", "ENZYMATIC-REACTION",
                    "ENZYMES-NOT-USED", "EQUILIBRIUM-CONSTANT", "HIDE-SLOT?", "IN-PATHWAY",
                    "INSTANCE-NAME-TEMPLATE", "LEFT", "MEMBER-SORT-FN", "ORPHAN?",
                    "PHYSIOLOGICALLY-RELEVANT?", "PREDECESSORS", "PRIMARIES", "REACTION-DIRECTION",
                    "REACTION-LIST", "REGULATED-BY", "REQUIREMENTS", "RIGHT", "RXN-LOCATIONS",
                    "SIGNAL", "SPECIES", "SPONTANEOUS?", "STD-REDUCTION-POTENTIAL", "SYNONYMS",
                    "SYSTEMATIC-NAME", "TEMPLATE-FILE"
                }
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}  {1} frame object records.", DbProperty.ToString, FrameObjects.Count)
        End Function

        Public Function GetTransportReactions() As MetaCyc.Schema.TransportReaction()
            Dim LQuery = (From ReactionObject As Slots.Reaction
                          In MyBase.Values
                          Where ReactionObject.IsTransportReaction
                          Select New MetaCyc.Schema.TransportReaction(ReactionObject)).ToArray
            Return LQuery
        End Function

        Protected Friend Shared Function Trim(Reaction As Slots.Reaction) As Slots.Reaction
            Reaction.Left = (From str As String In Reaction.Left Select Reactions.Trim(str)).AsList
            Reaction.Right = (From str As String In Reaction.Right Select Reactions.Trim(str)).AsList
            Return Reaction
        End Function

        Public Function getECDictionary() As Dictionary(Of String, Slots.Reaction())
            Dim ECIdlist As String() = (From item As Slots.Reaction
                                        In Me
                                        Let value As String = item.ECNumber
                                        Where Not String.IsNullOrEmpty(value)
                                        Select value
                                        Distinct
                                        Order By value Ascending).ToArray
            Dim Dict As Dictionary(Of String, Slots.Reaction()) = New Dictionary(Of String, Slots.Reaction())
            For Each strECId As String In ECIdlist
                Call Dict.Add(strECId.Replace("EC-", ""), (From item As Slots.Reaction
                                                           In Me.Values.AsParallel
                                                           Where String.Equals(item.ECNumber, strECId)
                                                           Select item).ToArray)
            Next

            Return Dict
        End Function

        Protected Friend Shared Function Trim(strData As String) As String
            If strData.First = "|"c AndAlso strData.Last = "|" Then
                strData = Mid(strData, 2)
                strData = Mid(strData, 1, Len(strData) - 1)
            End If
            Return strData
        End Function
    End Class
End Namespace
