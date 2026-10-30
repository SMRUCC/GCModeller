Imports System.Xml.Serialization

Namespace v2

    ''' <summary>
    ''' 细胞区室的类型
    ''' </summary>
    Public Enum CompartmentKind
        ''' <summary>
        ''' 细胞质基质
        ''' </summary>
        intracellular
        ''' <summary>
        ''' 周质空间(革兰氏阴性菌)
        ''' </summary>
        periplasm
        ''' <summary>
        ''' 细胞内膜
        ''' </summary>
        innerMembrane
        ''' <summary>
        ''' 细胞外膜
        ''' </summary>
        outerMembrane
        ''' <summary>
        ''' 细胞外环境(培养基)
        ''' </summary>
        extracellular
        ''' <summary>
        ''' 细胞器内部(真核细胞)
        ''' </summary>
        organelle
    End Enum

    ''' <summary>
    ''' 虚拟细胞模型之中的区室定义。
    ''' 
    ''' 虚拟细胞模型的反应方程以及化合物的定位所使用的区室编号必须在本表之中存在，
    ''' 这样可以保证模型文件本身是自描述的，而不需要依赖外部的区室映射表。
    ''' </summary>
    <XmlType("compartment", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class Compartment

        <XmlAttribute> Public Property id As String

        <XmlAttribute> Public Property name As String

        <XmlAttribute> Public Property kind As CompartmentKind

        ''' <summary>
        ''' 区室的体积，单位：飞升(fL)
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property volume As Double

        ''' <summary>
        ''' 区室的pH值
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property pH As Double

        ''' <summary>
        ''' 区室的离子强度，单位：mol/L
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property ionicStrength As Double

        ''' <summary>
        ''' 区室所相邻的膜结构编号
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property membrane As String

        Public Overrides Function ToString() As String
            Return $"[{kind}] {id}"
        End Function

    End Class

    ''' <summary>
    ''' 虚拟细胞的生理状态参数，用于约束模型模拟的初始条件
    ''' </summary>
    Public Class Physiology

        ''' <summary>
        ''' 培养温度，单位：摄氏度
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property temperature As Double = 37

        ''' <summary>
        ''' 细胞的倍增时间，单位：分钟
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property doublingTime As Double

        ''' <summary>
        ''' 细胞生长的比速率，单位：每小时
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property growthRate As Double

        ''' <summary>
        ''' 培养基的描述文本
        ''' </summary>
        ''' <returns></returns>
        <XmlElement> Public Property medium As String

        ''' <summary>
        ''' 氧气的利用方式：aerobic / anaerobic / facultative
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property oxygenRequirement As String

        <XmlAttribute> Public Property pH As Double

        Public Overrides Function ToString() As String
            Return $"{temperature}C; mu={growthRate}"
        End Function

    End Class

End Namespace
