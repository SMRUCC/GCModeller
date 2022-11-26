#Region "Microsoft.VisualBasic::9a40379bec813715a1741b72776e8427, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\Schemas\ProteinQuery.vb"

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


    ' Code Statistics:

    '   Total Lines: 45
    '    Code Lines: 31
    ' Comment Lines: 6
    '   Blank Lines: 8
    '     File Size: 1.71 KB


    '     Class ProteinQuery
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetAllComponentList
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem
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
