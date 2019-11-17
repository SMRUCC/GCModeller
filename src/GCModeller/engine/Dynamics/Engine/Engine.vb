#Region "Microsoft.VisualBasic::2d8de1fcb622c6e0ace5d3f0d3f8c3f3, engine\Dynamics\Engine\Engine.vb"

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

'     Class Engine
' 
'         Function: GetMass
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

Namespace Engine

    Public Delegate Sub DataStorageDriver(iteration%, data As Dictionary(Of String, Double))

    Public Class Engine : Implements ITaskDriver

        Dim mass As MassTable

        ''' <summary>
        ''' The biological flux simulator engine core module
        ''' </summary>
        Dim core As Vessel
        Dim def As Definition
        Dim model As CellularModule
        Dim iterations As Integer = 5000
        Dim dataStorageDriver As OmicsDataAdapter

        Sub New(def As Definition, Optional iterations% = 5000)
            Me.def = def
            Me.iterations = iterations
        End Sub

        Public Function AttachBiologicalStorage(driver As OmicsDataAdapter) As Engine
            dataStorageDriver = driver
            Return Me
        End Function

        Public Function LoadModel(virtualCell As CellularModule, Optional timeResolution# = 1000) As Engine
            Dim loader As New Loader(def)
            Dim cell As Core.Vessel = loader _
                .CreateEnvironment(virtualCell) _
                .Initialize(timeResolution)

            core = cell
            mass = loader.massTable
            model = virtualCell

            Call Reset()

            Return Me
        End Function

        ''' <summary>
        ''' Reset the reactor engine. (Do reset of the biological mass contents)
        ''' </summary>
        Public Sub Reset()
            For Each mass As Factor In Me.mass
                If def.status.ContainsKey(mass.ID) Then
                    mass.Value = def.status(mass.ID)
                Else
                    mass.Value = 1
                End If
            Next
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetMass(names As IEnumerable(Of String)) As IEnumerable(Of Factor)
            Return mass.GetByKey(names)
        End Function

        Public Function Run() As Integer Implements ITaskDriver.Run
            For i As Integer = 0 To iterations
                Call dataStorageDriver.FluxSnapshot(i, core.ContainerIterator().ToDictionary.FlatTable)
                Call dataStorageDriver.MassSnapshot(i, mass.GetMassValues)
            Next

            Return 0
        End Function
    End Class
End Namespace
