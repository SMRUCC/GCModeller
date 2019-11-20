#Region "Microsoft.VisualBasic::abda13c2cf51c01341ad8b4422234953, engine\GCModeller\EngineSystem\Engine\Config\Reader.vb"

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

    '     Class ConfigReader
    ' 
    '         Properties: ATP_Compensate, CommitLoopsInterval, Configs, CultivationMediumPath, DataStorageURL
    '                     DefaultCompartmentId, DumpData, ExperimentData, Initial_pH, Initial_Temperature
    '                     KernelLoops, LambdaWeight_mRNA, LambdaWeight_rRNA, LambdaWeight_tRNA, Model_ExpressionNetwork
    '                     Model_MetabolismSystem, PathHash, RootDIR, ScriptMountPoint, SuppressErrorMessage
    '                     SuppressPeriodicMessage, SuppressWarnMessage, TrimMetabolism
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetPath, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Language

Namespace EngineSystem.Engine.Configuration

    ''' <summary>
    ''' 配置数据的对象模型，假若需要操作文件对象，请使用<see cref="Configuration.Configurations"></see>对象
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ConfigReader

        Public ReadOnly Property Configs As Configurations

#Region "Public ReadOnly Properties"

        Public ReadOnly Property SuppressErrorMessage As Boolean
            Get
                Return _Configs.SuppressErrorMessage.ParseBoolean
            End Get
        End Property

        Public ReadOnly Property SuppressPeriodicMessage As Boolean
            Get
                Return _Configs.SuppressPeriodicMessage.ParseBoolean
            End Get
        End Property

        Public ReadOnly Property SuppressWarnMessage As Boolean
            Get
                Return _Configs.SuppressWarnMessage.ParseBoolean
            End Get
        End Property

        Public ReadOnly Property Initial_pH As Double
            Get
                Return _Configs.Initial_pH
            End Get
        End Property

        Public ReadOnly Property Initial_Temperature As Double
            Get
                Return _Configs.Initial_Temperature
            End Get
        End Property

        Public ReadOnly Property DefaultCompartmentId As String
            Get
                Return _Configs.DefaultCompartmentId.GetString
            End Get
        End Property

        Public ReadOnly Property CommitLoopsInterval As Integer
            Get
                Return _Configs.CommitLoopsInterval
            End Get
        End Property

        Public ReadOnly Property KernelLoops As Long
            Get
                Return _Configs.KernelCycles
            End Get
        End Property

        Public ReadOnly Property TrimMetabolism As Boolean
            Get
                Return _Configs.TrimMetabolismMetabolites.ParseBoolean
            End Get
        End Property

        Public ReadOnly Property CultivationMediumPath() As String
            Get
                Return _Configs.CultivationMediums.GetString
            End Get
        End Property

        Public ReadOnly Property ExperimentData As String
            Get
                Return _Configs.ExperimentData.GetString
            End Get
        End Property

        Public ReadOnly Property Model_MetabolismSystem As String
            Get
                Return _Configs.MetabolismModel.GetString
            End Get
        End Property

        Public ReadOnly Property Model_ExpressionNetwork As String
            Get
                Return _Configs.ExpressionRegulationNetwork.GetString
            End Get
        End Property

        Public ReadOnly Property ScriptMountPoint As String
            Get
                Return _Configs.ScriptMountPoint.GetString
            End Get
        End Property

        Public ReadOnly Property DumpData As String
            Get
                Return GetPath(_Configs.DumpData)
            End Get
        End Property

        Public ReadOnly Property ATP_Compensate As Boolean
            Get
                Return False  'Return GetBooleanValue(_configuration.ATP_Compensate)
            End Get
        End Property

        Public ReadOnly Property DataStorageURL() As KeyValuePair(Of Services.DataAcquisition.ManageSystem.DataStorageServiceTypes, String)
            Get
                Dim TypeId As String = Regex.Match(_Configs.DataStorageUrl, "(CSV://)|(MySQL://)", RegexOptions.IgnoreCase).Value

                If String.Equals(TypeId, "CSV://", StringComparison.OrdinalIgnoreCase) Then
                    Dim url = _Configs.DataStorageUrl.Replace(TypeId, "")
                    Return New KeyValuePair(Of Services.DataAcquisition.ManageSystem.DataStorageServiceTypes, String)(Services.DataAcquisition.ManageSystem.DataStorageServiceTypes.CSV, GetPath(url))
                ElseIf String.Equals(TypeId, "MYSQL://", StringComparison.OrdinalIgnoreCase) Then
                    Dim url = _Configs.DataStorageUrl.Replace(TypeId, "")
                    Return New KeyValuePair(Of Services.DataAcquisition.ManageSystem.DataStorageServiceTypes, String)(Services.DataAcquisition.ManageSystem.DataStorageServiceTypes.MySQL, url)
                Else
                    Throw New SyntaxErrorException(String.Format("{0} is not a valid required url format!", _Configs.DataStorageUrl))
                End If
            End Get
        End Property

        Public ReadOnly Property LambdaWeight_mRNA As Double
            Get
                Return Val(_Configs.mRNA_LambdaWeight)
            End Get
        End Property

        Public ReadOnly Property LambdaWeight_rRNA As Double
            Get
                Return Val(_Configs.rRNA_LambdaWeight)
            End Get
        End Property

        Public ReadOnly Property LambdaWeight_tRNA As Double
            Get
                Return Val(_Configs.tRNA_LambdaWeight)
            End Get
        End Property
#End Region

        Sub New(data As Configurations)
            _Configs = data
        End Sub

        ''' <summary>
        ''' The directory of this configuration data file.(配置数据文件所在的路径文件夹)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property RootDIR As String
            Get
                Return FileIO.FileSystem.GetParentPath(_Configs.FilePath)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return _Configs.ToString
        End Function

#Region "Common shared methods"

        Public Shared ReadOnly Property PathHash As Dictionary(Of String, String) =
            New Dictionary(Of String, String) From {
 _
            {"@desktop", My.Computer.FileSystem.SpecialDirectories.Desktop},
            {"@documents", My.Computer.FileSystem.SpecialDirectories.MyDocuments},
            {"@temp", My.Computer.FileSystem.SpecialDirectories.Temp}
        }

        Public Shared Function GetPath(strValue As Value(Of String)) As String
            If String.IsNullOrEmpty(strValue = (+strValue).GetString) Then
                Return ""
            Else
                Dim strTemp As String = strValue.value.Replace("\", "/").Split(CChar("/")).First
                If PathHash.ContainsKey(strTemp.ToLower) Then
                    Return strValue.value.Replace(strTemp, PathHash(strTemp.ToLower))
                Else
                    Return FileIO.FileSystem.GetDirectoryInfo(strValue).FullName
                End If
            End If
        End Function
#End Region
    End Class
End Namespace
