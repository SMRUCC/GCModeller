Imports System.Text.RegularExpressions
Imports System.Text
Imports LANS.SystemsBiology.Assembly.SBML.FLuxBalanceModel
Imports System.Runtime.CompilerServices
Imports LANS.SystemsBiology.Assembly.SBML.Level2.Elements
Imports LANS.SystemsBiology.Assembly.SBML.Specifics.MetaCyc
Imports Microsoft.VisualBasic

Namespace Level2

    Public Module API

        Public Const [Namespace] As String = "http://www.sbml.org/sbml/level2"

        ''' <summary>
        ''' Check if the metabolite is a entry node of a model.(判断这个代谢物节点是否为本模型的根节点)
        ''' </summary>
        ''' <param name="Metabolite"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension> Public Function IsEntry(sbml As XmlFile, metabolite As String) As Boolean
            Dim Query = From e In sbml.Model.listOfReactions Where e.Produce(metabolite) Select e '
            Return Not Query.Count > 0  '当该代谢物仅有消耗而无合成项目的时候，该代谢物为入口
        End Function

        ''' <summary>
        ''' Get all reaction that consume this metabolite.(获取消耗本代谢物质的所有反应)
        ''' </summary>
        ''' <param name="Metabolite"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension> Public Function GetAllConsume(sbml As XmlFile, Metabolite As String) As Elements.Reaction()
            Dim Query = From e In sbml.Model.listOfReactions Where e.Consume(Metabolite) Select e '
            Return Query.ToArray
        End Function

        <Extension> Public Function GetAllProduce(SBML As Level2.XmlFile, Metabolite As String) As Elements.Reaction()
            Dim Query = From e In SBML.Model.listOfReactions Where e.Produce(Metabolite) Select e '
            Return Query.ToArray
        End Function

        <Extension> Public Function RevertEscapes(file As XmlFile, Replacement As Escaping()) As XmlFile
            Dim model As Elements.Model = file.Model
            Call Escaping.Replace(Of SBML.Level2.Elements.Specie)(model.listOfSpecies, Replacement)
            Call Escaping.Replace(Of SBML.Level2.Elements.Reaction)(model.listOfReactions, Replacement)
            Dim List As List(Of speciesReference) = New List(Of speciesReference)
            For Each rxn In model.listOfReactions
                Call List.AddRange(rxn.Products)
                Call List.AddRange(rxn.Reactants)
            Next
            Call Escaping.Replace(List, Replacement)
            Return file
        End Function
    End Module
End Namespace