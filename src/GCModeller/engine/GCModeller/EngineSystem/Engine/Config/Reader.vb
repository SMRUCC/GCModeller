Imports System.Text.RegularExpressions

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
                Return _configs.SuppressErrorMessage.getBoolean
            End Get
        End Property

        Public ReadOnly Property SuppressPeriodicMessage As Boolean
            Get
                Return _configs.SuppressPeriodicMessage.getBoolean
            End Get
        End Property

        Public ReadOnly Property SuppressWarnMessage As Boolean
            Get
                Return _configs.SuppressWarnMessage.getBoolean
            End Get
        End Property

        Public ReadOnly Property Initial_pH As Double
            Get
                Return _configs.Initial_pH
            End Get
        End Property

        Public ReadOnly Property Initial_Temperature As Double
            Get
                Return _configs.Initial_Temperature
            End Get
        End Property

        Public ReadOnly Property DefaultCompartmentId As String
            Get
                Return _configs.DefaultCompartmentId.GetString
            End Get
        End Property

        Public ReadOnly Property CommitLoopsInterval As Integer
            Get
                Return _configs.CommitLoopsInterval
            End Get
        End Property

        Public ReadOnly Property KernelLoops As Long
            Get
                Return _configs.KernelCycles
            End Get
        End Property

        Public ReadOnly Property TrimMetabolism As Boolean
            Get
                Return _configs.TrimMetabolismMetabolites.getBoolean
            End Get
        End Property

        Public ReadOnly Property CultivationMediumPath() As String
            Get
                Return _configs.CultivationMediums.GetString
            End Get
        End Property

        Public ReadOnly Property ExperimentData As String
            Get
                Return _configs.ExperimentData.GetString
            End Get
        End Property

        Public ReadOnly Property Model_MetabolismSystem As String
            Get
                Return _configs.MetabolismModel.GetString
            End Get
        End Property

        Public ReadOnly Property Model_ExpressionNetwork As String
            Get
                Return _configs.ExpressionRegulationNetwork.GetString
            End Get
        End Property

        Public ReadOnly Property ScriptMountPoint As String
            Get
                Return _configs.ScriptMountPoint.GetString
            End Get
        End Property

        Public ReadOnly Property DumpData As String
            Get
                Return GetPath(_configs.DumpData)
            End Get
        End Property

        Public ReadOnly Property ATP_Compensate As Boolean
            Get
                Return False  'Return GetBooleanValue(_configuration.ATP_Compensate)
            End Get
        End Property

        Public ReadOnly Property DataStorageURL() As KeyValuePair(Of Services.DataAcquisition.ManageSystem.DataStorageServiceTypes, String)
            Get
                Dim TypeId As String = Regex.Match(_configs.DataStorageUrl, "(CSV://)|(MySQL://)", RegexOptions.IgnoreCase).Value

                If String.Equals(TypeId, "CSV://", StringComparison.OrdinalIgnoreCase) Then
                    Dim url = _configs.DataStorageUrl.Replace(TypeId, "")
                    Return New KeyValuePair(Of Services.DataAcquisition.ManageSystem.DataStorageServiceTypes, String)(Services.DataAcquisition.ManageSystem.DataStorageServiceTypes.CSV, GetPath(url))
                ElseIf String.Equals(TypeId, "MYSQL://", StringComparison.OrdinalIgnoreCase) Then
                    Dim url = _configs.DataStorageUrl.Replace(TypeId, "")
                    Return New KeyValuePair(Of Services.DataAcquisition.ManageSystem.DataStorageServiceTypes, String)(Services.DataAcquisition.ManageSystem.DataStorageServiceTypes.MySQL, url)
                Else
                    Throw New SyntaxErrorException(String.Format("{0} is not a valid required url format!", _configs.DataStorageUrl))
                End If
            End Get
        End Property

        Public ReadOnly Property LambdaWeight_mRNA As Double
            Get
                Return Val(_configs.mRNA_LambdaWeight)
            End Get
        End Property

        Public ReadOnly Property LambdaWeight_rRNA As Double
            Get
                Return Val(_configs.rRNA_LambdaWeight)
            End Get
        End Property

        Public ReadOnly Property LambdaWeight_tRNA As Double
            Get
                Return Val(_configs.tRNA_LambdaWeight)
            End Get
        End Property
#End Region

        Sub New(data As Configurations)
            _configs = data
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
            Return _configs.ToString
        End Function

#Region "Common shared methods"

        Public Shared ReadOnly Property PathHash As Dictionary(Of String, String) =
            New Dictionary(Of String, String) From {
 _
            {"@desktop", My.Computer.FileSystem.SpecialDirectories.Desktop},
            {"@documents", My.Computer.FileSystem.SpecialDirectories.MyDocuments},
            {"@temp", My.Computer.FileSystem.SpecialDirectories.Temp}
        }

        Public Shared Function GetPath(strValue As String) As String
            If String.IsNullOrEmpty(strValue.GetString.ShadowCopy(strValue)) Then
                Return ""
            Else
                Dim strTemp As String = strValue.Replace("\", "/").Split(CChar("/")).First
                If PathHash.ContainsKey(strTemp.ToLower) Then
                    Return strValue.Replace(strTemp, PathHash(strTemp.ToLower))
                Else
                    Return FileIO.FileSystem.GetDirectoryInfo(strValue).FullName
                End If
            End If
        End Function
#End Region
    End Class
End Namespace