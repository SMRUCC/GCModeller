#Region "Microsoft.VisualBasic::806e5955367bbbeb5ff52f83bf364c9a, vcellkit\Modeller\Modeller.vb"

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

    ' Module Modeller
    ' 
    '     Function: applyKinetics, LoadVirtualCell
    ' 
    '     Sub: createKineticsDbCache
    ' 
    ' /********************************************************************************/

#End Region

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

