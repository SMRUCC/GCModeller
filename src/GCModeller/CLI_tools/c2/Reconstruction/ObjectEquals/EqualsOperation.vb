#Region "Microsoft.VisualBasic::5d2e116a91e358ef9f44b41f6770490c, CLI_tools\c2\Reconstruction\ObjectEquals\EqualsOperation.vb"

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

    '     Class EqualsOperation
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Equals, Initialize, SelectFromReconstructed, SelectFromSubject
    '         Class ObjectSchema
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Function: ToString
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Extensions
Imports System.Reflection
Imports LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Reflection
Imports System.Text

Namespace Reconstruction.ObjectEquals

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class EqualsOperation

        ''' <summary>
        ''' 在Subject数据库和Reconstructed数据库之间相等价的对象的UniqueId列表
        ''' </summary>
        ''' <remarks>{Subject, Reconstructed()}</remarks>
        Protected Friend EqualsList As KeyValuePair(Of String, String())()
        Protected Friend Session As c2.Reconstruction.Operation.OperationSession

        Protected Friend Subject As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder
        Protected Friend Reconstructed As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder
        Protected Friend HomologousProteins As c2.Reconstruction.Operation.OperationSession.HomologousProteinsF

        Sub New(Session As c2.Reconstruction.Operation.OperationSession)
            Me.Session = Session
            If Session Is Nothing Then Return
            Me.Subject = Session.Subject
            Me.Reconstructed = Session.ReconstructedMetaCyc
            Me.HomologousProteins = Session.HomologousProteins
        End Sub

        ''' <summary>
        ''' Write down the EqualsList initialization code here.(请在这里编写EqualsList的初始化代码)
        ''' </summary>
        ''' <remarks></remarks>
        Public Overridable Function Initialize() As Integer
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' 没有查找到相应的等价对象则返回空字符串
        ''' </summary>
        ''' <param name="sbjUniqueId">Subject数据库中的对象的UniqueId</param>
        ''' <returns>返回Reconstructed数据库中的相应的对象的UniqueId</returns>
        ''' <remarks></remarks>
        Public Function SelectFromSubject(sbjUniqueId As String) As String()
            Dim LQuery As Generic.IEnumerable(Of String()) = From Item As KeyValuePair(Of String, String()) In Me.EqualsList.AsParallel Where String.Equals(sbjUniqueId, Item.Key) Select Item.Value '
            Dim Result = LQuery.ToArray
            If Result.IsNullOrEmpty Then
                Return New String() {}
            Else
                Return Result.First
            End If
        End Function

        ''' <summary>
        ''' 没有查找到相应的等价对象则返回空字符串
        ''' </summary>
        ''' <param name="rctUniqueId">Reconstructed数据库中的对象的UniqueId</param>
        ''' <returns>返回Subject数据库中的相应的对象的UniqueId</returns>
        ''' <remarks></remarks>
        Public Function SelectFromReconstructed(rctUniqueId As String) As String
            Dim LQuery = From Item As KeyValuePair(Of String, String()) In Me.EqualsList.AsParallel Where Array.IndexOf(Item.Value, rctUniqueId) > -1 Select Item.Key  '
            Dim Result = LQuery.ToArray
            If Result.IsNullOrEmpty Then
                Return ""
            Else
                Return Result.First
            End If
        End Function

        ''' <summary>
        ''' 判断两个UniqueId所代表的MetaCyc对象是否相等
        ''' </summary>
        ''' <param name="rctObj">Reconstructed的MetaCyc数据库中的某一个对象的UniqueId属性值</param>
        ''' <param name="sbjObj">Subject的MetaCyc数据库中的某一个对象的UniqueId属性值</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Overloads Function Equals(rctObj As String, sbjObj As String) As Boolean
            Dim Id As String = SelectFromReconstructed(rctUniqueId:=rctObj)
            If String.IsNullOrEmpty(Id) Then
                Return False
            Else
                Return String.Equals(Id, sbjObj)
            End If
        End Function

        ''' <summary>
        ''' Object Shcema of the MetaCyc database object instance.
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend Class ObjectSchema(Of T As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Object)
            Public ItemProperties As PropertyInfo() = New PropertyInfo() {}
            Public FieldAttributes As MetaCycField() = New MetaCycField() {}
            Public ReadOnly FieldList As String()

            Sub New(Optional FieldList As Generic.IEnumerable(Of String) = Nothing)
                Dim ItemProperties = New List(Of PropertyInfo), FieldAttributes = New List(Of MetaCycField)

                Call LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Reflection.FileStream.GetMetaCycField(Of T)(Me.ItemProperties, Me.FieldAttributes)

                If Not FieldList.IsNullOrEmpty Then
                    For i As Integer = 0 To Me.FieldAttributes.Count - 1
                        If Array.IndexOf(FieldList, Me.FieldAttributes(i).Name) > -1 Then
                            Call ItemProperties.Add(Me.ItemProperties(i))
                            Call FieldAttributes.Add(Me.FieldAttributes(i))
                        End If
                    Next

                    Me.ItemProperties = ItemProperties.ToArray
                    Me.FieldAttributes = FieldAttributes.ToArray
                End If

                Me.FieldList = FieldList.ToArray
            End Sub

            Public Overrides Function ToString() As String
                Dim sBuilder As StringBuilder = New StringBuilder(1024)
                Call sBuilder.AppendFormat("Schema Data for MetaCyc Object Type: {0}", GetType(T).FullName)
                For Each Field As String In FieldList
                    Call sBuilder.AppendLine(Field)
                Next

                Return sBuilder.ToString
            End Function
        End Class
    End Class
End Namespace
