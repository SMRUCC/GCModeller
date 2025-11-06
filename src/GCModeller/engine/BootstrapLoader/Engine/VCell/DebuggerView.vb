#Region "Microsoft.VisualBasic::11150e88e2c00b0e847a37384925774b, engine\BootstrapLoader\Engine\VCell\DebuggerView.vb"

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

'   Total Lines: 74
'    Code Lines: 56 (75.68%)
' Comment Lines: 7 (9.46%)
'    - Xml Docs: 85.71%
' 
'   Blank Lines: 11 (14.86%)
'     File Size: 2.55 KB


'     Class DebuggerView
' 
'         Properties: dataStorageDriver, mass, viewMetabolome, viewProteome, viewTranscriptome
' 
'         Constructor: (+1 Overloads) Sub New
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine

Namespace Engine

    Public Class DebuggerView

        ReadOnly engine As Engine
        ReadOnly cellular_id As String()

        Public ReadOnly Property mass As MassTable
            Get
                Return engine.getMassPool
            End Get
        End Property

        Public ReadOnly Property dataStorageDriver As IOmicsDataAdapter
            Get
                Return engine.dataStorageDriver
            End Get
        End Property

#Region "Debug views"

        Public ReadOnly Property viewTranscriptome As Dictionary(Of String, Double)
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return getByIds(dataStorageDriver.mass.transcriptome)
            End Get
        End Property

        Public ReadOnly Property viewProteome As Dictionary(Of String, Double)
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return getByIds(dataStorageDriver.mass.proteome)
            End Get
        End Property

        Public ReadOnly Property viewMetabolome As Dictionary(Of String, Double)
            Get
                Return getByIds(dataStorageDriver.mass.metabolome)
            End Get
        End Property

#End Region

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="engine"></param>
        ''' <param name="cellular_id">
        ''' the intracellular compartment id
        ''' </param>
        Sub New(engine As Engine, cellular_id As String())
            Me.engine = engine
            Me.cellular_id = cellular_id
        End Sub

        Private Function getByIds(idset As IEnumerable(Of String)) As Dictionary(Of String, Double)
            Dim debug As New Dictionary(Of String, Double)

            For Each compart_id As String In cellular_id
                For Each mass As Factor In Me.mass.GetByKey(idset, compart_id)
                    Call debug.Add(mass.ID, mass.Value)
                Next
            Next

            Return debug
        End Function
    End Class
End Namespace
