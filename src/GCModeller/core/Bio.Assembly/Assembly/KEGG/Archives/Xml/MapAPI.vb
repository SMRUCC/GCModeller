Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports LANS.SystemsBiology.Assembly.KEGG.DBGET
Imports LANS.SystemsBiology.ComponentModel.EquaionModel.DefaultTypes
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
            Dim allCompounds As String() = from.Compound.ToArray(Function(x) x.Key)
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
                Dim sId As String = x.Identifier
                If Array.IndexOf(cps, sId) = -1 Then  ' 当前的化合物不存在
                    Return False  ' 则当前的这个过程很明显不会发生
                End If
            Next

            Return True
        End Function
    End Module
End Namespace