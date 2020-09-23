#Region "Microsoft.VisualBasic::f895feb447c631744252b88f126a5868, engine\IO\GCMarkupLanguage\GCML_Documents\XmlElements\GCML_Documents.SignalTransductions\SignalTransductionNetwork.vb"

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

    '     Class SignalTransductionNetwork
    ' 
    '         Properties: CheBMethylesterase, CheBPhosphate, ChemotaxisSensing, CheRMethyltransferase, CrossTalk
    '                     HkAutoPhosphorus, OCSSensing, TFActive
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text

Namespace GCML_Documents.XmlElements.SignalTransductions

    Public Class SignalTransductionNetwork
        ''' <summary>
        ''' [MCP][CH3] -> MCP + -CH3  Enzyme:[CheB][PI]
        ''' 
        ''' Protein L-glutamate O(5)-methyl ester + H(2)O = protein L-glutamate + methanol
        ''' C00132
        '''
        ''' METOH
        '''
        ''' </summary>
        ''' <remarks></remarks>
        Public Property CheBMethylesterase As Metabolism.Reaction()
        ''' <summary>
        ''' MCP + -CH3 -> [MCP][CH3]   Enzyme:CheR
        ''' S-adenosyl-L-methionine
        ''' S-ADENOSYLMETHIONINE
        ''' C00019
        ''' 
        ''' S-ADENOSYLMETHIONINE                              ADENOSYL-HOMO-CYS
        ''' S-adenosyl-L-methionine + protein L-glutamate = S-adenosyl-L-homocysteine + protein L-glutamate methyl ester.
        ''' </summary>
        ''' <remarks></remarks>
        Public Property CheRMethyltransferase As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction()
        ''' <summary>
        ''' CheB + [ChA][PI] -> [CheB][PI] + CheA
        ''' </summary>
        ''' <remarks></remarks>
        Public Property CheBPhosphate As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction()
        ''' <summary>
        ''' [MCP][CH3] + Inducer &lt;--&gt; [MCP][CH3][Inducer]
        ''' </summary>
        ''' <remarks></remarks>
        Public Property ChemotaxisSensing As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.TransportationReaction()
        ''' <summary>
        ''' CheAHK + ATP -> [CheAHK][PI] + ADP   Enzyme: [MCP][CH3][Inducer]
        ''' </summary>
        ''' <remarks></remarks>
        Public Property HkAutoPhosphorus As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction()
        ''' <summary>
        ''' [CheAHK][PI] + RR -> [RR][PI] + CheAHK
        ''' [CheAHK][PI] + OCS -> CheAHK + [OCS][PI]
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Property CrossTalk As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction()
        Public Property OCSSensing As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction()

        ''' <summary>
        ''' 连接信号转导网络和调控模型的属性
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TFActive As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction()
    End Class
End Namespace
