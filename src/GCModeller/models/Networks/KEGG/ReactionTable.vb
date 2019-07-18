#Region "Microsoft.VisualBasic::8e3aab41b6f82108919a06ac8b2e1368, models\Networks\KEGG\ReactionTable.vb"

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

' Class ReactionTable
' 
'     Properties: definition, EC, entry, name, products
'                 substrates
' 
'     Function: __creates, Load, ToString
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Terminal.ProgressBar
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.ComponentModel.EquaionModel

''' <summary>
''' A simplify data model of KEGG reaction object.
''' 
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
        Dim proc As New SwayBar

        For Each file As String In (ls - l - r - "*.XML" <= br08201)
            Try
                Yield ReactionTable.creates(file.LoadXml(Of Reaction))
            Catch ex As Exception
                Call file.PrintException
                Call App.LogException(ex)
            Finally
                Call proc.Step()
            End Try
        Next
    End Function

    Private Shared Function creates(xml As Reaction) As ReactionTable
        Dim eq As DefaultTypes.Equation = xml.ReactionModel
        Return New ReactionTable With {
            .definition = xml.Definition,
            .EC = xml.Enzyme,
            .entry = xml.ID,
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
