Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace ComponentModel.Annotation

    Public Class PhyleticSample : Implements INamedValue

        ''' <summary>样本名称/标识符</summary>
        Public Property SampleId As String Implements INamedValue.Key
        ''' <summary>
        ''' 系统发育谱（phyletic pattern）
        ''' PfamID -> 存在(1)/缺失(0)
        ''' 论文：将每个样本中各Pfam家族的数量转化为存在(1)或缺失(0)的二元矩阵X
        ''' </summary>
        Public Property PhyleticProfile As New Dictionary(Of String, Integer)

        ''' <summary>
        ''' 获取特征向量中存在的Pfam家族数量
        ''' </summary>
        Public ReadOnly Property PfamCount As Integer
            Get
                Return Aggregate pfam As KeyValuePair(Of String, Integer)
                       In PhyleticProfile
                       Where pfam.Value = 1
                       Into Count
            End Get
        End Property

        ''' <summary>
        ''' 获取特征向量中存在的Pfam家族集合
        ''' </summary>
        Public Function GetPresentPfamIds() As List(Of String)
            Dim result As New List(Of String)
            For Each kvp As KeyValuePair(Of String, Integer) In PhyleticProfile
                If kvp.Value = 1 Then
                    result.Add(kvp.Key)
                End If
            Next
            Return result
        End Function

        ''' <summary>
        ''' 检查某Pfam家族是否存在
        ''' </summary>
        Public Function HasPfam(pfamId As String) As Boolean
            Return PhyleticProfile.ContainsKey(pfamId) AndAlso PhyleticProfile(pfamId) = 1
        End Function

    End Class
End Namespace