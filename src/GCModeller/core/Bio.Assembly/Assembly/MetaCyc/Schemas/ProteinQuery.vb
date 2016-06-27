Imports LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles
Imports LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem
Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.Schema

    Public Class ProteinQuery

        Dim MetaCyc As DatabaseLoadder

        Sub New(MetaCyc As DatabaseLoadder)
            Me.MetaCyc = MetaCyc
        End Sub

        ''' <summary>
        ''' 递归的获取某一个指定的蛋白质的所有Component对象
        ''' </summary>
        ''' <param name="ProteinId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetAllComponentList(ProteinId As String) As Slots.Object()
            Dim protein = MetaCyc.GetProteins.Item(ProteinId)

            If protein Is Nothing Then '目标对象则可能是Compound对象
                Dim Compound = MetaCyc.GetCompounds.Item(ProteinId)
                If Not Compound Is Nothing Then
                    Return New Slots.Object() {Compound}
                End If
            Else
                If protein.Components.IsNullOrEmpty Then '单体蛋白质
                    Return New Slots.Object() {protein}
                Else '蛋白质复合物，则必须要进行递归查找了
                    Dim objList As List(Of Slots.Object) = New List(Of Slots.Object)
                    For Each ComponentId As String In protein.Components
                        Call objList.AddRange(GetAllComponentList(ComponentId))
                    Next

                    Return objList.ToArray
                End If
            End If

            Return New Slots.Object() {}
        End Function
    End Class
End Namespace