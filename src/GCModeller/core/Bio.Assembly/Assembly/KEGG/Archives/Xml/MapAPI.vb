#Region "Microsoft.VisualBasic::15072a691982284553adada5b9be302e, GCModeller\core\Bio.Assembly\Assembly\KEGG\Archives\Xml\MapAPI.vb"

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

    '   Total Lines: 66
    '    Code Lines: 42
    ' Comment Lines: 17
    '   Blank Lines: 7
    '     File Size: 3.01 KB


    '     Module MapAPI
    ' 
    '         Function: EcParser, (+2 Overloads) GetReactions, IsThisReaction
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace Assembly.KEGG.Archives.Xml

    Module MapAPI

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="value">sensory histidine kinase CreC; K07641 two-component system, OmpR family, sensor histidine kinase CreC [EC:2.7.13.3] [KO:K07641] [EC:2.7.13.3]</param>
        ''' <returns></returns>
        <Extension> Public Function EcParser(value As String) As String()
            Dim LQuery As String() =
                LinqAPI.Exec(Of String) <= From m As Match
                                           In Regex.Matches(value, "\[EC.+?\]", RegexOptions.IgnoreCase)
                                           Let strValue As String = Mid(m.Value, 4)
                                           Let str = If(strValue.First = ":"c, Mid(strValue, 2), strValue)
                                           Let ss As String = Mid(str, 1, Len(str) - 1)
                                           Select ss.Split
                                           Distinct
            Return LQuery.Distinct.ToArray
        End Function

        ''' <summary>
        ''' 得到初步的代谢反应
        ''' </summary>
        ''' <param name="from"></param>
        ''' <param name="source"></param>
        ''' <returns></returns>
        Public Function GetReactions(from As bGetObject.Pathway, source As bGetObject.Reaction()) As bGetObject.Reaction()
            Dim allCompounds As String() = from.Compound.Select(Function(x) x.name).ToArray
            Return GetReactions(allCompounds, source)
        End Function

        ''' <summary>
        ''' 得到有Compound所构成的所有反应
        ''' </summary>
        ''' <param name="allCompounds"></param>
        ''' <param name="from"></param>
        ''' <returns></returns>
        Public Function GetReactions(allCompounds As String(), from As bGetObject.Reaction()) As bGetObject.Reaction()
            Dim LQuery = (From X As bGetObject.Reaction
                          In [from]
                          Where allCompounds.IsThisReaction(X)
                          Select X).ToArray
            Return LQuery
        End Function

        <Extension> Public Function IsThisReaction(cps As String(), rxn As bGetObject.Reaction) As Boolean
            Dim LDM As Equation = rxn.ReactionModel
            For Each x As CompoundSpecieReference In LDM.GetMetabolites
                Dim sId As String = x.ID
                If Array.IndexOf(cps, sId) = -1 Then  ' 当前的化合物不存在
                    Return False  ' 则当前的这个过程很明显不会发生
                End If
            Next

            Return True
        End Function
    End Module
End Namespace
