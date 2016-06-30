Imports System.Runtime.CompilerServices
Imports LANS.SystemsBiology.Assembly.MetaCyc.Schema
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module
Imports LANS.SystemsBiology.DatabaseServices.SabiorkKineticLaws.TabularDump

Namespace EngineSystem

    Module GCModellerCommons

        Public LoggingClient As Logging.LogFile

        <Extension> Public Function GetCompartment(Compartments As ICompartmentObject(), CompartmentId As String) As ICompartmentObject
            Dim LQuery = (From compX As ICompartmentObject
                          In Compartments
                          Where String.Equals(CompartmentId, compX.CompartmentId)
                          Select compX).FirstOrDefault
            Return LQuery
        End Function

        <Extension> Public Function GetValue(chunkBuffer As KeyValuePair(Of String, String)(), var As String) As String
            Dim LQuery = (From x In chunkBuffer Where String.Equals(x.Key, var) Select x.Value).FirstOrDefault
            Return LQuery
        End Function

        <Extension> Public Function GetKineticLaw(Enzymes As EnzymeCatalystKineticLaw(), EnzymeId As String, ReactionId As String) As EnzymeCatalystKineticLaw
            Dim LQuery = (From enzyme As EnzymeCatalystKineticLaw
                          In Enzymes
                          Where String.Equals(EnzymeId, enzyme.Enzyme) AndAlso
                              String.Equals(ReactionId, enzyme.KineticRecord)
                          Select enzyme).FirstOrDefault
            Return LQuery
        End Function

        ''' <summary>
        ''' 调用<see cref="EngineSystem.ObjectModels.[Module].FluxObject.Invoke"></see>方法，修改系统的状态
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Collection"></param>
        ''' <remarks></remarks>
        <Extension> Public Sub Invoke(Of T As FluxObject)(Collection As IEnumerable(Of T))
            Dim LQuery = (From fluxHandle As FluxObject
                          In Collection.Randomize
                          Select fluxHandle.Invoke).ToArray
        End Sub
    End Module
End Namespace