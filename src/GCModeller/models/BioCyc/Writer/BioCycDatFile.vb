Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace Writer

    ''' <summary>
    ''' 一个 BioCyc 平面数据文件的落盘入口：负责推导文件名、写出文件头以及全部对象记录。
    ''' 
    ''' 目标文件名由数据模型类型上面的 <see cref="XrefAttribute"/> 决定(与读取端使用同一份元数据)，
    ''' 因此写出与读取总是成对出现。
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    Public Class BioCycDatFile(Of T As Model)

        ''' <summary>
        ''' 文件头元数据
        ''' </summary>
        ''' <returns></returns>
        Public Property meta As FileMeta

        ''' <summary>
        ''' 当前文件之中待写出的对象集合
        ''' </summary>
        ''' <returns></returns>
        Public Property objects As IEnumerable(Of T)

        Public Property orthologs As Boolean = False

        ''' <summary>
        ''' 目标文件名，默认取 <see cref="XrefAttribute"/> 的名称
        ''' </summary>
        ''' <returns></returns>
        Public Property fileName As String

        Sub New()
            fileName = GetFileName()
        End Sub

        Sub New(meta As FileMeta, objects As IEnumerable(Of T))
            Me.New()

            Me.meta = meta
            Me.objects = objects
        End Sub

        ''' <summary>
        ''' 读取数据模型类型所声明的 BioCyc 文件名
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function GetFileName() As String
            Dim attr As XrefAttribute = GetType(T) _
                .GetCustomAttributes(inherit:=True) _
                .OfType(Of XrefAttribute)() _
                .FirstOrDefault

            If attr Is Nothing Then
                Throw New MissingFieldException($"no biocyc file name (XrefAttribute) was declared on the type '{GetType(T).FullName}'!")
            End If

            Return attr.Name
        End Function

        ''' <summary>
        ''' 当前文件之中可能出现的槽位名称列表
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function GetSlotNames() As String()
            Return ModelSlotSerializer.GetSlotNames(GetType(T))
        End Function

        ''' <summary>
        ''' 将当前的全部对象写出到目标目录，返回实际写出的文件路径
        ''' </summary>
        ''' <param name="dataDir"></param>
        ''' <returns></returns>
        Public Function Save(dataDir As String) As String
            Dim path As String = IO.Path.Combine(dataDir, fileName)
            Dim serializer As ModelSlotSerializer = ModelSlotSerializer.GetSerializer(GetType(T))

            Call Directory.CreateDirectory(dataDir)

            If meta Is Nothing Then
                meta = New FileMeta
            End If

            If String.IsNullOrEmpty(meta.fileName) Then
                meta.fileName = path.GetFullPath
            End If

            Using file As New AttrFileWriter(path)
                Call file.WriteHeader(meta, GetSlotNames(), orthologs)

                For Each obj As T In objects.SafeQuery.OrderBy(Function(o) o.uniqueId, StringComparer.Ordinal)
                    If String.IsNullOrEmpty(obj.uniqueId) Then
                        Continue For
                    End If

                    Call file.Write(serializer.Serialize(obj))
                Next
            End Using

            Return path
        End Function

        ''' <summary>
        ''' 写出指定目录下面的文件并返回对象数量
        ''' </summary>
        ''' <param name="dataDir"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function SaveCount(dataDir As String) As Integer
            Call Save(dataDir)

            Return objects.SafeQuery.Count
        End Function

        Public Overrides Function ToString() As String
            Return $"{fileName} ({objects.SafeQuery.Count} objects)"
        End Function
    End Class

    ''' <summary>
    ''' 直接使用 <see cref="AttrRecord"/> 作为数据载体的 BioCyc 平面文件。
    ''' 
    ''' 对于那些无法(或者不应当)通过反射模型来表达的文件——例如需要精确控制 ``LEFT``/``RIGHT``
    ''' 槽位上面的 ``^COEFFICIENT``/``^COMPARTMENT`` 注解的反应文件——转换器可以使用本类型直接写盘。
    ''' </summary>
    Public Class AttrRecordFile

        Public Property meta As FileMeta
        Public Property records As IEnumerable(Of AttrRecord)
        Public Property fileName As String
        Public Property attributes As String()
        Public Property orthologs As Boolean = False

        Sub New()
        End Sub

        Sub New(fileName As String, meta As FileMeta, records As IEnumerable(Of AttrRecord))
            Me.fileName = fileName
            Me.meta = meta
            Me.records = records
        End Sub

        Public Function Save(dataDir As String) As String
            Dim path As String = IO.Path.Combine(dataDir, fileName)
            Dim all As AttrRecord() = records.SafeQuery _
                .Where(Function(r) Not String.IsNullOrEmpty(r.uniqueId)) _
                .OrderBy(Function(r) r.uniqueId, StringComparer.Ordinal) _
                .ToArray

            Call Directory.CreateDirectory(dataDir)

            If meta Is Nothing Then
                meta = New FileMeta
            End If

            If String.IsNullOrEmpty(meta.fileName) Then
                meta.fileName = path.GetFullPath
            End If

            Dim names As String() = attributes

            If names.IsNullOrEmpty Then
                names = all _
                    .SelectMany(Function(r) r.slots) _
                    .Select(Function(s) s.Name) _
                    .Distinct() _
                    .ToArray
            End If

            names = New String() {"UNIQUE-ID"} _
                .Concat(names) _
                .Distinct() _
                .ToArray

            Using file As New AttrFileWriter(path)
                Call file.WriteHeader(meta, names, orthologs)

                For Each record As AttrRecord In all
                    Call file.Write(record)
                Next
            End Using

            Return path
        End Function
    End Class

End Namespace
