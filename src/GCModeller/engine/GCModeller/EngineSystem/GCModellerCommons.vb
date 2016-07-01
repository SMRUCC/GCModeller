#Region "Microsoft.VisualBasic::51afb2136668c404226086b4f1fa0c84, ..\GCModeller\engine\GCModeller\EngineSystem\GCModellerCommons.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.MetaCyc.Schema
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module
Imports SMRUCC.genomics.DatabaseServices.SabiorkKineticLaws.TabularDump

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
