Imports LANS.SystemsBiology.GCModeller.Framework.Kernel_Driver.DataStorage.FileModel
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream

Namespace EngineSystem.Services.DataAcquisition.DataSerializer

    Public Class Csv : Inherits DataSerializer
        Dim HandleList As HandleF()
        Dim ChunkBuffer As SerialsData()

        Public Class SerialsData : Inherits DataSerials(Of Double)

            Public Property DataPackage As List(Of Double)

            Public Shared Function GetPackage(DataChunk As Generic.IEnumerable(Of SerialsData), Handle As String) As SerialsData
                Dim LQuery = (From item In DataChunk Where item.Handle = Handle Select item).ToArray
                If LQuery.IsNullOrEmpty Then
                    Return Nothing
                Else
                    Return LQuery.First
                End If
            End Function

            Public Function CreateCsvData() As RowObject
                Dim Data As RowObject = New RowObject() +
                    UniqueId + (From col As Double
                                In DataPackage
                                Select CStr(col)).ToArray
                Return Data
            End Function

            Public Overrides Function ToString() As String
                Return String.Format("{0}. {1}", Handle, UniqueId)
            End Function
        End Class

        Public ReadOnly Property GetHandles As HandleF()
            Get
                Return HandleList
            End Get
        End Property

        Sub New(File As String)
            Call MyBase.New(Url:=File)
        End Sub

        Public Overrides Sub Initialize(Dir As String)
            If FileIO.FileSystem.FileExists(MyBase._Url) Then
                Call FileIO.FileSystem.DeleteFile(file:=_Url,
                                                  showUI:=FileIO.UIOption.OnlyErrorDialogs,
                                                  onUserCancel:=FileIO.UICancelOption.DoNothing,
                                                  recycle:=FileIO.RecycleOption.SendToRecycleBin)
            End If
        End Sub

        Public Overrides Sub CreateHandle(List() As HandleF, Table As String)
            HandleList = (From item In List Select item Order By item.Handle Ascending).ToArray

            Dim UpdateLQuery = (From i As Integer In HandleList.Sequence.AsParallel
                                Let Target As SerialsData = ChunkBuffer(i)
                                Let Update = Function() As Integer
                                                 Target.UniqueId = HandleList(i).Identifier
                                                 Return 0
                                             End Function Select Update()).ToArray
        End Sub

        Public Overrides Sub Close(arg As String)
            'Call EngineSystem.Services.DataExport.Export(Service:=Me).Save(Path:=_Url, LazySaved:=True)
            Dim LQuery = (From item In Me.ChunkBuffer Select item.CreateCsvData).ToArray
            Call New File(LQuery).Save(Path:=_Url, LazySaved:=True)
        End Sub

        ''' <summary>
        ''' 将基类中的<see cref="Csv._DataFlows"></see>转换为<see cref="Csv.ChunkBuffer"></see>中的数据
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub Append(DataPackage As IEnumerable(Of DataFlowF))
            If ChunkBuffer.IsNullOrEmpty Then '初始化句柄集合
                Dim LQuery = (From item In DataPackage Let ItemValue = New SerialsData With {.Handle = item.Handle, .DataPackage = New List(Of Double)} Select ItemValue Order By ItemValue.Handle Ascending).ToArray
                Me.ChunkBuffer = LQuery
            End If

            Dim DataPackageChunkBuffer = (From item In DataPackage Select item Order By item.Handle Ascending).ToArray

            Dim UpdateLQuery = (From i As Integer In DataPackageChunkBuffer.Sequence.AsParallel
                                Let Target As SerialsData = ChunkBuffer(i)
                                Let Update = Function() As Integer
                                                 Call Target.DataPackage.Add(DataPackageChunkBuffer(i).Value)
                                                 Return 0
                                             End Function Select Update()).ToArray
        End Sub

        Public Overrides Function CommitData(Optional arg As String = "") As Integer
            Return 0
        End Function
    End Class
End Namespace