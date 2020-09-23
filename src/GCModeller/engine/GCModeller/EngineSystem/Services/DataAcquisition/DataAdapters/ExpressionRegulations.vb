#Region "Microsoft.VisualBasic::4058aee5d4c7e807c94658d1982e1dc0, engine\GCModeller\EngineSystem\Services\DataAcquisition\DataAdapters\ExpressionRegulations.vb"

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

    '     Class TranscriptionRegulation
    ' 
    '         Properties: TableName
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: DataSource, DefHandles
    ' 
    '     Class TranslationRegulation
    ' 
    '         Properties: TableName
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: DataSource, DefHandles
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.ExpressionSystem
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.Services.DataAcquisition.DataAdapters

    Public Class TranscriptionRegulation : Inherits DataAdapter(Of ExpressionRegulationNetwork)
        Implements IDataAdapter

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "Regulation_Of_Transcription"
            End Get
        End Property

        Sub New(System As EngineSystem.ObjectModels.SubSystem.ExpressionSystem.ExpressionRegulationNetwork)
            Call MyBase.New(System)
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Dim LQuery = (From [Event] In MyBase.System.NetworkComponents Select New DataSource([Event].Handle, [Event].FluxValue)).ToArray
            Return LQuery
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Dim LQuery = (From [Event] In MyBase.System.NetworkComponents Select [Event].SerialsHandle).ToArray
            Return LQuery
        End Function
    End Class

    Public Class TranslationRegulation : Inherits DataAdapter(Of ExpressionRegulationNetwork)
        Implements IDataAdapter

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "Regulation_Of_Translation"
            End Get
        End Property

        Sub New(System As EngineSystem.ObjectModels.SubSystem.ExpressionSystem.ExpressionRegulationNetwork)
            Call MyBase.New(System)
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Dim LQuery = (From [Event] In MyBase.System._InternalEvent_Translations__ Select New DataSource([Event].Handle, [Event].RegulationValue)).ToArray
            Return LQuery
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Dim LQuery = (From [Event] In MyBase.System._InternalEvent_Translations__ Select [Event].SerialsHandle).ToArray
            Return LQuery
        End Function
    End Class
End Namespace
