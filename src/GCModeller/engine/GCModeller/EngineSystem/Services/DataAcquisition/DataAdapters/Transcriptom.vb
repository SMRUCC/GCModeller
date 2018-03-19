#Region "Microsoft.VisualBasic::ad91aea5de91a65754dbd18497009c7e, engine\GCModeller\EngineSystem\Services\DataAcquisition\DataAdapters\Transcriptom.vb"

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

    '     Class Transcriptome
    ' 
    '         Properties: TableName
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: DataSource, DefHandles
    ' 
    '     Class TranscriptionFlux
    ' 
    '         Properties: TableName
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: DataSource, DefHandles
    ' 
    '     Class BasalTranscription
    ' 
    '         Properties: TableName
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: DataSource, DefHandles
    ' 
    '     Class BasalTranslationFlux
    ' 
    '         Properties: TableName
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: DataSource, DefHandles
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.Services.DataAcquisition.DataAdapters

    ''' <summary>
    ''' 获取转录产物的实时浓度的数据转接器
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Transcriptome : Inherits DataAdapter(Of EngineSystem.ObjectModels.SubSystem.ExpressionSystem.ExpressionRegulationNetwork)
        Implements IDataAdapter

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "Transcriptome"
            End Get
        End Property

        Sub New(ExpressionRegulationNetwork As EngineSystem.ObjectModels.SubSystem.ExpressionSystem.ExpressionRegulationNetwork)
            Call MyBase.New(ExpressionRegulationNetwork)
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Dim LQuery = From Transcript As ObjectModels.Entity.Transcript
                         In MyBase.System._InternalTranscriptsPool
                         Select New DataSource(Transcript.Handle, Transcript.Quantity)  '
            Return LQuery.ToArray
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Dim retVal = (From item In MyBase.System._InternalTranscriptsPool Select New HandleF With {.Handle = item.Handle, .Identifier = item.Identifier.Replace("-transcript", "")}).ToArray
            Return retVal
        End Function
    End Class

    Public Class TranscriptionFlux : Inherits Transcriptome
        Implements IDataAdapter

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "transcription_flux"
            End Get
        End Property

        Sub New(System As EngineSystem.ObjectModels.SubSystem.ExpressionSystem.ExpressionRegulationNetwork)
            Call MyBase.New(System)
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Return MyBase.System.DataSource
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Return (From item In MyBase.System.NetworkComponents Select item.SerialsHandle).ToArray
        End Function
    End Class

    Public Class BasalTranscription : Inherits DataAdapter(Of EngineSystem.ObjectModels.SubSystem.ExpressionSystem.BasalExpressionKeeper)
        Implements IDataAdapter

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "basal_transcription_flux"
            End Get
        End Property

        Sub New(System As EngineSystem.ObjectModels.SubSystem.ExpressionSystem.BasalExpressionKeeper)
            Call MyBase.New(System)
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Return (From item In System.BasalExpressionFluxes Let Value = item.DataSource Select Value).ToArray
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Return (From item In System.BasalExpressionFluxes Select item.SerialsHandle).ToArray
        End Function
    End Class

    Public Class BasalTranslationFlux : Inherits SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataAdapter(Of EngineSystem.ObjectModels.SubSystem.ExpressionSystem.BasalExpressionKeeper)
        Implements IDataAdapter

        Sub New(System As EngineSystem.ObjectModels.SubSystem.ExpressionSystem.BasalExpressionKeeper)
            Call MyBase.New(System)
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Dim LQuery = (From Translation In System.BasalTranslationFluxs Let value = Translation.DataSource Select value).ToArray
            Return LQuery
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Dim LQuery = (From Translation In System.BasalTranslationFluxs Select Translation.SerialsHandle).ToArray
            Return LQuery
        End Function

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "BasalTranslationFlux"
            End Get
        End Property
    End Class
End Namespace
