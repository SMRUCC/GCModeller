Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.xml.MathML
Imports SMRUCC.genomics.Data.SABIORK.SBML
Imports SMRUCC.genomics.Data.SABIORK.TabularDump

Public Module ModelHelper

    Public Iterator Function CreateKineticsData(model As SbmlDocument) As IEnumerable(Of (rxnId As String, EnzymeCatalystKineticLaw))
        Dim indexer As New SBMLInternalIndexer(model)
        Dim mathList = model.mathML _
            .ToDictionary(Function(a) a.Name,
                          Function(a)
                              Return a.Value
                          End Function)

        For Each rxn As SBMLReaction In model.sbml.model.listOfReactions.AsEnumerable
            Dim mathId As String = "KL_" & rxn.kineticLawID
            Dim math As LambdaExpression = mathList.TryGetValue(mathId)

            If math Is Nothing OrElse math.lambda Is Nothing Then
                Continue For
                ' -1 situation: ci first maybe the kinetics id
            ElseIf (math.parameters.Length <> rxn.kineticLaw.math.apply.ci.Length) AndAlso
                (math.parameters.Length <> rxn.kineticLaw.math.apply.ci.Length - 1) Then
                Continue For
            ElseIf rxn.kineticLaw Is Nothing OrElse rxn.kineticLaw.listOfLocalParameters.IsNullOrEmpty Then
                Continue For
            Else
                Yield (rxn.id, EnzymeCatalystKineticLaw.Create(rxn, math, doc:=indexer))
            End If
        Next
    End Function
End Module
