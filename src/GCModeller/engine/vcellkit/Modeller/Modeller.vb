Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Data.SABIORK
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.Rsharp.Runtime

<Package("vcellkit.modeller")>
Module Modeller

    ''' <summary>
    ''' apply the kinetics parameters from the sabio-rk database.
    ''' </summary>
    ''' <param name="vcell"></param>
    ''' <returns></returns>
    <ExportAPI("apply.kinetics")>
    Public Function applyKinetics(vcell As VirtualCell, Optional cache$ = "./.cache", Optional env As Environment = Nothing) As VirtualCell
        Dim keggEnzymes = htext.GetInternalResource("ko01000").Hierarchical _
            .EnumerateEntries _
            .Where(Function(key) Not key.entryID.StringEmpty) _
            .GroupBy(Function(enz) enz.entryID) _
            .ToDictionary(Function(KO) KO.Key,
                          Function(ECNumber)
                              Return ECNumber.ToArray
                          End Function)
        Dim numbers As BriteHText()

        For Each enzyme As Enzyme In vcell.metabolismStructure.enzymes
            If keggEnzymes.ContainsKey(enzyme.KO) Then
                numbers = keggEnzymes(enzyme.KO)

                For Each number As String In numbers.Select(Function(num) num.parent.classLabel.Split.First)
                    Dim kinetics = WebRequest.QueryByECNumber(number, cache)
                Next
            Else
                env.AddMessage($"missing ECNumber mapping for '{enzyme.KO}'.", MSG_TYPES.WRN)
            End If
        Next

        Return vcell
    End Function

    <ExportAPI("cacheOf.enzyme_kinetics")>
    Public Sub createKineticsDbCache(Optional export$ = "./")
        Call htext.GetInternalResource("ko01000").QueryByECNumbers(export).ToArray
    End Sub

    ''' <summary>
    ''' read the virtual cell model file
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    <ExportAPI("read.vcell")>
    Public Function LoadVirtualCell(path As String) As VirtualCell
        Return path.LoadXml(Of VirtualCell)
    End Function
End Module
