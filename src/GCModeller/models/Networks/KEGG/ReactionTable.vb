#Region "Microsoft.VisualBasic::9ced045d140721cbcc88596a940eb121, ..\GCModeller\models\Networks\KEGG\ReactionTable.vb"

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

Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.ComponentModel.EquaionModel

''' <summary>
''' 对一个代谢反应过程的描述
''' </summary>
Public Class ReactionTable

    ''' <summary>
    ''' 反应编号
    ''' </summary>
    ''' <returns></returns>
    Public Property entry As String
    Public Property name As String
    Public Property definition As String
    ''' <summary>
    ''' 酶编号，可以通过这个编号和相对应的基因或者KO编号关联起来
    ''' </summary>
    ''' <returns></returns>
    Public Property EC As String()
    ''' <summary>
    ''' 底物列表
    ''' </summary>
    ''' <returns></returns>
    Public Property substrates As String()
    ''' <summary>
    ''' 产物列表
    ''' </summary>
    ''' <returns></returns>
    Public Property products As String()

    Public Overrides Function ToString() As String
        Return name
    End Function

    Public Shared Iterator Function Load(br08201$) As IEnumerable(Of ReactionTable)
        For Each file As String In (ls - l - r - "*.XML" <= br08201)
            Try
                Yield ReactionTable.__creates(file.LoadXml(Of Reaction))
            Catch ex As Exception
                Call file.PrintException
                Call App.LogException(ex)
            End Try
        Next
    End Function

    Private Shared Function __creates(xml As Reaction) As ReactionTable
        Dim eq As DefaultTypes.Equation = xml.ReactionModel
        Return New ReactionTable With {
            .definition = xml.Definition,
            .EC = xml.Enzyme,
            .entry = xml.Entry,
            .name = xml.CommonNames.JoinBy("; "),
            .products = eq.Products _
                .Select(Function(x) x.ID) _
                .ToArray,
            .substrates = eq.Reactants _
                .Select(Function(x) x.ID) _
                .ToArray
        }
    End Function
End Class

