' ============================================================================
' ScaledSvmCodec.vb
'
' Ensemble\Storage 内置的 SvmCodec 只识别 SvmClassifier / SvmRegressor，
' 它们不做特征缩放；metaTraits 的基学习器是 ScaledSvmClassifier /
' ScaledSvmRegressor，内部保留 RangeTransform。这里注册一个专用编解码器，
' 把完整的 SVMModel（含缩放与类别编码）沿用 SvmModelJSON 落盘，
' 从而让 EnsembleModelStore 能以零重训方式保存/恢复这类基模型。
' ============================================================================

Imports System
Imports System.IO
Imports Microsoft.VisualBasic.MachineLearning.Bootstrapping.Ensemble
Imports Microsoft.VisualBasic.MachineLearning.Bootstrapping.Ensemble.Persistence
Imports Microsoft.VisualBasic.MachineLearning.SVM
Imports Microsoft.VisualBasic.MachineLearning.SVM.StorageProcedure
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Traitar.Modules

    ''' <summary>
    ''' 带 <see cref="RangeTransform"/> 的 SVM 分类/回归编解码器。
    ''' </summary>
    Public NotInheritable Class ScaledSvmCodec
        Implements IModelCodec

        ''' <summary>算法标识（写入清单，加载期据此分派）</summary>
        Public Const AlgorithmName As String = "scaled-svm"

        Public ReadOnly Property Algorithm As String Implements IModelCodec.Algorithm
            Get
                Return AlgorithmName
            End Get
        End Property

        Public Function CanHandle(model As Object) As Boolean Implements IModelCodec.CanHandle
            Return TypeOf model Is ScaledSvmClassifier OrElse TypeOf model Is ScaledSvmRegressor
        End Function

        Public Function SaveModel(model As Object, bundle As ModelBundle, folder As String) As ModelManifest Implements IModelCodec.SaveModel
            Dim svm As SVMModel
            Dim kind As String
            Dim inputOffset As Integer
            Dim indexOffset As Integer = 1
            Dim classes As Double() = Nothing
            Dim classifier As ScaledSvmClassifier = TryCast(model, ScaledSvmClassifier)

            If classifier IsNot Nothing Then
                svm = classifier.Fitted
                kind = "classifier"
                inputOffset = classifier.InputOffset
                indexOffset = classifier.IndexOffset
                classes = CType(classifier.Classes.Clone(), Double())
            Else
                Dim regressor As ScaledSvmRegressor = DirectCast(model, ScaledSvmRegressor)

                svm = regressor.Fitted
                kind = "regressor"
                inputOffset = regressor.InputOffset
            End If

            If svm Is Nothing OrElse svm.model Is Nothing Then
                Throw New InvalidOperationException("the scaled svm adapter holds no fitted model!")
            End If

            Dim manifest As ModelManifest = ModelManifest.Create(kind, "base", AlgorithmName)

            manifest.featureWidth = GetFeatureWidth(svm)
            manifest.SetInteger("inputOffset", inputOffset)
            manifest.SetInteger("indexOffset", indexOffset)

            If classes IsNot Nothing Then
                manifest.classes = classes
            End If

            ' 完整 SVM 模型（含 RangeTransform）以 JSON 附表形式写入
            Dim relative As String = ModelBundle.CombineRelative(folder, "svm-model.json")
            Dim fullPath As String = bundle.ResolvePath(relative)
            Dim parent As String = System.IO.Path.GetDirectoryName(fullPath)

            If Not String.IsNullOrEmpty(parent) AndAlso Not Directory.Exists(parent) Then
                Call Directory.CreateDirectory(parent)
            End If

            Call File.WriteAllText(fullPath, SvmModelJSON.CreateJSONModel(svm).GetJson)
            manifest.data("svmModel") = relative

            Return manifest
        End Function

        Public Function LoadModel(manifest As ModelManifest, bundle As ModelBundle) As Object Implements IModelCodec.LoadModel
            Dim relative As String

            If manifest.data Is Nothing OrElse Not manifest.data.TryGetValue("svmModel", relative) Then
                Throw New InvalidDataException("the scaled svm manifest has no 'svmModel' data file!")
            End If

            Dim svm As SVMModel = File _
                .ReadAllText(bundle.ResolvePath(relative)) _
                .LoadJSON(Of SvmModelJSON)() _
                .CreateSVMModel()

            If String.Equals(manifest.kind, "classifier", StringComparison.OrdinalIgnoreCase) Then
                Return New ScaledSvmClassifier(svm, manifest.classes,
                                               GetInteger(manifest, "indexOffset", 1),
                                               GetInteger(manifest, "inputOffset", 1))
            Else
                Return New ScaledSvmRegressor(svm, GetInteger(manifest, "inputOffset", 1))
            End If
        End Function

        Private Shared Function GetFeatureWidth(svm As SVMModel) As Integer
            ' RangeTransform 的字段是 Friend，借其 JSON 数据模型读取维度
            If TypeOf svm.transform Is RangeTransform Then
                Return New RangeTransformModel(svm.transform).length
            End If

            Return 0
        End Function

        Private Shared Function GetInteger(manifest As ModelManifest, name As String, fallback As Integer) As Integer
            Dim value As Integer

            If manifest.integers IsNot Nothing AndAlso manifest.integers.TryGetValue(name, value) Then
                Return value
            End If

            Return fallback
        End Function

    End Class

    ''' <summary>把 <see cref="ScaledSvmCodec"/> 注册到全局编解码器注册表</summary>
    Public Module ScaledSvmCodecRegistration

        Private _registered As Boolean
        Private ReadOnly _sync As New Object()

        Public Sub EnsureRegistered()
            If _registered Then
                Return
            End If

            SyncLock _sync
                If Not _registered Then
                    Call EnsembleModelStore.Register(New ScaledSvmCodec())
                    _registered = True
                End If
            End SyncLock
        End Sub

    End Module
End Namespace
