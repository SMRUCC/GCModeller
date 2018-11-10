#Region "Microsoft.VisualBasic::0250b80e2bdcb9814085d0612b5cceeb, Model\CentralDogma.vb"

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

' Structure CentralDogma
' 
'     Properties: IsRNAGene, RNAName
' 
'     Function: ToString
' 
' Enum RNATypes
' 
'     ribosomalRNA, tRNA
' 
'  
' 
' 
' 
' Structure Protein
' 
'     Constructor: (+1 Overloads) Sub New
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

''' <summary>
''' Transcription and Translation.
''' (一个中心法则对象就是一个基因表达的过程，这个基因表达过程的名称为<see cref="ToString"/>方法的返回值)
''' 
''' ```
''' CDS -> RNA
''' ORF -> mRNA -> polypeptide
''' ```
''' </summary>
Public Structure CentralDogma : Implements INamedValue

    ''' <summary>
    ''' 可以使用这个基因编号属性来作为主键
    ''' </summary>
    Public Property geneID As String Implements IKeyedEntity(Of String).Key

    Dim RNA As NamedValue(Of RNATypes)
    Dim polypeptide As String
    ''' <summary>
    ''' 一般是KO编号
    ''' </summary>
    Dim orthology As String

    ''' <summary>
    ''' 如果这个属性返回false就说明不是编码蛋白序列的基因
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property IsRNAGene As Boolean
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return polypeptide.StringEmpty
        End Get
    End Property

    Public ReadOnly Property RNAName As String
        Get
            Return $"{geneID}::{RNA.Value.Description}"
        End Get
    End Property

    ''' <summary>
    ''' 获取得到这个表达过程的名称
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        If IsRNAGene Then
            Return {geneID, RNAName, polypeptide}.JoinBy(" => ")
        Else
            Return {geneID, RNAName}.JoinBy(" -> ")
        End If
    End Function
End Structure

Public Enum RNATypes As Byte
    mRNA = 0
    tRNA
    ribosomalRNA
End Enum

''' <summary>
''' Protein Modification
''' 
''' ``{polypeptide} + compounds -> protein``
''' </summary>
Public Structure Protein

    Dim polypeptides As String()
    Dim compounds As String()

    ''' <summary>
    ''' 这个蛋白质是由一条多肽链所构成的
    ''' </summary>
    ''' <param name="proteinId"></param>
    Sub New(proteinId As String)
        polypeptides = {proteinId}
    End Sub

End Structure
