#Region "Microsoft.VisualBasic::2638983bd86496eb0d23a46dc751ab15, GCModeller\engine\BootstrapLoader\Engine\VCell\DebuggerView.vb"

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

    '   Total Lines: 65
    '    Code Lines: 54
    ' Comment Lines: 0
    '   Blank Lines: 11
    '     File Size: 2.13 KB


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
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine

Namespace Engine

    Public Class DebuggerView

        ReadOnly engine As Engine

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
                Return mass _
                    .GetByKey(dataStorageDriver.mass.transcriptome) _
                    .ToDictionary(Function(mass) mass.ID,
                                  Function(mass)
                                      Return mass.Value
                                  End Function)
            End Get
        End Property

        Public ReadOnly Property viewProteome As Dictionary(Of String, Double)
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return mass _
                    .GetByKey(dataStorageDriver.mass.proteome) _
                    .ToDictionary(Function(mass) mass.ID,
                                  Function(mass)
                                      Return mass.Value
                                  End Function)
            End Get
        End Property

        Public ReadOnly Property viewMetabolome As Dictionary(Of String, Double)
            Get
                Return mass _
                    .GetByKey(dataStorageDriver.mass.metabolome) _
                    .ToDictionary(Function(mass) mass.ID,
                                  Function(mass)
                                      Return mass.Value
                                  End Function)
            End Get
        End Property

#End Region

        Sub New(engine As Engine)
            Me.engine = engine
        End Sub
    End Class
End Namespace
