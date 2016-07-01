Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.SBML.Level2.Elements
Imports SMRUCC.genomics.Assembly.SBML.Specifics.MetaCyc
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace Models.rFBA

    <PackageNamespace("rFBA.ModelAPI")>
    Public Module ModelAPI

        ''' <summary>
        ''' 只是将反应过程给Dump出来，没有涉及到调控信息
        ''' </summary>
        ''' <param name="model"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("Model.Dump")>
        <Extension> Public Function DumpModel(model As rFBAMetabolism) As Simpheny.RXN()
            Dim upBounds As Double() = model.__getUpbound
            Dim lowBound As Double() = model.__getLowerbound
            Dim rxns As Simpheny.RXN() = model.fluxColumns.ToArray(
                Function(sId, i) __creates(model._fluxs(sId),
                                           upBounds(i),
                                           lowBound(i),
                                           If(model.__fluxObjective.IndexOf(sId) = -1, 0, 1)))
            Return rxns
        End Function

        Private Function __creates(rxn As Reaction, up As Double, lower As Double, objective As Double) As Simpheny.RXN
            Dim props As New FluxPropReader(rxn.Notes)
            Dim model As New Simpheny.RXN With {
                .UPPER_BOUND = up,
                .LOWER_BOUND = lower,
                .Reversible = rxn.reversible,
                .ReactionId = rxn.id,
                .OfficialName = rxn.name,
                .Abbreviation = rxn.__getEquation,
                .Enzyme = props.GENE_ASSOCIATION,
                .Objective = objective
            }
            Return model
        End Function

        <Extension> Private Function __getEquation(rxn As Reaction) As String
            Return EquationBuilder.ToString(rxn)
        End Function
    End Module
End Namespace