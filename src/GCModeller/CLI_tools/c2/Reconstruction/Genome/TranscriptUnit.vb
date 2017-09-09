#Region "Microsoft.VisualBasic::96a4df1a96a7475cbb866e3492dfda40, ..\CLI_tools\c2\Reconstruction\Genome\TranscriptUnit.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.ConsoleDevice.STDIO

Namespace Reconstruction

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' 主要的工作是将启动子与相应的转录单元中的基因蔟连接起来
    ''' </remarks>
    Public Class TranscriptUnit : Inherits Operation

        Dim GenesTable As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Genes
        Dim Promoters As Operation.ReconstructedList(Of LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Promoter)

        Sub New(Entity As OperationSession, Promoters As Operation.ReconstructedList(Of LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Promoter))
            Call MyBase.New(Entity)
            Me.GenesTable = MyBase.Reconstructed.GetGenes
            Me.Promoters = Promoters
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 将转录单元和启动子对象相互连接起来
        ''' </remarks>
        Public Overrides Function Performance() As Integer
            Dim n As Integer

            Dim rctPromoters = MyBase.Reconstructed.GetPromoters

            For Each TU In MyBase.Reconstructed.GetTransUnits
                Dim FirstGeneId As String = GetFirstGene(TU)

                If String.IsNullOrEmpty(FirstGeneId) Then
                    Continue For
                Else
                    '在重建的启动子列表中查找
                    Dim LQuery = (From p In rctPromoters Where p.ComponentOf.IndexOf(FirstGeneId) > -1 Select p).ToArray '
                    If Not LQuery.IsNullOrEmpty Then
                        Dim Promoter = LQuery.First

                        TU.Components.Add(Promoter.Identifier)
                        Promoter.ComponentOf.Add(TU.Identifier)

                        Call Printf("Linked:: PM_%s <---> %s", Promoter.Identifier, TU.Identifier)

                        n += 1
                    End If
                End If
            Next

            Return n
        End Function

        ''' <summary>
        ''' 获取一个转录单元中的第一个基因
        ''' </summary>
        ''' <param name="TU">假设转录单元中的每一个基因的转录方向都是一致的</param>
        ''' <returns>第一个基因的UniqueId属性</returns>
        ''' <remarks></remarks>
        Private Function GetFirstGene(TU As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.TransUnit) As String
            Dim GeneList As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Gene() =
                (From ComponentId As String In TU.Components
                 Let Gene As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Gene = Me.GenesTable.Get(UniqueId:=ComponentId)
                 Where Not Gene Is Nothing
                 Select Gene).ToArray
            If GeneList.Count = 0 Then '当前的这个转录单元对象中没有基因
                Return ""
            ElseIf GeneList.First.TranscriptionDirection.First = "+"c Then '正义链上面的，返回LEFT最小的那个
                GeneList = (From Gene As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Gene In GeneList Select Gene Order By Val(Gene.LeftEndPosition) Ascending).ToArray
            Else
                GeneList = (From Gene As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Gene In GeneList Select Gene Order By Val(Gene.LeftEndPosition) Descending).ToArray
            End If
            Return GeneList.First.Identifier
        End Function
    End Class
End Namespace
