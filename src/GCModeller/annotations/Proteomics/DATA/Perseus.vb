#Region "Microsoft.VisualBasic::aa5ff3dc7e6fb7591b4f4cc3a0c0ce85, GCModeller\annotations\Proteomics\DATA\Perseus.vb"

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

    '   Total Lines: 71
    '    Code Lines: 49
    ' Comment Lines: 12
    '   Blank Lines: 10
    '     File Size: 2.95 KB


    ' Class Perseus
    ' 
    '     Properties: Data, ExpressionValues, geneID, Intensity, Majority_proteinIDs
    '                 Molweight, MSMSCount, OnlyIdentifiedBySite, Peptides, Potential_contaminant
    '                 ProteinIDs, Qvalue, Razor_unique_peptides, Reverse, Score
    '                 Sequence_coverage, Unique_peptides, Unique_razor_sequence_coverage, Unique_sequence_coverage
    ' 
    '     Function: ToString, TotalMSDivideMS, TotalPeptides
    ' 
    ' /********************************************************************************/

#End Region

#If netcore5 = 0 Then
Imports System.Web.Script.Serialization
#End If
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' perseus output data csv
''' </summary>
Public Class Perseus : Implements INamedValue

    Public Property geneID As String Implements INamedValue.Key

    <Column("Only identified by site")> Public Property OnlyIdentifiedBySite As String
    <Column("Reverse")> Public Property Reverse As String
    <Column("Potential contaminant")> Public Property Potential_contaminant As String
    <Column("Peptides")> Public Property Peptides As Double
    <Column("Razor + unique peptides")> Public Property Razor_unique_peptides As String
    <Column("Unique peptides")> Public Property Unique_peptides As String
    <Column("Sequence coverage [%]")> Public Property Sequence_coverage As String
    <Column("Unique + razor sequence coverage [%]")> Public Property Unique_razor_sequence_coverage As String
    <Column("Unique sequence coverage [%]")> Public Property Unique_sequence_coverage As String
    <Column("Mol. weight [kDa]")> Public Property Molweight As String
    <Column("Q-value")> Public Property Qvalue As String
    <Column("Score")> Public Property Score As String
    <Column("Intensity")> Public Property Intensity As String
    <Column("MS/MS Count")> Public Property MSMSCount As String

    ''' <summary>
    ''' 蛋白质搜库的结果
    ''' </summary>
    ''' <returns></returns>
    <Collection("Protein IDs", ";")> Public Property ProteinIDs As String()
    <Collection("Majority protein IDs", ";")> Public Property Majority_proteinIDs As String()

    Public Property Data As Dictionary(Of String, String)

    Const Tag As String = "LFQ intensity"

    <ScriptIgnore>
    Public ReadOnly Property ExpressionValues As NamedValue(Of Double)()
        Get
            Return Data _
                .Where(Function(x) InStr(x.Key, Tag, CompareMethod.Text) = 1) _
                .Select(Function(x) New NamedValue(Of Double) With {
                    .Name = x.Key,
                    .Value = Val(x.Value)
                }) _
                .ToArray
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Public Shared Function TotalPeptides(data As IEnumerable(Of Perseus)) As Double
        Return data.Sum(Function(prot) prot.Peptides)
    End Function

    ''' <summary>
    ''' Total MS/MS count
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    Public Shared Function TotalMSDivideMS(data As IEnumerable(Of Perseus)) As Double
        Return data.Sum(Function(prot) prot.MSMSCount)
    End Function
End Class
