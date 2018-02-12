#Region "Microsoft.VisualBasic::b070b89f182c34c99aa64ee948d0b146, CARMEN\Analysis\Reaction.vb"

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

    ' Class Reaction
    ' 
    '     Properties: ECs, lstEnzyme, lstGene, Pathway, rnId
    ' 
    '     Function: __getECs, __getGeneIDAssociation, CreateObject, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports SMRUCC.genomics.Model.SBML

Public Class Reaction

    ''' <summary>
    ''' KEGG reaction Id
    ''' </summary>
    ''' <returns></returns>
    Public Property rnId As String
    Public Property ECs As String()
    Public Property lstGene As String()
    Public Property lstEnzyme As String()
    Public Property Pathway As String

    Public Overrides Function ToString() As String
        Return rnId
    End Function

    Public Shared Function CreateObject(SbmlModel As Level3.Reaction) As Reaction
        Dim Reaction As Reaction = New Reaction With {
            .rnId = SbmlModel.id,
            .ECs = __getECs(SbmlModel.name)
        }
        Dim associats As Dictionary(Of String, String) = __getGeneIDAssociation(SbmlModel)
#If DEBUG Then
        If associats.Count > 0 Then
            Call Console.Write(SbmlModel.id & "  ")
        End If
#End If
        Reaction.lstGene = (From ModifierReference In SbmlModel.listOfModifiers
                            Let GetItemValue As String = If(associats.ContainsKey(ModifierReference.species), associats.Item(ModifierReference.species), "")
                            Select GetItemValue).ToArray
        Reaction.lstEnzyme = associats.Keys.ToArray
        Return Reaction
    End Function

    Private Shared Function __getECs(name As String) As String()
        Dim list = name.Split
        list = (From s As String In list
                Let x As String = Regex.Match(s, "\d+(\.\d+)+").Value
                Where Not String.IsNullOrEmpty(x)
                Select x).ToArray
        Return list
    End Function

    ''' <summary>
    ''' 得到和这个代谢途径相关的基因的列表
    ''' </summary>
    ''' <param name="Reaction"></param>
    ''' <returns></returns>
    Private Shared Function __getGeneIDAssociation(Reaction As Level3.Reaction) As Dictionary(Of String, String)
        Dim ProteinName As String() = (From m As Match
                                       In Regex.Matches(Reaction.Notes.Text, "PROTEIN_ASSOCIATION: [a-z0-9]+", RegexOptions.Singleline + RegexOptions.IgnoreCase)
                                       Select Strings.Split(m.Value, ": ").Last).ToArray
        Dim GeneId As String() = (From m As Match
                                  In Regex.Matches(Reaction.Notes.Text, "GENE_ASSOCIATION: [0-9a-z_]+", RegexOptions.Singleline + RegexOptions.IgnoreCase)
                                  Let strData As String = m.Value
                                  Where Not String.Equals(strData, "GENE_ASSOCIATION: No")
                                  Select Strings.Split(strData, ": ").Last).ToArray
        If GeneId.IsNullOrEmpty Then
            Return New Dictionary(Of String, String)
        Else
            Dim LQuery = (From i As Integer
                          In GeneId.Sequence
                          Select New KeyValuePair(Of String, String)(ProteinName(i), GeneId(i))).ToArray
            Dim Dict As Dictionary(Of String, String) = New Dictionary(Of String, String)
            For Each item In LQuery
                If Not Dict.ContainsKey(item.Key) Then Call Dict.Add(item.Key, item.Value)
            Next

            Return Dict
        End If
    End Function
End Class
