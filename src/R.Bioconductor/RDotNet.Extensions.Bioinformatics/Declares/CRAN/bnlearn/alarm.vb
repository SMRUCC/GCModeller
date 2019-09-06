#Region "Microsoft.VisualBasic::e4d555e6656513c363b27f4292620d5c, RDotNet.Extensions.Bioinformatics\Declares\CRAN\bnlearn\alarm.vb"

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

    '     Class alarm
    ' 
    '         Function: RScript
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNet.Extensions.VisualBasic

Namespace declares.bnlearn

    ''' <summary>
    ''' The alarm data set contains the following 37 variables:
    ''' 
    ''' CVP (central venous pressure): a three-level factor With levels LOW, NORMAL And HIGH.
    ''' PCWP (pulmonary capillary wedge pressure): a three-level factor With levels LOW, NORMAL And HIGH.
    ''' HIST (history): a two-level factor With levels True And False.
    ''' TPR (total peripheral resistance): a three-level factor With levels LOW, NORMAL And HIGH.
    ''' BP (blood pressure): a three-level factor With levels LOW, NORMAL And HIGH.
    ''' CO (cardiac output): a three-level factor With levels LOW, NORMAL And HIGH.
    ''' HRBP (heart rate / blood pressure): a three-level factor With levels LOW, NORMAL And HIGH.
    ''' HREK (heart rate measured by an EKG monitor): a three-level factor With levels LOW, NORMAL And HIGH.
    ''' HRSA (heart rate / oxygen saturation): a three-level factor With levels LOW, NORMAL And HIGH.
    ''' PAP (pulmonary artery pressure): a three-level factor With levels LOW, NORMAL And HIGH.
    ''' SAO2 (arterial oxygen saturation): a three-level factor With levels LOW, NORMAL And HIGH.
    ''' FIO2 (fraction of inspired oxygen): a two-level factor With levels LOW And NORMAL.
    ''' PRSS (breathing pressure): a four-level factor With levels ZERO, LOW, NORMAL And HIGH.
    ''' ECO2 (expelled CO2): a four-level factor With levels ZERO, LOW, NORMAL And HIGH.
    ''' MINV (minimum volume): a four-level factor With levels ZERO, LOW, NORMAL And HIGH.
    ''' MVS (minimum volume set): a three-level factor With levels LOW, NORMAL And HIGH.
    ''' HYP (hypovolemia): a two-level factor With levels True And False.
    ''' LVF (left ventricular failure): a two-level factor With levels True And False.
    ''' APL (anaphylaxis): a two-level factor With levels True And False.
    ''' ANES (insufficient anesthesia/analgesia): a two-level factor With levels True And False.
    ''' PMB (pulmonary embolus): a two-level factor With levels True And False.
    ''' INT (intubation): a three-level factor With levels NORMAL, ESOPHAGEAL And ONESIDED.
    ''' KINK (kinked tube): a two-level factor With levels True And False.
    ''' DISC (disconnection): a two-level factor With levels True And False.
    ''' LVV (left ventricular end-diastolic volume): a three-level factor With levels LOW, NORMAL And HIGH.
    ''' STKV (stroke volume): a three-level factor With levels LOW, NORMAL And HIGH.
    ''' CCHL (catecholamine): a two-level factor With levels NORMAL And HIGH.
    ''' ERLO (error low output): a two-level factor With levels True And False.
    ''' HR (heart rate): a three-level factor With levels LOW, NORMAL And HIGH.
    ''' ERCA (electrocauter): a two-level factor With levels True And False.
    ''' SHNT (shunt): a two-level factor With levels NORMAL And HIGH.
    ''' PVS (pulmonary venous oxygen saturation): a three-level factor With levels LOW, NORMAL And HIGH.
    ''' ACO2 (arterial CO2): a three-level factor With levels LOW, NORMAL And HIGH.
    ''' VALV (pulmonary alveoli ventilation): a four-level factor With levels ZERO, LOW, NORMAL And HIGH.
    ''' VLNG (lung ventilation): a four-level factor With levels ZERO, LOW, NORMAL And HIGH.
    ''' VTUB (ventilation tube): a four-level factor With levels ZERO, LOW, NORMAL And HIGH.
    ''' VMCH (ventilation machine): a four-level factor With levels ZERO, LOW, NORMAL And HIGH.
    ''' </summary>
    Public Class alarm : Inherits bnlearnBase

        ''' <summary>
        ''' The ALARM ("A Logical Alarm Reduction Mechanism") is a Bayesian network designed to provide an alarm message system for patient monitoring.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>这个是一个数据集而非一个函数来的</remarks>
        Public Overrides Function RScript() As String
            Return "data(alarm)"
        End Function
    End Class
End Namespace
