#Region "Microsoft.VisualBasic::ea998afaf6bd88a0e584baba06e6af04, engine\BootstrapLoader\Definition\Definition.vb"

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

'   Total Lines: 269
'    Code Lines: 191 (71.00%)
' Comment Lines: 50 (18.59%)
'    - Xml Docs: 92.00%
' 
'   Blank Lines: 28 (10.41%)
'     File Size: 9.91 KB


'     Enum GeneralCompound
' 
'         Compound, DNA, Protein, RNA
' 
'  
' 
' 
' 
'     Class Transmembrane
' 
'         Properties: passive
' 
'     Class Definition
' 
'         Properties: ADP, AminoAcid, Ammonia, ATP, CultureMedium
'                     GDP, GenericCompounds, GTP, NucleicAcid, Oxygen
'                     PI, PPI, status, transmembrane, Water
' 
'         Function: GenericEnumerator, GetInfinitySource, initMassValue, KEGG, MetaCyc
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Settings.Inf
Imports Microsoft.VisualBasic.Linq

Namespace Definitions

    ''' <summary>
    ''' The definition for the mass environment for run the simulation, use for context mapping across database
    ''' </summary>
    ''' <remarks>
    ''' 因为物质编号可能会来自于不同的数据库，所以会需要使用这个对象将一些关键的物质映射为计算引擎所能够被识别的对象
    ''' </remarks>
    ''' 
    <ClassName("define")>
    Public Class Definition : Implements Enumeration(Of String)

#Region "Object maps"

        ' 当初主要是使用这种固定的映射来处理一些特定的模板事件

#Region "转录和翻译"
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property ATP As String
        Public Property ADP As String
        Public Property PPI As String
        Public Property PI As String

        Public Property GTP As String
        Public Property GDP As String

#End Region

#Region "模板或者说无限来源的物质"
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property Water As String
        Public Property Oxygen As String
        Public Property Ammonia As String
#End Region

        ''' <summary>
        ''' nucleotide metabolite id mapping for construct of the RNA molecule in the transcription process.
        ''' </summary>
        ''' <returns></returns>
        Public Property NucleicAcid As NucleicAcid
        ''' <summary>
        ''' amino acid metabolite id mapping for construct of the polypeptide in translation process
        ''' </summary>
        ''' <returns></returns>
        Public Property AminoAcid As AminoAcid

        ''' <summary>
        ''' define the id mapping to some general compounds
        ''' </summary>
        ''' <returns></returns>
        Public Property GenericCompounds As Dictionary(Of String, GeneralCompound)
#End Region

        Public Property transmembrane As Transmembrane

        ''' <summary>
        ''' the compartment id of the Culture medium
        ''' </summary>
        ''' <returns></returns>
        Public Property CultureMedium As String = "Extracellular"

        ''' <summary>
        ''' define of the cell initial status.
        ''' </summary>
        ''' <returns>
        ''' [cell_id => [metabolite => content data]]
        ''' </returns>
        Public Property context As CellContext()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function LoadIni(file As String) As Definition
            Return ClassMapper.LoadIni(Of Definition)(file)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetInfinitySource() As Index(Of String)
            Return {Water, Oxygen}.Where(Function(ref) Not ref Is Nothing).Indexing
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of String) Implements Enumeration(Of String).GenericEnumerator
            If Not ADP.StringEmpty(, True) Then Yield ADP
            If Not ATP.StringEmpty(, True) Then Yield ATP
            If Not Water.StringEmpty(, True) Then Yield Water
            If Not Oxygen.StringEmpty(, True) Then Yield Oxygen

            If Not GenericCompounds Is Nothing Then
                For Each key As String In GenericCompounds.Keys
                    Yield key
                Next
            End If

            If Not context Is Nothing Then
                For Each key As String In context.Keys
                    Yield key
                Next
            End If

            If Not NucleicAcid Is Nothing Then
                If Not NucleicAcid.A.StringEmpty(, True) Then Yield NucleicAcid.A
                If Not NucleicAcid.C.StringEmpty(, True) Then Yield NucleicAcid.C
                If Not NucleicAcid.G.StringEmpty(, True) Then Yield NucleicAcid.G
                If Not NucleicAcid.U.StringEmpty(, True) Then Yield NucleicAcid.U
            End If

            If Not AminoAcid Is Nothing Then
                For Each aa As String In AminoAcid.AsEnumerable
                    If Not aa.StringEmpty(, True) Then
                        Yield aa
                    End If
                Next
            End If
        End Function
    End Class
End Namespace
