#Region "Microsoft.VisualBasic::a6900db395d3193877ca00b59a332233, engine\GCModeller\EngineSystem\ObjectModels\SubSystem\Matrix.vb"

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

    ' 
    ' /********************************************************************************/

#End Region

'Imports Microsoft.VisualBasic.Extensions
'Imports Microsoft.VisualBasic.ComponentModel.Settings

'Imports System.Text

'Namespace EngineSystem.ObjectModels.System

'    ''' <summary>
'    ''' [Gene_id],[Expression_factor],[parameters....]
'    ''' </summary>
'    ''' <remarks></remarks>
'    Public Class Matrix : Inherits SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.MathematicsModels.MathematicsModel

'        ''' <summary>
'        ''' 将保存于磁盘文件中的参数加载进入基因表达调控网络对象模型之中，对于文件之中不存在的数据，则使用默认数值
'        ''' </summary>
'        ''' <param name="File"></param>
'        ''' <param name="ExpressionRegulationNetwork"></param>
'        ''' <returns></returns>
'        ''' <remarks></remarks>
'        Friend Shared Function Apply(File As String, ExpressionRegulationNetwork As ObjectModels.System.ExpressionSystem.ExpressionRegulationNetwork) As Boolean
'            Dim Vectors = Load(File) '加载配置数据
'            Dim Query = From ExpressionObject As ObjectModels.Module.CentralDogmaInstance.CentralDogma In ExpressionRegulationNetwork.NetworkComponents Select Apply(ExpressionObject, Vectors) '
'            Return Query.ToArray.Count > 0
'        End Function

'        ''' <summary>
'        ''' 使用表格文件配置代谢组对象模型
'        ''' </summary>
'        ''' <param name="File"></param>
'        ''' <param name="MetabolismNetwork"></param>
'        ''' <returns></returns>
'        ''' <remarks></remarks>
'        Friend Shared Function Apply(File As String, MetabolismNetwork As ObjectModels.System.MetabolismCompartment) As Boolean
'            Dim Vectors = Load(File)
'            Dim Query = From MetabolismFlux As ObjectModels.Module.MetabolismFlux In MetabolismNetwork.DelegateSystem.NetworkComponents Select Apply(MetabolismFlux, Vectors) '
'            Return Query.ToArray.Count > 0
'        End Function

'        Private Shared Function Apply(ExpressionObject As ObjectModels.Module.CentralDogma, Vectors As Vector()) As Integer
'            Dim Handle = FindAt(Vectors, ExpressionObject.UniqueId)
'            'If Handle = -1 Then  '默认数据
'            '    ExpressionObject.Parameter = New [Module].ExpressionObject.ParameterF With {
'            '        .n = 1, .Lamda1_plus = 0.005, .Lamda1_minus = 0.03, .Lamda2 = 0.07, .Sigma2 = 0.005, .Lamda3 = 0.2, .Sigma3 = 0.0004}
'            'Else
'            '    Dim Vec = Vectors(Handle)
'            '    ExpressionObject.Parameter = New [Module].ExpressionObject.ParameterF With {
'            '        .n = Vec.Parameters(0),
'            '        .Lamda1_plus = Vec.Parameters(1), .Lamda1_minus = Vec.Parameters(2),
'            '        .Lamda2 = Vec.Parameters(3), .Sigma2 = Vec.Parameters(4),
'            '        .Lamda3 = Vec.Parameters(5), .Sigma3 = Vec.Parameters(6)}
'            'End If
'            Return Handle
'        End Function

'        Private Shared Function Apply(MetabolismFlux As ObjectModels.Module.MetabolismFlux, Vectors As Vector()) As Integer
'            Dim Handle = FindAt(Vectors, MetabolismFlux.UniqueId)
'            If Handle > -1 Then '默认数据
'                'If MetabolismFlux.Parameter.K1 = 0 AndAlso MetabolismFlux.Parameter.K2 = 0 Then  '确保在这里不会覆写模型中的数据
'                '    If MetabolismFlux.BaseType.Reversible Then
'                '        MetabolismFlux.Parameter = New [Module].MetabolismFlux.ParameterF With {.K1 = 1, .K2 = 0.65}
'                '    Else
'                '        MetabolismFlux.Parameter = New [Module].MetabolismFlux.ParameterF With {.K1 = 1, .K2 = 0.01}
'                '    End If
'                'End If
'                '      Else
'                Dim Vec = Vectors(Handle)
'                '  MetabolismFlux._Parameters = New [Module].MetabolismFlux.ParameterF With {.Keq = Vec.Parameters(0), .KmVector = Vec.Parameters.Skip(1).ToArray}
'            End If
'            Return Handle
'        End Function

'        Protected Friend Shared Function FindAt(Vectors As Vector(), UniqueId As String) As Integer
'            For i As Integer = 0 To Vectors.Count - 1
'                If String.Equals(Vectors(i).UniqueId, UniqueId) Then
'                    Return i
'                End If
'            Next

'            Return -1
'        End Function

'        Public Shared Function Load(File As String) As Vector()
'            If String.IsNullOrEmpty(File) OrElse FileIO.FileSystem.FileExists(File) Then
'                Return New Vector() {}
'            Else
'                Dim Csv = Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Load(File)
'                Dim Query = From row In Csv.AsParallel Let Vector = Vector.CastTo(row) Select Vector Order By Vector.UniqueId '
'                Return Query.ToArray
'            End If
'        End Function

'        Public Structure Vector
'            Public UniqueId As String
'            Public Parameters As Double()

'            Public Overrides Function ToString() As String
'                Dim sBuilder As StringBuilder = New StringBuilder
'                For Each p In Parameters
'                    sBuilder.AppendFormat("{0},", p)
'                Next
'                Return String.Format("{0}____[{2}]", UniqueId, sBuilder.ToString)
'            End Function

'            Public Shared Function CastTo(row As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Row) As Vector
'                Dim PQuery = From c As String In row.Skip(1) Select Val(c) '
'                Return New Vector With {.UniqueId = row.First, .Parameters = PQuery.ToArray}
'            End Function
'        End Structure

'        Public Class MatrixFile : Inherits Microsoft.VisualBasic.ComponentModel.ITextFile
'            Implements Global.System.IDisposable
'            Implements Microsoft.VisualBasic.ComponentModel.Settings.IProfile

'            <ProfileItem> <XmlElement> Public Property Metabolism As String
'            <ProfileItem> <XmlElement> Public Property Expression As String

'            Public Shared Function Load(Path As String) As MatrixFile
'                If Not FileIO.FileSystem.FileExists(Path) Then
'                    Return New MatrixFile
'                Else
'                    Dim MatrixFile As MatrixFile = Path.LoadXml(Of MatrixFile)()
'                    MatrixFile.FilePath = Path
'                    Return MatrixFile
'                End If
'            End Function

'            Public Overrides Sub Save(Optional FilePath As String = "", Optional Encoding As Global.System.Text.Encoding = Nothing)
'                Call Me.GetXml.SaveTo(FilePath, Encoding)
'            End Sub

'            Protected Overrides Sub Dispose(disposing As Boolean)
'                Call Me.Save()
'                Call MyBase.Dispose(disposing)
'            End Sub
'        End Class
'    End Class
'End Namespace
